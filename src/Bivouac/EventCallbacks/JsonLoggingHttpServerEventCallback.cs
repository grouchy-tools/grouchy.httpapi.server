using Bivouac.Abstractions;
using Bivouac.Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Bivouac.EventCallbacks
{
   public class JsonLoggingHttpServerEventCallback : IHttpServerEventCallback
   {
      private readonly ILogger<JsonLoggingHttpServerEventCallback> _logger;
      private readonly JsonSerializerSettings _jsonSettings;

      public JsonLoggingHttpServerEventCallback(ILogger<JsonLoggingHttpServerEventCallback> logger)
      {
         _logger = logger;
         _jsonSettings = new JsonSerializerSettings
         {
            ContractResolver = new EventContractResolver<IHttpServerEvent>(),
            Converters = new JsonConverter[] { new ExceptionJsonConverter() },
            NullValueHandling = NullValueHandling.Ignore
         };
      }

      public void Invoke(IHttpServerEvent @event)
      {
         _logger.LogInformation(JsonConvert.SerializeObject(@event, _jsonSettings));
      }
   }
}