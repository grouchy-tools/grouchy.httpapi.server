namespace Bivouac.Tests.IdentifyingScenarios
{
   using System.Collections.Generic;
   using Bivouac.Events;
   using Newtonsoft.Json.Linq;
   using Xunit;
   using Shouldly;

   public class ids_exists_in_header_only : IClassFixture<ids_exists_in_header_only.fixture>
   {
      public class fixture : IdentifyingFixture
      {
         public readonly JObject IdsFromContext;

         public fixture()
         {
            var headers = new Dictionary<string, string>
            {
               { "request-id", RequestId.ToString() },
               { "correlation-id", CorrelationId.ToString() }
            };

            var response = TestHost.Get("/get-ids-from-context", headers);
            var content = response.Content.ReadAsStringAsync().Result;
            IdsFromContext = JObject.Parse(content);
         }
      }

      private readonly fixture _fixture;

      public ids_exists_in_header_only(fixture fixture)
      {
         _fixture = fixture;
      }

      [Fact]
      public void returns_ids_from_headers()
      {
         _fixture.IdsFromContext["requestId"].Value<string>().ShouldBe(_fixture.RequestId.ToString());
         _fixture.IdsFromContext["correlationId"].Value<string>().ShouldBe(_fixture.CorrelationId.ToString());
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
         @event.Uri.ShouldBe("/get-ids-from-context");
         @event.Method.ShouldBe("GET");
         @event.Tags.ShouldContainKeyAndValue("request-id", _fixture.RequestId.ToString());
         @event.Tags.ShouldContainKeyAndValue("correlation-id", _fixture.CorrelationId.ToString());
      }
   }
}
