using System.Reflection;
using System.Runtime.InteropServices;
using Grouchy.Abstractions;
using Microsoft.Extensions.Hosting;

namespace Grouchy.HttpApi.Server.Services
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
