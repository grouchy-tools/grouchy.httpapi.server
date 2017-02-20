namespace Bivouac.Tests
{
   using Bivouac.Abstractions;

   public class StubAssemblyVersionGetter : IGetAssemblyVersion
   {
      public string Version { get; set; }

      public string Get()
      {
         return Version;
      }
   }
}