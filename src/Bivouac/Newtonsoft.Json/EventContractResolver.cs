using System.Linq;
using System.Reflection;
using Burble.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bivouac.Newtonsoft.Json
{
    public class EventContractResolver<TEvent> : CamelCasePropertyNamesContractResolver where TEvent : IEvent
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            var implementsHttpClientEvent = property.DeclaringType.GetInterfaces().Contains(typeof(TEvent));

            if (implementsHttpClientEvent && property.PropertyName == "tags")
            {
                property.ShouldSerialize = value =>
                {
                    var @event = (TEvent)value;
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