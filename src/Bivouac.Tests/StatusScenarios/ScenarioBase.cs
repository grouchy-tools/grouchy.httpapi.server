using System;
using Banshee;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Bivouac.Abstractions;
using Bivouac.Extensions;
using Bivouac.Middleware;
using NUnit.Framework;

namespace Bivouac.Tests.StatusScenarios
{
   public abstract class ScenarioBase
   {
      protected StubServiceNameGetter StubServiceNameGetter { get; private set; }

      protected StubServiceVersionGetter StubServiceVersionGetter { get; private set; }

      protected LightweightWebApiHost TestHost { get; private set; }

      [OneTimeSetUp]
      public void setup_scenario_base()
      {
         StubServiceNameGetter = new StubServiceNameGetter();
         StubServiceVersionGetter = new StubServiceVersionGetter();

         TestHost = new LightweightWebApiHost(services =>
         {
            services.AddDefaultServices("test-host");

            services.AddSingleton<IGetServiceName>(StubServiceNameGetter);
            services.AddSingleton<IGetServiceVersion>(StubServiceVersionGetter);
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