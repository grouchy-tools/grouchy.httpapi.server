namespace Bivouac.Tests.PathMatchingScenarios
{
   using System.Collections.Generic;
   using Bivouac;
   using Xunit;

   public class token_matching
   {
      [Theory]
      [InlineData("/api/get/xyz", "GET", "/api/get/{id}", new[] { "id" }, new[] { "xyz" })]
      [InlineData("/api/get/xyz/id", "GET", "/api/get/{id}/id", new[] { "id" }, new[] { "xyz" })]
      public void should_match(string path, string method, string pattern, string[] expectedTokenKeys, string[] expectedTokenValues)
      {
         var request = new StubHttpRequest { Method = method, Path = path };

         IDictionary<string, string> tokens;
         var result = request.IsFor("GET", pattern, out tokens);

         Assert.True(result);
         Assert.Equal(expectedTokenKeys.Length, tokens.Count);

         for (var i = 0; i < expectedTokenKeys.Length; i++)
         {
            Assert.Equal(expectedTokenValues[i], tokens[expectedTokenKeys[i]]);
         }
      }

      [Theory]
      [InlineData("/api/get/xyz", "PUT", "/api/get/{id}")]
      [InlineData("/api/put/xyz", "GET", "/api/get/{id}")]
      [InlineData("/api/get/xyz/id", "GET", "/api/get/{id}/name")]
      [InlineData("/api/get/more", "GET", "/get/more")]
      [InlineData("/api/get/more", "GET", "/api/get")]
      public void should_not_match(string path, string method, string pattern)
      {
         var request = new StubHttpRequest { Method = method, Path = path };

         IDictionary<string, string> tokens;
         var result = request.IsFor("GET", pattern, out tokens);

         Assert.False(result);
         Assert.Null(tokens);
      }
   }
}
