namespace Bivouac.Tests
{
   using System;
   using System.Collections.Generic;
   using System.Net.Http;
   using System.Threading;
   using System.Threading.Tasks;
   using Burble.Abstractions;

   public class TestHttpClient : IHttpClient
   {
      private readonly HttpClient _httpClient;
      private readonly IHttpClientEventCallback _callback;

      public TestHttpClient(
         HttpClient httpClient,
         IHttpClientEventCallback callback)
      {
         _httpClient = httpClient;
         _callback = callback;
      }

      public Uri BaseAddress => _httpClient.BaseAddress;

      public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         _callback?.Invoke(new Event { Request = request });
         return _httpClient.SendAsync(request, cancellationToken);
      }

      public class Event : IHttpClientEvent
      {
         public string EventType { get; set; }
         public DateTimeOffset Timestamp { get; set; }
         public string Uri { get; set; }
         public string Method { get; set; }
         public IDictionary<string, object> Tags { get; } = new Dictionary<string, object>();
         public HttpRequestMessage Request { get; set; }
      }
   }
}
