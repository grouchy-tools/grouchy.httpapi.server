namespace Bivouac.Tests.StatusScenarios
{
   using System.Net;
   using System.Net.Http;
   using Xunit;
   //using Bivouac.Model;
   //using Microsoft.Extensions.DependencyInjection;

   public class ping : IClassFixture<ping.fixture>
   {
      public class fixture : StatusFixture
      {
         public readonly HttpResponseMessage Response;

         public fixture()
         {
            Response = TestHost.Get("/ping");
         }

         //protected override void ConfigureServicesBuilder(IServiceCollection services)
         //{
         //   base.ConfigureServicesBuilder(services);

         //   var status = new Status { Name = "myDependency", Availability = Availability.Limited };

         //   services.AddSingleton<IStatusEndpointDependency>(new StubStatusEndpointDependency { Status = status });
         //}
      }

      private readonly fixture _fixture;

      public ping(fixture fixture)
      {
         _fixture = fixture;
      }
      //public ping()
      //{
      //   //Action<IServiceCollection> configureServices = services =>
      //   //{
      //   //   Startup.ConfigureServices(services);
      //   //   services.AddSingleton<IServerLoggingService, NoOpServerLoggingService>();
      //   //};
      //   //var testHost = new WebApiTestHost(configureServices, Startup.Configure);

      //   _response = testHost.Get("/ping");
      //}

      [Fact]
      public void should_return_status_code_200()
      {
         Assert.Equal(_fixture.Response.StatusCode, HttpStatusCode.OK);
      }

      [Fact]
      public void should_return_content_pong()
      {
         var content = _fixture.Response.Content.ReadAsStringAsync().Result;

         Assert.Equal("Pong!", content);
      }
   }
}
