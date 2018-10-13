﻿using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bivouac.Model;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Bivouac.Tests.StatusScenarios
{
   // ReSharper disable once InconsistentNaming
   public class happy_path : ScenarioBase
   {
      private HttpResponseMessage _response;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         StubServiceNameGetter.Name = "myName";
         StubServiceVersionGetter.Version = "myVersion";

         _response = await TestHost.GetAsync("/.status");
      }

      [Test]
      public void should_return_status_code_okay()
      {
         Assert.AreEqual(200, (int)_response.StatusCode);
      }

      [Test]
      public async Task should_return_json_content()
      {
         var content = await _response.Content.ReadAsStringAsync();
         var status = JsonConvert.DeserializeObject<Status>(content);

         Assert.AreEqual("myName", status.Name);
         Assert.AreEqual(Availability.Available, status.Availability);
         Assert.AreEqual("myVersion", status.Version);
         Assert.AreEqual("http://localhost", status.Host);
      }

      [Test]
      public void should_return_json_content_type()
      {
         _response.Content.Headers.TryGetValues("Content-Type", out var values).ShouldBe(true);
         values.Single().ShouldBe("application/json");
      }

      [Test]
      public async Task should_return_exact_json_content()
      {
         var content = await _response.Content.ReadAsStringAsync();

         Assert.AreEqual("{\"name\":\"myName\",\"availability\":\"Available\",\"version\":\"myVersion\",\"host\":\"http://localhost\"}", content);
      }
   }
}
