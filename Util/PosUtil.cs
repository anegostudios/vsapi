using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

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
    }
}
