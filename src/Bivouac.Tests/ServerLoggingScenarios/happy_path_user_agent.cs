﻿namespace Bivouac.Tests.ServerLoggingScenarios
{
   using System.Collections.Generic;
   using System.Net.Http;
   using System.Net.Http.Headers;
   using Microsoft.Net.Http.Headers;
   using Newtonsoft.Json;
   using Xunit;

   public class happy_path_user_agent : IClassFixture<happy_path_user_agent.fixture>
   {
      public class fixture : ServerLoggingFixture
      {
         public readonly HttpResponseMessage Response;

         public fixture()
         {
            Response = TestHost.Get("/happy-path", new Dictionary<string, string> { { HeaderNames.UserAgent, "myUserAgent" } });
         }
      }

      private readonly fixture _fixture;

      public happy_path_user_agent(fixture fixture)
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
         var json = JsonConvert.SerializeObject(new { eventType = "serverRequest", requestId = _fixture.RequestId, correlationId = _fixture.CorrelationId, method = "GET", uri = "/happy-path", userAgent = "myUserAgent" });

         Assert.Equal(json, _fixture.StubEventLogger.LoggedEvents[0]);
      }

      [Fact]
      public void should_log_server_response()
      {
         var log = JsonConvert.DeserializeAnonymousType(_fixture.StubEventLogger.LoggedEvents[1], new { eventType = "", requestId = "", correlationId = "", method = "", uri = "", userAgent = "", duration = 0, statusCode = 0 });

         Assert.Equal("serverResponse", log.eventType);
         Assert.Equal(_fixture.RequestId.ToString(), log.requestId);
         Assert.Equal(_fixture.CorrelationId.ToString(), log.correlationId);
         Assert.Equal("GET", log.method);
         Assert.Equal("/happy-path", log.uri);
         Assert.Null(log.userAgent);
         Assert.InRange(log.duration, 0, int.MaxValue);
         Assert.Equal(200, log.statusCode);
      }
   }
}