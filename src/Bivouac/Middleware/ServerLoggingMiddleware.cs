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
      private readonly IServerLoggingService _serverLoggingService;

      public ServerLoggingMiddleware(
         RequestDelegate next,
         IServerLoggingService serverLoggingService)
      {
         if (next == null) throw new ArgumentNullException(nameof(next));
         if (serverLoggingService == null) throw new ArgumentNullException(nameof(serverLoggingService));

         _next = next;
         _serverLoggingService = serverLoggingService;
      }

      public async Task Invoke(HttpContext context)
      {
         if (context == null) throw new ArgumentNullException(nameof(context));

         var stopwatch = Stopwatch.StartNew();
         SafeInvoke(() => _serverLoggingService.LogRequest(context));

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

            SafeInvoke(() => _serverLoggingService.LogError(context, e));
            // TODO: Log stack trace out through debug/console
         }

         stopwatch.Stop();
         SafeInvoke(() => _serverLoggingService.LogResponse(context, stopwatch.ElapsedMilliseconds));
      }

      private static async Task WriteResponse(HttpContext context, HttpStatusCode statusCode, string plainText)
      {
         context.Response.StatusCode = (int)statusCode;
         context.Response.ContentType = "text/plain";
         await context.Response.WriteAsync(plainText);
      }

      private static void SafeInvoke(Action action)
      {
         try
         {
            action();
         }
         catch
         {
            // Swallow exceptions
         }
      }
   }
}