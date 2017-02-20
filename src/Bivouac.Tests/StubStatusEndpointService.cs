namespace Bivouac.Tests
{
   using Bivouac.Abstractions;
   
   public class StubStatusEndpointService : IStatusEndpointService
   {
      public string Name { get; set; }

      public string Version { get; set; }

      public string GetName()
      {
         return Name;
      }

      public string GetVersion()
      {
         return Version;
      }
   }
}