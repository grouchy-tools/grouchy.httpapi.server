namespace Bivouac.Tests.StatusScenarios
{
   using System.Net.Http;
   using Newtonsoft.Json;
   using Xunit;
   using Bivouac.Model;

   public class passthrough_to_next_middleware : IClassFixture<passthrough_to_next_middleware.fixture>
   {
      public class fixture : StatusFixture
      {
         public readonly HttpResponseMessage Response;

         public fixture()
         {
            Response = TestHost.Get("/another-endpoint");
         }
      }

      private readonly fixture _fixture;

      public passthrough_to_next_middleware(fixture fixture)
      {
         _fixture = fixture;
      }

      [Fact]
      public void should_return_status_code_from_another_endpoint()
      {
         Assert.Equal(201, (int)_fixture.Response.StatusCode);
      }

      [Fact]
      public void should_return_content_from_another_middleware()
      {
         var content = _fixture.Response.Content.ReadAsStringAsync().Result;

         Assert.Equal("another-response", content);
      }
   }
}
