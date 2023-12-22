using Vintagestory.API.MathTools;

namespace VintagestoryAPI.Math.Vector
{
    public class Vec3iAndFacingFlags
    {
        private static int extChunkSize;
        public int X, Y, Z;
        public int FacingFlags;
        public int OppositeFlags;
        public int extIndexOffset;
        public int OppositeFlagsUpperOrLeft;
        public int OppositeFlagsLowerOrRight;

        public static void Initialize(int value)
        {
            extChunkSize = value;
        }

        public Vec3iAndFacingFlags(int x, int y, int z, int flags, int oppositeFlags)
        {
            X = x;
            Y = y;
            Z = z;
            FacingFlags = flags;
            OppositeFlags = oppositeFlags;
            extIndexOffset = MapUtil.Index3d(x, y, z, extChunkSize, extChunkSize);
            OppositeFlagsUpperOrLeft = oppositeFlags;
            OppositeFlagsLowerOrRight = oppositeFlags;
        }

        public Vec3iAndFacingFlags(int x, int y, int z, int flags, int oppositeFlags, int flagsUL, int flagsLR)
        {
            X = x;
            Y = y;
            Z = z;
            FacingFlags = flags;
            OppositeFlags = oppositeFlags;
            extIndexOffset = MapUtil.Index3d(x, y, z, extChunkSize, extChunkSize);
            OppositeFlagsUpperOrLeft = flagsUL;
            OppositeFlagsLowerOrRight = flagsLR;
        }
    }
}
