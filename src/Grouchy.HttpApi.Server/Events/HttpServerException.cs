using System;
using System.Collections.Generic;
using Grouchy.HttpApi.Server.Abstractions.Events;
using Microsoft.AspNetCore.Http;

namespace Grouchy.HttpApi.Server.Events
{
   public class HttpServerException : IHttpServerExceptionEvent
   {
      public string EventType => nameof(HttpServerException);

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method { get; set; }

      public IDictionary<string, object> Tags { get; } = new Dictionary<string, object>();

      public long DurationMs { get; set; }

      public Exception Exception { get; set; }

      public static HttpServerException Create(HttpContext context, long durationMs, Exception exception)
      {
         return new HttpServerException
         {
            Timestamp = DateTimeOffset.UtcNow,
            Uri = context.Request.Path + context.Request.QueryString,
            Method = context.Request.Method,
            DurationMs = durationMs,
            Exception = exception
         };
      }
   }
}