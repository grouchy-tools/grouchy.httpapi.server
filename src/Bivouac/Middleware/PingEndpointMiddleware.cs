namespace Bivouac.Middleware
{
   using System;
   using System.Threading.Tasks;
   using Microsoft.AspNetCore.Http;

   public class PingEndpointMiddleware
   {
      private readonly RequestDelegate _next;

      public PingEndpointMiddleware(RequestDelegate next)
      {
         if (next == null) throw new ArgumentNullException(nameof(next));

         _next = next;
      }

      public async Task Invoke(HttpContext context)
      {
         if (context == null) throw new ArgumentNullException(nameof(context));

         if (context.Request.Path == "/ping")
         {
            context.Response.StatusCode = 200;
            await context.Response.WriteAsync("Pong!");
         }
         else
         {
            await _next(context);
         }
      }
   }
}