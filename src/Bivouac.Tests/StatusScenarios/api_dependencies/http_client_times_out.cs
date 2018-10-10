using System;
using System.Net.Http;
using System.Threading;
using NUnit.Framework;
using Bivouac.Model;
using Bivouac.Services;
using Burble.Abstractions.Exceptions;

namespace Bivouac.Tests.StatusScenarios.api_dependencies
{
   public class http_client_times_out : ScenarioBase
   {
      private Status _result;

      [OneTimeSetUp]
      public void setup_scenario()
      {
         var httpClient = new StubHttpClient<Status>
         {
            BaseAddress = new Uri("http://stubbaseaddress"),
            Exception = new HttpClientTimeoutException(HttpMethod.Get, new Uri("http://stubbaseaddress/status"))
         };

         var testSubject = new ApiStatusEndpointDependency("expectedDependencyName", httpClient);

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
