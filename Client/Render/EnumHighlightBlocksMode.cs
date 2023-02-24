using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Client
{
    public enum EnumHighlightBlocksMode
    {
        Absolute = 0,
        CenteredToSelectedBlock = 1,
        CenteredToSelectedBlockFollowTerrain = 2,
        AttachedToSelectedBlock = 3,

        CenteredToBlockSelectionIndex = 4,
        AttachedToBlockSelectionIndex = 5
    }
}
