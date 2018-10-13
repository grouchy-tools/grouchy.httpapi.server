using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Bivouac.Tests.StatusScenarios
{
   // ReSharper disable once InconsistentNaming
   public class passthrough_to_next_middleware : ScenarioBase
   {
      private HttpResponseMessage _response;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         _response = await TestHost.GetAsync("/another-endpoint");
      }

      [Test]
      public void should_return_status_code_from_another_endpoint()
      {
         Assert.AreEqual(201, (int)_response.StatusCode);
      }

      [Test]
      public async Task should_return_content_from_another_middleware()
      {
         var content = await _response.Content.ReadAsStringAsync();

         Assert.AreEqual("another-response", content);
      }
   }
}
