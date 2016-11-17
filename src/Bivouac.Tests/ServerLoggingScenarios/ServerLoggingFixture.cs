namespace Bivouac.Tests.ServerLoggingScenarios
{
   using System;
   using System.Net;
   using Banshee;
   using Microsoft.AspNetCore.Builder;
   using Microsoft.Extensions.DependencyInjection;
   using Microsoft.AspNetCore.Http;
   using Bivouac.Abstractions;
   using Bivouac.Events;
   using Bivouac.Exceptions;
   using Bivouac.Middleware;

   public class ServerLoggingFixture
   {
      private readonly string _requestId;
      private readonly string _correlationId;
      private readonly StubRequestIdGetter _stubRequestIdGetter;
      private readonly StubCorrelationIdGetter _stubCorrelationIdGetter;
      private readonly StubHttpServerEventCallback _stubCallback;
      private readonly LightweightWebApiHost _testHost;

      public ServerLoggingFixture()
      {
         _requestId = Guid.NewGuid().ToString();
         _correlationId = Guid.NewGuid().ToString();
         _stubRequestIdGetter = new StubRequestIdGetter { RequestId = _requestId };
         _stubCorrelationIdGetter = new StubCorrelationIdGetter { CorrelationId = _correlationId };
         _stubCallback = new StubHttpServerEventCallback();
         var correlatingCallback = new CorrelatingHttpServerEventCallback(_stubRequestIdGetter, _stubCorrelationIdGetter, _stubCallback);
         _testHost = new LightweightWebApiHost(services =>
         {
            services.AddServerLoggingServices();

            services.AddSingleton<IGetRequestId>(_stubRequestIdGetter);
            services.AddSingleton<IGetCorrelationId>(_stubCorrelationIdGetter);
            services.AddSingleton<IHttpServerEventCallback>(correlatingCallback);
         }, Configure);
      }

      public string RequestId => _requestId;

      public string CorrelationId => _correlationId;

      public StubRequestIdGetter StubRequestIdGetter => _stubRequestIdGetter;

      public StubCorrelationIdGetter StubCorrelationIdGetter => _stubCorrelationIdGetter;

      public StubHttpServerEventCallback StubHttpServerEventCallback => _stubCallback;

      public LightweightWebApiHost TestHost => _testHost;

      private void Configure(IApplicationBuilder app)
      {
         app.UseServerLoggingMiddleware();

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
