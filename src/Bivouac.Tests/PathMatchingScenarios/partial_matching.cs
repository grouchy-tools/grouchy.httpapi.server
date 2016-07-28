namespace Bivouac.Tests.PathMatchingScenarios
{
   using System.Collections.Generic;
   using Microsoft.AspNetCore.Http;
   using Bivouac;
   using Xunit;

   public class partial_matching
   {
      //TODO
      //api/get/2016-12-30/idg/ds/fe

      [Theory]
      [InlineData("/api/get", "GET", "/api/get", new string[] { }, new string[] { }, "")]
      [InlineData("/api/get/id", "GET", "/api/get", new string[] { }, new string[] { }, "/id")]
      [InlineData("/api/get/2015-12-30", "GET", "/api/get/{date}", new[] { "date" }, new[] { "2015-12-30" }, "")]
      [InlineData("/api/get/2015-12-30/id", "GET", "/api/get/{date}", new[] { "date" }, new[] { "2015-12-30" }, "/id")]
      [InlineData("/api/get/2015-12-30/id", "GET", "/api/get/{date}/id", new[] { "date" }, new[] { "2015-12-30" }, "")]
      [InlineData("/api/get/Two%2BWords/id", "GET", "/api/get/{name}/id", new[] { "name" }, new[] { "Two%2BWords" }, "")]
      [InlineData("/api/get/2015-12-30/id", "GET", "/api/get/{date}/{field}", new[] { "date", "field" }, new[] { "2015-12-30", "id" }, "")]
      [InlineData("/api/get/more", "GET", "/api/get", new string[] { }, new string[] { }, "/more")]
      public void should_match(string path, string method, string pattern, string[] expectedTokenKeys, string[] expectedTokenValues, string expectedRemainder)
      {
         var request = new StubHttpRequest { Method = method, Path = path };

         IDictionary<string, string> tokens;
         PathString remainder;
         var result = request.IsFor(method, pattern, out tokens, out remainder);
         
         Assert.True(result);
         Assert.Equal(expectedTokenKeys.Length, tokens.Count);

         for (var i = 0; i < expectedTokenKeys.Length; i++)
         {
            Assert.Equal(expectedTokenValues[i], tokens[expectedTokenKeys[i]]);
         }

         Assert.Equal(expectedRemainder, remainder);
      }

      [Theory]
      [InlineData("/api/get", "PUT", "/api/get")]
      [InlineData("/api/put/2015-12-30", "GET", "/api/get/{date}")]
      [InlineData("/api/get/2015-12-30/id", "GET", "/api/get/{date}/name")]
      [InlineData("/api/get/more", "GET", "/get/more")]
      [InlineData("/api/get", "GET", "/api/get/more")]
      public void should_not_match(string path, string method, string pattern)
      {
         var request = new StubHttpRequest { Method = method, Path = path };

         IDictionary<string, string> tokens;
         PathString remainder;
         var result = request.IsFor("GET", pattern, out tokens, out remainder);

         Assert.False(result);
         Assert.Null(tokens);
      }
   }
}
