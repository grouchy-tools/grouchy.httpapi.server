using Grouchy.HttpApi.Server.Extensions;
using NUnit.Framework;

namespace Grouchy.HttpApi.Server.Tests.PathMatchingScenarios
{
   // ReSharper disable once InconsistentNaming
   public class token_matching
   {
      [TestCase("/api/get/xyz", "GET", "/api/get/{id}", new[] { "id" }, new[] { "xyz" })]
      [TestCase("/api/get/xyz/id", "GET", "/api/get/{id}/id", new[] { "id" }, new[] { "xyz" })]
      public void should_match(string path, string method, string pattern, string[] expectedTokenKeys, string[] expectedTokenValues)
      {
         var request = new StubHttpRequest { Method = method, Path = path };

         var result = request.IsFor("GET", pattern, out var tokens);

         Assert.True(result);
         Assert.AreEqual(expectedTokenKeys.Length, tokens.Count);

         for (var i = 0; i < expectedTokenKeys.Length; i++)
         {
            Assert.AreEqual(expectedTokenValues[i], tokens[expectedTokenKeys[i]]);
         }
      }

      [TestCase("/api/get/xyz", "PUT", "/api/get/{id}")]
      [TestCase("/api/put/xyz", "GET", "/api/get/{id}")]
      [TestCase("/api/get/xyz/id", "GET", "/api/get/{id}/name")]
      [TestCase("/api/get/more", "GET", "/get/more")]
      [TestCase("/api/get/more", "GET", "/api/get")]
      public void should_not_match(string path, string method, string pattern)
      {
         var request = new StubHttpRequest { Method = method, Path = path };

         var result = request.IsFor("GET", pattern, out var tokens);

         Assert.False(result);
         Assert.Null(tokens);
      }
   }
}
