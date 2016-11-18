namespace Bivouac.Events
{
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Bivouac.Abstractions;
   using Burble.Abstractions;

   public class IdentifyingHttpClientEventCallback : IHttpClientEventCallback
   {
      private readonly IGetRequestId _requestIdGetter;
      private readonly IGetCorrelationId _correlationIdGetter;
      private readonly IHttpClientEventCallback _next;

      public IdentifyingHttpClientEventCallback(
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
         var requestId = SafeGetter(_requestIdGetter.Get);
         if (requestId != null)
         {
            @event.Tags.Add("origin-request-id", requestId);
         }

         var correlationId = SafeGetter(_correlationIdGetter.Get);
         if (correlationId != null)
         {
            @event.Tags.Add("correlation-id", correlationId);
         }

         IEnumerable<string> values;
         if (@event.Request.Headers.TryGetValues("request-id", out values))
         {
            @event.Tags.Add("request-id", values.First());
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
