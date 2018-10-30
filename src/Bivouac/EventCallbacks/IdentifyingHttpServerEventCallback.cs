using System;
using Bivouac.Abstractions;
using Burble.Abstractions;
using Burble.Abstractions.Identifying;

namespace Bivouac.EventCallbacks
{
   public class IdentifyingHttpServerEventCallback : IHttpServerEventCallback
   {
      private readonly IGetRequestId _requestIdGetter;
      private readonly IGetCorrelationId _correlationIdGetter;
      private readonly IApplicationInfo _applicationInfo;

      public IdentifyingHttpServerEventCallback(
         IGetRequestId requestIdGetter,
         IGetCorrelationId correlationIdGetter,
         IApplicationInfo applicationInfo)
      {
         _requestIdGetter = requestIdGetter ?? throw new ArgumentNullException(nameof(requestIdGetter));
         _correlationIdGetter = correlationIdGetter ?? throw new ArgumentNullException(nameof(correlationIdGetter));
         _applicationInfo = applicationInfo ?? throw new ArgumentNullException(nameof(applicationInfo));
      }

      public void Invoke(IHttpServerEvent @event)
      {
         AddTag(@event, "request-id", _requestIdGetter.Get);
         AddTag(@event, "correlation-id", _correlationIdGetter.Get);
         AddTag(@event, "service", () => _applicationInfo.Name);
         AddTag(@event, "version", () => _applicationInfo.Version);
      }

      private static void AddTag(IHttpServerEvent @event, string key, Func<string> valueGetter)
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
   }
}