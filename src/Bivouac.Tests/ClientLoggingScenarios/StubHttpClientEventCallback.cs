namespace Bivouac.Tests.ClientLoggingScenarios
{
   using System.Collections.Generic;
   using System.Linq;
   using Burble.Abstractions;
   using Burble.Events;

   public class StubHttpClientEventCallback : IHttpClientEventCallback
   {
      public List<IHttpClientEvent> Events { get; } = new List<IHttpClientEvent>();

      public HttpClientRequestInitiated[] RequestsInitiated => Events.OfType<HttpClientRequestInitiated>().ToArray();

      public HttpClientResponseReceived[] ResponsesReceived => Events.OfType<HttpClientResponseReceived>().ToArray();

      public void Invoke(IHttpClientEvent @event)
      {
         Events.Add(@event);
      }
   }
}
