﻿using System;
using System.Collections.Generic;
using Grouchy.HttpApi.Server.Abstractions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Grouchy.HttpApi.Server.Events
{
   public class HttpServerResponse : IHttpServerEvent
   {
      public string EventType => nameof(HttpServerResponse);

      public DateTimeOffset Timestamp { get; set; }

      public string Uri { get; set; }

      public string Method => Request.Method;

      public IDictionary<string, object> Tags { get; } = new Dictionary<string, object>();

      [JsonIgnore]
      public HttpRequest Request { get; set; }

      public int StatusCode { get; set; }

      public long DurationMs { get; set; }

      public static HttpServerResponse Create(HttpContext context, long durationMs)
      {
         return new HttpServerResponse
         {
            Timestamp = DateTimeOffset.UtcNow,
            Uri = context.Request.Path + context.Request.QueryString,
            Request = context.Request,
            StatusCode = context.Response.StatusCode,
            DurationMs = durationMs
         };
      }
   }
}