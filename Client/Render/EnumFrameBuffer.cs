using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Client
{
    public enum EnumFrameBuffer
    {
        Default = -1,
        Primary = 0,
        Transparent = 1,
        BlurHorizontalMedRes = 2,
        BlurVerticalMedRes = 3,
        FindBright = 4,
        GodRays = 7,
        BlurVerticalLowRes = 8,
        BlurHorizontalLowRes = 9,
        Luma = 10,
        ShadowmapFar = 11,
        ShadowmapNear = 12
    }

}
