using System;
using System.Linq;
using System.Threading.Tasks;
using Bivouac.Abstractions;
using Bivouac.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Bivouac.Extensions
{
   public static class ApplicationBuilderExtensions
   {
      private const string GET = "GET";
      private const string POST = "POST";

      public static IApplicationBuilder UseDefaultMiddleware(this IApplicationBuilder app)
      {
         if (app == null) throw new ArgumentNullException(nameof(app));

         return app
            .UseMiddleware<ServerLoggingMiddleware>()
            .UseMiddleware<PingEndpointMiddleware>()
            .UseMiddleware<StatusEndpointMiddleware>();
      }
      
      [Obsolete]
      public static void Map(this IApplicationBuilder app, string path, string method, Func<HttpContext, Task> handler)
      {
         app.MapWhen(
            context => context.Request.Path.Value == path && context.Request.Method == method,
            builder =>
            {
               RequestDelegate rd = context => handler(context); 
               builder.Run(rd);
            });
      }

      [Obsolete]
      public static void HandleGet<TApiHandler>(this IApplicationBuilder app, string path)
         where TApiHandler : IApiHandler
      {
         Map(app, path, GET, CreateHandler<TApiHandler>);
      }

      [Obsolete]
      public static void HandlePost<TApiHandler>(this IApplicationBuilder app, string path)
         where TApiHandler : IApiHandler
      {
         Map(app, path, POST, CreateHandler<TApiHandler>);
      }

      // TODO: Use ActivatorUtilities.CreateInstance instead of manually calling constructor using reflection
      private static async Task CreateHandler<TApiHandler>(HttpContext context)
         where TApiHandler : IApiHandler
      {
         var c = typeof(TApiHandler).GetConstructors().Single();
         var parameters = c.GetParameters();
         var args = new object[parameters.Length];

         // TODO: This needs much more work
         for (var i = 0; i < parameters.Length; i++)
         {
            args[i] = context.RequestServices.GetRequiredService(parameters[i].ParameterType);
         }

         var apiHandler = (TApiHandler)c.Invoke(args);

         await apiHandler.Handle(context);
      }
   }
}