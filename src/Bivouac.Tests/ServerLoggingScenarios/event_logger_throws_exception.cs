using System;
using System.Net.Http;
using NUnit.Framework;

namespace Bivouac.Tests.ServerLoggingScenarios
{
   public class event_logger_throws_exception : ScenarioBase
   {
      private HttpResponseMessage _response;

      [OneTimeSetUp]
      public void setup_scenario()
      {         
         StubHttpServerEventCallback.Exception = new Exception();

         _response = TestHost.Get("/exception");
      }

      [Test]
      public void should_return_status_code_500()
      {
         Assert.AreEqual(500, (int)_response.StatusCode);
      }

      [Test]
      public void should_return_content()
      {
         var content = _response.Content.ReadAsStringAsync().Result;

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
