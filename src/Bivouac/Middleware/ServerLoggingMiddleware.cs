using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Bivouac.Abstractions;
using Bivouac.Events;
using Bivouac.Exceptions;
using Bivouac.Extensions;
using Microsoft.AspNetCore.Http;

namespace Bivouac.Middleware
{
   public class ServerLoggingMiddleware
   {
      private readonly RequestDelegate _next;

      public ServerLoggingMiddleware(RequestDelegate next)
      {
         _next = next ?? throw new ArgumentNullException(nameof(next));
      }

      public async Task Invoke(HttpContext context, IEnumerable<IHttpServerEventCallback> callbacks)
      {
         if (context == null) throw new ArgumentNullException(nameof(context));
         if (callbacks == null) throw new ArgumentNullException(nameof(callbacks));

         var stopwatch = Stopwatch.StartNew();
         callbacks.Invoke(() => HttpServerRequest.Create(context));

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

            callbacks.Invoke(() => HttpServerException.Create(context, e));
         }

         stopwatch.Stop();
         callbacks.Invoke(() => HttpServerResponse.Create(context, stopwatch.ElapsedMilliseconds));
      }

      private static async Task WriteResponse(HttpContext context, HttpStatusCode statusCode, string plainText)
      {
         context.Response.StatusCode = (int)statusCode;
         context.Response.ContentType = "text/plain";
         await context.Response.WriteAsync(plainText);
      }
   }
}