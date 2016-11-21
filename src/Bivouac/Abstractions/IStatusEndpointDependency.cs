namespace Bivouac.Abstractions
{
   using System.Threading;
   using System.Threading.Tasks;
   using Bivouac.Model;

   public interface IStatusEndpointDependency
   {
      string Name { get; }

      Task<Status> GetStatus(CancellationToken cancellationToken);
   }
}