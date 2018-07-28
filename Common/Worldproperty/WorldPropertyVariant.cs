using Newtonsoft.Json;
using Vintagestory.API.Common;

namespace Vintagestory.API.Common
{
    [JsonObject(MemberSerialization.OptIn)]
    public class WorldPropertyVariant
    {
        [JsonProperty]
        public AssetLocation Code;
    }
}
