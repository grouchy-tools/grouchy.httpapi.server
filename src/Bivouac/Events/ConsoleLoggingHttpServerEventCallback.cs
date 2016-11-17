namespace Bivouac.Events
{
   using System;
   using Bivouac.Abstractions;
   using Newtonsoft.Json;
   using Newtonsoft.Json.Serialization;

   public class ConsoleLoggingHttpServerEventCallback : IHttpServerEventCallback
   {
      private readonly JsonSerializerSettings _jsonSettings;

      public ConsoleLoggingHttpServerEventCallback()
      {
         _jsonSettings = new JsonSerializerSettings
         {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
         };
      }

      public void Invoke(IHttpServerEvent @event)
      {
         Console.WriteLine(JsonConvert.SerializeObject(@event, _jsonSettings));
      }
   }
}