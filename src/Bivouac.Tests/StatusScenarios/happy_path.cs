using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Bivouac.Model;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Bivouac.Tests.StatusScenarios
{
   public class happy_path : ScenarioBase
   {
      private HttpResponseMessage _response;

      [OneTimeSetUp]
      public void setup_scenario()
      {
         StubServiceNameGetter.Name = "myName";
         StubServiceVersionGetter.Version = "myVersion";

         _response = TestHost.Get("/status");
      }

      [Test]
      public void should_return_status_code_okay()
      {
         Assert.AreEqual(200, (int)_response.StatusCode);
      }

      [Test]
      public void should_return_json_content()
      {
         var content = _response.Content.ReadAsStringAsync().Result;
         var status = JsonConvert.DeserializeObject<Status>(content);

         Assert.AreEqual("myName", status.Name);
         Assert.AreEqual(Availability.Available, status.Availability);
         Assert.AreEqual("myVersion", status.Version);
         Assert.AreEqual("http://localhost", status.Host);
      }

      [Test]
      public void should_return_json_content_type()
      {
         IEnumerable<string> values;
         _response.Content.Headers.TryGetValues("Content-Type", out values).ShouldBe(true);
         values.Single().ShouldBe("application/json");
      }

      [Test]
      public void should_return_exact_json_content()
      {
         var content = _response.Content.ReadAsStringAsync().Result;

         Assert.AreEqual("{\"name\":\"myName\",\"availability\":\"Available\",\"version\":\"myVersion\",\"host\":\"http://localhost\"}", content);
      }
   }
}
