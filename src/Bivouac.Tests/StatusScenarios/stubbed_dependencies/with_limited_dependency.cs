namespace Bivouac.Tests.StatusScenarios
{
   using System.Net.Http;
   using Xunit;
   using Bivouac.Abstractions;
   using Bivouac.Model;
   using Microsoft.Extensions.DependencyInjection;

   public class with_limited_dependency : IClassFixture<with_limited_dependency.fixture>
   {
      public class fixture : StatusFixture
      {
         public readonly HttpResponseMessage Response;

         public fixture() : base(ConfigureServices)
         {
            StubStatusEndpointService.Name = "myName";

            Response = TestHost.Get("/status");
         }

         private static void ConfigureServices(IServiceCollection services)
         {
            var status = new Status { Name = "myDependency", Availability = Availability.Limited };

            services.AddSingleton<IStatusEndpointDependency>(new StubStatusEndpointDependency { Status = status });
         }
      }

      private readonly fixture _fixture;

      public with_limited_dependency(fixture fixture)
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

         Assert.Equal("{\"name\":\"myName\",\"availability\":\"Limited\",\"host\":\"http://localhost\",\"dependencies\":[{\"name\":\"myDependency\",\"availability\":\"Limited\"}]}", content);
      }
   }
}
