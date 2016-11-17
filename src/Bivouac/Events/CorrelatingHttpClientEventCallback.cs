namespace Bivouac.Events
{
   using System;
   using System.Collections.Generic;
   using Bivouac.Abstractions;
   using Burble.Abstractions;

   public class CorrelatingHttpClientEventCallback : IHttpClientEventCallback
   {
      private readonly IGetRequestId _requestIdGetter;
      private readonly IGetCorrelationId _correlationIdGetter;
      private readonly IHttpClientEventCallback _next;

      public CorrelatingHttpClientEventCallback(
         IGetRequestId requestIdGetter,
         IGetCorrelationId correlationIdGetter,
         IHttpClientEventCallback next)
      {
         if (requestIdGetter == null) throw new ArgumentNullException(nameof(requestIdGetter));
         if (correlationIdGetter == null) throw new ArgumentNullException(nameof(correlationIdGetter));
         if (next == null) throw new ArgumentNullException(nameof(next));

         _requestIdGetter = requestIdGetter;
         _correlationIdGetter = correlationIdGetter;
         _next = next;
      }

      public void Invoke(IHttpClientEvent @event)
      {
         if (@event.Tags == null)
         {
            @event.Tags = new Dictionary<string, object>();
         }

         var requestId = SafeGetter(_requestIdGetter.Get);
         if (requestId != null)
         {
            @event.Tags.Add("originRequestId", requestId);
         }

         var correlationId = SafeGetter(_correlationIdGetter.Get);
         if (correlationId != null)
         {
            @event.Tags.Add("correlationId", correlationId);
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
