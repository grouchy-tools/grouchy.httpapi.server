namespace Bivouac
{
   using System;
   using System.Collections.Generic;
   using Microsoft.AspNetCore.Http;
   using System.Linq;
   using System.Net;
   using System.Text.RegularExpressions;

   public static class Extensions
   {
      public static bool IsFor(this HttpRequest request, string method, string match)
      {
         if (request.Method != method)
         {
            return false;
         }

         return request.Path.Value.Equals(match, StringComparison.OrdinalIgnoreCase);
      }

      public static bool IsFor(this HttpRequest request, string method, string match, out IDictionary<string, string> tokens)
      {
         PathString remaining;
         if (!request.IsFor(method, match, out tokens, out remaining) || !string.IsNullOrEmpty(remaining))
         {
            tokens = null;
            return false;
         }

         return true;
      }

      public static bool IsFor(this HttpRequest request, string method, string match, out IDictionary<string, string> tokens, out PathString remainder)
      {
         if (request.Method != method)
         {
            tokens = null;
            remainder = PathString.Empty;
            return false;
         }

         var regex = new Regex(match.ToRegex());

         var result = regex.Match(WebUtility.UrlDecode(request.Path));

         if (!result.Success || result.Groups["0"].Value != request.Path)
         {
            tokens = null;
            remainder = PathString.Empty;
            return false;
         }

         tokens = new Dictionary<string, string>();
         var groupNames = regex.GetGroupNames();

         // Ignore first (whole match) and second (remainder) groups
         for (var i = 2; i < groupNames.Length; i++)
         {
            tokens.Add(groupNames[i], result.Groups[groupNames[i]].Value);
         }

         var remainderGroup = result.Groups["1"].Value;
         remainder = !string.IsNullOrEmpty(remainderGroup) ? new PathString(remainderGroup) : PathString.Empty;

         return true;
      }

      public static string ToRegex(this string path)
      {
         var segments = path.Split('/').Select(Transform);

         return string.Join(@"\/", segments) + @"(\/.*)?";
      }

      private static string Transform(string segment)
      {
         if (segment.StartsWith("{") && segment.EndsWith("}"))
         {
            var field = segment.Substring(1, segment.Length - 2);
            return $"(?<{field}>[^/]*)";
         }

         return segment;
      }
   }
}
