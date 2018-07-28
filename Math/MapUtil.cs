using Vintagestory.API.MathTools;

namespace Vintagestory.API.MathTools
{
    public class MapUtil
    {
        public static long Index3dL(int x, int y, int z, int sizex, int sizez)
        {
            return ((long)y * sizez + z) * sizex + x;
        }

        public static int Index3d(int x, int y, int z, int sizex, int sizez)
        {
            return (y * sizez + z) * sizex + x;
        }

        public static int Index2d(int x, int y, int sizex)
        {
            return y * sizex + x;
        }

        public static long Index2dL(int x, int y, int sizex)
        {
            return (long)y * sizex + x;
        }


        public static void PosInt3d(int index, int sizex, int sizez, Vec3i ret)
        {
            int x = index % sizex;
            int y = index / (sizex * sizez);
            int z = (index / sizex) % sizez;
            
            ret.X = x;
            ret.Y = y;
            ret.Z = z;
        }

        public static void PosInt3d(long index, int sizex, int sizez, Vec3i ret)
        {
            int x = (int)(index % sizex);
            int y = (int)(index / (sizex * sizez));
            int z = (int)((index / sizex) % sizez);
            
            ret.X = x;
            ret.Y = y;
            ret.Z = z;
        }

        public static void PosInt2d(long index, int sizex, Vec2i ret)
        {
            int x = (int)(index % sizex);
            int y = (int)(index / sizex);
            ret.X = x;
            ret.Y = y;
        }




        public static int PosX(int index, int sizex, int sizey)
        {
            return index % sizex;
        }

        public static int PosZ(int index, int sizex, int sizey)
        {
            return (index / sizex) % sizey;
        }

        public static int PosY(int index, int sizex, int sizey)
        {
            return index / (sizex * sizey);
        }
    }
}
