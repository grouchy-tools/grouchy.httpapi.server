namespace Bivouac.Middleware
{
   using System;
   using System.Diagnostics;
   using System.Net.Http;
   using System.Threading;
   using System.Threading.Tasks;
   using Bivouac.Abstractions;

   public class LoggingHttpClientHandler : HttpClientHandler
   {
      private const string RequestIdKey = "request-id";
      private const string CorrelationIdKey = "correlation-id";

      private readonly ILogEvents _eventLogger;
      private readonly IGenerateGuids _guidGenerator;
      private readonly IGetRequestId _requestIdGetter;
      private readonly IGetCorrelationId _correlationIdGetter;

      public LoggingHttpClientHandler(
         ILogEvents eventLogger,
         IGenerateGuids guidGenerator,
         IGetRequestId requestIdGetter,
         IGetCorrelationId correlationIdGetter)
      {
         if (eventLogger == null) throw new ArgumentNullException(nameof(eventLogger));
         if (guidGenerator == null) throw new ArgumentNullException(nameof(guidGenerator));
         if (requestIdGetter == null) throw new ArgumentNullException(nameof(requestIdGetter));
         if (correlationIdGetter == null) throw new ArgumentNullException(nameof(correlationIdGetter));

         _eventLogger = eventLogger;
         _guidGenerator = guidGenerator;
         _requestIdGetter = requestIdGetter;
         _correlationIdGetter = correlationIdGetter;
      }

      protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         if (request == null) throw new ArgumentNullException(nameof(request));

         var requestId = _guidGenerator.Generate();
         var callerRequestId = _requestIdGetter.Get();
         var correlationId = _correlationIdGetter.Get();

         _eventLogger.Log($"{{\"eventType\":\"clientRequest\",\"requestId\":\"{requestId}\",\"callerRequestId\":\"{callerRequestId}\",\"correlationId\":\"{correlationId}\",\"uri\":\"{request.RequestUri}\"}}");

         request.Headers.Add(RequestIdKey, requestId.ToString());
         request.Headers.Add(CorrelationIdKey, correlationId.ToString());

         var stopwatch = Stopwatch.StartNew();

         HttpResponseMessage response;
         int? statusCode = null;
         try
         {
            response = await base.SendAsync(request, cancellationToken);
            statusCode = (int)response.StatusCode;
         }
         finally
         {
            stopwatch.Stop();
            _eventLogger.Log($"{{\"eventType\":\"clientResponse\",\"requestId\":\"{requestId}\",\"callerRequestId\":\"{callerRequestId}\",\"correlationId\":\"{correlationId}\",\"statusCode\":\"{statusCode}\",\"duration\":\"{stopwatch.ElapsedMilliseconds}\",\"uri\":\"{request.RequestUri}\"}}");
         }

         return response;
      }
   }
}
