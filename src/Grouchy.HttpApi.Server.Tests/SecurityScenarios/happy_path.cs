using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Grouchy.HttpApi.Server.Tests.SecurityScenarios
{
   // ReSharper disable once InconsistentNaming
   public class happy_path : ScenarioBase
   {
      private HttpResponseMessage _response;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         _response = await TestHost.GetAsync("/");
      }

      [Test]
      public void should_add_hsts_to_response_header()
      {
         _response.Headers.TryGetValues("strict-transport-security", out var values).ShouldBe(true);
         values.Single().ShouldBe("max-age=31536000; includeSubDomains");
      }

      [Test]
      public void should_remove_identifying_header()
      {
         _response.Headers.Contains("Server").ShouldBe(false);
      }
   }
}
