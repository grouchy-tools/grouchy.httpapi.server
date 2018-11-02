using System.Threading;
using System.Threading.Tasks;
using Grouchy.HttpApi.Server.Model;

namespace Grouchy.HttpApi.Server.Abstractions
{
   public interface IStatusEndpointDependency
   {
      string Name { get; }

      Task<Dependency> GetStatusAsync(CancellationToken cancellationToken);
   }
}