using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Grouchy.HttpApi.Server.Middleware
{
   public class AddHttpStrictTransportSecurityMiddleware
   {
      private readonly RequestDelegate _next;

      public AddHttpStrictTransportSecurityMiddleware(RequestDelegate next)
      {
         if (next == null) throw new ArgumentNullException(nameof(next));

         _next = next;
      }

      public async Task Invoke(HttpContext context)
      {
         if (context == null) throw new ArgumentNullException(nameof(context));

         context.Response.Headers.Add("Strict-Transport-Security", new[] { "max-age=31536000; includeSubDomains" });

         await _next(context);
      }
   }
}