namespace Bivouac.Tests.StatusScenarios
{
   using System.Diagnostics;
   using System.Net.Http;
   using Xunit;
   using Bivouac.Abstractions;
   using Bivouac.Model;
   using Microsoft.Extensions.DependencyInjection;

   public class multiple_dependencies_with_timeout : IClassFixture<multiple_dependencies_with_timeout.fixture>
   {
      public class fixture : StatusFixture
      {
         public readonly HttpResponseMessage Response;
         public readonly long Duration;

         public fixture()
         {
            var stopwatch = Stopwatch.StartNew();

            Response = TestHost.Get("/status");

            stopwatch.Stop();
            Duration = stopwatch.ElapsedMilliseconds;
         }

         protected override void ConfigureServicesBuilder(IServiceCollection services)
         {
            base.ConfigureServicesBuilder(services);

            var status1 = new Status { Name = "myDep1", Availability = Availability.Limited };
            var status2 = new Status { Name = "myDep2", Availability = Availability.Unknown };

            services.AddSingleton<IStatusEndpointDependency>(new StubStatusEndpointDependency  { Name = "myDep1", Status = status1, DelayMs = 5000 });
            services.AddSingleton<IStatusEndpointDependency>(new StubStatusEndpointDependency { Name = "myDep2", Status = status2 });
         }
      }

      private readonly fixture _fixture;

      public multiple_dependencies_with_timeout(fixture fixture)
      {
         _fixture = fixture;
      }

      [Fact]
      public void should_return_status_code_okay()
      {
         Assert.Equal(200, (int)_fixture.Response.StatusCode);
      }

      [Fact]
      public void should_return_exact_json_content()
      {
         var content = _fixture.Response.Content.ReadAsStringAsync().Result;

         // Dependency that is timed-out will be status Unknown
         Assert.Equal("{\"name\":null,\"availability\":\"Unknown\",\"host\":\"http://localhost\",\"dependencies\":[{\"name\":\"myDep1\",\"availability\":\"Unknown\"},{\"name\":\"myDep2\",\"availability\":\"Unknown\"}]}", content);
      }

      [Fact]
      public void duration_should_be_around_three_seconds()
      {
         Assert.InRange(_fixture.Duration, 2800, 3400);
      }
   }
}
