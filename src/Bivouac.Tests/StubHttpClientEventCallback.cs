using System.Collections.Generic;
using Burble.Abstractions;

namespace Bivouac.Tests
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
