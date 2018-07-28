using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Vintagestory.API
{
    /// <summary>
    /// A datastructure to hold 2 dimensional data in the form of ints. 
    /// Can be used to perfrom bilinear interpolation between individual values
    /// </summary>
    [ProtoContract]
    public class IntMap
    {
        [ProtoMember(1)]
        public int[] Data;

        /// <summary>
        /// Width and Length of the map (square)
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

        public static IntMap CreateEmpty()
        {
            return new IntMap()
            {
                Data = new int[0],
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

            return GameMath.BiLerp(x - ix, z - iz,
                Data[(iz + TopLeftPadding) * Size + ix + TopLeftPadding],
                Data[(iz + TopLeftPadding) * Size + ix + 1 + TopLeftPadding],
                Data[(iz + 1 + TopLeftPadding) * Size + ix + TopLeftPadding],
                Data[(iz + 1 + TopLeftPadding) * Size + ix + 1 + TopLeftPadding]
            );
        }


    }
}
