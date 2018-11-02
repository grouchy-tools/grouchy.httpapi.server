﻿using System;
using System.Collections.Generic;
using Grouchy.HttpApi.Server.Abstractions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Grouchy.HttpApi.Server.Events
{
   public class HttpServerException : IHttpServerEvent
   {
      public string EventType => nameof(HttpServerException);

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method => Request.Method;

      public IDictionary<string, object> Tags { get; } = new Dictionary<string, object>();

      [JsonIgnore]
      public HttpRequest Request { get; set; }

      public Exception Exception { get; set; }

      public static HttpServerException Create(HttpContext context, Exception exception)
      {
         return new HttpServerException
         {
            Timestamp = DateTimeOffset.UtcNow,
            Uri = context.Request.Path + context.Request.QueryString,
            Request = context.Request,
            Exception = exception
         };
      }
   }
}