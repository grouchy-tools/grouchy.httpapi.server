namespace Bivouac.Tests
{
   using System;
   using Bivouac.Abstractions;

   public class StubCorrelationIdGetter : IGetCorrelationId
   {
      public Guid CorrelationId { get; set; }

      public Exception Exception { get; set; }

      public Guid Get()
      {
         if (Exception != null)
         {
            throw Exception;
         }

         return CorrelationId;
      }
   }
}