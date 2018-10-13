using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Bivouac.Model;
using Bivouac.Services;
using Burble.Abstractions.Exceptions;

namespace Bivouac.Tests.StatusScenarios.api_dependencies
{
   // ReSharper disable once InconsistentNaming
   public class http_client_times_out : ScenarioBase
   {
      private Status _result;

      [OneTimeSetUp]
      public async Task setup_scenario()
      {
         var httpClient = new StubHttpClient<Status>
         {
            BaseAddress = new Uri("http://stubbaseaddress"),
            Exception = new HttpClientTimeoutException(HttpMethod.Get, new Uri("http://stubbaseaddress/.status"))
         };

         var testSubject = new ApiStatusEndpointDependency("expectedDependencyName", httpClient);

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
