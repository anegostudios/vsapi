using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Server
{
    public interface ITreeGenerator
    {
        void GrowTree(IBlockAccessor blockAccessor, BlockPos pos, bool skipForestFloor, float sizeModifier = 1f, float vineGrowthChance = 0, float otherblockChance = 1f, int treesInChunkGenerated = 0);
    }


}
