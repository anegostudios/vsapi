using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Datastructures
{
    /// <summary>
    /// A datastructure to hold 2 dimensional data in the form of ints. 
    /// Can be used to perfrom bilinear interpolation between individual values
    /// </summary>
    [ProtoContract]
    public class IntDataMap2D
    {
        [ProtoMember(1, IsPacked = true)]
        public int[] Data;

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

        public static IntDataMap2D CreateEmpty()
        {
            return new IntDataMap2D()
            {
                Data = new int[1],
                Size = 0
            };
        }



        public int GetInt(int x, int z)
        {
            return Data[z * Size + x];
        }

        public int GetUnpaddedInt(int x, int z)
        {
            return Data[(z + TopLeftPadding) * Size + x + TopLeftPadding];
        }

        public int GetUnpaddedColorLerped(float x, float z)
        {
            int ix = (int)x;
            int iz = (int)z;

            return GameMath.BiLerpRgbColor(x - ix, z - iz,
                Data[(iz + TopLeftPadding) * Size + ix + TopLeftPadding],
                Data[(iz + TopLeftPadding) * Size + ix + 1 + TopLeftPadding],
                Data[(iz + 1 + TopLeftPadding) * Size + ix + TopLeftPadding],
                Data[(iz + 1 + TopLeftPadding) * Size + ix + 1 + TopLeftPadding]
            );
        }


        public float GetUnpaddedIntLerped(float x, float z)
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


        public float GetIntLerpedCorrectly(float x, float z)
        {
            int posXLeft = (int)Math.Floor(x - 0.5f);
            int posXRight = posXLeft + 1;

            int posZLeft = (int)Math.Floor(z - 0.5f);
            int posZRight = posZLeft + 1;

            float fx = x - (posXLeft + 0.5f);
            float fz = z - (posZLeft + 0.5f);
            
            return GameMath.BiLerp(
                Data[(posZLeft + TopLeftPadding) * Size + posXLeft + TopLeftPadding],
                Data[(posZLeft + TopLeftPadding) * Size + posXRight + TopLeftPadding],
                Data[(posZRight + TopLeftPadding) * Size + posXLeft + TopLeftPadding],
                Data[(posZRight + TopLeftPadding) * Size + posXRight + TopLeftPadding],
                fx, fz
            );
        }

        public int GetColorLerpedCorrectly(float x, float z)
        {
            int posXLeft = (int)Math.Floor(x - 0.5f);
            int posXRight = posXLeft + 1;

            int posZLeft = (int)Math.Floor(z - 0.5f);
            int posZRight = posZLeft + 1;

            float fx = x - (posXLeft + 0.5f);
            float fz = z - (posZLeft + 0.5f);

            return GameMath.BiLerpRgbColor(
                fx, fz,
                Data[(posZLeft + TopLeftPadding) * Size + posXLeft + TopLeftPadding],
                Data[(posZLeft + TopLeftPadding) * Size + posXRight + TopLeftPadding],
                Data[(posZRight + TopLeftPadding) * Size + posXLeft + TopLeftPadding],
                Data[(posZRight + TopLeftPadding) * Size + posXRight + TopLeftPadding]
            );
        }


    }
}
