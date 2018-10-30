using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Burble.Abstractions.CircuitBreaking;
using Microsoft.Extensions.Hosting;

namespace Bivouac.CircuitBreaking
{
   public class CircuitBreakingHostedService : IHostedService
   {
      private readonly ICircuitBreakingStateManager<HttpStatusCode> _circuitBreakingStateManager;

      public CircuitBreakingHostedService(ICircuitBreakingStateManager<HttpStatusCode> circuitBreakingStateManager)
      {
         _circuitBreakingStateManager = circuitBreakingStateManager;
      }

      public Task StartAsync(CancellationToken cancellationToken)
      {
         return Task.CompletedTask;
      }

      public Task StopAsync(CancellationToken cancellationToken)
      {
         return _circuitBreakingStateManager.StopMonitoringAsync(cancellationToken);
      }
   }
}