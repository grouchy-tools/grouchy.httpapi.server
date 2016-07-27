namespace Bivouac.Tests
{
   using System;
   using Bivouac.Abstractions;

   public class StubRequestIdGetter : IGetRequestId
   {
      public Guid RequestId { get; set; }

      public Exception Exception { get; set; }

      public Guid Get()
      {
         if (Exception != null)
         {
            throw Exception;
         }

         return RequestId;
      }
   }
}
