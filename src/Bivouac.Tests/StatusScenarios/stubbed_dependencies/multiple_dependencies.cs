namespace Bivouac.Tests.StatusScenarios
{
   using System.Net.Http;
   using Xunit;
   using Bivouac.Abstractions;
   using Bivouac.Model;
   using Microsoft.Extensions.DependencyInjection;

   public class multiple_dependencies : IClassFixture<multiple_dependencies.fixture>
   {
      public class fixture : StatusFixture
      {
         public readonly HttpResponseMessage Response;

         public fixture() : base(ConfigureServices)
         {
            Response = TestHost.Get("/status");
         }

         private static void ConfigureServices(IServiceCollection services)
         {
            var status1 = new Status { Name = "myDep1", Availability = Availability.Limited };
            var status2 = new Status { Name = "myDep2", Availability = Availability.Unknown };

            services.AddSingleton<IStatusEndpointDependency>(new StubStatusEndpointDependency { Status = status1 });
            services.AddSingleton<IStatusEndpointDependency>(new StubStatusEndpointDependency { Status = status2 });
         }
      }

      private readonly fixture _fixture;

      public multiple_dependencies(fixture fixture)
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

         Assert.Equal("{\"name\":null,\"availability\":\"Limited\",\"host\":\"http://localhost\",\"dependencies\":[{\"name\":\"myDep1\",\"availability\":\"Limited\"},{\"name\":\"myDep2\",\"availability\":\"Unknown\"}]}", content);
      }
   }
}
