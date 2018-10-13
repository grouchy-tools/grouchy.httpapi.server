using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Bivouac.Tests.StatusScenarios
{
   // ReSharper disable once InconsistentNaming
   public class without_version_and_build : ScenarioBase
   {
      private HttpResponseMessage _response;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         _response = await TestHost.GetAsync("/.status");
      }

      [Test]
      public void should_return_status_code_okay()
      {
         Assert.AreEqual(200, (int)_response.StatusCode);
      }

      [Test]
      public async Task should_return_exact_json_content()
      {
         var content = await _response.Content.ReadAsStringAsync();

         Assert.AreEqual("{\"name\":null,\"availability\":\"Available\",\"host\":\"http://localhost\"}", content);
      }
   }
}
