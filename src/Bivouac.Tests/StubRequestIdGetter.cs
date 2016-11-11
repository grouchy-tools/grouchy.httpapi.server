namespace Bivouac.Tests
{
   using System;
   using Bivouac.Abstractions;

   public class StubRequestIdGetter : IGetRequestId
   {
      public string RequestId { get; set; }

      public Exception Exception { get; set; }

      public string Get()
      {
         if (Exception != null)
         {
            throw Exception;
         }

         return RequestId;
      }
   }
}
