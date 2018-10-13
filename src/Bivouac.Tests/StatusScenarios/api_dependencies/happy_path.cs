using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Bivouac.Model;
using Bivouac.Services;

namespace Bivouac.Tests.StatusScenarios.api_dependencies
{
   // ReSharper disable once InconsistentNaming
   public class happy_path : ScenarioBase
   {
      private Status _result;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         var httpClient = new StubHttpClient<Status>
         {
            Response = new Status { Name = "downstreamApiName", Availability = Availability.Available }
         };
         var testSubject = new ApiStatusEndpointDependency("dependencyName", httpClient);

         _result = await testSubject.GetStatusAsync(CancellationToken.None);
      }

      [Test]
      public void should_return_status_object()
      {
         Assert.IsInstanceOf<Status>(_result);
      }

      [Test]
      public void should_return_name()
      {
         Assert.AreEqual("downstreamApiName", _result.Name);
      }

      [Test]
      public void should_return_availability()
      {
         Assert.AreEqual(Availability.Available, _result.Availability);
      }
   }
}
