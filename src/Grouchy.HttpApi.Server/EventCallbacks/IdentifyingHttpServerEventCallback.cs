using System;
using Grouchy.Abstractions;
using Grouchy.Abstractions.Tagging;
using Grouchy.HttpApi.Server.Abstractions.EventCallbacks;
using Grouchy.HttpApi.Server.Abstractions.Events;
using Grouchy.HttpApi.Server.Abstractions.Tagging;

namespace Grouchy.HttpApi.Server.EventCallbacks
{
   public class IdentifyingHttpServerEventCallback : IHttpServerEventCallback
   {
      private readonly ISessionIdAccessor _sessionIdAccessor;
      private readonly ICorrelationIdAccessor _correlationIdAccessor;
      private readonly IInboundRequestIdAccessor _inboundRequestIdAccessor;
      private readonly IApplicationInfo _applicationInfo;

      public IdentifyingHttpServerEventCallback(
         ISessionIdAccessor sessionIdAccessor,
         ICorrelationIdAccessor correlationIdAccessor,
         IInboundRequestIdAccessor inboundRequestIdAccessor,
         IApplicationInfo applicationInfo)
      {
         _sessionIdAccessor = sessionIdAccessor ?? throw new ArgumentNullException(nameof(sessionIdAccessor));
         _correlationIdAccessor = correlationIdAccessor ?? throw new ArgumentNullException(nameof(correlationIdAccessor));
         _inboundRequestIdAccessor = inboundRequestIdAccessor ?? throw new ArgumentNullException(nameof(inboundRequestIdAccessor));
         _applicationInfo = applicationInfo ?? throw new ArgumentNullException(nameof(applicationInfo));
      }

      public void Invoke(IHttpServerEvent @event)
      {
         AddTag(@event, "requestId", () => _inboundRequestIdAccessor.InboundRequestId);
         AddTag(@event, "correlationId", () => _correlationIdAccessor.CorrelationId);
         AddTag(@event, "sessionId", () => _sessionIdAccessor.SessionId);
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
            // Ignore any exceptions thrown by the getters
         }
      }
   }
}