using System.Net.Http;
using System.Threading.Tasks;
using Bivouac.Abstractions;
using Bivouac.Model;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Bivouac.Tests.StatusScenarios.stubbed_dependencies
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
         var status = new Status
         {
            Name = "level1",
            Availability = Availability.Available,
            Version = "version1",
            Dependencies = new[]
            {
               new Status
               {
                  Name="level2",
                  Version= "version2",
                  Availability = Availability.Available
               }
            }
         };

         services.AddSingleton<IStatusEndpointDependency>(new StubStatusEndpointDependency { Status = status });
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
         Assert.AreEqual(1, level1.Dependencies.Length);
      }

      [Test]
      public void should_return_level_2_dependency()
      {
         var status = JsonConvert.DeserializeObject<Status>(_content);
         var level2 = status.Dependencies[0].Dependencies[0];

         Assert.AreEqual("level2", level2.Name);
         Assert.AreEqual(Availability.Available, level2.Availability);
         Assert.AreEqual("version2", level2.Version);
         Assert.Null(level2.Dependencies);
      }
   }
}
