using System;
using System.Collections.Generic;
using System.Linq;
using Grouchy.HttpApi.Server.Abstractions;
using Grouchy.HttpApi.Server.Events;

namespace Grouchy.HttpApi.Server.Tests
{
   public class StubHttpServerEventCallback : IHttpServerEventCallback
   {
      public Exception Exception { get; set; }

      public List<IHttpServerEvent> Events { get; } = new List<IHttpServerEvent>();

      public HttpServerRequest[] Requests => Events.OfType<HttpServerRequest>().ToArray();

      public HttpServerResponse[] Responses => Events.OfType<HttpServerResponse>().ToArray();

      public HttpServerException[] Exceptions => Events.OfType<HttpServerException>().ToArray();

      public virtual void Invoke(IHttpServerEvent @event)
      {
         if (Exception != null)
         {
            throw Exception;
         }

         Events.Add(@event);
      }
   }
}