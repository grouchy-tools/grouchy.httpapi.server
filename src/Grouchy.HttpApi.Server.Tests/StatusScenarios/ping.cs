using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.HttpApi.Server.Tests.StatusScenarios
{
   // ReSharper disable once InconsistentNaming
   public class ping : ScenarioBase
   {
      private HttpResponseMessage _response;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         _response = await TestHost.GetAsync("/.ping");
      }

      [Test]
      public void should_return_status_code_200()
      {
         Assert.AreEqual(_response.StatusCode, HttpStatusCode.OK);
      }

      [Test]
      public void should_return_json_content_type()
      {
         _response.Content.Headers.TryGetValues("Content-Type", out var values).ShouldBe(true);
         values.Single().ShouldBe("text/plain");
      }

      [Test]
      public async Task should_return_content_pong()
      {
         var content = await _response.Content.ReadAsStringAsync();

         Assert.AreEqual("Pong!", content);
      }

      [Test]
      public void should_not_invoke_server_logging()
      {
         Assert.AreEqual(0, StubHttpServerEventCallback.Events.Count);
      }
   }
}
