using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Server
{
    public delegate void GrowTreeDelegate(IBlockAccessor blockAccessor, BlockPos pos, bool skipForestFloor, float sizeModifier = 1f, float vineGrowthChance = 0, float otherBlockChance = 1f, int treesInChunkGenerated = 0);

    public interface ITreeGenerator
    {
        void GrowTree(IBlockAccessor blockAccessor, BlockPos pos, bool skipForestFloor, float sizeModifier = 1f, float vineGrowthChance = 0, float otherBlockChance = 1f, int treesInChunkGenerated = 0);
    }


}
