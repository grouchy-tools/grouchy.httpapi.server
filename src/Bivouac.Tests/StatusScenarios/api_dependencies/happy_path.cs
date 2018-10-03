using System.Threading;
using NUnit.Framework;
using Bivouac.Model;
using Bivouac.Services;

namespace Bivouac.Tests.StatusScenarios.api_dependencies
{
   public class happy_path : ScenarioBase
   {
      private Status _result;

      [OneTimeSetUp]
      public void setup_scenario()
      {
         var httpClient = new StubHttpClient<Status>
         {
            Response = new Status { Name = "downstreamApiName", Availability = Availability.Available }
         };
         var testSubject = new ApiStatusEndpointDependency("dependencyName", httpClient);

         _result = testSubject.GetStatus(CancellationToken.None).Result;
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
