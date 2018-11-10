using System.Threading;
using System.Threading.Tasks;
using Grouchy.HttpApi.Server.Abstractions;
using Grouchy.HttpApi.Server.Abstractions.Model;
using Grouchy.Resilience.Abstractions.CircuitBreaking;

namespace Grouchy.HttpApi.Server.Services
{
   public class CircuitBreakingStatusEndpointDependency<TResponse> : IStatusEndpointDependency
   {
      private readonly IStatusEndpointDependency _statusEndpointDependency;
      private readonly ICircuitBreakerState _circuitBreakerState;

      public CircuitBreakingStatusEndpointDependency(
         IStatusEndpointDependency statusEndpointDependency,
         ICircuitBreakerState circuitBreakerState)
      {
         _statusEndpointDependency = statusEndpointDependency;
         _circuitBreakerState = circuitBreakerState;
      }

      public string Name => _statusEndpointDependency.Name;

      public async Task<Dependency> GetStatusAsync(CancellationToken cancellationToken)
      {
         var dependency = await _statusEndpointDependency.GetStatusAsync(cancellationToken);

         dependency.ClosedPct = _circuitBreakerState.ClosedPct;

         return dependency;
      }
   }
}