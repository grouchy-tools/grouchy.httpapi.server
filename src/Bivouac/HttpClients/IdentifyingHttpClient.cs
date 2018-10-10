using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Burble.Abstractions;
using Bivouac.Abstractions;
using Microsoft.Net.Http.Headers;

namespace Bivouac.HttpClients
{
   /// <summary>
   /// Add correlation-id and request-id to the request header, creating a new id if necessary
   /// </summary>
   public class IdentifyingHttpClient : IHttpClient
   {
      private readonly IHttpClient _httpClient;
      private readonly IGetCorrelationId _correlationIdGetter;
      private readonly IGenerateGuids _guidGenerator;
      private readonly IGetServiceVersion _serviceVersionGetter;
      private readonly string _userAgent;

      // TODO: Need a better environment
      public IdentifyingHttpClient(
         IHttpClient httpClient,
         IGetCorrelationId correlationIdGetter,
         IGenerateGuids guidGenerator,
         IGetServiceName serviceNameGetter,
         IGetServiceVersion serviceVersionGetter,
         string environment)
      {
         _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
         _correlationIdGetter = correlationIdGetter ?? throw new ArgumentNullException(nameof(correlationIdGetter));
         _guidGenerator = guidGenerator ?? throw new ArgumentNullException(nameof(guidGenerator));
         _serviceVersionGetter = serviceVersionGetter ?? throw new ArgumentNullException(nameof(serviceVersionGetter));
         _userAgent = BuildUserAgent(serviceNameGetter.Get(), environment);
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
         var version = _serviceVersionGetter.Get();

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