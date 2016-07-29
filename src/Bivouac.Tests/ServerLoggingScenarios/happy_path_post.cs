namespace Bivouac.Tests.ServerLoggingScenarios
{
   using System.Net.Http;
   using Newtonsoft.Json;
   using Xunit;

   public class happy_path_post : IClassFixture<happy_path_post.fixture>
   {
      public class fixture : ServerLoggingFixture
      {
         public readonly HttpResponseMessage Response;

         public fixture()
         {
            Response = TestHost.Post("/happy-path", "{}");
         }
      }

      private readonly fixture _fixture;

      public happy_path_post(fixture fixture)
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

         Assert.Equal(content, "Complete Post!");
      }

      [Fact]
      public void should_log_two_server_events()
      {
         Assert.Equal(2, _fixture.StubEventLogger.LoggedEvents.Count);
      }

      [Fact]
      public void should_log_server_request()
      {
         var json = JsonConvert.SerializeObject(new { eventType = "serverRequest", requestId = _fixture.RequestId, correlationId = _fixture.CorrelationId, method="POST", uri = "/happy-path" });

         Assert.Equal(json, _fixture.StubEventLogger.LoggedEvents[0]);
      }

      [Fact]
      public void should_log_server_response()
      {
         var log = JsonConvert.DeserializeAnonymousType(_fixture.StubEventLogger.LoggedEvents[1], new { eventType = "", requestId = "", correlationId = "", method = "", uri = "", duration = 0, statusCode = 0 });

         Assert.Equal("serverResponse", log.eventType);
         Assert.Equal(_fixture.RequestId.ToString(), log.requestId);
         Assert.Equal(_fixture.CorrelationId.ToString(), log.correlationId);
         Assert.Equal("POST", log.method);
         Assert.Equal("/happy-path", log.uri);
         Assert.InRange(log.duration, 0, int.MaxValue);
         Assert.Equal(200, log.statusCode);
      }
   }
}
