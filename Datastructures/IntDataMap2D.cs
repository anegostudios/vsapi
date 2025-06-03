using ProtoBuf;
using System;
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
                Data = new int[0],
                Size = 0
            };
        }



        public int GetInt(int x, int z)
        {
            return Data[z * Size + x];
        }

        public void SetInt(int x, int z, int value)
        {
            Data[z * Size + x] = value;
        }

        public int GetUnpaddedInt(int x, int z)
        {
            return Data[(z + TopLeftPadding) * Size + x + TopLeftPadding];
        }

        public int GetUnpaddedColorLerped(float x, float z)
        {
            int ix = (int)x;
            int iz = (int)z;
            int index = (iz + TopLeftPadding) * Size + ix + TopLeftPadding;

            if (index < 0 || index + Size + 1 >= Data.Length) throw new IndexOutOfRangeException("MapRegion data, index was " + (index + Size + 1) + " but length was " + Data.Length);
            return GameMath.BiLerpRgbColor(x - ix, z - iz,
                Data[index],
                Data[index + 1],
                Data[index + Size],
                Data[index + Size + 1]
            );
        }

        /// <summary>
        /// The parameters should both be in the range 0..1.  They represent the position within the MapRegion.  Calling code may need to use the (float)((double)val % 1.0) technique to ensure enough bits of precision when taking the fractional part (% 1.0), if val is large (for example a BlockPos in a 8Mx8M world)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public int GetUnpaddedColorLerpedForNormalizedPos(float x, float z)
        {
            int innerSize = InnerSize;
            return GetUnpaddedColorLerped(x * innerSize, z * innerSize);
        }

        public int GetUnpaddedIntLerpedForBlockPos(int x, int z, int regionSize)
        {
            // Weird type casting is required to not loose precision on very large coordinates
            float posXInRegionMap = (float)((double)x / regionSize - x / regionSize);
            float posZInRegionMap = (float)((double)z / regionSize - z / regionSize);

            return GetUnpaddedColorLerpedForNormalizedPos(posXInRegionMap, posZInRegionMap);
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

        public int GetColorLerpedCorrectly(float x, float z)
        {
            int posXLeft = (int)Math.Floor(x - 0.5f);
            int posZLeft = (int)Math.Floor(z - 0.5f);

            float fx = x - (posXLeft + 0.5f);
            float fz = z - (posZLeft + 0.5f);

            int index = (posZLeft + TopLeftPadding) * Size + posXLeft + TopLeftPadding;

            return GameMath.BiLerpRgbColor(
                fx, fz,
                Data[index],
                Data[index + 1],
                Data[index + Size],
                Data[index + Size + 1]
            );
        }

        /// <summary>
        /// Takes global chunk coordinates, returns the values of the 4 corners in that region.
        /// </summary>
        public IntMapChunkData GetValues(int chunkX, int chunkZ)
        {
            if (Data.Length == 0) return new IntMapChunkData();

            int rlX = chunkX % 16;
            int rlZ = chunkZ % 16;

            float factor = (float)InnerSize / 16;

            IntMapChunkData mapData;

            mapData.UpperLeft = GetUnpaddedInt((int)(rlX * factor), (int)(rlZ * factor));
            mapData.UpperRight = GetUnpaddedInt((int)((rlX * factor) + factor), (int)(rlZ * factor));
            mapData.BottomLeft = GetUnpaddedInt((int)(rlX * factor), (int)((rlZ * factor) + factor));
            mapData.BottomRight = GetUnpaddedInt((int)((rlX * factor) + factor), (int)((rlZ * factor) + factor));

            return mapData;
        }

        public struct IntMapChunkData
        {
            public int UpperLeft;
            public int UpperRight;
            public int BottomLeft;
            public int BottomRight;

            public readonly float LerpForChunk(int localChunkX, int localChunkZ)
            {
                const float chunkBlockDelta = 1f / 32;

                float result = GameMath.BiLerp(UpperLeft, UpperRight, BottomLeft, BottomRight, localChunkX * chunkBlockDelta, localChunkZ * chunkBlockDelta);

                return result;
            }
        }
    }
}
