using System;
using System.Collections.Generic;
using System.Net.Http;
using Grouchy.HttpApi.Client.Abstractions.Events;
using Newtonsoft.Json;

namespace Grouchy.HttpApi.Server.Tests.EventCallbackScenarios
{
   public class StubHttpClientEvent : IHttpClientEvent
   {
      public string EventType { get; set; }
         
      public DateTimeOffset Timestamp { get; set; }

      public string Method { get; set; }

      public string TargetService { get; set; }

      public string Uri { get; set; }
                  
      [JsonIgnore]
      public HttpRequestMessage Request { get; set; }
         
      public string SomethingElse { get; set; }
         
      public IDictionary<string, object> Tags { get; set; } = new Dictionary<string, object>();
         
      public Exception Exception { get; set; }
   }
}