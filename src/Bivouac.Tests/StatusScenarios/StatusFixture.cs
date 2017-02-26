namespace Bivouac.Tests.StatusScenarios
{
   using System;
   using Banshee;
   using Microsoft.AspNetCore.Builder;
   using Microsoft.Extensions.DependencyInjection;
   using Microsoft.AspNetCore.Http;
   using Bivouac.Abstractions;
   using Bivouac.Middleware;

   public class StatusFixture
   {
      private readonly StubStatusEndpointService _statusEndpointService;
      private readonly LightweightWebApiHost _testHost;

      public StatusFixture(Action<IServiceCollection> configureServices = null)
      {
         _statusEndpointService = new StubStatusEndpointService();

         _testHost = new LightweightWebApiHost(services =>
         {
            services.AddStatusEndpointServices("test-host");

            services.AddSingleton<IStatusEndpointService>(_statusEndpointService);
            configureServices?.Invoke(services);
         }, Configure);
      }

      public StubStatusEndpointService StubStatusEndpointService => _statusEndpointService;

      public LightweightWebApiHost TestHost => _testHost;

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