namespace Bivouac.Events
{
   using System;
   using System.Collections.Generic;
   using Bivouac.Abstractions;

   public class CorrelatingHttpServerEventCallback : IHttpServerEventCallback
   {
      private readonly IGetRequestId _requestIdGetter;
      private readonly IGetCorrelationId _correlationIdGetter;
      private readonly IHttpServerEventCallback _next;

      public CorrelatingHttpServerEventCallback(
         IGetRequestId requestIdGetter,
         IGetCorrelationId correlationIdGetter,
         IHttpServerEventCallback next)
      {
         if (requestIdGetter == null) throw new ArgumentNullException(nameof(requestIdGetter));
         if (correlationIdGetter == null) throw new ArgumentNullException(nameof(correlationIdGetter));
         if (next == null) throw new ArgumentNullException(nameof(next));

         _requestIdGetter = requestIdGetter;
         _correlationIdGetter = correlationIdGetter;
         _next = next;
      }

      public void Invoke(IHttpServerEvent @event)
      {
         var requestId = SafeGetter(_requestIdGetter.Get);
         if (requestId != null)
         {
            @event.Tags.Add("request-id", requestId);
         }

         var correlationId = SafeGetter(_correlationIdGetter.Get);
         if (correlationId != null)
         {
            @event.Tags.Add("correlation-id", correlationId);
         }

         _next.Invoke(@event);
      }

      private static string SafeGetter(Func<string> getter)
      {
         try
         {
            return getter();
         }
         catch
         {
            return null;
         }
      }
   }
}