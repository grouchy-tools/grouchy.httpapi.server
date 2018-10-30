using System.Threading;
using System.Threading.Tasks;
using Bivouac.Abstractions;
using Bivouac.Model;
using Burble.Abstractions.CircuitBreaking;

namespace Bivouac.Services
{
   public class CircuitBreakingStatusEndpointDependency<TResponse> : IStatusEndpointDependency
   {
      private readonly IStatusEndpointDependency _statusEndpointDependency;
      private readonly ICircuitBreakingState<TResponse> _circuitBreakingState;

      public CircuitBreakingStatusEndpointDependency(
         IStatusEndpointDependency statusEndpointDependency,
         ICircuitBreakingState<TResponse> circuitBreakingState)
      {
         _statusEndpointDependency = statusEndpointDependency;
         _circuitBreakingState = circuitBreakingState;
      }

      public string Name => _statusEndpointDependency.Name;

      public async Task<Dependency> GetStatusAsync(CancellationToken cancellationToken)
      {
         var dependency = await _statusEndpointDependency.GetStatusAsync(cancellationToken);

         dependency.ClosedPct = _circuitBreakingState.ClosedPct;

         return dependency;
      }
   }
}