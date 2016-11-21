namespace Bivouac.Tests.IdentifyingScenarios
{
   using Bivouac.Events;
   using Newtonsoft.Json.Linq;
   using Xunit;
   using Shouldly;

   public class ids_do_not_exist : IClassFixture<ids_do_not_exist.fixture>
   {
      public class fixture : IdentifyingFixture
      {
         public readonly JObject Ids;

         public fixture()
         {
            StubGuidGenerator.Add(RequestId, CorrelationId);

            var response = TestHost.Get("/get-ids-from-context");
            var content = response.Content.ReadAsStringAsync().Result;
            Ids = JObject.Parse(content);
         }
      }

      private readonly fixture _fixture;

      public ids_do_not_exist(fixture fixture)
      {
         _fixture = fixture;
      }

      [Fact]
      public void returns_ids_from_context()
      {
         _fixture.Ids["requestId"].Value<string>().ShouldBe(_fixture.RequestId.ToString());
         _fixture.Ids["correlationId"].Value<string>().ShouldBe(_fixture.CorrelationId.ToString());
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
