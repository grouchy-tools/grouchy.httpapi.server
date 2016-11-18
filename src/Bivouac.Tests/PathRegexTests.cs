namespace Bivouac.Tests
{
   using Xunit;

   public class PathRegexTests
   {
      [Fact]
      public void single_segment()
      {
         var result = "/api".ToRegex();

         Assert.Equal(result, @"\/api(\/.*)?");
      }

      [Fact]
      public void includes_token()
      {
         var result = "/api/get/{date}".ToRegex();

         Assert.Equal(result, @"\/api\/get\/(?<date>[^/]*)(\/.*)?");
      }

      [Fact]
      public void suffix_following_token()
      {
         var result = "/api/get/{date}/id".ToRegex();

         Assert.Equal(result, @"\/api\/get\/(?<date>[^/]*)\/id(\/.*)?");
      }
   }
}
