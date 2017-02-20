namespace Bivouac.Middleware
{
   using System;
   using Bivouac.Abstractions;
   using Bivouac.Services;
   using Microsoft.AspNetCore.Http;
   using Microsoft.Extensions.DependencyInjection;
   using Microsoft.Extensions.DependencyInjection.Extensions;

   public static class ServiceCollectionExtensions
   {
      public static IServiceCollection AddServerLoggingServices(this IServiceCollection services)
      {
         if (services == null) throw new ArgumentNullException(nameof(services));

         services.TryAdd(ServiceDescriptor.Scoped<IGetRequestId, RequestIdGetter>());
         services.TryAdd(ServiceDescriptor.Scoped<IGetCorrelationId, CorrelationIdGetter>());
         services.TryAdd(ServiceDescriptor.Scoped<HttpContext>(sp => sp.GetService<IHttpContextAccessor>().HttpContext));
         services.TryAdd(ServiceDescriptor.Singleton<IHttpContextAccessor, HttpContextAccessor>());
         services.TryAdd(ServiceDescriptor.Singleton<IGenerateGuids, GuidGenerator>());
         services.TryAdd(ServiceDescriptor.Singleton<IGetAssemblyVersion, AssemblyVersionGetter>());

         return services;
      }

      public static IServiceCollection AddStatusEndpointServices(this IServiceCollection services, string name)
      {
         if (services == null) throw new ArgumentNullException(nameof(services));

         services.TryAdd(ServiceDescriptor.Singleton<IStatusEndpointService>(new StatusEndpointService { Name = name }));
         services.TryAdd(ServiceDescriptor.Singleton<IStatusAvailabilityService, StatusAvailabilityService>());

         return services;
      }
   }
}