using System.Net.Http;
using NUnit.Framework;

namespace Bivouac.Tests.StatusScenarios
{
   public class without_version_and_build : ScenarioBase
   {
      private HttpResponseMessage _response;

      [OneTimeSetUp]
      public void setup_scenario()
      {
         _response = TestHost.Get("/status");
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

         Assert.AreEqual("{\"name\":null,\"availability\":\"Available\",\"host\":\"http://localhost\"}", content);
      }
   }
}
