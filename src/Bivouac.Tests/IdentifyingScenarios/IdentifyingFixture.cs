﻿namespace Bivouac.Tests.IdentifyingScenarios
{
   using System;
   using Banshee;
   using Microsoft.AspNetCore.Builder;
   using Microsoft.AspNetCore.Http;
   using Microsoft.Extensions.DependencyInjection;
   using Bivouac.Abstractions;
   using Bivouac.Events;
   using Bivouac.Middleware;
   using Newtonsoft.Json;

   public class IdentifyingFixture
   {
      private readonly StubGuidGenerator _stubGuidGenerator;
      private readonly Guid _requestId;
      private readonly Guid _correlationId;
      private readonly StubRequestIdGetter _stubRequestIdGetter;
      private readonly StubCorrelationIdGetter _stubCorrelationIdGetter;
      private readonly StubHttpServerEventCallback _stubCallback;
      private readonly LightweightWebApiHost _testHost;

      public IdentifyingFixture(Action<IApplicationBuilder, Guid, Guid> preConfigure = null)
      {
         _stubGuidGenerator = new StubGuidGenerator();
         _requestId = Guid.NewGuid();
         _correlationId = Guid.NewGuid();
         _stubRequestIdGetter = new StubRequestIdGetter { RequestId = _requestId.ToString() };
         _stubCorrelationIdGetter = new StubCorrelationIdGetter { CorrelationId = _correlationId.ToString() };
         _stubCallback = new StubHttpServerEventCallback(new CorrelatingHttpServerEventCallback(_stubRequestIdGetter, _stubCorrelationIdGetter));
         _testHost = new LightweightWebApiHost(services =>
         {
            services.AddServerLoggingServices();

            services.AddSingleton<IGenerateGuids>(_stubGuidGenerator);
            services.AddSingleton<IHttpServerEventCallback>(_stubCallback);
         }, builder =>
         {
            preConfigure?.Invoke(builder,_requestId, _correlationId);
            Configure(builder);
         });
      }

      public StubGuidGenerator StubGuidGenerator => _stubGuidGenerator;

      public Guid RequestId => _requestId;

      public Guid CorrelationId => _correlationId;

      public StubRequestIdGetter StubRequestIdGetter => _stubRequestIdGetter;

      public StubCorrelationIdGetter StubCorrelationIdGetter => _stubCorrelationIdGetter;

      public StubHttpServerEventCallback StubHttpServerEventCallback => _stubCallback;

      public LightweightWebApiHost TestHost => _testHost;

      private void Configure(IApplicationBuilder app)
      {
         app.UseServerLoggingMiddleware();

         app.Map("/get-ids-from-context", "GET", async context =>
         {
            var response = new { requestId = context.Items["request-id"], correlationId = context.Items["correlation-id"] };
            var json = JsonConvert.SerializeObject(response);

            context.Response.StatusCode = 200;
            await context.Response.WriteAsync(json);
         });
      }
   }
}
