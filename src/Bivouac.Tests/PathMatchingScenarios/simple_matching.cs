namespace Bivouac.Tests.PathMatchingScenarios
{
   using Bivouac;
   using Xunit;

   public class simple_matching
   {
      [Theory]
      [InlineData("/api/get", "GET", "/api/get")]
      public void should_match(string path, string method, string pattern)
      {
         var request = new StubHttpRequest { Method = method, Path = path };

         Assert.True(request.IsFor("GET", pattern));
      }

      [Theory]
      [InlineData("/api/files", "PUT", "/api/files")]
      [InlineData("/api/get", "GET", "/api/put")]
      [InlineData("/api/get/more", "GET", "/get/more")]
      [InlineData("/api/get/more", "GET", "/api/get")]
      public void should_not_match(string path, string method, string pattern)
      {
         var request = new StubHttpRequest { Method = method, Path = path };

         Assert.False(request.IsFor("GET", pattern));
      }
   }
}
