namespace Bivouac.Tests.ServerLoggingScenarios
{
   using System;
   using System.Net.Http;
   using Newtonsoft.Json;
   using Xunit;

   public class correlation_id_getter_throws_exception : IClassFixture<correlation_id_getter_throws_exception.fixture>
   {
      public class fixture : ServerLoggingFixture
      {
         public readonly HttpResponseMessage Response;

         public fixture()
         {
            StubCorrelationIdGetter.Exception = new Exception("Problem with CorrelationIdGetter");

            Response = TestHost.Get("/happy-path");
         }
      }

      private readonly fixture _fixture;

      public correlation_id_getter_throws_exception(fixture fixture)
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
         var json = JsonConvert.SerializeObject(new { eventType = "serverRequest", requestId = _fixture.RequestId, correlationId = Guid.Empty, method = "GET", uri = "/happy-path" });

         Assert.Equal(json, _fixture.StubEventLogger.LoggedEvents[0]);
      }

      [Fact]
      public void should_log_server_response()
      {
         var log = JsonConvert.DeserializeAnonymousType(_fixture.StubEventLogger.LoggedEvents[1], new { eventType = "", requestId = "", correlationId = "", method = "", uri = "", duration = 0, statusCode = 0 });

         Assert.Equal("serverResponse", log.eventType);
         Assert.Equal(_fixture.RequestId.ToString(), log.requestId);
         Assert.Equal(Guid.Empty.ToString(), log.correlationId);
         Assert.Equal("GET", log.method);
         Assert.Equal("/happy-path", log.uri);
         Assert.InRange(log.duration, 0, int.MaxValue);
         Assert.Equal(200, log.statusCode);
      }
   }
}
