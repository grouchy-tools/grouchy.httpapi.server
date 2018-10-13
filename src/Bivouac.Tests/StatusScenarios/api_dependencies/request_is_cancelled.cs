using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Bivouac.Model;
using Bivouac.Services;

namespace Bivouac.Tests.StatusScenarios.api_dependencies
{
   // ReSharper disable once InconsistentNaming
   public class request_is_cancelled : ScenarioBase
   {
      private Status _result;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         var httpClient = new StubHttpClient<Status>
         {
            BaseAddress = new Uri("http://stubbaseaddress"),
            Latency = TimeSpan.FromSeconds(3)
         };

         var testSubject = new ApiStatusEndpointDependency("expectedDependencyName", httpClient);

         _result = await testSubject.GetStatusAsync(new CancellationTokenSource(20).Token);
      }

      [Test]
      public void should_return_status_object()
      {
         Assert.IsInstanceOf<Status>(_result);
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

      [Test]
      public void should_return_host()
      {
         Assert.AreEqual("http://stubbaseaddress", _result.Host);
      }
   }
}
