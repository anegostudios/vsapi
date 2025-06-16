using ProtoBuf;
using System;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    /// <summary>
    /// A datastructure to hold 3 dimensional data in the form of floats 
    /// Can be used to perfrom trilinear interpolation between individual values
    /// </summary>
    [ProtoContract]
    public class FloatDataMap3D
    {
        [ProtoMember(1)]
        public float[] Data;
        [ProtoMember(2)]
        public int Width;
        [ProtoMember(3)]
        public int Length;
        [ProtoMember(4)]
        public int Height;

        public FloatDataMap3D()
        {

        }


        public FloatDataMap3D(int width, int height, int length)
        {
            this.Width = width;
            this.Length = length;
            this.Height = height;

            this.Data = new float[width * height * length];
        }

        public float GetValue(int x, int y, int z)
        {
            return Data[(y * Length + z) * Width + x];
        }

        public void SetValue(int x, int y, int z, float value)
        {
            Data[(y * Length + z) * Width + x] = value;
        }

        public void AddValue(int x, int y, int z, float value)
        {
            Data[(y * Length + z) * Width + x] += value;
        }


        public float GetLerped(float x, float y, float z)
        {
            int posXLeft = (int)x;
            int posXRight = posXLeft + 1;

            int posYBot = (int)y;
            int posYTop = posYBot + 1;

            int posZLeft = (int)z;
            int posZRight = posZLeft + 1;

            float fx = x - (int)x;
            float fy = y - (int)y;
            float fz = z - (int)z;

            float down = GameMath.BiLerp(
                Data[(posYBot * Length + posZLeft) * Width + posXLeft],
                Data[(posYBot * Length + posZLeft) * Width + posXRight],
                Data[(posYBot * Length + posZRight) * Width + posXLeft],
                Data[(posYBot * Length + posZRight) * Width + posXRight],
                fx, fz
            );

            float up = GameMath.BiLerp(
                Data[(posYTop * Length + posZLeft) * Width + posXLeft],
                Data[(posYTop * Length + posZLeft) * Width + posXRight],
                Data[(posYTop * Length + posZRight) * Width + posXLeft],
                Data[(posYTop * Length + posZRight) * Width + posXRight],
                fx, fz
            );

            return GameMath.Lerp(down, up, fy);
        }

        public float GetLerpedCenterPixel(float x, float y, float z)
        {
            int posXLeft = (int)Math.Floor(x - 0.5f);
            int posXRight = posXLeft + 1;

            int posYBot = (int)Math.Floor(y - 0.5f);
            int posYTop = posYBot + 1;

            int posZLeft = (int)Math.Floor(z - 0.5f);
            int posZRight = posZLeft + 1;

            float fx = x - (posXLeft + 0.5f);
            float fy = y - (posYBot + 0.5f);
            float fz = z - (posZLeft + 0.5f);

            float up = GameMath.BiLerp(
                Data[(posYBot * Length + posZLeft) * Width + posXLeft],
                Data[(posYBot * Length + posZLeft) * Width + posXRight],
                Data[(posYBot * Length + posZRight) * Width + posXLeft],
                Data[(posYBot * Length + posZRight) * Width + posXRight],
                fx, fz
            );

            float down = GameMath.BiLerp(
                Data[(posYTop * Length + posZLeft) * Width + posXLeft],
                Data[(posYTop * Length + posZLeft) * Width + posXRight],
                Data[(posYTop * Length + posZRight) * Width + posXLeft],
                Data[(posYTop * Length + posZRight) * Width + posXRight],
                fx, fz
            );

            return GameMath.Lerp(down, up, fy);
        }



    }
}
