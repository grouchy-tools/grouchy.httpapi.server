﻿using System.Collections.Generic;
using Bivouac.Events;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Shouldly;

namespace Bivouac.Tests.IdentifyingScenarios
{

   public class ids_exists_in_header_only : ScenarioBase
   {
      private JObject _idsFromContext;

      [OneTimeSetUp]
      public void setup_scenario()
      {
         var headers = new Dictionary<string, string>
         {
            { "request-id", RequestId.ToString() },
            { "correlation-id", CorrelationId.ToString() }
         };

         var response = TestHost.Get("/get-ids-from-context", headers);
         var content = response.Content.ReadAsStringAsync().Result;
         _idsFromContext = JObject.Parse(content);
      }

      [Test]
      public void returns_ids_from_headers()
      {
         _idsFromContext["requestId"].Value<string>().ShouldBe(RequestId.ToString());
         _idsFromContext["correlationId"].Value<string>().ShouldBe(CorrelationId.ToString());
      }

      [Test]
      public void should_log_server_request()
      {
         StubHttpServerEventCallback.Events[0].ShouldBeOfType<HttpServerRequest>();
      }

      [Test]
      public void should_log_server_request_with_content()
      {
         var @event = (HttpServerRequest)StubHttpServerEventCallback.Events[0];

         @event.EventType.ShouldBe("HttpServerRequest");
         @event.Uri.ShouldBe("/get-ids-from-context");
         @event.Method.ShouldBe("GET");
         @event.Tags.ShouldContainKeyAndValue("request-id", RequestId.ToString());
         @event.Tags.ShouldContainKeyAndValue("correlation-id", CorrelationId.ToString());
      }
   }
}
