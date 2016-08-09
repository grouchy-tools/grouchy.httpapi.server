namespace Bivouac.Model
{
   using Newtonsoft.Json;
   using Newtonsoft.Json.Converters;

   public class Status
   {
      public string Name { get; set; }

      [JsonConverter(typeof(StringEnumConverter))]
      public Availability Availability { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public string Version { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public string Build { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public Status[] Dependencies { get; set; }
   }
}
