namespace Bivouac.Middleware
{
   using System;
   using Microsoft.AspNetCore.Builder;

   public static class ApplicationBuilderExtensions
   {
      public static IApplicationBuilder UseServerLoggingMiddleware(this IApplicationBuilder app)
      {
         if (app == null) throw new ArgumentNullException(nameof(app));

         return app.UseMiddleware<ServerLoggingMiddleware>();
      }
   }
}