using System.Threading;
using System.Threading.Tasks;
using Grouchy.Resilience.Abstractions.CircuitBreaking;
using Microsoft.Extensions.Hosting;

namespace Grouchy.HttpApi.Server.CircuitBreaking
{
   public class CircuitBreakerHostedService : IHostedService
   {
      private readonly ICircuitBreakerManager _circuitBreakerManager;

      public CircuitBreakerHostedService(ICircuitBreakerManager circuitBreakerManager)
      {
         _circuitBreakerManager = circuitBreakerManager;
      }

      public Task StartAsync(CancellationToken cancellationToken)
      {
         return Task.CompletedTask;
      }

      public Task StopAsync(CancellationToken cancellationToken)
      {
         return _circuitBreakerManager.StopMonitoringAsync(cancellationToken);
      }
   }
}