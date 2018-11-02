using System.Net.Http;
using System.Threading.Tasks;
using Grouchy.HttpApi.Server.Events;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.HttpApi.Server.Tests.ServerLoggingScenarios
{
   // ReSharper disable once InconsistentNaming
   public class general_exception : ScenarioBase
   {
      private HttpResponseMessage _response;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {         
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

      [Test]
      public void should_log_three_server_events()
      {
         Assert.AreEqual(3, StubHttpServerEventCallback.Events.Count);
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
         @event.Uri.ShouldBe("/exception");
         @event.Method.ShouldBe("GET");
         @event.Tags.ShouldContainKeyAndValue("request-id", RequestId);
         @event.Tags.ShouldContainKeyAndValue("correlation-id", CorrelationId);
      }

      [Test]
      public void should_log_server_exception()
      {
         StubHttpServerEventCallback.Events[1].ShouldBeOfType<HttpServerException>();
      }

      [Test]
      public void should_log_server_exception_with_content()
      {
         var @event = (HttpServerException)StubHttpServerEventCallback.Events[1];

         @event.EventType.ShouldBe("HttpServerException");
         @event.Uri.ShouldBe("/exception");
         @event.Method.ShouldBe("GET");
         @event.Exception.ShouldBeOfType<CustomException>();
         @event.Tags.ShouldContainKeyAndValue("request-id", RequestId);
         @event.Tags.ShouldContainKeyAndValue("correlation-id", CorrelationId);
      }

      [Test]
      public void should_log_server_response()
      {
         StubHttpServerEventCallback.Events[2].ShouldBeOfType<HttpServerResponse>();
      }

      [Test]
      public void should_log_server_response_with_content()
      {
         var @event = (HttpServerResponse)StubHttpServerEventCallback.Events[2];

         @event.EventType.ShouldBe("HttpServerResponse");
         @event.Uri.ShouldBe("/exception");
         @event.Method.ShouldBe("GET");
         @event.DurationMs.ShouldBeInRange(0, int.MaxValue);
         @event.StatusCode.ShouldBe(500);
         @event.Tags.ShouldContainKeyAndValue("request-id", RequestId);
         @event.Tags.ShouldContainKeyAndValue("correlation-id", CorrelationId);
      }
   }
}
