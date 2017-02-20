namespace Bivouac.Services
{
   using System.Reflection;
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
         return Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
      }
   }
}