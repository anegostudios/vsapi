using Newtonsoft.Json.Linq;
using System;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    class JsonObject_ReadOnly : JsonObject
    {
        public JsonObject_ReadOnly(JsonObject original) : base(original, false)
        {
        }

        public override JToken Token
        {
            get { return base.Token; }
            set {
                throw new Exception("Modifying a JsonObject once it has become read-only is not allowed, sorry.  Mods should DeepClone the JsonObject first");
            }
        }

        public override void FillPlaceHolder(string key, string value)
        {
            throw new Exception("Modifying a JsonObject once it has become read-only is not allowed, sorry.  Mods should DeepClone the JsonObject first");
        }
    }
}
