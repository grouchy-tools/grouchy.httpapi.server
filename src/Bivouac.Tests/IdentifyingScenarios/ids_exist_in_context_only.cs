namespace Bivouac.Tests.IdentifyingScenarios
{
   using System;
   using Bivouac.Events;
   using Microsoft.AspNetCore.Builder;
   using Newtonsoft.Json;
   using Newtonsoft.Json.Linq;
   using Xunit;
   using Shouldly;

   public class ids_in_context_only : IClassFixture<ids_in_context_only.fixture>
   {
      public class fixture : IdentifyingFixture
      {
         public readonly JObject IdsFromContext;

         public fixture() : base(Configure)
         {
            var response = TestHost.Get("/get-ids-from-context");
            var content = response.Content.ReadAsStringAsync().Result;
            IdsFromContext = JObject.Parse(content);
         }

         private static void Configure(IApplicationBuilder builder, Guid requestId, Guid correlationId)
         {
            // Inject the request-id and correlation-id into the pipeline
            builder.Use(async (context, next) =>
            {
               context.Items.Add("request-id", requestId);
               context.Items.Add("correlation-id", correlationId);

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
         @event.RequestId = _fixture.RequestId.ToString();
         @event.Tags.ShouldContainKeyAndValue("correlationId", _fixture.CorrelationId.ToString());
      }
   }
}
