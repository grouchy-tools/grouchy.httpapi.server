﻿using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Grouchy.HttpApi.Server.Abstractions;
using Grouchy.HttpApi.Server.Abstractions.Model;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Grouchy.HttpApi.Server.Tests.StatusScenarios.stubbed_dependencies
{
   // ReSharper disable once InconsistentNaming
   public class multiple_dependencies_with_timeout : ScenarioBase
   {
      private HttpResponseMessage _response;
      private long _duration;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         var stopwatch = Stopwatch.StartNew();

         _response = await TestHost.GetAsync("/.status");

         stopwatch.Stop();
         _duration = stopwatch.ElapsedMilliseconds;
      }

      protected override void ConfigureServices(IServiceCollection services)
      {
         var status1 = new Dependency { Name = "myDep1", Availability = Availability.Limited };
         var status2 = new Dependency { Name = "myDep2", Availability = Availability.Unknown };

         services.AddSingleton<IStatusEndpointDependency>(new StubStatusEndpointDependency  { Name = "myDep1", Dependency = status1, DelayMs = 5000 });
         services.AddSingleton<IStatusEndpointDependency>(new StubStatusEndpointDependency { Name = "myDep2", Dependency = status2 });
      }

      [Test]
      public void should_return_status_code_gateway_timeout()
      {
         Assert.AreEqual(HttpStatusCode.GatewayTimeout, _response.StatusCode);
      }

      [Test]
      public async Task should_return_exact_json_content()
      {
         var content = await _response.Content.ReadAsStringAsync();

         // Dependency that is timed-out will be status Unknown
         Assert.AreEqual("{\"name\":null,\"availability\":\"Unknown\",\"dependencies\":[{\"name\":\"myDep1\",\"availability\":\"Unknown\"},{\"name\":\"myDep2\",\"availability\":\"Unknown\"}]}", content);
      }

      [Test]
      public void duration_should_be_around_one_second()
      {
         Assert.That(_duration, Is.InRange(800, 1600));
      }
   }
}
