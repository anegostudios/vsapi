using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

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
