using Newtonsoft.Json;

namespace Vintagestory.API.Common
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MetalPropertyVariant : WorldPropertyVariant
    {
        [JsonProperty]
        public int Tier;
    }
}
