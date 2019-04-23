using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    public class PlayStyle
    {
        [JsonProperty]
        public string Code;
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
