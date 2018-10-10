using System.Net.Http;
using Bivouac.Abstractions;
using Bivouac.Model;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Bivouac.Tests.StatusScenarios.stubbed_dependencies
{
   public class with_unavailable_dependency : ScenarioBase
   {
      private HttpResponseMessage _response;

      [OneTimeSetUp]
      public void setup_scenario()
      {
         StubServiceNameGetter.Name = "myName";

         _response = TestHost.Get("/status");
      }

      protected override void ConfigureServices(IServiceCollection services)
      {
         var status = new Status { Name = "myDependency", Availability = Availability.Unavailable };

         services.AddSingleton<IStatusEndpointDependency>(new StubStatusEndpointDependency { Status = status });
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

         Assert.AreEqual("{\"name\":\"myName\",\"availability\":\"Limited\",\"host\":\"http://localhost\",\"dependencies\":[{\"name\":\"myDependency\",\"availability\":\"Unavailable\"}]}", content);
      }
   }
}
