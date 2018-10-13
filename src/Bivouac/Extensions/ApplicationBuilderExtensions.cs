using System;
using Bivouac.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Bivouac.Extensions
{
   public static class ApplicationBuilderExtensions
   {
      public static IApplicationBuilder UseDefaultMiddleware(this IApplicationBuilder app)
      {
         if (app == null) throw new ArgumentNullException(nameof(app));

         return app
            .UseMiddleware<PingEndpointMiddleware>()
            .UseMiddleware<ServerLoggingMiddleware>()
            .UseMiddleware<StatusEndpointMiddleware>();
      }
   }
}