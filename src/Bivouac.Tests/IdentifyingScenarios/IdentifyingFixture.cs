namespace Bivouac.Tests.IdentifyingScenarios
{
   using System;
   using Banshee;
   using Microsoft.AspNetCore.Builder;
   using Microsoft.AspNetCore.Http;
   using Microsoft.Extensions.DependencyInjection;
   using Bivouac.Abstractions;
   using Bivouac.Middleware;
   using Bivouac.Services;
   using Newtonsoft.Json;

   public class IdentifyingFixture
   {
      private readonly StubGuidGenerator _stubGuidGenerator;
      private readonly StubEventLogger _stubEventLogger;
      private readonly WebApiTestHost _testHost;

      public IdentifyingFixture(Action<IApplicationBuilder> preConfigure = null)
      {
         _stubGuidGenerator = new StubGuidGenerator();
         _stubEventLogger = new StubEventLogger();
         _testHost = new WebApiTestHost(services =>
         {
            services.AddServerLoggingServices();

            services.AddSingleton<IGenerateGuids>(_stubGuidGenerator);
            services.AddSingleton<ILogEvents>(_stubEventLogger);
         }, builder =>
         {
            preConfigure?.Invoke(builder);
            Configure(builder);
         });
      }

      public StubGuidGenerator StubGuidGenerator => _stubGuidGenerator;

      public StubEventLogger StubEventLogger => _stubEventLogger;

      public WebApiTestHost TestHost => _testHost;

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
