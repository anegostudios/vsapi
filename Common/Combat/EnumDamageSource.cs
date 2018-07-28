using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vintagestory.API.Common
{
    public enum EnumDamageSource
    {
        Block, 
        Player,
        Fall,
        Drown,
        Respawn,
        Void,
        Suicide,         // /kill command
        Internal,
        Entity,
        Explosion,
        Unknown
    }
}
