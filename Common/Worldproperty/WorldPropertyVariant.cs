using Newtonsoft.Json;

#nullable disable

namespace Vintagestory.API.Common
{
    [JsonObject(MemberSerialization.OptIn)]
    public class WorldPropertyVariant
    {
        [JsonProperty]
        public AssetLocation Code;
    }
}
