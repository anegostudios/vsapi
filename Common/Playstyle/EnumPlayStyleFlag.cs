using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    [Flags]
    public enum EnumPlayStyleFlag
    {
        WildernessSurvival = 0x1,
        SurviveAndBuild    = 0x2,
        SurviveAndAutomate = 0x4,
        CreativeBuilding   = 0x8,
        All = WildernessSurvival | SurviveAndBuild |
              SurviveAndAutomate | CreativeBuilding
    }
}
