using System.Net.Http;
using System.Threading.Tasks;
using Grouchy.HttpApi.Server.Abstractions;
using Grouchy.HttpApi.Server.Model;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Grouchy.HttpApi.Server.Tests.StatusScenarios.stubbed_dependencies
{
   // ReSharper disable once InconsistentNaming
   public class dependency : ScenarioBase
   {
      private HttpResponseMessage _response;
      private string _content;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         _response = await TestHost.GetAsync("/.status");
         _content = await _response.Content.ReadAsStringAsync();
      }

      protected override void ConfigureServices(IServiceCollection services)
      {
         var status = new Dependency
         {
            Name = "level1",
            Availability = Availability.Available,
            Version = "version1"
         };

         services.AddSingleton<IStatusEndpointDependency>(new StubStatusEndpointDependency { Dependency = status });
      }
      
      [Test]
      public void should_return_status_code_okay()
      {
         Assert.AreEqual(200, (int)_response.StatusCode);
      }

      [Test]
      public void should_return_json_content_with_dependency()
      {
         var status = JsonConvert.DeserializeObject<Status>(_content);

         Assert.AreEqual(Availability.Available, status.Availability);
         Assert.AreEqual(1, status.Dependencies.Length);
      }

      [Test]
      public void should_return_level_1_dependency()
      {
         var status = JsonConvert.DeserializeObject<Status>(_content);
         var level1 = status.Dependencies[0];

         Assert.AreEqual("level1", level1.Name);
         Assert.AreEqual(Availability.Available, level1.Availability);
         Assert.AreEqual("version1", level1.Version);
      }
   }
}
