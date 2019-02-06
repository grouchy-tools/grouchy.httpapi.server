using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Grouchy.Abstractions;
using Grouchy.HttpApi.Server.Abstractions.EventCallbacks;
using Grouchy.HttpApi.Server.Extensions;
using Grouchy.HttpApi.Server.Testing;
using Grouchy.Resilience.Abstractions.CircuitBreaking;

namespace Grouchy.HttpApi.Server.Tests.SecurityScenarios
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
         app.Use((context, next) =>
         {
            context.Response.Headers.Add("Server", new StringValues("serverName"));
            return next();
         });

         app.UseDefaultMiddleware();

         app.Map("/", "GET", async context =>
         {
            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{}");
         });
      }
   }
}