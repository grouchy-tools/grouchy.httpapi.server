using System;
using System.Collections.Generic;
using Grouchy.HttpApi.Server.Abstractions.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Grouchy.HttpApi.Server.Events
{
   public class HttpServerRequest : IHttpServerRequestEvent
   {
      public string EventType => nameof(HttpServerRequest);

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method { get; set; }

      public IDictionary<string, object> Tags { get; } = new Dictionary<string, object>();

      public string UserAgent { get; set; }

      public static HttpServerRequest Create(HttpContext context)
      {
         return new HttpServerRequest
         {
            Timestamp = DateTimeOffset.UtcNow,
            Uri = context.Request.Path + context.Request.QueryString,
            Method = context.Request.Method,
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
