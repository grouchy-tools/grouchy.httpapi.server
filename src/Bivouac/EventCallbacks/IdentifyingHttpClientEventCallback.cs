using System;
using System.Linq;
using System.Net.Http;
using Bivouac.Abstractions;
using Burble.Abstractions;
using Burble.Abstractions.Identifying;

namespace Bivouac.EventCallbacks
{
   public class IdentifyingHttpClientEventCallback : IHttpClientEventCallback
   {
      private readonly IGetRequestId _requestIdGetter;
      private readonly IGetCorrelationId _correlationIdGetter;
      private readonly IApplicationInfo _applicationInfo;

      public IdentifyingHttpClientEventCallback(
         IGetRequestId requestIdGetter,
         IGetCorrelationId correlationIdGetter,
         IApplicationInfo applicationInfo)
      {
         _requestIdGetter = requestIdGetter ?? throw new ArgumentNullException(nameof(requestIdGetter));
         _correlationIdGetter = correlationIdGetter ?? throw new ArgumentNullException(nameof(correlationIdGetter));
         _applicationInfo = applicationInfo ?? throw new ArgumentNullException(nameof(applicationInfo));
      }

      public void Invoke(IHttpClientEvent @event)
      {
         AddTag(@event, "upstream-request-id", _requestIdGetter.Get);
         AddTag(@event, "correlation-id", _correlationIdGetter.Get);
         AddTag(@event, "request-id", () => GetRequestIdFromHeader(@event.Request));         
         AddTag(@event, "service", () => _applicationInfo.Name);
         AddTag(@event, "version", () => _applicationInfo.Version);
      }

      private static void AddTag(IHttpClientEvent @event, string key, Func<string> valueGetter)
      {
         try
         {
            var value = valueGetter();
            if (value != null)
            {
               @event.Tags.Add(key, value);
            }
         }
         catch
         {
            // Ignore any exceptions
         }
      }

      private static string GetRequestIdFromHeader(HttpRequestMessage request)
      {
         if (request == null) return null;

         if (request.Headers.TryGetValues("request-id", out var values))
         {
            return values.First();
         }

         return null;
      }
   }
}
