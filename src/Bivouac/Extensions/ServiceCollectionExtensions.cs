using System;
using System.Collections.Generic;
using System.Net;
using Bivouac.Abstractions;
using Bivouac.CircuitBreaking;
using Bivouac.EventCallbacks;
using Bivouac.Services;
using Burble.Abstractions;
using Burble.Abstractions.CircuitBreaking;
using Burble.Abstractions.Configuration;
using Burble.Abstractions.Identifying;
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
         
         services.AddHostedService<CircuitBreakingHostedService>();

         services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
         services.AddTransient<IGenerateGuids, GuidGenerator>();
         services.AddTransient<IApplicationInfo, ApplicationInfo>();
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

      public static IServiceCollection AddHttpApi<TConfiguration, TContract, TService>(this IServiceCollection services, IConfiguration configuration)
         where TConfiguration : class, IHttpApiConfiguration, new()
         where TContract : class
         where TService : class, TContract
      {
         if (services == null) throw new ArgumentNullException(nameof(services));

         services.AddConfiguration<TConfiguration>(configuration);

         services.AddTransient<TContract>(sp =>
         {
            var (httpClient, _) = BuildHttpClient<TConfiguration>(sp);

            return (TContract)ActivatorUtilities.CreateInstance<TService>(sp, httpClient);
         });

         services.AddTransient<IStatusEndpointDependency>(sp =>
         {
            var circuitBreakingStateManager = sp.GetRequiredService<ICircuitBreakingStateManager<HttpStatusCode>>();
            var (httpClient, typedConfiguration) = BuildHttpClient<TConfiguration>(sp);
            
            var httpApiStatusEndpointDependency = new HttpApiStatusEndpointDependency(httpClient, typedConfiguration);

            if (typedConfiguration is IHttpApiWithCircuitBreaking httpApiWithCircuitBreaking)
            {
               return new CircuitBreakingStatusEndpointDependency<HttpStatusCode>(httpApiStatusEndpointDependency, circuitBreakingStateManager.Get(httpApiWithCircuitBreaking));
            }
            
            return httpApiStatusEndpointDependency;
         });

         return services;
      }

      // TODO: Move this behind an interface
      private static (IHttpClient, TConfiguration) BuildHttpClient<TConfiguration>(IServiceProvider sp) where TConfiguration : IHttpApiConfiguration
      {
         var httpClientRegistry = sp.GetRequiredService<IHttpClientRegistry>();
         var httpClientDecorators = sp.GetRequiredService<IEnumerable<IHttpClientDecorator>>();
         var configuration = sp.GetRequiredService<TConfiguration>();

         var httpClient = httpClientRegistry.Get(configuration);

         foreach (var httpClientDecorator in httpClientDecorators)
         {
            httpClient = httpClientDecorator.Decorate(httpClient, configuration);
         }

         return (httpClient, configuration);
      }
   }
}
