namespace Bivouac.Tests
{
   using System;
   using Bivouac.Abstractions;

   public class StubCorrelationIdGetter : IGetCorrelationId
   {
      public string CorrelationId { get; set; }

      public Exception Exception { get; set; }

      public string Get()
      {
         if (Exception != null)
         {
            throw Exception;
         }

         return CorrelationId;
      }
   }
}