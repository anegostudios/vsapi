using System;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    public static class NormalUtil
    {
        public static int NegBit = 1 << 9;

        public static int tenBitMask = 0x3FF;
        public static int nineBitMask = 0x1FF;

        public static int tenthBitMask = 0x200;

        public static void FromPackedNormal(int normal, ref Vec4f toFill)
        {
            int normal0 = normal;
            int normal1 = normal >> 10;
            int normal2 = normal >> 20;

            bool xNeg = (tenthBitMask & normal0) > 0;
            bool yNeg = (tenthBitMask & normal1) > 0;
            bool zNeg = (tenthBitMask & normal2) > 0;

            toFill.X = (xNeg ? (~normal0 & nineBitMask) : normal0 & nineBitMask) / 512f;
            toFill.Y = (yNeg ? (~normal1 & nineBitMask) : normal1 & nineBitMask) / 512f;
            toFill.Z = (zNeg ? (~normal2 & nineBitMask) : normal2 & nineBitMask) / 512f;
            toFill.W = normal >> 30;
        }

        public static void FromPackedNormal(int normal, ref float[] toFill)
        {
            int normal0 = normal;
            int normal1 = normal >> 10;
            int normal2 = normal >> 20;

            bool xNeg = (tenthBitMask & normal0) > 0;
            bool yNeg = (tenthBitMask & normal1) > 0;
            bool zNeg = (tenthBitMask & normal2) > 0;

            toFill[0] = (xNeg ? (~normal0 & nineBitMask) : normal0 & nineBitMask) / 512f;
            toFill[1] = (yNeg ? (~normal1 & nineBitMask) : normal1 & nineBitMask) / 512f;
            toFill[2] = (zNeg ? (~normal2 & nineBitMask) : normal2 & nineBitMask) / 512f;
            toFill[3] = normal >> 30;
        }

        public static int PackNormal(Vec4f normal)
        {
            bool xNeg = normal.X < 0;
            bool yNeg = normal.Y < 0;
            bool zNeg = normal.Z < 0;

            int normalX = (int)Math.Abs(normal.X * 511);
            int normalY = (int)Math.Abs(normal.Y * 511);
            int normalZ = (int)Math.Abs(normal.Z * 511);

            return
               ((xNeg ? NegBit | (~normalX & nineBitMask) : normalX) << 0) |
               ((yNeg ? NegBit | (~normalY & nineBitMask) : normalY) << 10) |
               ((zNeg ? NegBit | (~normalZ & nineBitMask) : normalZ) << 20) |
               ((int)normal.W << 30)
            ;
        }


        public static int PackNormal(float x, float y, float z)
        {
            bool xNeg = x < 0;
            bool yNeg = y < 0;
            bool zNeg = z < 0;

            int normalX = xNeg ? (NegBit | ~(int)Math.Abs(x * 511) & nineBitMask) : ((int)(x * 511) & nineBitMask);
            int normalY = yNeg ? (NegBit | ~(int)Math.Abs(y * 511) & nineBitMask) : ((int)(y * 511) & nineBitMask);
            int normalZ = zNeg ? (NegBit | ~(int)Math.Abs(z * 511) & nineBitMask) : ((int)(z * 511) & nineBitMask);

            return (normalX << 0) | (normalY << 10) | (normalZ << 20);
        }


        internal static int PackNormal(float[] normal)
        {
            bool xNeg = normal[0] < 0;
            bool yNeg = normal[1] < 0;
            bool zNeg = normal[2] < 0;

            int normalX = (int)Math.Abs(normal[0] * 511);
            int normalY = (int)Math.Abs(normal[1] * 511);
            int normalZ = (int)Math.Abs(normal[2] * 511);

            return
               ((xNeg ? NegBit | (~normalX & nineBitMask) : normalX) << 0) |
               ((yNeg ? NegBit | (~normalY & nineBitMask) : normalY) << 10) |
               ((zNeg ? NegBit | (~normalZ & nineBitMask) : normalZ) << 20) |
               ((int)normal[3] << 30)
            ;
        }

        internal static void FromPackedNormal(int normal, ref double[] toFill)
        {
            int normal0 = normal;
            int normal1 = normal >> 10;
            int normal2 = normal >> 20;

            bool xNeg = (tenthBitMask & normal0) > 0;
            bool yNeg = (tenthBitMask & normal1) > 0;
            bool zNeg = (tenthBitMask & normal2) > 0;

            toFill[0] = (xNeg ? (~normal0 & nineBitMask) : normal0 & nineBitMask) / 512f;
            toFill[1] = (yNeg ? (~normal1 & nineBitMask) : normal1 & nineBitMask) / 512f;
            toFill[2] = (zNeg ? (~normal2 & nineBitMask) : normal2 & nineBitMask) / 512f;
            toFill[3] = normal >> 30;
        }

        internal static int PackNormal(double[] normal)
        {
            bool xNeg = normal[0] < 0;
            bool yNeg = normal[1] < 0;
            bool zNeg = normal[2] < 0;

            int normalX = (int)Math.Abs(normal[0] * 512);
            int normalY = (int)Math.Abs(normal[1] * 512);
            int normalZ = (int)Math.Abs(normal[2] * 512);

            return
               ((xNeg ? NegBit | (~normalX & tenBitMask) : normalX) << 0) |
               ((yNeg ? NegBit | (~normalY & tenBitMask) : normalY) << 10) |
               ((zNeg ? NegBit | (~normalZ & tenBitMask) : normalZ) << 20) |
               ((int)normal[3] << 30)
            ;
        }
    }
}
