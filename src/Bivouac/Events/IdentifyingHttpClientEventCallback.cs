namespace Bivouac.Events
{
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Net.Http;
   using Bivouac.Abstractions;
   using Burble.Abstractions;

   public class IdentifyingHttpClientEventCallback : IHttpClientEventCallback
   {
      private readonly IGetRequestId _requestIdGetter;
      private readonly IGetCorrelationId _correlationIdGetter;
      private readonly IGetAssemblyVersion _assemblyVersionGetter;
      private readonly IHttpClientEventCallback _next;

      public IdentifyingHttpClientEventCallback(
         IGetRequestId requestIdGetter,
         IGetCorrelationId correlationIdGetter,
         IGetAssemblyVersion assemblyVersionGetter,
         IHttpClientEventCallback next)
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

      public void Invoke(IHttpClientEvent @event)
      {
         AddTag(@event, "origin-request-id", _requestIdGetter.Get);
         AddTag(@event, "correlation-id", _correlationIdGetter.Get);
         AddTag(@event, "request-id", () => GetRequestIdFromHeader(@event.Request));         
         AddTag(@event, "version", _assemblyVersionGetter.Get);

         _next.Invoke(@event);
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
         IEnumerable<string> values;
         if (request.Headers.TryGetValues("request-id", out values))
         {
            return values.First();
         }

         return null;
      }
   }
}
