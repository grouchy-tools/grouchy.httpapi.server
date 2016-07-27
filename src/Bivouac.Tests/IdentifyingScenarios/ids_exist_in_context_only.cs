namespace Bivouac.Tests.IdentifyingScenarios
{
   using System;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Microsoft.AspNetCore.Builder;
   using Newtonsoft.Json;
   using Newtonsoft.Json.Linq;
   using Xunit;

   public class ids_in_context_only : IClassFixture<ids_in_context_only.fixture>
   {
      public class fixture : IdentifyingFixture
      {
         public static readonly Guid RequestId;
         public static readonly Guid CorrelationId;

         public readonly JObject IdsFromContext;

         static fixture()
         {
            RequestId = Guid.NewGuid();
            CorrelationId = Guid.NewGuid();
         }

         public fixture() : base(Configure)
         {
            var response = TestHost.Get("/get-ids-from-context");
            var content = response.Content.ReadAsStringAsync().Result;
            IdsFromContext = JObject.Parse(content);
         }

         private static void Configure(IApplicationBuilder builder)
         {
            // Inject the request-id and correlation-id into the pipeline
            builder.Use(async (context, next) =>
            {
               context.Items.Add("request-id", RequestId);
               context.Items.Add("correlation-id", CorrelationId);

               await next();
            });
         }
      }

      private readonly fixture _fixture;

      public ids_in_context_only(fixture fixture)
      {
         _fixture = fixture;
      }

      [Fact]
      public void returns_new_ids_from_context()
      {
         Assert.Equal(fixture.RequestId.ToString(), _fixture.IdsFromContext["requestId"].Value<string>());
         Assert.Equal(fixture.CorrelationId.ToString(), _fixture.IdsFromContext["correlationId"].Value<string>());
      }

      [Fact]
      public void should_log_server_request()
      {
         var json = JsonConvert.SerializeObject(new { eventType = "serverRequest", requestId = fixture.RequestId, correlationId = fixture.CorrelationId, uri = "/get-ids-from-context" });

         Assert.Equal(json, _fixture.StubEventLogger.LoggedEvents[0]);
      }
   }
}
