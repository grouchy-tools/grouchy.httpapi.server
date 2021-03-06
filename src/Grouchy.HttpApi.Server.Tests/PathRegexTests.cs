﻿using Grouchy.HttpApi.Server.Extensions;
using NUnit.Framework;

namespace Grouchy.HttpApi.Server.Tests
{
   public class PathRegexTests
   {
      [Test]
      public void single_segment()
      {
         var result = "/api".ToRegex();
         
         Assert.AreEqual(result, @"\/api(\/.*)?");
      }

      [Test]
      public void includes_token()
      {
         var result = "/api/get/{date}".ToRegex();

         Assert.AreEqual(result, @"\/api\/get\/(?<date>[^/]*)(\/.*)?");
      }

      [Test]
      public void suffix_following_token()
      {
         var result = "/api/get/{date}/id".ToRegex();

         Assert.AreEqual(result, @"\/api\/get\/(?<date>[^/]*)\/id(\/.*)?");
      }
   }
}
