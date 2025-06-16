using System.Runtime.CompilerServices;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client.Tesselation
{
    public class TileSideEnum
    {
        public const int North = 0;
        public const int East = 1;
        public const int South = 2;
        public const int West = 3;
        public const int Up = 4;
        public const int Down = 5;

        public const int SideCount = 6;

        public static int[] Opposites = new int[]
        {
            2, 3, 0, 1, 5, 4
        };

        public static int[] AxisByTileSide = new int[]
        {
            2, 0, 2, 0, 1, 1
        };

        public static FastVec3i[] OffsetByTileSide = new FastVec3i[]
        {
            new FastVec3i(0,0,-1),
            new FastVec3i(1,0,0),
            new FastVec3i(0,0,1),
            new FastVec3i(-1,0,0),
            new FastVec3i(0,1,0),
            new FastVec3i(0,-1,0)
        };

        public static int[] MoveIndex = new int[6];

        public static string[] Codes = new string[] { "north", "east", "south", "west", "up", "down" };

        // <summary>
        // Convert to TileSideEnumFlags
        // </summary>
        public static int ToFlags(int nValue)
        {
            switch (nValue)
            {
                case Up: return TileSideFlagsEnum.Up;
                case Down: return TileSideFlagsEnum.Down;
                case West: return TileSideFlagsEnum.West;
                case East: return TileSideFlagsEnum.East;
                case South: return TileSideFlagsEnum.South;
                case North: return TileSideFlagsEnum.North;
                default: return TileSideFlagsEnum.None;
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetOpposite(int tileSide)
        {
            // This piece of maths is equivalent to BlockFacing.ALLFACES[tileSide].Opposite.Index
            return tileSide ^ (2 - tileSide / 4);
        }

    }


    public class TileSideFlagsEnum
    {
        public const int None = 0;
        public const int North = 1;
        public const int East = 2;
        public const int South = 4;
        public const int West = 8;
        public const int Up = 16;
        public const int Down = 32;

        public const int All = North | East | South | West | Up | Down;

        public static bool HasFlag(int nFlagA, int nFlagB)
        {
            return (nFlagA & nFlagB) != None;
        }
    }


}
