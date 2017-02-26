namespace Bivouac.Events
{
   using System;
   using System.Net.Http;
   using System.Runtime.InteropServices;
   using System.Threading;
   using System.Threading.Tasks;
   using Burble.Abstractions;
   using Bivouac.Abstractions;
   using Microsoft.Net.Http.Headers;

   /// <summary>
   /// Add correlation-id and request-id to the request header, creating a new id if necessary
   /// </summary>
   public class IdentifyingHttpClient : IHttpClient
   {
      private readonly IHttpClient _httpClient;
      private readonly IGetCorrelationId _correlationIdGetter;
      private readonly IGenerateGuids _guidGenerator;
      private readonly IGetAssemblyVersion _assemblyVersionGetter;
      private readonly string _userAgent;

      public IdentifyingHttpClient(
         IHttpClient httpClient,
         IGetCorrelationId correlationIdGetter,
         IGenerateGuids guidGenerator,
         IGetAssemblyVersion assemblyVersionGetter,
         string service,
         string environment)
      {
         if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
         if (correlationIdGetter == null) throw new ArgumentNullException(nameof(correlationIdGetter));
         if (guidGenerator == null) throw new ArgumentNullException(nameof(guidGenerator));
         if (assemblyVersionGetter == null) throw new ArgumentNullException(nameof(assemblyVersionGetter));
         if (service == null) throw new ArgumentNullException(nameof(service));

         _httpClient = httpClient;
         _correlationIdGetter = correlationIdGetter;
         _guidGenerator = guidGenerator;
         _assemblyVersionGetter = assemblyVersionGetter;
         _userAgent = BuildUserAgent(service, environment);
      }

      public Uri BaseAddress => _httpClient.BaseAddress;

      public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         request.Headers.Add(HeaderNames.UserAgent, _userAgent);
         request.Headers.Add("correlation-id", _correlationIdGetter.Get());
         request.Headers.Add("request-id", _guidGenerator.Generate().ToString());

         return _httpClient.SendAsync(request, cancellationToken);
      }

      private string BuildUserAgent(string service, string environment)
      {
         var userAgent = service;
         var version = _assemblyVersionGetter.Get();

         if (!string.IsNullOrWhiteSpace(version))
         {
            userAgent += $"/{version}";
         }

         if (!string.IsNullOrWhiteSpace(environment))
         {
            userAgent += $" {environment}";
         }

         userAgent += $" ({RuntimeInformation.OSDescription.Trim()})";

         return userAgent;
      }
   }
}