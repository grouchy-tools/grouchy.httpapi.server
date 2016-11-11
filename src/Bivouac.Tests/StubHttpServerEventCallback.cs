namespace Bivouac.Tests
{
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Bivouac.Abstractions;
   using Bivouac.Events;

   public class StubHttpServerEventCallback : IHttpServerEventCallback
   {
      private readonly IHttpServerEventCallback _httpServerEventCallback;

      public Exception Exception { get; set; }

      public List<IHttpServerEvent> Events { get; } = new List<IHttpServerEvent>();

      public HttpServerRequest[] Requests => Events.OfType<HttpServerRequest>().ToArray();

      public HttpServerResponse[] Responses => Events.OfType<HttpServerResponse>().ToArray();

      public HttpServerException[] Exceptions => Events.OfType<HttpServerException>().ToArray();

      public StubHttpServerEventCallback(IHttpServerEventCallback httpServerEventCallback)
      {
         _httpServerEventCallback = httpServerEventCallback;
      }

      public virtual void Invoke(IHttpServerEvent @event)
      {
         if (Exception != null)
         {
            throw Exception;
         }

         _httpServerEventCallback.Invoke(@event);
         Events.Add(@event);
      }
   }
}