namespace Bivouac.Tests.StatusScenarios.api_dependencies
{
   using System;
   using System.Net.Http;
   using System.Threading;
   using Xunit;
   using Bivouac.Model;
   using Bivouac.Services;
   using Burble.Abstractions;

   public class http_client_times_out : IClassFixture<http_client_times_out.fixture>
   {
      public class fixture
      {
         public readonly Status Result;

         public fixture()
         {
            var httpClient = new StubHttpClient<Status>
            {
               BaseAddress = new Uri("http://stubbaseaddress"),
               Exception = new HttpClientTimeoutException(HttpMethod.Get, new Uri("http://stubbaseaddress/status"))
            };

            var testSubject = new ApiStatusEndpointDependency("expectedDependencyName", httpClient);

            Result = testSubject.GetStatus(CancellationToken.None).Result;
         }
      }

      private readonly fixture _fixture;

      public http_client_times_out(fixture fixture)
      {
         _fixture = fixture;
      }

      [Fact]
      public void should_return_status_object()
      {
         Assert.IsType<Status>(_fixture.Result);
      }

      [Fact]
      public void should_return_name()
      {
         Assert.Equal("expectedDependencyName", _fixture.Result.Name);
      }

      [Fact]
      public void should_return_unknown()
      {
         Assert.Equal(Availability.Unknown, _fixture.Result.Availability);
      }

      [Fact]
      public void should_return_host()
      {
         Assert.Equal("http://stubbaseaddress", _fixture.Result.Host);
      }
   }
}
