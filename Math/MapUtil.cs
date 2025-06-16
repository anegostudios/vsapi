using System.Runtime.CompilerServices;

#nullable disable

namespace Vintagestory.API.MathTools
{
    public class MapUtil
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Index3dL(int x, int y, int z, long sizex, long sizez)
        {
            return ((long)y * sizez + z) * sizex + x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Index3d(int x, int y, int z, long sizex, long sizez)
        {
            return (int)((y * sizez + z) * sizex + x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Index3d(int x, int y, int z, int sizex, int sizez)
        {
            return (y * sizez + z) * sizex + x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Index2d(int x, int y, int sizex)
        {
            return y * sizex + x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Index2dL(int x, int y, int sizex)
        {
            return (long)y * sizex + x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PosInt3d(int index, long sizex, long sizez, Vec3i ret)
        {
            long x = index % sizex;
            long y = index / (sizex * sizez);
            long z = (index / sizex) % sizez;
            
            ret.X = (int)x;
            ret.Y = (int)y;
            ret.Z = (int)z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PosInt3d(long index, long sizex, long sizez, Vec3i ret)
        {
            int x = (int)(index % sizex);
            int y = (int)(index / (sizex * sizez));
            int z = (int)((index / sizex) % sizez);
            
            ret.X = x;
            ret.Y = y;
            ret.Z = z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PosInt2d(long index, long sizex, Vec2i ret)
        {
            int x = (int)(index % sizex);
            int y = (int)(index / sizex);
            ret.X = x;
            ret.Y = y;
        }




        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PosX(int index, int sizex, int sizey)
        {
            return index % sizex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PosZ(int index, int sizex, int sizey)
        {
            return (index / sizex) % sizey;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PosY(int index, int sizex, int sizey)
        {
            return index / (sizex * sizey);
        }
    }
}
