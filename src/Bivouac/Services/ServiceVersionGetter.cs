using System.Reflection;
using Bivouac.Abstractions;

namespace Bivouac.Services
{
   public class ServiceVersionGetter : IGetServiceVersion
   {
      public string Get()
      {
         return Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
      }
   }
}
