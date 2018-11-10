using System;
using Grouchy.HttpApi.Server.Abstractions.Tagging;

namespace Grouchy.HttpApi.Server.Tests
{
   public class StubInboundRequestIdAccessor : IInboundRequestIdAccessor
   {
      public string Response { get; set; }

      public Exception Exception { get; set; }

      public string InboundRequestId
      {
         get
         {
            if (Exception != null)
            {
               throw Exception;
            }

            return Response;
         }
      }
   }
}
