namespace Bivouac.Services
{
   using System.Reflection;
   using Bivouac.Abstractions;

   public class AssemblyVersionGetter : IGetAssemblyVersion
   {
      public string Get()
      {
         return Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
      }
   }
}
