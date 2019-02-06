using System;
using Grouchy.HttpApi.Server.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Grouchy.HttpApi.Server.Extensions
{
   public static class ApplicationBuilderExtensions
   {
      public static IApplicationBuilder UseDefaultMiddleware(this IApplicationBuilder app)
      {
         if (app == null) throw new ArgumentNullException(nameof(app));

         return app
            .UseMiddleware<AddHttpStrictTransportSecurityMiddleware>()
            .UseMiddleware<RemoveServerHeaderMiddleware>()
            .UseMiddleware<PingEndpointMiddleware>()
            .UseMiddleware<ServerLoggingMiddleware>()
            .UseMiddleware<StatusEndpointMiddleware>();
      }
   }
}