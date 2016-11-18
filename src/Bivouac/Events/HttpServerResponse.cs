namespace Bivouac.Events
{
   using System;
   using System.Collections.Generic;
   using Bivouac.Abstractions;
   using Microsoft.AspNetCore.Http;

   public class HttpServerResponse : IHttpServerEvent
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
            Method = context.Request.Method,
            Uri = context.Request.Path + context.Request.QueryString,
            StatusCode = (int)context.Response.StatusCode,
            DurationMs = durationMs
         };
      }
   }
}
