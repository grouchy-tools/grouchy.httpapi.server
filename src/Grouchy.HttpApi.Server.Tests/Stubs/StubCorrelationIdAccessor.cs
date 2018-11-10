using System;
using Grouchy.Abstractions.Tagging;

namespace Grouchy.HttpApi.Server.Tests
{
   public class StubCorrelationIdAccessor : ICorrelationIdAccessor
   {
      public string Response { get; set; }

      public Exception Exception { get; set; }

      public string CorrelationId
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