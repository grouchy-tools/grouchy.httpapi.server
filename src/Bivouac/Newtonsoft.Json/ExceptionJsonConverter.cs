using System;
using Newtonsoft.Json;

namespace Bivouac.Newtonsoft.Json
{
    public class ExceptionJsonConverter : JsonConverter
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

            if (!string.IsNullOrEmpty(e.StackTrace))
            {
                writer.WritePropertyName("stackTrace");
                writer.WriteValue(e.StackTrace);
            }

            if (e.InnerException != null)
            {
                writer.WritePropertyName("innerException");
                WriteException(writer, e.InnerException);
            }

            writer.WriteEndObject();
        }
    }
}