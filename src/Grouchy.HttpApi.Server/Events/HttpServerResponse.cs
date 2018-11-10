using System;
using System.Collections.Generic;
using Grouchy.HttpApi.Server.Abstractions.Events;
using Microsoft.AspNetCore.Http;

namespace Grouchy.HttpApi.Server.Events
{
   public class HttpServerResponse : IHttpServerResponseEvent
   {
      public string EventType => nameof(HttpServerResponse);

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method { get; set; }

      public IDictionary<string, object> Tags { get; } = new Dictionary<string, object>();

      public int StatusCode { get; set; }

      public long DurationMs { get; set; }

      public static HttpServerResponse Create(HttpContext context, long durationMs)
      {
         return new HttpServerResponse
         {
            Timestamp = DateTimeOffset.UtcNow,
            Uri = context.Request.Path + context.Request.QueryString,
            Method = context.Request.Method,
            StatusCode = context.Response.StatusCode,
            DurationMs = durationMs
         };
      }
   }
}
