namespace Bivouac.Tests.StatusScenarios.api_dependencies
{
   using System;
   using System.Net;
   using System.Threading;
   using Xunit;
   using Bivouac.Model;
   using Bivouac.Services;

   public class http_client_returns_error_code : IClassFixture<http_client_returns_error_code.fixture>
   {
      public class fixture
      {
         public readonly Status Result;

         public fixture()
         {
            var httpClient = new StubHttpClient<Status>
            {
               BaseAddress = new Uri("http://stubbaseaddress"),
               StatusCode = HttpStatusCode.InternalServerError
            };

            var testSubject = new ApiStatusEndpointDependency("dependencyName", httpClient);

            Result = testSubject.GetStatus(CancellationToken.None).Result;
         }
      }

      private readonly fixture _fixture;

      public http_client_returns_error_code(fixture fixture)
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
         Assert.Equal("dependencyName", _fixture.Result.Name);
      }

      [Fact]
      public void should_return_unavailable()
      {
         Assert.Equal(Availability.Unavailable, _fixture.Result.Availability);
      }

      [Fact]
      public void should_return_host()
      {
         Assert.Equal("http://stubbaseaddress", _fixture.Result.Host);
      }
   }
}
