using Newtonsoft.Json;

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
