namespace Bivouac.Tests.IdentifyingScenarios
{
   using System;
   using System.Net.Http;
   using Newtonsoft.Json;
   using Newtonsoft.Json.Linq;
   using Xunit;

   public class ids_does_not_exist : IClassFixture<ids_does_not_exist.fixture>
   {
      public class fixture : IdentifyingFixture
      {
         public readonly Guid RequestId;
         public readonly Guid CorrelationId;

         public readonly JObject IdsFromContext;

         public fixture()
         {
            RequestId = Guid.NewGuid();
            CorrelationId = Guid.NewGuid();

            StubGuidGenerator.Add(RequestId, CorrelationId);

            var response = TestHost.Get("/get-ids-from-context");
            IdsFromContext = JObject.Parse(response.Content.ReadAsStringAsync().Result);
         }
      }

      private readonly fixture _fixture;

      public ids_does_not_exist(fixture fixture)
      {
         _fixture = fixture;
      }

      [Fact]
      public void returns_new_ids_from_context()
      {
         Assert.Equal(_fixture.RequestId.ToString(), _fixture.IdsFromContext["requestId"].Value<string>());
         Assert.Equal(_fixture.CorrelationId.ToString(), _fixture.IdsFromContext["correlationId"].Value<string>());
      }

      [Fact]
      public void should_log_server_request()
      {
         var json = JsonConvert.SerializeObject(new { eventType = "serverRequest", requestId = _fixture.RequestId, correlationId = _fixture.CorrelationId, method="GET", uri = "/get-ids-from-context" });

         Assert.Equal(json, _fixture.StubEventLogger.LoggedEvents[0]);
      }
   }
}
