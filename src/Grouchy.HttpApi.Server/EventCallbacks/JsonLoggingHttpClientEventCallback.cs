using Grouchy.HttpApi.Server.Newtonsoft.Json;
using Grouchy.HttpApi.Client.Abstractions.EventCallbacks;
using Grouchy.HttpApi.Client.Abstractions.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Grouchy.HttpApi.Server.EventCallbacks
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
            ContractResolver = new EventContractResolver(),
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