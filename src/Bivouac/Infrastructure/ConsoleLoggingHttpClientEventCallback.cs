namespace Bivouac.Infrastructure
{
   using System;
   using Burble.Abstractions;
   using Newtonsoft.Json;
   using Newtonsoft.Json.Serialization;

   public class ConsoleLoggingHttpClientEventCallback : IHttpClientEventCallback
   {
      private readonly JsonSerializerSettings _jsonSettings;

      public ConsoleLoggingHttpClientEventCallback()
      {
         _jsonSettings = new JsonSerializerSettings
         {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
         };
      }

      public void Invoke(IHttpClientEvent @event)
      {
         Console.WriteLine(JsonConvert.SerializeObject(@event, _jsonSettings));
      }
   }
}