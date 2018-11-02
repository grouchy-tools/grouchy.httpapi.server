using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grouchy.HttpApi.Server.Events;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.HttpApi.Server.Tests.ServerLoggingScenarios
{
   // ReSharper disable once InconsistentNaming
   public class request_id_getter_throws_exception : ScenarioBase
   {
      private HttpResponseMessage _response;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {         
         StubRequestIdGetter.Exception = new Exception("Problem with RequestIdGetter");

         _response = await TestHost.GetAsync("/happy-path");
      }

      [Test]
      public void should_return_status_code_from_next_middleware()
      {
         Assert.AreEqual(200, (int)_response.StatusCode);
      }

      [Test]
      public async Task should_return_content_from_next_middleware()
      {
         var content = await _response.Content.ReadAsStringAsync();

         Assert.AreEqual(content, "Complete!");
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
         var @event = (HttpServerRequest)StubHttpServerEventCallback.Events[0];

         @event.EventType.ShouldBe("HttpServerRequest");
         @event.Uri.ShouldBe("/happy-path");
         @event.Method.ShouldBe("GET");
         @event.Tags.ShouldNotContainKey("request-id");
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
         @event.Uri.ShouldBe("/happy-path");
         @event.Method.ShouldBe("GET");
         @event.Tags.ShouldNotContainKey("request-id");
         @event.Tags.ShouldContainKeyAndValue("correlation-id", CorrelationId);
         @event.DurationMs.ShouldBeInRange(0, int.MaxValue);
         @event.StatusCode.ShouldBe(200);
      }
   }
}
