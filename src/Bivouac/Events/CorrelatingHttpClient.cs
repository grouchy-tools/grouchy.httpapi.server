namespace Bivouac.Events
{
   using System;
   using System.Net.Http;
   using System.Threading.Tasks;
   using Burble.Abstractions;
   using Bivouac.Abstractions;

   /// <summary>
   /// Add correlation-id and request-id to the request header, creating a new id if necessary
   /// </summary>
   public class CorrelatingHttpClient : IHttpClient
   {
      private readonly IHttpClient _httpClient;
      private readonly IGetCorrelationId _correlationIdGetter;
      private readonly IGenerateGuids _guidGenerator;

      public CorrelatingHttpClient(
         IHttpClient httpClient,
         IGetCorrelationId correlationIdGetter,
         IGenerateGuids guidGenerator)
      {
         if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
         if (correlationIdGetter == null) throw new ArgumentNullException(nameof(correlationIdGetter));
         if (guidGenerator == null) throw new ArgumentNullException(nameof(guidGenerator));

         _httpClient = httpClient;
         _correlationIdGetter = correlationIdGetter;
         _guidGenerator = guidGenerator;
      }

      public Uri BaseAddress => _httpClient.BaseAddress;

      public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
      {
         request.Headers.Add("correlation-id", _correlationIdGetter.Get());
         request.Headers.Add("request-id", _guidGenerator.Generate().ToString());

         return _httpClient.SendAsync(request);
      }
   }
}