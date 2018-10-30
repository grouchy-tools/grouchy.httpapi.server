using System;
using System.Net;
using Banshee;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Bivouac.Abstractions;
using Bivouac.Extensions;
using Bivouac.Middleware;
using Burble.Abstractions.CircuitBreaking;
using Burble.Abstractions.Identifying;
using Burble.CircuitBreaking;
using NUnit.Framework;

namespace Bivouac.Tests.StatusScenarios
{
   public abstract class ScenarioBase
   {
      protected StubApplicationInfo StubApplicationInfo { get; private set; }

      protected StubHttpServerEventCallback StubHttpServerEventCallback { get; private set; }

      protected LightweightWebApiHost TestHost { get; private set; }

      [OneTimeSetUp]
      public void setup_scenario_base()
      {
         StubApplicationInfo = new StubApplicationInfo();
         StubHttpServerEventCallback = new StubHttpServerEventCallback();

         TestHost = new LightweightWebApiHost(services =>
         {
            services.AddDefaultServices();

            services.AddSingleton<ICircuitBreakingStateManager<HttpStatusCode>, CircuitBreakingStateManager<HttpStatusCode>>();
            services.AddSingleton<IApplicationInfo>(StubApplicationInfo);
            services.AddSingleton<IHttpServerEventCallback>(StubHttpServerEventCallback);
            ConfigureServices(services);
         }, Configure);
      }

      protected virtual void ConfigureServices(IServiceCollection services)
      {
      }

      private static void Configure(IApplicationBuilder app)
      {
         app.UseDefaultMiddleware();

         app.Map("/another-endpoint", "GET", async context =>
         {
            context.Response.StatusCode = 201;
            await context.Response.WriteAsync("another-response");
         });
      }
   }
}