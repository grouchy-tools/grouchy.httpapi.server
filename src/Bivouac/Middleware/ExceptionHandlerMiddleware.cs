namespace Bivouac.Middleware
{
   using System;
   using System.Threading.Tasks;
   using Bivouac.Exceptions;
   using Microsoft.AspNetCore.Http;

   public class ExceptionHandlerMiddleware
   {
      private readonly RequestDelegate _next;

      public ExceptionHandlerMiddleware(RequestDelegate next)
      {
         if (next == null) throw new ArgumentNullException(nameof(next));

         _next = next;
      }

      public async Task Invoke(HttpContext context)
      {
         try
         {
            await _next(context);
         }
         catch (HttpNotFoundException e)
         {
            context.Response.StatusCode = (int)e.StatusCode;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(e.Message);
         }
         catch (Exception e)
         {
            Console.WriteLine(e.Message);
            throw;
         }
      }
   }
}
