using System.Runtime.CompilerServices;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Util
{
    public static class PosUtil
    {
        public static BlockPos SetOrCreate(this BlockPos pos, BlockPos sourcePos)
        {
            if (sourcePos == null) return null;
            if (pos == null) pos = new BlockPos();

            pos.Set(sourcePos);
            return pos;
        }

        public static Vec3f SetOrCreate(this Vec3f pos, Vec3f sourcePos)
        {
            if (sourcePos == null) return null;
            if (pos == null) pos = new Vec3f();

            pos.Set(sourcePos);
            return pos;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Block GetBlockAbove(this IBlockAccessor accessor, BlockPos pos, int dy = 1, int layer = BlockLayersAccess.Default)
        {
            pos.Y += dy;
            Block result = accessor.GetBlock(pos, layer);
            pos.Y -= dy;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Block GetBlockBelow(this IBlockAccessor accessor, BlockPos pos, int dy = 1, int layer = BlockLayersAccess.Default)
        {
            pos.Y -= dy;
            Block result = accessor.GetBlock(pos, layer);
            pos.Y += dy;
            return result;
        }

        public static Block GetBlockOnSide(this IBlockAccessor accessor, BlockPos pos, BlockFacing face, int layer = BlockLayersAccess.Default)
        {
            pos.Offset(face);
            Block result = accessor.GetBlock(pos, layer);
            pos.Add(face, -1);
            return result;
        }
    }
}
