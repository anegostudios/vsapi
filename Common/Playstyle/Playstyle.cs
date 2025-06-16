using Newtonsoft.Json;
using Vintagestory.API.Datastructures;

#nullable disable

namespace Vintagestory.API.Common
{
    public class PlayStyle
    {
        [JsonProperty]
        public string Code;
        [JsonProperty]
        public string PlayListCode;
        [JsonProperty]
        public string LangCode;
        [JsonProperty]
        public double ListOrder;
        [JsonProperty]
        public string[] Mods;
        [JsonProperty]
        public string WorldType;
        [JsonProperty, JsonConverter(typeof(JsonAttributesConverter))]
        public JsonObject WorldConfig;
    }
}
