using System.Net.Http;
using Bivouac.Abstractions;
using Bivouac.Model;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Bivouac.Tests.StatusScenarios.stubbed_dependencies
{
   public class multiple_dependencies : ScenarioBase
   {
      private HttpResponseMessage _response;

      [OneTimeSetUp]
      public void setup_scenario()
      {
         _response = TestHost.Get("/status");
      }

      protected override void ConfigureServices(IServiceCollection services)
      {
         var status1 = new Status { Name = "myDep1", Availability = Availability.Limited };
         var status2 = new Status { Name = "myDep2", Availability = Availability.Unknown };

         services.AddSingleton<IStatusEndpointDependency>(new StubStatusEndpointDependency { Status = status1 });
         services.AddSingleton<IStatusEndpointDependency>(new StubStatusEndpointDependency { Status = status2 });
      }

      [Test]
      public void should_return_status_code_okay()
      {
         Assert.AreEqual(200, (int)_response.StatusCode);
      }

      [Test]
      public void should_return_exact_json_content()
      {
         var content = _response.Content.ReadAsStringAsync().Result;

         Assert.AreEqual("{\"name\":null,\"availability\":\"Limited\",\"host\":\"http://localhost\",\"dependencies\":[{\"name\":\"myDep1\",\"availability\":\"Limited\"},{\"name\":\"myDep2\",\"availability\":\"Unknown\"}]}", content);
      }
   }
}
