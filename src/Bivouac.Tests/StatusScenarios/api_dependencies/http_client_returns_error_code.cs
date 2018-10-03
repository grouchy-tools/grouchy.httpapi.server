using System;
using System.Net;
using System.Threading;
using NUnit.Framework;
using Bivouac.Model;
using Bivouac.Services;

namespace Bivouac.Tests.StatusScenarios.api_dependencies
{
   public class http_client_returns_error_code : ScenarioBase
   {
      private Status _result;

      [OneTimeSetUp]
      public void setup_scenario()
      {
         var httpClient = new StubHttpClient<Status>
         {
            BaseAddress = new Uri("http://stubbaseaddress"),
            StatusCode = HttpStatusCode.InternalServerError
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
         Assert.AreEqual("dependencyName", _result.Name);
      }

      [Test]
      public void should_return_unavailable()
      {
         Assert.AreEqual(Availability.Unavailable, _result.Availability);
      }

      [Test]
      public void should_return_host()
      {
         Assert.AreEqual("http://stubbaseaddress", _result.Host);
      }
   }
}
