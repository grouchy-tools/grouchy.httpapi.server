using System;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Grouchy.HttpApi.Server.Abstractions;
using Grouchy.HttpApi.Server.EventCallbacks;
using Grouchy.HttpApi.Server.Exceptions;
using Grouchy.HttpApi.Server.Extensions;
using Grouchy.Abstractions;
using Grouchy.HttpApi.Server.Testing;
using Grouchy.Resilience.Abstractions.CircuitBreaking;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Grouchy.HttpApi.Server.Tests.ServerLoggingScenarios
{
   public abstract class ScenarioBase
   {
      public string RequestId { get; private set; }

      public string CorrelationId { get; private set; }

      public string Service { get; private set; }
      
      public string Version { get; private set; }

      public StubRequestIdGetter StubRequestIdGetter { get; private set; }

      public StubCorrelationIdGetter StubCorrelationIdGetter { get; private set; }

      public StubApplicationInfo StubApplicationInfo { get; private set; }

      public StubHttpServerEventCallback StubHttpServerEventCallback { get; private set; }

      public LightweightHttpApiHost TestHost { get; private set; }

      [OneTimeSetUp]
      public void setup_scenario_base()
      {
         RequestId = Guid.NewGuid().ToString();
         CorrelationId = Guid.NewGuid().ToString();
         Service = "theService";
         Version = "1.2.3-server";
         StubRequestIdGetter = new StubRequestIdGetter { RequestId = RequestId };
         StubCorrelationIdGetter = new StubCorrelationIdGetter { CorrelationId = CorrelationId };
         StubApplicationInfo = new StubApplicationInfo { Name = Service, Version = Version};
         StubHttpServerEventCallback = new StubHttpServerEventCallback();
         var identifyingCallback = new IdentifyingHttpServerEventCallback(StubRequestIdGetter, StubCorrelationIdGetter, StubApplicationInfo);
         TestHost = new LightweightHttpApiHost(services =>
         {
            services.AddDefaultServices();

            services.AddSingleton<ICircuitBreakerManager, StubCircuitBreakerManager>();
            services.AddSingleton<IApplicationInfo>(StubApplicationInfo);
            services.AddSingleton<IGetRequestId>(StubRequestIdGetter);
            services.AddSingleton<IGetCorrelationId>(StubCorrelationIdGetter);
            services.AddSingleton<IHttpServerEventCallback>(identifyingCallback);
            services.AddSingleton<IHttpServerEventCallback>(StubHttpServerEventCallback);
         }, Configure);
      }

      private void Configure(IApplicationBuilder app)
      {
         app.UseDefaultMiddleware();

         app.Map("/happy-path", "GET", async context =>
         {
            context.Response.StatusCode = 200;
            await context.Response.WriteAsync("Complete!");
         });

         app.Map("/happy-path", "POST", async context =>
         {
            context.Response.StatusCode = 200;
            await context.Response.WriteAsync("Complete Post!");
         });

         app.Map("/not-found-exception", "GET", context =>
         {
            throw new HttpNotFoundException("Thing not found");
         });

         app.Map("/http-exception", "GET", context =>
         {
            throw new HttpException(HttpStatusCode.NotAcceptable, "Some http exception");
         });

         app.Map("/exception", "GET", context =>
         {
            throw new CustomException("Custom exception message");
         });
      }

      public class CustomException : Exception
      {
         public CustomException(string message) : base(message)
         {
         }
      }
   }
}
