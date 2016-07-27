namespace Bivouac.Middleware
{
   using System;
   using System.Diagnostics;
   using System.Net;
   using System.Threading.Tasks;
   using Bivouac.Abstractions;
   using Bivouac.Exceptions;
   using Microsoft.AspNetCore.Http;

   public class ServerLoggingMiddleware
   {
      private readonly RequestDelegate _next;
      private readonly ILogEvents _eventLogger;
      private readonly IGetRequestId _requestIdGetter;
      private readonly IGetCorrelationId _correlationIdGetter;

      public ServerLoggingMiddleware(
         RequestDelegate next,
         ILogEvents eventLogger,
         IGetRequestId requestIdGetter,
         IGetCorrelationId correlationIdGetter)
      {
         if (next == null) throw new ArgumentNullException(nameof(next));
         if (eventLogger == null) throw new ArgumentNullException(nameof(eventLogger));
         if (requestIdGetter == null) throw new ArgumentNullException(nameof(requestIdGetter));
         if (correlationIdGetter == null) throw new ArgumentNullException(nameof(correlationIdGetter));

         _next = next;
         _eventLogger = eventLogger;
         _requestIdGetter = requestIdGetter;
         _correlationIdGetter = correlationIdGetter;
      }

      public async Task Invoke(HttpContext context)
      {
         if (context == null) throw new ArgumentNullException(nameof(context));

         var stopwatch = Stopwatch.StartNew();

         var requestId = SafeGetter(_requestIdGetter.Get);
         var correlationId = SafeGetter(_correlationIdGetter.Get);

         SafeLog($"{{\"eventType\":\"serverRequest\",\"requestId\":\"{requestId}\",\"correlationId\":\"{correlationId}\",\"uri\":\"{context.Request.Path}\"}}");

         try
         {
            await _next.Invoke(context);
         }
         catch (HttpException e)
         {
            await WriteResponse(context, e.StatusCode, e.Message);
         }
         catch (Exception e)
         {
            await WriteResponse(context, HttpStatusCode.InternalServerError, "FAIL!");

            SafeLog($"{{\"eventType\":\"serverError\",\"requestId\":\"{requestId}\",\"correlationId\":\"{correlationId}\",\"uri\":\"{context.Request.Path}\",\"exceptionType\":\"{e.GetType().FullName}\",\"message\":\"{e.Message}\"}}");
            // TODO: Log stack trace out through debug/console
         }

         stopwatch.Stop();
         SafeLog($"{{\"eventType\":\"serverResponse\",\"requestId\":\"{requestId}\",\"correlationId\":\"{correlationId}\",\"statusCode\":\"{context.Response.StatusCode}\",\"duration\":\"{stopwatch.ElapsedMilliseconds}\",\"uri\":\"{context.Request.Path}\"}}");
      }

      private async Task WriteResponse(HttpContext context, HttpStatusCode statusCode, string plainText)
      {
         context.Response.StatusCode = (int)statusCode;
         context.Response.ContentType = "text/plain";
         await context.Response.WriteAsync(plainText);
      }

      private Guid SafeGetter(Func<Guid> getter)
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

      private void SafeLog(string json)
      {
         try
         {
            _eventLogger.Log(json);
         }
         catch
         {
            // Swallow logging exception
         }
      }
   }
}