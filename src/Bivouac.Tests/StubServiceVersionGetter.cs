namespace Bivouac.Tests
{
   using Bivouac.Abstractions;

   public class StubServiceVersionGetter : IGetServiceVersion
   {
      public string Version { get; set; }

      public string Get()
      {
         return Version;
      }
   }
}