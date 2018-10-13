using Bivouac.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Bivouac.Services
{
   public class ServiceNameGetter : IGetServiceName
   {
      private readonly IConfiguration _configuration;

      public ServiceNameGetter(IConfiguration configuration)
      {
         _configuration = configuration;
      }

      public string Get()
      {
         // Get name defined in <AssemblyName> tag in .csproj
         return _configuration.GetValue<string>("applicationName");
      }
   }
}