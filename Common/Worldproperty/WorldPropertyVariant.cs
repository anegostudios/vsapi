using Newtonsoft.Json;

namespace Vintagestory.API.Common
{
    [JsonObject(MemberSerialization.OptIn)]
    public class WorldPropertyVariant
    {
        [JsonProperty]
        public AssetLocation Code;
    }
}
