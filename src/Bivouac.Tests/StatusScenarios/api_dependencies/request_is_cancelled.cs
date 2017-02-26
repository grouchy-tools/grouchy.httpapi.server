namespace Bivouac.Tests.StatusScenarios.api_dependencies
{
   using System;
   using System.Threading;
   using Xunit;
   using Bivouac.Model;
   using Bivouac.Services;

   public class request_is_cancelled : IClassFixture<request_is_cancelled.fixture>
   {
      public class fixture
      {
         public readonly Status Result;

         public fixture()
         {
            var httpClient = new StubHttpClient<Status>
            {
               BaseAddress = new Uri("http://stubbaseaddress"),
               Latency = TimeSpan.FromSeconds(3)
            };

            var testSubject = new ApiStatusEndpointDependency("expectedDependencyName", httpClient);

            Result = testSubject.GetStatus(new CancellationTokenSource(20).Token).Result;
         }
      }

      private readonly fixture _fixture;

      public request_is_cancelled(fixture fixture)
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
