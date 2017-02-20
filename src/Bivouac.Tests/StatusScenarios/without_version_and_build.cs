namespace Bivouac.Tests.StatusScenarios
{
   using System.Net.Http;
   using Newtonsoft.Json;
   using Xunit;
   using Bivouac.Model;

   public class without_version_and_build : IClassFixture<without_version_and_build.fixture>
   {
      public class fixture : StatusFixture
      {
         public readonly HttpResponseMessage Response;

         public fixture()
         {
            Response = TestHost.Get("/status");
         }
      }

      private readonly fixture _fixture;

      public without_version_and_build(fixture fixture)
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

         Assert.Equal("{\"name\":null,\"availability\":\"Available\",\"host\":\"http://localhost\"}", content);
      }
   }
}
