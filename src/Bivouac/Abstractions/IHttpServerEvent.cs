namespace Bivouac.Abstractions
{
   using System;
   using System.Collections.Generic;
   using Microsoft.AspNetCore.Http;

   public interface IHttpServerEvent
   {
      string EventType { get; }

      DateTimeOffset Timestamp { get; }

      string Uri { get; }

      string Method { get; }

      IDictionary<string, object> Tags { get; }

      HttpRequest Request { get; }
   }
}