using System;
using System.Threading;
using System.Threading.Tasks;
using Grouchy.HttpApi.Server.Abstractions.Model;
using NUnit.Framework;
using Grouchy.HttpApi.Server.Services;

namespace Grouchy.HttpApi.Server.Tests.StatusScenarios.api_dependencies
{
   // ReSharper disable once InconsistentNaming
   public class request_is_cancelled : ScenarioBase
   {
      private Dependency _result;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         var httpClient = new StubHttpClient<Status>
         {
            BaseAddress = new Uri("http://stubbaseaddress"),
            Latency = TimeSpan.FromSeconds(3)
         };

         var testSubject = new HttpApiStatusEndpointDependency(httpClient, new HttpApiConfiguration { Name = "expectedDependencyName"});

         _result = await testSubject.GetStatusAsync(new CancellationTokenSource(20).Token);
      }

      [Test]
      public void should_return_instance()
      {
         Assert.That(_result, Is.Not.Null);
      }

      [Test]
      public void should_return_name()
      {
         Assert.AreEqual("expectedDependencyName", _result.Name);
      }

      [Test]
      public void should_return_unknown()
      {
         Assert.AreEqual(Availability.Unknown, _result.Availability);
      }
   }
}
