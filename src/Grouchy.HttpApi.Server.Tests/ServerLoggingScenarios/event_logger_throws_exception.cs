using System;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Grouchy.HttpApi.Server.Tests.ServerLoggingScenarios
{
   // ReSharper disable once InconsistentNaming
   public class event_logger_throws_exception : ScenarioBase
   {
      private HttpResponseMessage _response;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {         
         StubHttpServerEventCallback.Exception = new Exception();

         _response = await TestHost.GetAsync("/exception");
      }

      [Test]
      public void should_return_status_code_500()
      {
         Assert.AreEqual(500, (int)_response.StatusCode);
      }

      [Test]
      public async Task should_return_content()
      {
         var content = await _response.Content.ReadAsStringAsync();

         Assert.AreEqual("FAIL!", content);
      }

      [Test]
      public void should_return_content_type_text_plain()
      {
         var contentType = _response.Content.Headers.ContentType.MediaType;

         Assert.AreEqual("text/plain", contentType);
      }
   }
}
