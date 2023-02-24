using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common.Entities
{
    [Flags]
    public enum EnumEntityActivity
    {
        None = 0,
        Idle = 1,
        Move = 2,
        SprintMode = 4,
        SneakMode = 8,
        Fly = 16,
        Swim = 32,
        Jump = 64,
        Fall = 128,
        Climb = 256,
        FloorSitting = 512,
        Dead = 1024,
        Break = 2048,
        Place = 4096,
        Glide = 8192,
        Mounted = 8192*2
    }
}