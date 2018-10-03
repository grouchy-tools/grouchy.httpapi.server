using System.Net.Http;
using NUnit.Framework;

namespace Bivouac.Tests.StatusScenarios
{
   public class passthrough_to_next_middleware : ScenarioBase
   {
      private HttpResponseMessage _response;

      [OneTimeSetUp]
      public void setup_scenario()
      {
         _response = TestHost.Get("/another-endpoint");
      }

      [Test]
      public void should_return_status_code_from_another_endpoint()
      {
         Assert.AreEqual(201, (int)_response.StatusCode);
      }

      [Test]
      public void should_return_content_from_another_middleware()
      {
         var content = _response.Content.ReadAsStringAsync().Result;

         Assert.AreEqual("another-response", content);
      }
   }
}
