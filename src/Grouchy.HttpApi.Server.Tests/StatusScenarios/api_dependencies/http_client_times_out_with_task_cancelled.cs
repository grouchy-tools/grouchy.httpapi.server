using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Grouchy.HttpApi.Server.Model;
using Grouchy.HttpApi.Server.Services;

namespace Grouchy.HttpApi.Server.Tests.StatusScenarios.api_dependencies
{
   // ReSharper disable once InconsistentNaming
   public class http_client_times_out_with_task_cancelled : ScenarioBase
   {
      private Dependency _result;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         var httpClient = new StubHttpClient<Status>
         {
            BaseAddress = new Uri("http://stubbaseaddress"),
            Exception = new TaskCanceledException()
         };

         var testSubject = new HttpApiStatusEndpointDependency(httpClient, new HttpApiConfiguration { Name = "expectedDependencyName"});

         _result = await testSubject.GetStatusAsync(CancellationToken.None);
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
