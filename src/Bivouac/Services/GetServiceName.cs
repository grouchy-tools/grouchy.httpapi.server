using Bivouac.Abstractions;

namespace Bivouac.Services
{
   public class GetServiceName : IGetServiceName
   {
      public string Name { get; set; }

      public string Get()
      {
         return Name;
      }
   }
}