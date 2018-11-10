using System.Threading;
using System.Threading.Tasks;
using Grouchy.HttpApi.Server.Abstractions;
using Grouchy.HttpApi.Server.Abstractions.Model;

namespace Grouchy.HttpApi.Server.Tests
{
   public class StubStatusEndpointDependency : IStatusEndpointDependency
   {
      public string Name { get; set; }

      public Dependency Dependency { get; set; }

      public int DelayMs { get; set; }

      public async Task<Dependency> GetStatusAsync(CancellationToken cancellationToken)
      {
         if (DelayMs != 0)
         {
            await Task.Delay(DelayMs, cancellationToken);
         }

         return Dependency;
      }
   }
}