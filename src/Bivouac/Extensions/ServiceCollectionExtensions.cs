using System;
using System.Collections.Generic;
using Bivouac.Abstractions;
using Bivouac.EventCallbacks;
using Bivouac.Services;
using Burble.Abstractions;
using Burble.Abstractions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bivouac.Extensions
{
   public static class ServiceCollectionExtensions
   {
      public static IServiceCollection AddDefaultServices(this IServiceCollection services)
      {
         if (services == null) throw new ArgumentNullException(nameof(services));

         services.Add(ServiceDescriptor.Scoped<IGetRequestId, RequestIdGetter>());
         services.Add(ServiceDescriptor.Scoped<IGetCorrelationId, CorrelationIdGetter>());
         services.Add(ServiceDescriptor.Scoped<HttpContext>(sp => sp.GetService<IHttpContextAccessor>().HttpContext));
         
         services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
         services.AddTransient<IGenerateGuids, GuidGenerator>();
         services.AddTransient<IGetServiceName, ServiceNameGetter>();
         services.AddTransient<IGetServiceVersion, ServiceVersionGetter>();
         services.AddTransient<IStatusAvailabilityService, StatusAvailabilityService>();

         services.AddTransient<IHttpServerEventCallback, IdentifyingHttpServerEventCallback>();
         services.AddTransient<IHttpServerEventCallback, JsonLoggingHttpServerEventCallback>();
         services.AddTransient<IHttpClientEventCallback, IdentifyingHttpClientEventCallback>();
         services.AddTransient<IHttpClientEventCallback, JsonLoggingHttpClientEventCallback>();

         return services;
      }

      public static IServiceCollection AddConfiguration<TConfiguration>(this IServiceCollection services, IConfiguration configuration)
         where TConfiguration : class, new()
      {
         if (services == null) throw new ArgumentNullException(nameof(services));

         var typedConfiguration = new TConfiguration();
         new ConfigureFromConfigurationOptions<TConfiguration>(configuration).Action(typedConfiguration);

         services.AddSingleton<TConfiguration>(typedConfiguration);

         return services;
      }

      public static IServiceCollection AddHttpApi<TConfiguration, TContract, TConcrete>(this IServiceCollection services, string name, IConfiguration configuration)
         where TConfiguration : class, IHttpApiConfiguration, new()
         where TContract : class
         where TConcrete : class, TContract
      {
         if (services == null) throw new ArgumentNullException(nameof(services));

         services.AddConfiguration<TConfiguration>(configuration);

         services.AddTransient<TContract>(sp =>
         {
            var httpClient = BuildHttpClient<TConfiguration>(sp);

            return (TContract)ActivatorUtilities.CreateInstance<TConcrete>(sp, httpClient);
         });

         services.AddTransient<IStatusEndpointDependency>(sp =>
         {
            var httpClient = BuildHttpClient<TConfiguration>(sp);
            
            return new ApiStatusEndpointDependency(name, httpClient);
         });

         return services;
      }

      private static IHttpClient BuildHttpClient<TConfiguration>(IServiceProvider sp) where TConfiguration : IHttpApiConfiguration
      {
         var httpClientRegistry = sp.GetRequiredService<IHttpClientRegistry>();
         var httpClientDecorators = sp.GetRequiredService<IEnumerable<IHttpClientDecorator>>();
         var typedConfig = sp.GetRequiredService<TConfiguration>();

         var httpClient = httpClientRegistry.Get(typedConfig);

         foreach (var httpClientDecorator in httpClientDecorators)
         {
            httpClient = httpClientDecorator.Decorate(httpClient);
         }

         return httpClient;
      }
   }
}
