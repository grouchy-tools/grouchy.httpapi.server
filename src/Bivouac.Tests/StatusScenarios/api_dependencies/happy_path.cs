namespace Bivouac.Tests.StatusScenarios.api_dependencies
{
   using System.Threading;
   using Xunit;
   using Bivouac.Model;
   using Bivouac.Services;

   public class happy_path : IClassFixture<happy_path.fixture>
   {
      public class fixture
      {
         public readonly Status Result;

         public fixture()
         {
            var httpClient = new StubHttpClient<Status>
            {
               Response = new Status { Name = "downstreamApiName", Availability = Availability.Available }
            };
            var testSubject = new ApiStatusEndpointDependency("dependencyName", httpClient);

            Result = testSubject.GetStatus(CancellationToken.None).Result;
         }
      }

      private readonly fixture _fixture;

      public happy_path(fixture fixture)
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
         Assert.Equal("downstreamApiName", _fixture.Result.Name);
      }

      [Fact]
      public void should_return_availability()
      {
         Assert.Equal(Availability.Available, _fixture.Result.Availability);
      }
   }
}
