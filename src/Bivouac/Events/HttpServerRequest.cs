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

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method => Request.Method;

      public IDictionary<string, object> Tags { get; } = new Dictionary<string, object>();

      public HttpRequest Request { get; set; }

      public string UserAgent { get; set; }

      public static HttpServerRequest Create(HttpContext context)
      {
         return new HttpServerRequest
         {
            Timestamp = DateTimeOffset.UtcNow,
            Uri = context.Request.Path + context.Request.QueryString,
            Request = context.Request,
            UserAgent = GetUserAgentProperty(context)
         };
      }

      private static string GetUserAgentProperty(HttpContext context)
      {
         if (!context.Request.Headers.TryGetValue(HeaderNames.UserAgent, out var userAgentHeader))
         {
            return null;
         }
         return userAgentHeader[0];
      }
   }
}
