using Bivouac.Abstractions;

namespace Bivouac.Tests
{   
   public class StubServiceNameGetter : IGetServiceName
   {
      public string Name { get; set; }

      public string Get()
      {
         return Name;
      }
   }
}