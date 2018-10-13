using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Bivouac.Abstractions;
using Bivouac.Model;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Bivouac.Tests.StatusScenarios.stubbed_dependencies
{
   // ReSharper disable once InconsistentNaming
   public class with_unavailable_dependency : ScenarioBase
   {
      private HttpResponseMessage _response;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         StubServiceNameGetter.Name = "myName";

         _response = await TestHost.GetAsync("/.status");
      }

      protected override void ConfigureServices(IServiceCollection services)
      {
         var status = new Status { Name = "myDependency", Availability = Availability.Unavailable };

         services.AddSingleton<IStatusEndpointDependency>(new StubStatusEndpointDependency { Status = status });
      }

      [Test]
      public void should_return_status_code_bad_gateway()
      {
         Assert.AreEqual(HttpStatusCode.BadGateway, _response.StatusCode);
      }

      [Test]
      public async Task should_return_exact_json_content()
      {
         var content = await _response.Content.ReadAsStringAsync();

         Assert.AreEqual("{\"name\":\"myName\",\"availability\":\"Limited\",\"host\":\"http://localhost\",\"dependencies\":[{\"name\":\"myDependency\",\"availability\":\"Unavailable\"}]}", content);
      }
   }
}
