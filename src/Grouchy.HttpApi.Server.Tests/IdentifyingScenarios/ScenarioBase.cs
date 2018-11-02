using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Grouchy.HttpApi.Server.Abstractions;
using Grouchy.HttpApi.Server.EventCallbacks;
using Grouchy.HttpApi.Server.Extensions;
using Grouchy.Abstractions;
using Grouchy.HttpApi.Server.Testing;
using Grouchy.Resilience.Abstractions.CircuitBreaking;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Grouchy.HttpApi.Server.Tests.IdentifyingScenarios
{
   public abstract class ScenarioBase
   {
      protected StubGuidGenerator StubGuidGenerator { get; private set; }

      protected Guid RequestId { get; private set; }

      protected Guid CorrelationId { get; private set; }

      protected StubHttpServerEventCallback StubHttpServerEventCallback { get; private set; }

      protected LightweightHttpApiHost TestHost { get; private set; }

      [OneTimeSetUp]
      public void setup_scenario_base()
      {
         StubGuidGenerator = new StubGuidGenerator();
         RequestId = Guid.NewGuid();
         CorrelationId = Guid.NewGuid();
         StubHttpServerEventCallback = new StubHttpServerEventCallback();
         TestHost = new LightweightHttpApiHost(services =>
         {
            services.AddDefaultServices();

            services.AddSingleton<ICircuitBreakerManager, StubCircuitBreakerManager>();
            services.AddSingleton<IGenerateGuids>(StubGuidGenerator);
            services.AddSingleton<IHttpServerEventCallback>(CreateIdentifyingCallback);
            services.AddSingleton<IHttpServerEventCallback>(StubHttpServerEventCallback);
         }, Configure);
      }

      private static IdentifyingHttpServerEventCallback CreateIdentifyingCallback(IServiceProvider serviceProvider)
      {
         var requestIdGetter = serviceProvider.GetService<IGetRequestId>();
         var correlationIdGetter = serviceProvider.GetService<IGetCorrelationId>();
         var applicationInfo = serviceProvider.GetService<IApplicationInfo>();

         var identifyingCallback = new IdentifyingHttpServerEventCallback(requestIdGetter, correlationIdGetter, applicationInfo);

         return identifyingCallback;
      }

      private void Configure(IApplicationBuilder app)
      {
         app.UseDefaultMiddleware();

         app.Map("/get-ids-from-context", "GET", async context =>
         {
            var requestIdGetter = context.RequestServices.GetService<IGetRequestId>();
            var correlationIdGetter = context.RequestServices.GetService<IGetCorrelationId>();

            var response = new { requestId = requestIdGetter.Get(), correlationId = correlationIdGetter.Get() };
            var json = JsonConvert.SerializeObject(response);

            context.Response.StatusCode = 200;
            await context.Response.WriteAsync(json);
         });
      }
   }
}
