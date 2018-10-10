using Bivouac.Newtonsoft.Json;
using Burble.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Bivouac.EventCallbacks
{
   public class JsonLoggingHttpClientEventCallback : IHttpClientEventCallback
   {
      private readonly ILogger<JsonLoggingHttpClientEventCallback> _logger;
      private readonly JsonSerializerSettings _jsonSettings;

      public JsonLoggingHttpClientEventCallback(ILogger<JsonLoggingHttpClientEventCallback> logger)
      {
         _logger = logger;
         _jsonSettings = new JsonSerializerSettings
         {
            ContractResolver = new EventContractResolver<IHttpClientEvent>(),
            Converters = new JsonConverter[] { new ExceptionJsonConverter() },
            NullValueHandling = NullValueHandling.Ignore
         };
      }

      public void Invoke(IHttpClientEvent @event)
      {
         _logger.LogInformation(JsonConvert.SerializeObject(@event, _jsonSettings));
      }
   }
}