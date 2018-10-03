using System.Diagnostics;
using System.Net.Http;
using Bivouac.Abstractions;
using Bivouac.Model;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Bivouac.Tests.StatusScenarios.stubbed_dependencies
{
   public class multiple_dependencies_with_timeout : ScenarioBase
   {
      private HttpResponseMessage _response;
      private long _duration;

      [OneTimeSetUp]
      public void setup_scenario()
      {
         var stopwatch = Stopwatch.StartNew();

         _response = TestHost.Get("/status");

         stopwatch.Stop();
         _duration = stopwatch.ElapsedMilliseconds;
      }

      protected override void ConfigureServices(IServiceCollection services)
      {
         var status1 = new Status { Name = "myDep1", Availability = Availability.Limited };
         var status2 = new Status { Name = "myDep2", Availability = Availability.Unknown };

         services.AddSingleton<IStatusEndpointDependency>(new StubStatusEndpointDependency  { Name = "myDep1", Status = status1, DelayMs = 5000 });
         services.AddSingleton<IStatusEndpointDependency>(new StubStatusEndpointDependency { Name = "myDep2", Status = status2 });
      }

      [Test]
      public void should_return_status_code_okay()
      {
         Assert.AreEqual(200, (int)_response.StatusCode);
      }

      [Test]
      public void should_return_exact_json_content()
      {
         var content = _response.Content.ReadAsStringAsync().Result;

         // Dependency that is timed-out will be status Unknown
         Assert.AreEqual("{\"name\":null,\"availability\":\"Unknown\",\"host\":\"http://localhost\",\"dependencies\":[{\"name\":\"myDep1\",\"availability\":\"Unknown\"},{\"name\":\"myDep2\",\"availability\":\"Unknown\"}]}", content);
      }

      [Test]
      public void duration_should_be_around_three_seconds()
      {
         Assert.That(_duration, Is.InRange(2800, 3400));
      }
   }
}
