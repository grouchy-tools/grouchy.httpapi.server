namespace Bivouac.Events
{
   using System;
   using System.Collections.Generic;
   using Bivouac.Abstractions;
   using Microsoft.AspNetCore.Http;
   using Microsoft.Extensions.Primitives;
   using Microsoft.Net.Http.Headers;

   public class HttpServerRequest : IHttpServerEvent
   {
      public string EventType => nameof(HttpServerRequest);

      public string RequestId { get; set; }

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method { get; set; }

      public IDictionary<string, object> Tags { get; set; }

      public string UserAgent { get; set; }

      public static HttpServerRequest Create(string requestId, HttpContext context)
      {
         return new HttpServerRequest
         {
            RequestId = requestId,
            Timestamp = DateTimeOffset.UtcNow,
            Method = context.Request.Method,
            Uri = context.Request.Path + context.Request.QueryString,
            UserAgent = GetUserAgentProperty(context)
         };
      }

      private static string GetUserAgentProperty(HttpContext context)
      {
         StringValues userAgentHeader;
         if (!context.Request.Headers.TryGetValue(HeaderNames.UserAgent, out userAgentHeader))
         {
            return null;
         }
         return userAgentHeader[0];
      }
   }
}
