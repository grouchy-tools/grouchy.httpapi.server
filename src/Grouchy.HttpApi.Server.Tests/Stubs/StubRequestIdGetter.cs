namespace Grouchy.HttpApi.Server.Tests
{
   using System;
   using Grouchy.HttpApi.Server.Abstractions;

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
