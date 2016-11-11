namespace Bivouac.Tests.IdentifyingScenarios
{
   using Bivouac.Events;
   using Newtonsoft.Json.Linq;
   using Xunit;
   using Shouldly;

   public class ids_does_not_exist : IClassFixture<ids_does_not_exist.fixture>
   {
      public class fixture : IdentifyingFixture
      {
         public readonly JObject IdsFromContext;

         public fixture()
         {
            StubGuidGenerator.Add(RequestId, CorrelationId);

            var response = TestHost.Get("/get-ids-from-context");
            var content = response.Content.ReadAsStringAsync().Result;
            IdsFromContext = JObject.Parse(content);
         }
      }

      private readonly fixture _fixture;

      public ids_does_not_exist(fixture fixture)
      {
         _fixture = fixture;
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
         @event.RequestId.ShouldBe(_fixture.RequestId.ToString());
         @event.Tags.ShouldContainKeyAndValue("correlationId", _fixture.CorrelationId.ToString());
      }
   }
}
