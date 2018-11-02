using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Grouchy.HttpApi.Server.Middleware
{
   public class PingEndpointMiddleware
   {
      private readonly RequestDelegate _next;

      public PingEndpointMiddleware(RequestDelegate next)
      {
         _next = next ?? throw new ArgumentNullException(nameof(next));
      }

      public async Task Invoke(HttpContext context)
      {
         if (context == null) throw new ArgumentNullException(nameof(context));

         if (context.Request.Path == "/.ping")
         {
            context.Response.StatusCode = 200;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("Pong!");
         }
         else
         {
            await _next(context);
         }
      }
   }
}