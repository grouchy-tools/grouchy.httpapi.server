using System.Net.Http;
using System.Threading.Tasks;
using Bivouac.Events;
using NUnit.Framework;
using Shouldly;

namespace Bivouac.Tests.ServerLoggingScenarios
{
   // ReSharper disable once InconsistentNaming
   public class http_not_found_exception : ScenarioBase
   {
      private HttpResponseMessage _response;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {         
         _response = await TestHost.GetAsync("/not-found-exception");
      }

      [Test]
      public void should_return_status_code_not_found()
      {
         Assert.AreEqual(404, (int)_response.StatusCode);
      }

      [Test]
      public async Task should_return_content_not_found()
      {
         var content = await _response.Content.ReadAsStringAsync();

         Assert.AreEqual(content, "Thing not found");
      }

      [Test]
      public void should_return_content_type_text_plain()
      {
         var contentType = _response.Content.Headers.ContentType.MediaType;

         Assert.AreEqual(contentType, "text/plain");
      }

      [Test]
      public void should_log_two_server_events()
      {
         Assert.AreEqual(2, StubHttpServerEventCallback.Events.Count);
      }
      
      [Test]
      public void should_log_server_request()
      {
         StubHttpServerEventCallback.Events[0].ShouldBeOfType<HttpServerRequest>();
      }

      [Test]
      public void should_log_server_request_with_content()
      {
         var @event = StubHttpServerEventCallback.Events[0];

         @event.EventType.ShouldBe("HttpServerRequest");
         @event.Uri.ShouldBe("/not-found-exception");
         @event.Method.ShouldBe("GET");
         @event.Tags.ShouldContainKeyAndValue("request-id", RequestId);
         @event.Tags.ShouldContainKeyAndValue("correlation-id", CorrelationId);
      }

      [Test]
      public void should_log_server_response()
      {
         StubHttpServerEventCallback.Events[1].ShouldBeOfType<HttpServerResponse>();
      }

      [Test]
      public void should_log_server_response_with_content()
      {
         var @event = (HttpServerResponse)StubHttpServerEventCallback.Events[1];

         @event.EventType.ShouldBe("HttpServerResponse");
         @event.Uri.ShouldBe("/not-found-exception");
         @event.Method.ShouldBe("GET");
         @event.Tags.ShouldContainKeyAndValue("request-id", RequestId);
         @event.Tags.ShouldContainKeyAndValue("correlation-id", CorrelationId);
         @event.DurationMs.ShouldBeInRange(0, int.MaxValue);
         @event.StatusCode.ShouldBe(404);
      }
   }
}
