using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Bivouac.Tests
{
    public static class ApplicationBuilderExtensions
    {
        public static void Map(this IApplicationBuilder app, string path, string method, Func<HttpContext, Task> handler)
        {
            app.MapWhen(
                context => context.Request.Path.Value == path && context.Request.Method == method,
                builder =>
                {
                    Task Delegate(HttpContext context) => handler(context);
                    
                    builder.Run(Delegate);
                });
        }   
    }
}