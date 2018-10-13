using System.Threading;
using System.Threading.Tasks;
using Bivouac.Model;

namespace Bivouac.Abstractions
{
   public interface IStatusEndpointDependency
   {
      string Name { get; }

      Task<Status> GetStatusAsync(CancellationToken cancellationToken);
   }
}