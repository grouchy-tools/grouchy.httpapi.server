using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grouchy.HttpApi.Client.Abstractions.EventCallbacks;
using Grouchy.HttpApi.Client.Abstractions.Events;
using Grouchy.HttpApi.Client.Abstractions.HttpClients;

namespace Grouchy.HttpApi.Server.Tests
{
   public class TestHttpClient : IHttpClient
   {
      private readonly HttpClient _httpClient;
      private readonly IHttpClientEventCallback[] _callbacks;

      public TestHttpClient(
         HttpClient httpClient,
         params IHttpClientEventCallback[] callbacks)
      {
         _httpClient = httpClient;
         _callbacks = callbacks;
      }

      public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         var @event = new Event { Request = request };
         foreach (var callback in _callbacks)
         {
            callback.Invoke(@event);
         }
         return _httpClient.SendAsync(request, cancellationToken);
      }

      public class Event : IHttpClientEvent
      {
         public string EventType { get; set; }
         public DateTimeOffset Timestamp { get; set; }
         public string Method { get; set; }
         public string TargetService { get; set; }
         public string Uri { get; set; }
         public IDictionary<string, object> Tags { get; } = new Dictionary<string, object>();
         public HttpRequestMessage Request { get; set; }
      }
   }
}
