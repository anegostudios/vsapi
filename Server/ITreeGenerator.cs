using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Server
{
    public delegate void GrowTreeDelegate(IBlockAccessor blockAccessor, BlockPos pos, TreeGenParams treeGenParams);

    public interface ITreeGenerator
    {
        void GrowTree(IBlockAccessor blockAccessor, BlockPos pos, TreeGenParams treeGenParams, IRandom random);
    }


    public class TreeGenParams
    {
        public EnumHemisphere hemisphere;
        public bool skipForestFloor;
        public float size = 1f;
        public float vinesGrowthChance = 0f;
        public float mossGrowthChance = 0f;
        public float otherBlockChance = 1f;
        public int treesInChunkGenerated = 0;
    }



}
