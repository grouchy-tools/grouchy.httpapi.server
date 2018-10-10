using System.Linq;

namespace Bivouac.Extensions
{
    public static class StringExtensions
    {
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