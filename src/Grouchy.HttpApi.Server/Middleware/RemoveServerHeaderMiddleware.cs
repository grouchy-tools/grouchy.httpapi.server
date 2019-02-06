using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Grouchy.HttpApi.Server.Middleware
{
   public class RemoveServerHeaderMiddleware
   {
      private readonly RequestDelegate _next;

      public RemoveServerHeaderMiddleware(RequestDelegate next)
      {
         if (next == null) throw new ArgumentNullException(nameof(next));

         _next = next;
      }

      public async Task Invoke(HttpContext context)
      {
         if (context == null) throw new ArgumentNullException(nameof(context));

         if (context.Response.Headers.ContainsKey("Server"))
         {
            context.Response.Headers.Remove("Server");
         }

         await _next(context);
      }
   }
}