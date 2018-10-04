﻿using System;
using Banshee;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Bivouac.Abstractions;
using Bivouac.Events;
using Bivouac.Middleware;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Bivouac.Tests.IdentifyingScenarios
{
   public abstract class ScenarioBase
   {
      public StubGuidGenerator StubGuidGenerator { get; private set; }

      public Guid RequestId { get; private set; }

      public Guid CorrelationId { get; private set; }

      public StubHttpServerEventCallback StubHttpServerEventCallback { get; private set; }

      public LightweightWebApiHost TestHost { get; private set; }

      [OneTimeSetUp]
      public void setup_scenario_base()
      {
         StubGuidGenerator = new StubGuidGenerator();
         RequestId = Guid.NewGuid();
         CorrelationId = Guid.NewGuid();
         StubHttpServerEventCallback = new StubHttpServerEventCallback();
         TestHost = new LightweightWebApiHost(services =>
         {
            services.AddServerLoggingServices();

            services.AddSingleton<IGenerateGuids>(StubGuidGenerator);
            services.AddTransient<IHttpServerEventCallback>(sp => CreateIdentifyingCallbackCallback(sp, StubHttpServerEventCallback));
         }, builder =>
         {
            Configure(builder);
         });
      }

      private static IHttpServerEventCallback CreateIdentifyingCallbackCallback(IServiceProvider serviceProvider, StubHttpServerEventCallback callback)
      {
         var requestIdGetter = serviceProvider.GetService<IGetRequestId>();
         var correlationIdGetter = serviceProvider.GetService<IGetCorrelationId>();
         var assemblyVersionGetter = serviceProvider.GetService<IGetAssemblyVersion>();

         var identifyingCallback = new IdentifyingHttpServerEventCallback(requestIdGetter, correlationIdGetter, assemblyVersionGetter, callback);

         return identifyingCallback;
      }

      private void Configure(IApplicationBuilder app)
      {
         app.UseServerLoggingMiddleware();

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