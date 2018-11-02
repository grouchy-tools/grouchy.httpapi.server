using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Grouchy.HttpApi.Server.Model
{
   public class Status
   {
      public string Name { get; set; }

      [JsonConverter(typeof(StringEnumConverter))]
      public Availability Availability { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public string Version { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public Dependency[] Dependencies { get; set; }
   }
}
