using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Grouchy.HttpApi.Server.Abstractions.Model;
using NUnit.Framework;
using Grouchy.HttpApi.Server.Services;

namespace Grouchy.HttpApi.Server.Tests.StatusScenarios.api_dependencies
{
   // ReSharper disable once InconsistentNaming
   public class http_client_returns_error_code : ScenarioBase
   {
      private Dependency _result;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         var httpClient = new StubHttpClient<Status>
         {
            BaseAddress = new Uri("http://stubbaseaddress"),
            StatusCode = HttpStatusCode.InternalServerError
         };

         var testSubject = new HttpApiStatusEndpointDependency(httpClient, new HttpApiConfiguration { Name = "dependencyName"});

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
         Assert.AreEqual("dependencyName", _result.Name);
      }

      [Test]
      public void should_return_unavailable()
      {
         Assert.AreEqual(Availability.Unavailable, _result.Availability);
      }
   }
}
