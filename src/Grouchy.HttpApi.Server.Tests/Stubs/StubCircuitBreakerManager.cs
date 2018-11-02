using System.Threading;
using System.Threading.Tasks;
using Grouchy.Resilience.Abstractions.CircuitBreaking;

namespace Grouchy.HttpApi.Server.Tests
{
   public class StubCircuitBreakerManager : ICircuitBreakerManager
   {
      public ICircuitBreakerState State { get; set; } = new StubCircuitBreakerState();
      
      public void Register(
         string policy,
         ICircuitBreakerAnalyser circuitBreakerAnalyser,
         ICircuitBreakerOpeningRates circuitBreakerOpeningRates,
         ICircuitBreakerPeriod circuitBreakerPeriod)
      {
         throw new System.NotImplementedException();
      }

      public ICircuitBreakerState GetState(string policy)
      {
         return State;
      }

      public Task StopMonitoringAsync(CancellationToken cancellationToken)
      {
         throw new System.NotImplementedException();
      }
   }
}