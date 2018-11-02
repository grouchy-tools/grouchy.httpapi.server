using System;
using Grouchy.Resilience.Abstractions.CircuitBreaking;

namespace Grouchy.HttpApi.Server.Tests
{
   public class StubCircuitBreakerState : ICircuitBreakerState
   {
      public bool ShouldAccept()
      {
         throw new NotImplementedException();
      }

      public void LogSuccessResponse(string key = null)
      {
         throw new NotImplementedException();
      }

      public void LogFailureResponse(string key = null)
      {
         throw new NotImplementedException();
      }

      public void LogTimeout()
      {
         throw new NotImplementedException();
      }

      public void LogException(Exception exception)
      {
         throw new NotImplementedException();
      }

      public void LogWithheld()
      {
         throw new NotImplementedException();
      }

      public double ClosedPct { get; }
   }
}