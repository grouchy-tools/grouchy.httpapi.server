using System;
using System.Collections.Generic;
using System.Net;
using Grouchy.HttpApi.Server.Abstractions;
using Grouchy.HttpApi.Server.CircuitBreaking;
using Grouchy.HttpApi.Server.EventCallbacks;
using Grouchy.HttpApi.Server.Services;
using Grouchy.Abstractions;
using Grouchy.Abstractions.Tagging;
using Grouchy.HttpApi.Client.Abstractions;
using Grouchy.HttpApi.Client.Abstractions.Configuration;
using Grouchy.HttpApi.Client.Abstractions.EventCallbacks;
using Grouchy.HttpApi.Client.Abstractions.HttpClients;
using Grouchy.HttpApi.Client.Abstractions.Tagging;
using Grouchy.HttpApi.Server.Abstractions.EventCallbacks;
using Grouchy.HttpApi.Server.Abstractions.Tagging;
using Grouchy.HttpApi.Server.Tagging;
using Grouchy.Resilience.Abstractions.CircuitBreaking;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Grouchy.HttpApi.Server.Extensions
{
   public static class ServiceCollectionExtensions
   {
      public static IServiceCollection AddDefaultServices(this IServiceCollection services)
      {
         if (services == null) throw new ArgumentNullException(nameof(services));

         services.AddSingleton<IOutboundRequestIdAccessor, OutboundRequestIdAccessor>();
         services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

         services.AddScoped<IInboundRequestIdAccessor, InboundRequestIdGetter>();
         services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();
         services.AddScoped<ISessionIdAccessor, SessionIdAccessor>();
         
         services.AddTransient<IGenerateGuids, GuidGenerator>();
         services.AddTransient<IApplicationInfo, ApplicationInfo>();

         services.AddTransient<IHttpServerEventCallback, IdentifyingHttpServerEventCallback>();
         services.AddTransient<IHttpServerEventCallback, JsonLoggingHttpServerEventCallback>();
         services.AddTransient<IHttpClientEventCallback, IdentifyingHttpClientEventCallback>();
         services.AddTransient<IHttpClientEventCallback, JsonLoggingHttpClientEventCallback>();

         return services;
      }

      public static IServiceCollection AddCircuitBreakerStateManager<TCircuitBreakerManager>(this IServiceCollection services, Action<TCircuitBreakerManager> configureManager)
         where TCircuitBreakerManager: ICircuitBreakerManager, new()
      {
         services.AddHostedService<CircuitBreakerHostedService>();

         services.AddSingleton<ICircuitBreakerManager>(sp =>
         {
            var manager = new TCircuitBreakerManager();
            configureManager(manager);
            return manager;
         });

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

      public static IServiceCollection AddHttpApi<TConfiguration, TContract, TService>(this IServiceCollection services)
         where TConfiguration : class, IHttpApiConfiguration, new()
         where TContract : class
         where TService : class, TContract
      {
         if (services == null) throw new ArgumentNullException(nameof(services));

         services.AddTransient<TContract>(sp =>
         {
            var (httpClient, _) = BuildHttpClient<TConfiguration>(sp);

            return (TContract)ActivatorUtilities.CreateInstance<TService>(sp, httpClient);
         });

         services.AddTransient<IStatusEndpointDependency>(sp =>
         {
            var (httpClient, typedConfiguration) = BuildHttpClient<TConfiguration>(sp);
            var httpApiStatusEndpointDependency = new HttpApiStatusEndpointDependency(httpClient, typedConfiguration);

            if (typedConfiguration is IHttpApiWithCircuitBreaking httpApiWithCircuitBreaking)
            {
               var circuitBreakerManager = sp.GetRequiredService<ICircuitBreakerManager>();
               
               return new CircuitBreakingStatusEndpointDependency<HttpStatusCode>(httpApiStatusEndpointDependency, circuitBreakerManager.GetState(httpApiWithCircuitBreaking.CircuitBreakerPolicy));
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
