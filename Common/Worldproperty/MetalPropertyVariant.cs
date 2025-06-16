using Newtonsoft.Json;

#nullable disable

namespace Vintagestory.API.Common
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MetalPropertyVariant : WorldPropertyVariant
    {
        [JsonProperty]
        public int Tier;
    }
}
