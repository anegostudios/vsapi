using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Server
{
    public class PathfinderTask
    {
        public BlockPos startBlockPos;
        public BlockPos targetBlockPos;
        public int maxFallHeight;
        public float stepHeight;
        public Cuboidf collisionBox;
        public int searchDepth;
        public int mhdistanceTolerance = 0;
        public List<Vec3d> waypoints;
        public EnumAICreatureType CreatureType;

        public bool Finished;

        public PathfinderTask(BlockPos startBlockPos, BlockPos targetBlockPos, int maxFallHeight, float stepHeight, Cuboidf collisionBox, int searchDepth, int mhdistanceTolerance = 0, EnumAICreatureType creatureType = EnumAICreatureType.Default)
        {
            this.startBlockPos = startBlockPos;
            this.targetBlockPos = targetBlockPos;
            this.maxFallHeight = maxFallHeight;
            this.stepHeight = stepHeight;
            this.collisionBox = collisionBox;
            this.searchDepth = searchDepth;
            this.mhdistanceTolerance = mhdistanceTolerance;
            this.CreatureType = creatureType;
        }
    }
}
