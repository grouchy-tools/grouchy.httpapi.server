namespace Bivouac.Services
{
   using Bivouac.Abstractions;

   public class NoOpEventLogger : ILogEvents
   {
      public void Log(string json)
      {
      }
   }
}
