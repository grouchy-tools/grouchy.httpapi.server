using Grouchy.Abstractions;

namespace Grouchy.HttpApi.Server.Tests
{   
   public class StubApplicationInfo : IApplicationInfo
   {
      public string Name { get; set; }
      
      public string Version { get; set; }
      
      public string Environment { get; set; }
      
      public string OperatingSystem { get; set; }
   }
}