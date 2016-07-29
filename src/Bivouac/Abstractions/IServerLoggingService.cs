namespace Bivouac.Abstractions
{
   using System;
   using Microsoft.AspNetCore.Http;

   public interface IServerLoggingService
   {
      void LogRequest(HttpContext context);

      void LogResponse(HttpContext context, long duration);

      void LogError(HttpContext context, Exception e);
   }
}