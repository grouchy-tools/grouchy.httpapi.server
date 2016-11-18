namespace Bivouac.Tests.IdentifyingScenarios
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
      private readonly StubHttpServerEventCallback _stubCallback;
      private readonly LightweightWebApiHost _testHost;

      public IdentifyingFixture(Action<IApplicationBuilder, Guid, Guid> preConfigure = null)
      {
         _stubGuidGenerator = new StubGuidGenerator();
         _requestId = Guid.NewGuid();
         _correlationId = Guid.NewGuid();
         _stubCallback = new StubHttpServerEventCallback();
         _testHost = new LightweightWebApiHost(services =>
         {
            services.AddServerLoggingServices();

            services.AddSingleton<IGenerateGuids>(_stubGuidGenerator);
            services.AddSingleton<IHttpServerEventCallback>(sp => CreateIdentifyingCallbackCallback(sp, _stubCallback));
         }, builder =>
         {
            preConfigure?.Invoke(builder,_requestId, _correlationId);
            Configure(builder);
         });
      }

      private static IHttpServerEventCallback CreateIdentifyingCallbackCallback(IServiceProvider serviceProvider, StubHttpServerEventCallback callback)
      {
         var requestIdGetter = serviceProvider.GetService<IGetRequestId>();
         var correlationIdGetter = serviceProvider.GetService<IGetCorrelationId>();

         var identifyingCallback = new IdentifyingHttpServerEventCallback(requestIdGetter, correlationIdGetter, callback);

         return identifyingCallback;
      }

      public StubGuidGenerator StubGuidGenerator => _stubGuidGenerator;

      public Guid RequestId => _requestId;

      public Guid CorrelationId => _correlationId;

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
