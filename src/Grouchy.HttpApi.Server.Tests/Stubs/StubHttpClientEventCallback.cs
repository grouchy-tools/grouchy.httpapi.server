using System.Collections.Generic;
using Grouchy.HttpApi.Client.Abstractions.EventCallbacks;
using Grouchy.HttpApi.Client.Abstractions.Events;

namespace Grouchy.HttpApi.Server.Tests
{
   public class StubHttpClientEventCallback : IHttpClientEventCallback
   {
      public List<IHttpClientEvent> Events { get; } = new List<IHttpClientEvent>();

      public void Invoke(IHttpClientEvent @event)
      {
         Events.Add(@event);
      }
   }
}
