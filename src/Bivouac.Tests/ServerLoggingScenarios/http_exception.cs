namespace Bivouac.Tests.ServerLoggingScenarios
{
   using System.Net.Http;
   using Bivouac.Events;
   using Xunit;
   using Shouldly;

   public class http_found_exception : IClassFixture<http_found_exception.fixture>
   {
      public class fixture : ServerLoggingFixture
      {
         public readonly HttpResponseMessage Response;

         public fixture()
         {
            Response = TestHost.Get("/http-exception");
         }
      }

      private readonly fixture _fixture;

      public http_found_exception(fixture fixture)
      {
         _fixture = fixture;
      }

      [Fact]
      public void should_return_status_code()
      {
         Assert.Equal(406, (int)_fixture.Response.StatusCode);
      }

      [Fact]
      public void should_return_content_not_found()
      {
         var content = _fixture.Response.Content.ReadAsStringAsync().Result;

         Assert.Equal(content, "Some http exception");
      }

      [Fact]
      public void should_return_content_type_text_plain()
      {
         var contentType = _fixture.Response.Content.Headers.ContentType.MediaType;

         Assert.Equal(contentType, "text/plain");
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
         var @event = (HttpServerRequest)_fixture.StubHttpServerEventCallback.Events[0];

         @event.EventType.ShouldBe("HttpServerRequest");
         @event.Uri.ShouldBe("/http-exception");
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
         @event.Uri.ShouldBe("/http-exception");
         @event.Method.ShouldBe("GET");
         @event.Tags.ShouldContainKeyAndValue("request-id", _fixture.RequestId);
         @event.Tags.ShouldContainKeyAndValue("correlation-id", _fixture.CorrelationId);
         @event.DurationMs.ShouldBeInRange(0, int.MaxValue);
         @event.StatusCode.ShouldBe(406);
      }
   }
}
