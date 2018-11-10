using System;
using Grouchy.Abstractions;
using Grouchy.Abstractions.Tagging;
using Grouchy.HttpApi.Client.Abstractions.EventCallbacks;
using Grouchy.HttpApi.Client.Abstractions.Events;
using Grouchy.HttpApi.Client.Abstractions.Tagging;
using Grouchy.HttpApi.Server.Abstractions.Tagging;

namespace Grouchy.HttpApi.Server.EventCallbacks
{
   public class IdentifyingHttpClientEventCallback : IHttpClientEventCallback
   {
      private readonly ISessionIdAccessor _sessionIdAccessor;
      private readonly ICorrelationIdAccessor _correlationIdAccessor;
      private readonly IInboundRequestIdAccessor _inboundRequestIdAccessor;
      private readonly IOutboundRequestIdAccessor _outboundRequestIdAccessor;
      private readonly IApplicationInfo _applicationInfo;

      public IdentifyingHttpClientEventCallback(
         ISessionIdAccessor sessionIdGetter,
         ICorrelationIdAccessor correlationIdGetter,
         IInboundRequestIdAccessor inboundRequestIdGetter,
         IOutboundRequestIdAccessor outboundRequestIdGetter,
         IApplicationInfo applicationInfo)
      {
         _sessionIdAccessor = sessionIdGetter ?? throw new ArgumentNullException(nameof(sessionIdGetter));
         _correlationIdAccessor = correlationIdGetter ?? throw new ArgumentNullException(nameof(correlationIdGetter));
         _inboundRequestIdAccessor = inboundRequestIdGetter ?? throw new ArgumentNullException(nameof(inboundRequestIdGetter));
         _outboundRequestIdAccessor = outboundRequestIdGetter ?? throw new ArgumentNullException(nameof(outboundRequestIdGetter));
         _applicationInfo = applicationInfo ?? throw new ArgumentNullException(nameof(applicationInfo));
      }

      public void Invoke(IHttpClientEvent @event)
      {
         AddTag(@event, "outboundRequestId", () => _outboundRequestIdAccessor.OutboundRequestId);
         AddTag(@event, "inboundRequestId", () => _inboundRequestIdAccessor.InboundRequestId);
         AddTag(@event, "correlationId", () => _correlationIdAccessor.CorrelationId);
         AddTag(@event, "sessionId", () => _sessionIdAccessor.SessionId);
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
            // Ignore any exceptions thrown by the getters
         }
      }
   }
}
