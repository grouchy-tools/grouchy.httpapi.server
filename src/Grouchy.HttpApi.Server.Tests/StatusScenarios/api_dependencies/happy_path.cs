using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Grouchy.HttpApi.Server.Model;
using Grouchy.HttpApi.Server.Services;

namespace Grouchy.HttpApi.Server.Tests.StatusScenarios.api_dependencies
{
   // ReSharper disable once InconsistentNaming
   public class happy_path : ScenarioBase
   {
      private Dependency _result;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         var httpClient = new StubHttpClient<string>
         {
            Response = "Hey!"
         };
         var testSubject = new HttpApiStatusEndpointDependency(httpClient, new HttpApiConfiguration { Name = "downstreamApiName" });

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
         Assert.AreEqual("downstreamApiName", _result.Name);
      }

      [Test]
      public void should_return_availability()
      {
         Assert.AreEqual(Availability.Available, _result.Availability);
      }
   }
}
