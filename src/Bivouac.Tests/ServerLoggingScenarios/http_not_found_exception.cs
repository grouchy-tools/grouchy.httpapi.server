namespace Bivouac.Tests.ServerLoggingScenarios
{
   using System.Net.Http;
   using Newtonsoft.Json;
   using Xunit;

   public class http_not_found_exception : IClassFixture<http_not_found_exception.fixture>
   {
      public class fixture : ServerLoggingFixture
      {
         public readonly HttpResponseMessage Response;

         public fixture()
         {
            Response = TestHost.Get("/not-found-exception");
         }
      }

      private readonly fixture _fixture;

      public http_not_found_exception(fixture fixture)
      {
         _fixture = fixture;
      }

      [Fact]
      public void should_return_status_code_not_found()
      {
         Assert.Equal(404, (int)_fixture.Response.StatusCode);
      }

      [Fact]
      public void should_return_content_not_found()
      {
         var content = _fixture.Response.Content.ReadAsStringAsync().Result;

         Assert.Equal(content, "Thing not found");
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
         Assert.Equal(2, _fixture.StubEventLogger.LoggedEvents.Count);
      }

      [Fact]
      public void should_log_server_request()
      {
         var json = JsonConvert.SerializeObject(new { eventType = "serverRequest", requestId = _fixture.RequestId, correlationId = _fixture.CorrelationId, uri = "/not-found-exception" });

         Assert.Equal(json, _fixture.StubEventLogger.LoggedEvents[0]);
      }

      [Fact]
      public void should_log_server_response()
      {
         var log = JsonConvert.DeserializeAnonymousType(_fixture.StubEventLogger.LoggedEvents[1], new { eventType = "", requestId = "", correlationId = "", uri = "", duration = 0, statusCode = 0 });

         Assert.Equal("serverResponse", log.eventType);
         Assert.Equal(_fixture.RequestId.ToString(), log.requestId);
         Assert.Equal(_fixture.CorrelationId.ToString(), log.correlationId);
         Assert.Equal("/not-found-exception", log.uri);
         Assert.InRange(log.duration, 0, int.MaxValue);
         Assert.Equal(404, log.statusCode);
      }
   }
}
