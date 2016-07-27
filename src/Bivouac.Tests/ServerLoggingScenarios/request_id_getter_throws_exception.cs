namespace Bivouac.Tests.ServerLoggingScenarios
{
   using System;
   using System.Net.Http;
   using Newtonsoft.Json;
   using Xunit;

   public class request_id_getter_throws_exception : IClassFixture<request_id_getter_throws_exception.fixture>
   {
      public class fixture : ServerLoggingFixture
      {
         public readonly HttpResponseMessage Response;

         public fixture()
         {
            StubRequestIdGetter.Exception = new Exception("Problem with RequestIdGetter");

            Response = TestHost.Get("/happy-path");
         }
      }

      private readonly fixture _fixture;

      public request_id_getter_throws_exception(fixture fixture)
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
         Assert.Equal(2, _fixture.StubEventLogger.LoggedEvents.Count);
      }
      
      [Fact]
      public void should_log_server_request()
      {
         var json = JsonConvert.SerializeObject(new { eventType = "serverRequest", requestId = Guid.Empty, correlationId = _fixture.CorrelationId, uri = "/happy-path" });

         Assert.Equal(json, _fixture.StubEventLogger.LoggedEvents[0]);
      }

      [Fact]
      public void should_log_server_response()
      {
         var log = JsonConvert.DeserializeAnonymousType(_fixture.StubEventLogger.LoggedEvents[1], new { eventType = "", requestId = "", correlationId = "", uri = "", duration = 0, statusCode = 0 });

         Assert.Equal("serverResponse", log.eventType);
         Assert.Equal(Guid.Empty.ToString(), log.requestId);
         Assert.Equal(_fixture.CorrelationId.ToString(), log.correlationId);
         Assert.Equal("/happy-path", log.uri);
         Assert.InRange(log.duration, 0, int.MaxValue);
         Assert.Equal(200, log.statusCode);
      }
   }
}
