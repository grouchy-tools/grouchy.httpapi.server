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

      public ServerLoggingMiddleware(RequestDelegate next)
      {
         if (next == null) throw new ArgumentNullException(nameof(next));

         _next = next;
      }

      public async Task Invoke(HttpContext context, IHttpServerEventCallback callback)
      {
         if (context == null) throw new ArgumentNullException(nameof(context));
         if (callback == null) throw new ArgumentNullException(nameof(callback));

         var stopwatch = Stopwatch.StartNew();
         EventCallback(callback, () => HttpServerRequest.Create(context));

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

            EventCallback(callback, () => HttpServerException.Create(context, e));
         }

         stopwatch.Stop();
         EventCallback(callback, () => HttpServerResponse.Create(context, stopwatch.ElapsedMilliseconds));
      }

      private static async Task WriteResponse(HttpContext context, HttpStatusCode statusCode, string plainText)
      {
         context.Response.StatusCode = (int)statusCode;
         context.Response.ContentType = "text/plain";
         await context.Response.WriteAsync(plainText);
      }

      private static void EventCallback<TEvent>(IHttpServerEventCallback callback, Func<TEvent> eventFactory) where TEvent : IHttpServerEvent
      {
         try
         {
            var @event = eventFactory();
            callback.Invoke(@event);
         }
         catch
         {
            // Swallow exceptions
         }
      }
   }
}