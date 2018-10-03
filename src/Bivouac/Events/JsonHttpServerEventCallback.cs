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
            Converters = new JsonConverter[] { new ExceptionJsonConverter() },
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

      private class ExceptionJsonConverter : JsonConverter
      {
         public override bool CanRead => false;

         public override bool CanWrite => true;

         public override bool CanConvert(Type objectType)
         {
            return typeof(Exception).IsAssignableFrom(objectType);
         }

         public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
         {
            throw new NotSupportedException();
         }

         public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
         {
            if (value == null)
            {
               writer.WriteNull();
               return;
            }

            WriteException(writer, (Exception)value);
         }

         private void WriteException(JsonWriter writer, Exception e)
         {
            writer.WriteStartObject();

            writer.WritePropertyName("type");
            writer.WriteValue(e.GetType().Name);

            writer.WritePropertyName("message");
            writer.WriteValue(e.Message);

            if (e.InnerException != null)
            {
               writer.WritePropertyName("innerException");
               WriteException(writer, e.InnerException);
            }

            writer.WriteEndObject();
         }
      }
   }
}