﻿using Bivouac.Events;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Shouldly;

namespace Bivouac.Tests.IdentifyingScenarios
{
   public class ids_do_not_exist : ScenarioBase
   {
      private JObject _ids;

      [OneTimeSetUp]
      public void setup_scenario()
      {
         StubGuidGenerator.Add(RequestId, CorrelationId);

         var response = TestHost.Get("/get-ids-from-context");
         var content = response.Content.ReadAsStringAsync().Result;
         _ids = JObject.Parse(content);
      }

      [Test]
      public void returns_ids_from_context()
      {
         _ids["requestId"].Value<string>().ShouldBe(RequestId.ToString());
         _ids["correlationId"].Value<string>().ShouldBe(CorrelationId.ToString());
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
