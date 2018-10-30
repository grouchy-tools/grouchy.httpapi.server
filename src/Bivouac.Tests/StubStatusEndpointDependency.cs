using System.Threading;
using System.Threading.Tasks;
using Bivouac.Abstractions;
using Bivouac.Model;

namespace Bivouac.Tests
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