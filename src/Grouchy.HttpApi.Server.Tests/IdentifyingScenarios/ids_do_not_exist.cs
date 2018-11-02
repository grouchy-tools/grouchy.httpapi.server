using System.Threading.Tasks;
using Grouchy.HttpApi.Server.Events;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.HttpApi.Server.Tests.IdentifyingScenarios
{
   // ReSharper disable once InconsistentNaming
   public class ids_do_not_exist : ScenarioBase
   {
      private JObject _ids;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         StubGuidGenerator.Add(RequestId, CorrelationId);

         var response = await TestHost.GetAsync("/get-ids-from-context");
         var content = await response.Content.ReadAsStringAsync();
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
