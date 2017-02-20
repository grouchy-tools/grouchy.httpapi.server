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
         var version = GetAssemblyInfoVersion().Split('-');

         return version[0];
      }

      public string GetBuild()
      {
         var version = GetAssemblyInfoVersion().Split('-');

         if (version.Length > 1)
         {
            return version[1];
         }

         return null;
      }

      private static string GetAssemblyInfoVersion()
      {
         return Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
      }
   }
}