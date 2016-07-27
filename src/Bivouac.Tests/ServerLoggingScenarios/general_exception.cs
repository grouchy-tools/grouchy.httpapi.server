namespace Bivouac.Tests.ServerLoggingScenarios
{
   using System.Net.Http;
   using Newtonsoft.Json;
   using Xunit;

   public class general_exception : IClassFixture<general_exception.fixture>
   {
      public class fixture : ServerLoggingFixture
      {
         public readonly HttpResponseMessage Response;

         public fixture()
         {
            Response = TestHost.Get("/exception");
         }
      }

      private readonly fixture _fixture;

      public general_exception(fixture fixture)
      {
         _fixture = fixture;
      }

      [Fact]
      public void should_return_status_code_500()
      {
         Assert.Equal(500, (int)_fixture.Response.StatusCode);
      }

      [Fact]
      public void should_return_content()
      {
         var content = _fixture.Response.Content.ReadAsStringAsync().Result;

         Assert.Equal("FAIL!", content);
      }

      [Fact]
      public void should_return_content_type_text_plain()
      {
         var contentType = _fixture.Response.Content.Headers.ContentType.MediaType;

         Assert.Equal("text/plain", contentType);
      }

      [Fact]
      public void should_log_three_server_events()
      {
         Assert.Equal(3, _fixture.StubEventLogger.LoggedEvents.Count);
      }

      [Fact]
      public void should_log_server_request()
      {
         var json = JsonConvert.SerializeObject(new { eventType = "serverRequest", requestId = _fixture.RequestId, correlationId = _fixture.CorrelationId, uri = "/exception" });

         Assert.Equal(json, _fixture.StubEventLogger.LoggedEvents[0]);
      }

      [Fact]
      public void should_log_server_error()
      {
         var json = JsonConvert.SerializeObject(new { eventType = "serverError", requestId = _fixture.RequestId, correlationId = _fixture.CorrelationId, uri = "/exception", exceptionType = "Bivouac.Tests.ServerLoggingScenarios.ServerLoggingFixture+CustomException", message = "Custom exception message" });

         Assert.Equal(json, _fixture.StubEventLogger.LoggedEvents[1]);
      }

      [Fact]
      public void should_log_server_response()
      {
         var log = JsonConvert.DeserializeAnonymousType(_fixture.StubEventLogger.LoggedEvents[2], new { eventType = "", requestId = "", correlationId = "", uri = "", duration = 0, statusCode = 0 });

         Assert.Equal("serverResponse", log.eventType);
         Assert.Equal(_fixture.RequestId.ToString(), log.requestId);
         Assert.Equal(_fixture.CorrelationId.ToString(), log.correlationId);
         Assert.Equal("/exception", log.uri);
         Assert.InRange(log.duration, 0, int.MaxValue);
         Assert.Equal(500, log.statusCode);
      }
   }
}
