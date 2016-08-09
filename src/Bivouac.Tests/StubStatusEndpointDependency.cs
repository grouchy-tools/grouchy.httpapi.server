namespace Bivouac.Tests
{
   using System.Threading.Tasks;
   using Bivouac.Abstractions;
   using Bivouac.Model;

   public class StubStatusEndpointDependency : IStatusEndpointDependency
   {
      public string Name { get; set; }

      public Status Status { get; set; }

      public int DelayMs { get; set; }

      public async Task<Status> GetStatus()
      {
         if (DelayMs != 0)
         {
            await Task.Delay(DelayMs);
         }

         return Status;
      }
   }
}