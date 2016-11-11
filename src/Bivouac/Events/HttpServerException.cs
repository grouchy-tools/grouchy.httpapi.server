namespace Bivouac.Events
{
   using System;
   using System.Collections.Generic;
   using Bivouac.Abstractions;
   using Microsoft.AspNetCore.Http;

   public class HttpServerException : IHttpServerEvent
   {
      public string EventType => nameof(HttpServerException);

      public string RequestId { get; set; }

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method { get; set; }

      public IDictionary<string, object> Tags { get; set; }

      public Exception Exception { get; set; }

      public static HttpServerException Create(string requestId, HttpContext context, Exception exception)
      {
         return new HttpServerException
         {
            RequestId = requestId,
            Timestamp = DateTimeOffset.UtcNow,
            Method = context.Request.Method,
            Uri = context.Request.Path + context.Request.QueryString,
            Exception = exception
         };
      }
   }
}