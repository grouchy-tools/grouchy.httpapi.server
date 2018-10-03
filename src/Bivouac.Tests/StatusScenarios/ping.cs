using System.Net;
using System.Net.Http;
using NUnit.Framework;

namespace Bivouac.Tests.StatusScenarios
{
   public class ping : ScenarioBase
   {
      private HttpResponseMessage _response;

         //protected override void ConfigureServicesBuilder(IServiceCollection services)
         //{
         //   base.ConfigureServicesBuilder(services);

         //   var status = new Status { Name = "myDependency", Availability = Availability.Limited };

         //   services.AddSingleton<IStatusEndpointDependency>(new StubStatusEndpointDependency { Status = status });
         //}

      [OneTimeSetUp]
      public void setup_scenario()
      {
         _response = TestHost.Get("/ping");
      }
      //public ping()
      //{
      //   //Action<IServiceCollection> configureServices = services =>
      //   //{
      //   //   Startup.ConfigureServices(services);
      //   //   services.AddSingleton<IServerLoggingService, NoOpServerLoggingService>();
      //   //};
      //   //var testHost = new LightweightWebApiHost(configureServices, Startup.Configure);

      //   _response = testHost.Get("/ping");
      //}

      [Test]
      public void should_return_status_code_200()
      {
         Assert.AreEqual(_response.StatusCode, HttpStatusCode.OK);
      }

      [Test]
      public void should_return_content_pong()
      {
         var content = _response.Content.ReadAsStringAsync().Result;

         Assert.AreEqual("Pong!", content);
      }
   }
}
