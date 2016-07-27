namespace Bivouac.Tests
{
   using System;
   using Bivouac.Abstractions;
   using System.Collections.Generic;

   public class StubEventLogger : ILogEvents
   {
      public IList<string> LoggedEvents { get; private set; } = new List<string>();

      public Exception Exception { get; set; }

      public void Log(string json)
      {
         if (Exception != null)
         {
            throw Exception;
         }

         LoggedEvents.Add(json);
      }
   }
}