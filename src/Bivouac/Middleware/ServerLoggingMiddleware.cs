namespace Bivouac.Middleware
{
   using System;
   using System.Diagnostics;
   using System.Net;
   using System.Threading.Tasks;
   using Bivouac.Abstractions;
   using Bivouac.Events;
   using Bivouac.Exceptions;
   using Microsoft.AspNetCore.Http;

   public class ServerLoggingMiddleware
   {
      private readonly RequestDelegate _next;
      private readonly IGetRequestId _requestIdGetter;
      private readonly IHttpServerEventCallback _callback;

      public ServerLoggingMiddleware(
         RequestDelegate next,
         IGetRequestId requestIdGetter,
         IHttpServerEventCallback callback)
      {
         if (next == null) throw new ArgumentNullException(nameof(next));
         if (requestIdGetter == null) throw new ArgumentNullException(nameof(requestIdGetter));
         if (callback == null) throw new ArgumentNullException(nameof(callback));

         _next = next;
         _requestIdGetter = requestIdGetter;
         _callback = callback;
      }

      public async Task Invoke(HttpContext context)
      {
         if (context == null) throw new ArgumentNullException(nameof(context));

         var requestId = SafeGetter(_requestIdGetter.Get);
         var stopwatch = Stopwatch.StartNew();
         EventCallback(() => HttpServerRequest.Create(requestId, context));

         try
         {
            await _next(context);
         }
         catch (HttpException e)
         {
            await WriteResponse(context, e.StatusCode, e.Message);
         }
         catch (Exception e)
         {
            await WriteResponse(context, HttpStatusCode.InternalServerError, "FAIL!");

            EventCallback(() => HttpServerException.Create(requestId, context, e));
         }

         stopwatch.Stop();
         EventCallback(() => HttpServerResponse.Create(requestId, context, stopwatch.ElapsedMilliseconds));
      }

      private static async Task WriteResponse(HttpContext context, HttpStatusCode statusCode, string plainText)
      {
         context.Response.StatusCode = (int)statusCode;
         context.Response.ContentType = "text/plain";
         await context.Response.WriteAsync(plainText);
      }

      private void EventCallback<TEvent>(Func<TEvent> eventFactory) where TEvent : IHttpServerEvent
      {
         try
         {
            var @event = eventFactory();
            _callback.Invoke(@event);
         }
         catch
         {
            // Swallow exceptions
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