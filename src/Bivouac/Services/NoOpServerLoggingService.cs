namespace Bivouac.Services
{
   using System;
   using Bivouac.Abstractions;
   using Microsoft.AspNetCore.Http;

   public class NoOpServerLoggingService : IServerLoggingService
   {
      public void LogRequest(HttpContext context)
      {
      }

      public void LogResponse(HttpContext context, long duration)
      {
      }

      public void LogError(HttpContext context, Exception e)
      {
      }
   }
}