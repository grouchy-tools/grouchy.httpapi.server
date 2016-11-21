namespace Bivouac.Events
{
   using System;
   using System.Linq;
   using System.Reflection;
   using Bivouac.Abstractions;
   using Newtonsoft.Json;
   using Newtonsoft.Json.Serialization;

   public class JsonHttpServerEventCallback : IHttpServerEventCallback
   {
      private readonly Action<string> _jsonCallback;
      private readonly JsonSerializerSettings _jsonSettings;

      public JsonHttpServerEventCallback(Action<string> jsonCallback)
      {
         _jsonCallback = jsonCallback;
         _jsonSettings = new JsonSerializerSettings
         {
            ContractResolver = new ServerEventResolver(),
            NullValueHandling = NullValueHandling.Ignore
         };
      }

      public void Invoke(IHttpServerEvent @event)
      {
         _jsonCallback(JsonConvert.SerializeObject(@event, _jsonSettings));
      }

      private class ServerEventResolver : CamelCasePropertyNamesContractResolver
      {
         protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
         {
            var property = base.CreateProperty(member, memberSerialization);

            var implementsHttpClientEvent = property.DeclaringType.GetInterfaces().Contains(typeof(IHttpServerEvent));

            if (implementsHttpClientEvent && property.PropertyName == "tags")
            {
               property.ShouldSerialize = value =>
               {
                  var @event = (IHttpServerEvent)value;
                  return @event.Tags != null && @event.Tags.Count != 0;
               };
            }
            else if (implementsHttpClientEvent && property.PropertyName == "request")
            {
               property.ShouldSerialize = value => false;
            }

            return property;
         }
      }
   }
}