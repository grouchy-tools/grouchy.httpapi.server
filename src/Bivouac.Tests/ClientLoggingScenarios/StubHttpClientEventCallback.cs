namespace Bivouac.Tests.ClientLoggingScenarios
{
   using System.Collections.Generic;
   using Burble.Abstractions;

   public class StubHttpClientEventCallback : IHttpClientEventCallback
   {
      public List<IHttpClientEvent> Events { get; } = new List<IHttpClientEvent>();

      public void Invoke(IHttpClientEvent @event)
      {
         Events.Add(@event);
      }
   }
}
