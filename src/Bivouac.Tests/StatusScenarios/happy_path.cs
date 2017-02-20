namespace Bivouac.Tests.StatusScenarios
{
   using System.Collections.Generic;
   using System.Linq;
   using System.Net.Http;
   using Newtonsoft.Json;
   using Xunit;
   using Bivouac.Model;
   using Shouldly;

   public class happy_path : IClassFixture<happy_path.fixture>
   {
      public class fixture : StatusFixture
      {
         public readonly HttpResponseMessage Response;

         public fixture()
         {
            StubStatusEndpointService.Name = "myName";
            StubStatusEndpointService.Version = "myVersion";

            Response = TestHost.Get("/status");
         }
      }

      private readonly fixture _fixture;

      public happy_path(fixture fixture)
      {
         _fixture = fixture;
      }

      [Fact]
      public void should_return_status_code_okay()
      {
         Assert.Equal(200, (int)_fixture.Response.StatusCode);
      }

      [Fact]
      public void should_return_json_content()
      {
         var content = _fixture.Response.Content.ReadAsStringAsync().Result;
         var status = JsonConvert.DeserializeObject<Status>(content);

         Assert.Equal("myName", status.Name);
         Assert.Equal(Availability.Available, status.Availability);
         Assert.Equal("myVersion", status.Version);
         Assert.Equal("http://localhost", status.Host);
      }

      [Fact]
      public void should_return_json_content_type()
      {
         IEnumerable<string> values;
         _fixture.Response.Content.Headers.TryGetValues("Content-Type", out values).ShouldBe(true);
         values.Single().ShouldBe("application/json");
      }

      [Fact]
      public void should_return_exact_json_content()
      {
         var content = _fixture.Response.Content.ReadAsStringAsync().Result;

         Assert.Equal("{\"name\":\"myName\",\"availability\":\"Available\",\"version\":\"myVersion\",\"host\":\"http://localhost\"}", content);
      }
   }
}
