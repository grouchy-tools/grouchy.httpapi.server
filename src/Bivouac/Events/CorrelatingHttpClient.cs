namespace Bivouac.Events
{
   using System;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Burble.Abstractions;
   using Bivouac.Abstractions;

   /// <summary>
   /// Add correlation-id to the request header, creating a new id if necessary
   /// </summary>
   public class CorrelatingHttpClient : IHttpClient
   {
      private readonly IHttpClient _httpClient;
      private readonly IGetCorrelationId _correlationIdGetter;

      public CorrelatingHttpClient(
         IHttpClient httpClient,
         IGetCorrelationId correlationIdGetter)
      {
         if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
         if (correlationIdGetter == null) throw new ArgumentNullException(nameof(correlationIdGetter));

         _httpClient = httpClient;
         _correlationIdGetter = correlationIdGetter;
      }

      public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
      {
         request.Headers.Add("correlation-id", _correlationIdGetter.Get());

         return _httpClient.SendAsync(request);
      }
   }
}