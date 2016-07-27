namespace Bivouac.Implementations
{
   using System;
   using Bivouac.Abstractions;

   public class ConsoleEventLogger : ILogEvents
   {
      public void Log(string json)
      {
         Console.WriteLine(json);
      }
   }
}
