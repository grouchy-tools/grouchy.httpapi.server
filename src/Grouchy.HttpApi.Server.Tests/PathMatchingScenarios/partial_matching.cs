using System.Collections.Generic;
using System.Net;
using Grouchy.HttpApi.Server.Extensions;
using Microsoft.AspNetCore.Http;
using Shouldly;
using NUnit.Framework;

namespace Grouchy.HttpApi.Server.Tests.PathMatchingScenarios
{
   // ReSharper disable once InconsistentNaming
   public class partial_matching
   {
      //TODO
      //api/get/2016-12-30/idg/ds/fe

      [TestCase("/api/get", "GET", "/api/get", new string[] { }, new string[] { }, "")]
      [TestCase("/api/get/id", "GET", "/api/get", new string[] { }, new string[] { }, "/id")]
      [TestCase("/api/get/2015-12-30", "GET", "/api/get/{date}", new[] { "date" }, new[] { "2015-12-30" }, "")]
      [TestCase("/api/get/2015-12-30/id", "GET", "/api/get/{date}", new[] { "date" }, new[] { "2015-12-30" }, "/id")]
      [TestCase("/api/get/2015-12-30/id", "GET", "/api/get/{date}/id", new[] { "date" }, new[] { "2015-12-30" }, "")]
      [TestCase("/api/get/Two+Words/id", "GET", "/api/get/{name}/id", new[] { "name" }, new[] { "Two Words" }, "")]
      [TestCase("/api/get/Two%20Words/id", "GET", "/api/get/{name}/id", new[] { "name" }, new[] { "Two Words" }, "")]
      [TestCase("/api/get/2015-12-30/id", "GET", "/api/get/{date}/{field}", new[] { "date", "field" }, new[] { "2015-12-30", "id" }, "")]
      [TestCase("/api/get/more", "GET", "/api/get", new string[] { }, new string[] { }, "/more")]
      public void should_match(string urlEncodedPath, string method, string pattern, string[] expectedTokenKeys, string[] expectedTokenValues, string expectedRemainder)
      {
         // PathString seemingly automatically encodes the string even if it's already encoded
         var request = new StubHttpRequest { Method = method, Path = WebUtility.UrlDecode(urlEncodedPath)};

         IDictionary<string, string> tokens;
         PathString remainder;
         var result = request.IsFor(method, pattern, out tokens, out remainder);
         
         result.ShouldBe(true);
         tokens.Count.ShouldBe(expectedTokenKeys.Length);

         for (var i = 0; i < expectedTokenKeys.Length; i++)
         {
            Assert.AreEqual(expectedTokenValues[i], tokens[expectedTokenKeys[i]]);
         }

         remainder.ToString().ShouldBe(expectedRemainder);
      }

      [TestCase("/api/get", "PUT", "/api/get")]
      [TestCase("/api/put/2015-12-30", "GET", "/api/get/{date}")]
      [TestCase("/api/get/2015-12-30/id", "GET", "/api/get/{date}/name")]
      [TestCase("/api/get/more", "GET", "/get/more")]
      [TestCase("/api/get", "GET", "/api/get/more")]
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
