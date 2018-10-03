using System;
using Banshee;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Bivouac.Abstractions;
using Bivouac.Middleware;
using NUnit.Framework;

namespace Bivouac.Tests.StatusScenarios
{
   public abstract class ScenarioBase
   {
      protected StubStatusEndpointService StubStatusEndpointService { get; private set; }

      protected LightweightWebApiHost TestHost { get; private set; }

      [OneTimeSetUp]
      public void setup_scenario_base()
      {
         StubStatusEndpointService = new StubStatusEndpointService();

         TestHost = new LightweightWebApiHost(services =>
         {
            services.AddStatusEndpointServices("test-host");

            services.AddSingleton<IStatusEndpointService>(StubStatusEndpointService);
            ConfigureServices(services);
         }, Configure);
      }

      protected virtual void ConfigureServices(IServiceCollection services)
      {
      }

      private static void Configure(IApplicationBuilder app)
      {
         app.UseStatusEndpointMiddleware();

         app.Map("/another-endpoint", "GET", async context =>
         {
            context.Response.StatusCode = 201;
            await context.Response.WriteAsync("another-response");
         });
      }
   }
}