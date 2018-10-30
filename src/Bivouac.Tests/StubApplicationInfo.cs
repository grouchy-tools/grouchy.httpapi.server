using Burble.Abstractions.Identifying;

namespace Bivouac.Tests
{   
   public class StubApplicationInfo : IApplicationInfo
   {
      public string Name { get; set; }
      
      public string Version { get; set; }
      
      public string Environment { get; set; }
      
      public string OperatingSystem { get; set; }
   }
}