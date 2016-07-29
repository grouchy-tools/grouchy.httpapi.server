namespace Bivouac.Services
{
   using System;
   using Bivouac.Abstractions;
   using Microsoft.AspNetCore.Http;
   using Microsoft.Extensions.Primitives;
   using Microsoft.Net.Http.Headers;

   public class ServerLoggingService : IServerLoggingService
   {
      private readonly ILogEvents _eventLogger;
      private readonly IGetRequestId _requestIdGetter;
      private readonly IGetCorrelationId _correlationIdGetter;

      public ServerLoggingService(
         ILogEvents eventLogger,
         IGetRequestId requestIdGetter,
         IGetCorrelationId correlationIdGetter)
      {
         if (eventLogger == null) throw new ArgumentNullException(nameof(eventLogger));
         if (requestIdGetter == null) throw new ArgumentNullException(nameof(requestIdGetter));
         if (correlationIdGetter == null) throw new ArgumentNullException(nameof(correlationIdGetter));

         _eventLogger = eventLogger;
         _requestIdGetter = requestIdGetter;
         _correlationIdGetter = correlationIdGetter;
      }

      public void LogRequest(HttpContext context)
      {
         var requestId = SafeGetter(_requestIdGetter.Get);
         var correlationId = SafeGetter(_correlationIdGetter.Get);
         var userAgentProperty = GetUserAgentProperty(context);

         _eventLogger.Log($"{{\"eventType\":\"serverRequest\",\"requestId\":\"{requestId}\",\"correlationId\":\"{correlationId}\",\"method\":\"{context.Request.Method}\",\"uri\":\"{context.Request.Path + context.Request.QueryString}\"{userAgentProperty}}}");
      }

      public void LogResponse(HttpContext context, long duration)
      {
         var requestId = SafeGetter(_requestIdGetter.Get);
         var correlationId = SafeGetter(_correlationIdGetter.Get);

         _eventLogger.Log($"{{\"eventType\":\"serverResponse\",\"requestId\":\"{requestId}\",\"correlationId\":\"{correlationId}\",\"statusCode\":\"{(int)context.Response.StatusCode}\",\"duration\":\"{duration}\",\"method\":\"{context.Request.Method}\",\"uri\":\"{context.Request.Path + context.Request.QueryString}\"}}");
      }

      public void LogError(HttpContext context, Exception e)
      {
         var requestId = SafeGetter(_requestIdGetter.Get);
         var correlationId = SafeGetter(_correlationIdGetter.Get);

         _eventLogger.Log($"{{\"eventType\":\"serverError\",\"requestId\":\"{requestId}\",\"correlationId\":\"{correlationId}\",\"method\":\"{context.Request.Method}\",\"uri\":\"{context.Request.Path + context.Request.QueryString}\",\"exceptionType\":\"{e.GetType().FullName}\",\"message\":\"{e.Message}\"}}");
      }

      private static Guid SafeGetter(Func<Guid> getter)
      {
         try
         {
            return getter();
         }
         catch
         {
            return Guid.Empty;
         }
      }

      private static string GetUserAgentProperty(HttpContext context)
      {
         StringValues userAgentHeader;
         if (!context.Request.Headers.TryGetValue(HeaderNames.UserAgent, out userAgentHeader))
         {
            return null;
         }
         return $",\"userAgent\":\"{userAgentHeader[0]}\"";
      }
   }
}