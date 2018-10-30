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
   public class multiple_dependencies : ScenarioBase
   {
      private HttpResponseMessage _response;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         _response = await TestHost.GetAsync("/.status");
      }

      protected override void ConfigureServices(IServiceCollection services)
      {
         var status1 = new Dependency { Name = "myDep1", Availability = Availability.Limited };
         var status2 = new Dependency { Name = "myDep2", Availability = Availability.Unknown };

         services.AddSingleton<IStatusEndpointDependency>(new StubStatusEndpointDependency { Dependency = status1 });
         services.AddSingleton<IStatusEndpointDependency>(new StubStatusEndpointDependency { Dependency = status2 });
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

         Assert.AreEqual("{\"name\":null,\"availability\":\"Limited\",\"dependencies\":[{\"name\":\"myDep1\",\"availability\":\"Limited\"},{\"name\":\"myDep2\",\"availability\":\"Unknown\"}]}", content);
      }
   }
}
