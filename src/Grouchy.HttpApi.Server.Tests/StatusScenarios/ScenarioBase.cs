using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Grouchy.HttpApi.Server.Extensions;
using Grouchy.Abstractions;
using Grouchy.HttpApi.Server.Abstractions.EventCallbacks;
using Grouchy.HttpApi.Server.Testing;
using Grouchy.Resilience.Abstractions.CircuitBreaking;
using NUnit.Framework;

namespace Grouchy.HttpApi.Server.Tests.StatusScenarios
{
   public abstract class ScenarioBase
   {
      protected StubApplicationInfo StubApplicationInfo { get; private set; }

      protected StubHttpServerEventCallback StubHttpServerEventCallback { get; private set; }

      protected LightweightHttpApiHost TestHost { get; private set; }

      [OneTimeSetUp]
      public void setup_scenario_base()
      {
         StubApplicationInfo = new StubApplicationInfo();
         StubHttpServerEventCallback = new StubHttpServerEventCallback();

         TestHost = new LightweightHttpApiHost(services =>
         {
            services.AddDefaultServices();

            services.AddSingleton<ICircuitBreakerManager, StubCircuitBreakerManager>();
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