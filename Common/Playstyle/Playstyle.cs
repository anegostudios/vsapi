using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    // Ok concept to allow for an exploration mode made through simple json patches
    // 1. Exploration mode mod. Depends on the survival mod
    // 2. Playstyles can require / exclude mods


    public class Playstyle
    {
        public string[] RequiredMods;
        public string[] ExludedMods;
        public JsonObject Attributes;
    }
    
}
