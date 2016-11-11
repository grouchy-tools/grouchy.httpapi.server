namespace Bivouac.Tests.ClientLoggingScenarios
{
   using System.Collections.Generic;
   using System.Linq;
   using Burble.Abstractions;
   using Burble.Events;

   public class StubHttpClientEventCallback : IHttpClientEventCallback
   {
      private readonly IHttpClientEventCallback _httpClientEventCallback;

      public List<IHttpClientEvent> Events { get; } = new List<IHttpClientEvent>();

      public HttpClientRequestInitiated[] RequestsInitiated => Events.OfType<HttpClientRequestInitiated>().ToArray();

      public HttpClientResponseReceived[] ResponsesReceived => Events.OfType<HttpClientResponseReceived>().ToArray();

      public StubHttpClientEventCallback(IHttpClientEventCallback httpClientEventCallback)
      {
         _httpClientEventCallback = httpClientEventCallback;
      }

      public void Invoke(IHttpClientEvent @event)
      {
         _httpClientEventCallback.Invoke(@event);
         Events.Add(@event);
      }
   }
}
