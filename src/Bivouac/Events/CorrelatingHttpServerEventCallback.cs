namespace Bivouac.Events
{
   using System;
   using System.Collections.Generic;
   using Bivouac.Abstractions;

   public class CorrelatingHttpServerEventCallback : IHttpServerEventCallback
   {
      private readonly IGetRequestId _requestIdGetter;
      private readonly IGetCorrelationId _correlationIdGetter;

      public CorrelatingHttpServerEventCallback(
         IGetRequestId requestIdGetter,
         IGetCorrelationId correlationIdGetter)
      {
         if (requestIdGetter == null) throw new ArgumentNullException(nameof(requestIdGetter));
         if (correlationIdGetter == null) throw new ArgumentNullException(nameof(correlationIdGetter));

         _requestIdGetter = requestIdGetter;
         _correlationIdGetter = correlationIdGetter;
      }

      public void Invoke(IHttpServerEvent @event)
      {
         if (@event.Tags == null)
         {
            @event.Tags = new Dictionary<string, object>();
         }

         var requestId = SafeGetter(_requestIdGetter.Get);
         if (requestId != null)
         {
            @event.Tags.Add("requestId", requestId);
         }

         var correlationId = SafeGetter(_correlationIdGetter.Get);
         if (correlationId != null)
         {
            @event.Tags.Add("correlationId", correlationId);
         }
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