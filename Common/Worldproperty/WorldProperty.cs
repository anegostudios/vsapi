using Newtonsoft.Json;

#nullable disable

namespace Vintagestory.API.Common
{
    [JsonObject(MemberSerialization.OptIn)]
    public class WorldProperty<T>
    {
        [JsonProperty]
        public AssetLocation Code;

        [JsonProperty]
        public T[] Variants;
    }

    public class StandardWorldProperty : WorldProperty<WorldPropertyVariant>
    {
        
    }

}
