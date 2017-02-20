namespace Bivouac.Events
{
   using System;
   using Bivouac.Abstractions;

   public class IdentifyingHttpServerEventCallback : IHttpServerEventCallback
   {
      private readonly IGetRequestId _requestIdGetter;
      private readonly IGetCorrelationId _correlationIdGetter;
      private readonly IGetAssemblyVersion _assemblyVersionGetter;
      private readonly IHttpServerEventCallback _next;

      public IdentifyingHttpServerEventCallback(
         IGetRequestId requestIdGetter,
         IGetCorrelationId correlationIdGetter,
         IGetAssemblyVersion assemblyVersionGetter,
         IHttpServerEventCallback next)
      {
         if (requestIdGetter == null) throw new ArgumentNullException(nameof(requestIdGetter));
         if (correlationIdGetter == null) throw new ArgumentNullException(nameof(correlationIdGetter));
         if (assemblyVersionGetter == null) throw new ArgumentNullException(nameof(assemblyVersionGetter));
         if (next == null) throw new ArgumentNullException(nameof(next));

         _requestIdGetter = requestIdGetter;
         _correlationIdGetter = correlationIdGetter;
         _assemblyVersionGetter = assemblyVersionGetter;
         _next = next;
      }

      public void Invoke(IHttpServerEvent @event)
      {
         AddTag(@event, "request-id", _requestIdGetter.Get);
         AddTag(@event, "correlation-id", _correlationIdGetter.Get);
         AddTag(@event, "version", _assemblyVersionGetter.Get);

         _next.Invoke(@event);
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