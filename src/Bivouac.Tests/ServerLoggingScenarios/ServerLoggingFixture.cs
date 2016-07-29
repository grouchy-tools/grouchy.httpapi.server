namespace Bivouac.Tests.ServerLoggingScenarios
{
   using System;
   using System.Net;
   using Banshee;
   using Microsoft.AspNetCore.Builder;
   using Microsoft.Extensions.DependencyInjection;
   using Microsoft.AspNetCore.Http;
   using Bivouac.Abstractions;
   using Bivouac.Exceptions;
   using Bivouac.Middleware;
   using Bivouac.Services;

   public class ServerLoggingFixture
   {
      private readonly Guid _requestId;
      private readonly Guid _correlationId;
      private readonly StubRequestIdGetter _stubRequestIdGetter;
      private readonly StubCorrelationIdGetter _stubCorrelationIdGetter;
      private readonly StubEventLogger _stubEventLogger;
      private readonly WebApiTestHost _testHost;

      public ServerLoggingFixture()
      {
         _requestId = Guid.NewGuid();
         _correlationId = Guid.NewGuid();
         _stubRequestIdGetter = new StubRequestIdGetter { RequestId = _requestId };
         _stubCorrelationIdGetter = new StubCorrelationIdGetter { CorrelationId = _correlationId };
         _stubEventLogger = new StubEventLogger();
         _testHost = new WebApiTestHost(services =>
         {
            services.AddServerLoggingServices();

            services.AddSingleton<IGetRequestId>(_stubRequestIdGetter);
            services.AddSingleton<IGetCorrelationId>(_stubCorrelationIdGetter);
            services.AddSingleton<ILogEvents>(_stubEventLogger);
         }, Configure);
      }

      public StubRequestIdGetter StubRequestIdGetter => _stubRequestIdGetter;

      public StubCorrelationIdGetter StubCorrelationIdGetter => _stubCorrelationIdGetter;

      public StubEventLogger StubEventLogger => _stubEventLogger;

      public Guid RequestId => _requestId;

      public Guid CorrelationId => _correlationId;

      public WebApiTestHost TestHost => _testHost;

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

      private class CustomException : Exception
      {
         public CustomException(string message) : base(message)
         {
         }
      }
   }
}
