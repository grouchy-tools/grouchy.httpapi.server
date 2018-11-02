using Grouchy.HttpApi.Server.Extensions;
using Shouldly;
using NUnit.Framework;

namespace Grouchy.HttpApi.Server.Tests.PathMatchingScenarios
{
   // ReSharper disable once InconsistentNaming
   public class simple_matching
   {
      [TestCase("/api/get", "GET", "/api/get")]
      public void should_match(string path, string method, string pattern)
      {
         var request = new StubHttpRequest { Method = method, Path = path };

         request.IsFor("GET", pattern).ShouldBe(true);
      }

      [TestCase("/api/files", "PUT", "/api/files")]
      [TestCase("/api/get", "GET", "/api/put")]
      [TestCase("/api/get/more", "GET", "/get/more")]
      [TestCase("/api/get/more", "GET", "/api/get")]
      public void should_not_match(string path, string method, string pattern)
      {
         var request = new StubHttpRequest { Method = method, Path = path };

         request.IsFor("GET", pattern).ShouldBe(false);
      }
   }
}
