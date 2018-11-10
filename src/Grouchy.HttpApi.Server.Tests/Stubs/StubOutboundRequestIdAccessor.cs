using System;
using Grouchy.HttpApi.Client.Abstractions.Tagging;

namespace Grouchy.HttpApi.Server.Tests
{
   public class StubOutboundRequestIdAccessor : IOutboundRequestIdAccessor
   {
      public string Response { get; set; }

      public Exception Exception { get; set; }

      public string OutboundRequestId
      {
         get
         {
            if (Exception != null)
            {
               throw Exception;
            }

            return Response;
         }
         set => Response = value;
      }
   }
}