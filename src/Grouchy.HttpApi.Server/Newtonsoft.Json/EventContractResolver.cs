using System.Collections;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Grouchy.HttpApi.Server.Newtonsoft.Json
{
    public class EventContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                property.ShouldSerialize = instance =>
                {
                    var enumerable = property.ValueProvider.GetValue(instance) as IEnumerable;

                    if (enumerable == null) return false;

                    return enumerable.OfType<object>().Count() != 0;
                };
            }

            return property;
        }
    }
}