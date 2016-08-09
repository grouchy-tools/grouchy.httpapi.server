namespace Bivouac.Tests.StatusScenarios
{
   using Banshee;
   using Microsoft.AspNetCore.Builder;
   using Microsoft.Extensions.DependencyInjection;
   using Microsoft.AspNetCore.Http;
   using Bivouac.Abstractions;
   using Bivouac.Middleware;
   using Microsoft.AspNetCore.Hosting.Internal;

   public class StatusFixture
   {
      private readonly StubStatusEndpointService _statusEndpointService;
      private readonly WebApiTestHost _testHost;

      public StatusFixture()
      {
         _statusEndpointService = new StubStatusEndpointService();

         _testHost = new WebApiTestHost(services =>
         {
            services.AddStatusEndpointServices("test-host");

            services.AddSingleton<IStatusEndpointService>(_statusEndpointService);
            ConfigureServicesBuilder(services);
         }, Configure);
      }

      public StubStatusEndpointService StubStatusEndpointService => _statusEndpointService;

      public WebApiTestHost TestHost => _testHost;

      protected virtual void ConfigureServicesBuilder(IServiceCollection services)
      {
      }

      private void Configure(IApplicationBuilder app)
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