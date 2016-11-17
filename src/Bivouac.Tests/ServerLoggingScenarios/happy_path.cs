namespace Bivouac.Tests.ServerLoggingScenarios
{
   using System.Net.Http;
   using Bivouac.Events;
   using Xunit;
   using Shouldly;

   public class happy_path : IClassFixture<happy_path.fixture>
   {
      public class fixture : ServerLoggingFixture
      {
         public readonly HttpResponseMessage Response;

         public fixture()
         {
            Response = TestHost.Get("/happy-path");
         }
      }

      private readonly fixture _fixture;

      public happy_path(fixture fixture)
      {
         _fixture = fixture;
      }

      [Fact]
      public void should_return_status_code_from_next_middleware()
      {
         Assert.Equal(200, (int)_fixture.Response.StatusCode);
      }

      [Fact]
      public void should_return_content_from_next_middleware()
      {
         var content = _fixture.Response.Content.ReadAsStringAsync().Result;

         Assert.Equal(content, "Complete!");
      }

      [Fact]
      public void should_log_two_server_events()
      {
         Assert.Equal(2, _fixture.StubHttpServerEventCallback.Events.Count);
      }
      
      [Fact]
      public void should_log_server_request()
      {
         _fixture.StubHttpServerEventCallback.Events[0].ShouldBeOfType<HttpServerRequest>();
      }

      [Fact]
      public void should_log_server_request_with_content()
      {
         var @event = _fixture.StubHttpServerEventCallback.Events[0];

         @event.EventType.ShouldBe("HttpServerRequest");
         @event.Uri.ShouldBe("/happy-path");
         @event.Method.ShouldBe("GET");
         @event.Tags.ShouldContainKeyAndValue("request-id", _fixture.RequestId);
         @event.Tags.ShouldContainKeyAndValue("correlation-id", _fixture.CorrelationId);
      }

      [Fact]
      public void should_log_server_response()
      {
         _fixture.StubHttpServerEventCallback.Events[1].ShouldBeOfType<HttpServerResponse>();
      }

      [Fact]
      public void should_log_server_response_with_content()
      {
         var @event = (HttpServerResponse)_fixture.StubHttpServerEventCallback.Events[1];

         @event.EventType.ShouldBe("HttpServerResponse");
         @event.Uri.ShouldBe("/happy-path");
         @event.Method.ShouldBe("GET");
         @event.Tags.ShouldContainKeyAndValue("request-id", _fixture.RequestId);
         @event.Tags.ShouldContainKeyAndValue("correlation-id", _fixture.CorrelationId);
         @event.DurationMs.ShouldBeInRange(0, int.MaxValue);
         @event.StatusCode.ShouldBe(200);
      }
   }
}
