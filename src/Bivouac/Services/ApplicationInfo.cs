using System.Reflection;
using System.Runtime.InteropServices;
using Burble.Abstractions;
using Burble.Abstractions.Identifying;
using Microsoft.Extensions.Hosting;

namespace Bivouac.Services
{
   public class ApplicationInfo : IApplicationInfo
   {
      private readonly IHostingEnvironment _hostingEnvironment;

      public ApplicationInfo(IHostingEnvironment hostingEnvironment)
      {
         _hostingEnvironment = hostingEnvironment;
      }

      public string Name => _hostingEnvironment.ApplicationName;

      public string Version => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

      public string Environment => _hostingEnvironment.EnvironmentName;

      public string OperatingSystem => RuntimeInformation.OSDescription.Trim();
   }
}
