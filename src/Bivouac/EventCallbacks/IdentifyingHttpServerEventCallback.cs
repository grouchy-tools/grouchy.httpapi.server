using System;
using Bivouac.Abstractions;

namespace Bivouac.EventCallbacks
{
   public class IdentifyingHttpServerEventCallback : IHttpServerEventCallback
   {
      private readonly IGetRequestId _requestIdGetter;
      private readonly IGetCorrelationId _correlationIdGetter;
      private readonly IGetServiceName _serviceNameGetter;
      private readonly IGetServiceVersion _serviceVersionGetter;

      public IdentifyingHttpServerEventCallback(
         IGetRequestId requestIdGetter,
         IGetCorrelationId correlationIdGetter,
         IGetServiceName serviceNameGetter,
         IGetServiceVersion serviceVersionGetter)
      {
         _requestIdGetter = requestIdGetter ?? throw new ArgumentNullException(nameof(requestIdGetter));
         _correlationIdGetter = correlationIdGetter ?? throw new ArgumentNullException(nameof(correlationIdGetter));
         _serviceNameGetter = serviceNameGetter ?? throw new ArgumentNullException(nameof(serviceNameGetter));
         _serviceVersionGetter = serviceVersionGetter ?? throw new ArgumentNullException(nameof(serviceVersionGetter));
      }

      public void Invoke(IHttpServerEvent @event)
      {
         AddTag(@event, "request-id", _requestIdGetter.Get);
         AddTag(@event, "correlation-id", _correlationIdGetter.Get);
         AddTag(@event, "service", _serviceNameGetter.Get);
         AddTag(@event, "version", _serviceVersionGetter.Get);
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