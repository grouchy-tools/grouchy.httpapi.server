﻿namespace Bivouac.Tests.IdentifyingScenarios
{
   using System;
   using System.Collections.Generic;
   using Bivouac.Events;
   using Microsoft.AspNetCore.Builder;
   using Newtonsoft.Json.Linq;
   using Xunit;
   using Shouldly;

   public class ids_exists_in_context_and_header : IClassFixture<ids_exists_in_context_and_header.fixture>
   {
      public class fixture : IdentifyingFixture
      {
         public readonly JObject IdentifyingIds;

         public fixture() : base(Configure)
         {
            var headers = new Dictionary<string, string>
            {
               { "request-id", Guid.NewGuid().ToString() },
               { "correlation-id", Guid.NewGuid().ToString() }
            };

            var response = TestHost.Get("/get-ids-from-context", headers);
            var content = response.Content.ReadAsStringAsync().Result;
            IdentifyingIds = JObject.Parse(content);
         }

         private static void Configure(IApplicationBuilder builder, Guid requestId, Guid correlationId)
         {
            // Inject the request-id and correlation-id into the pipeline
            builder.Use(async (context, next) =>
            {
               context.Items.Add("request-id", requestId.ToString());
               context.Items.Add("correlation-id", correlationId.ToString());

               await next();
            });
         }
      }

      private readonly fixture _fixture;

      public ids_exists_in_context_and_header(fixture fixture)
      {
         _fixture = fixture;
      }

      [Fact]
      public void returns_ids_from_context()
      {
         _fixture.IdentifyingIds["requestId"].Value<string>().ShouldBe(_fixture.RequestId.ToString());
         _fixture.IdentifyingIds["correlationId"].Value<string>().ShouldBe(_fixture.CorrelationId.ToString());
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
