namespace Bivouac.Services
{
   using Bivouac.Abstractions;

   public class StatusEndpointService : IStatusEndpointService
   {
      public string Name { get; set; }

      public string GetName()
      {
         return Name;
      }

      public string GetVersion()
      {
         // TODO: Implement
         return null;
      }

      public string GetBuild()
      {
         // TODO: Implement
         return null;
      }
   }
}