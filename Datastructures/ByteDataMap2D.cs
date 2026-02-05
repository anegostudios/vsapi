using ProtoBuf;
using System;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    /// <summary>
    /// A datastructure to hold 2 dimensional data in the form of ints. 
    /// Can be used to perfrom bilinear interpolation between individual values
    /// </summary>
    [ProtoContract]
    public class ByteDataMap2D
    {
        [ProtoMember(1, IsPacked = true)]
        public byte[] Data;

        /// <summary>
        /// Full Width and Length of the map (square)
        /// </summary>
        [ProtoMember(2)]
        public int Size;

        /// <summary>
        /// Top and Left padding
        /// </summary>
        [ProtoMember(3)]
        public int TopLeftPadding;

        /// <summary>
        /// Bottom and Right padding
        /// </summary>
        [ProtoMember(4)]
        public int BottomRightPadding;

        /// <summary>
        /// Width and Length of the map excluding any padding
        /// </summary>
        public int InnerSize
        {
            get { return Size - TopLeftPadding - BottomRightPadding; }
        }

        public static ByteDataMap2D CreateEmpty()
        {
            return new ByteDataMap2D()
            {
                Data = Array.Empty<byte>(),
                Size = 0
            };
        }



        public byte Get(int x, int z)
        {
            return Data[z * Size + x];
        }

        public void Set(int x, int z, byte value)
        {
            Data[z * Size + x] = value;
        }

        public byte GetUnpadded(int x, int z)
        {
            return Data[(z + TopLeftPadding) * Size + x + TopLeftPadding];
        }


        public float GetUnpaddedLerped(float x, float z)
        {
            int ix = (int)x;
            int iz = (int)z;

            return GameMath.BiLerp(
                Data[(iz + TopLeftPadding) * Size + ix + TopLeftPadding],
                Data[(iz + TopLeftPadding) * Size + ix + 1 + TopLeftPadding],
                Data[(iz + 1 + TopLeftPadding) * Size + ix + TopLeftPadding],
                Data[(iz + 1 + TopLeftPadding) * Size + ix + 1 + TopLeftPadding],
                x - ix, z - iz
            );
        }


        public float GetLerpedCorrectly(float x, float z)
        {
            int posXLeft = (int)Math.Floor(x - 0.5f);
            int posZLeft = (int)Math.Floor(z - 0.5f);

            float fx = x - (posXLeft + 0.5f);
            float fz = z - (posZLeft + 0.5f);

            int index = (posZLeft + TopLeftPadding) * Size + posXLeft + TopLeftPadding;
            return GameMath.BiLerp(
                Data[index],
                Data[index + 1],
                Data[index + Size],
                Data[index + Size + 1],
                fx, fz
            );
        }
    }
}
