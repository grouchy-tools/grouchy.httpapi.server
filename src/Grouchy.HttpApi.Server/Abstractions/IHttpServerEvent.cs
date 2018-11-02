using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Grouchy.HttpApi.Server.Abstractions
{
   // TODO: Should this inherit IEvent?
   public interface IHttpServerEvent
   {
      string EventType { get; }

      DateTimeOffset Timestamp { get; }

      IDictionary<string, object> Tags { get; }

      string Uri { get; }

      string Method { get; }

      HttpRequest Request { get; }
   }
}