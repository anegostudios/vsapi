using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.MathTools
{
    public class Cardinal
    {
        private static Dictionary<Vec3i, Cardinal> byNormali = new Dictionary<Vec3i, Cardinal>();
        private static Dictionary<string, Cardinal> byInitial = new Dictionary<string, Cardinal>();

        public static readonly Cardinal North = new Cardinal("north", "n", new Vec3i(0,0,-1), 0, 4, false);
        public static readonly Cardinal NorthEast = new Cardinal("northeast", "ne", new Vec3i(1, 0, -1), 1, 5, true);
        public static readonly Cardinal East = new Cardinal("east", "e", new Vec3i(1, 0, 0), 2, 6, false);
        public static readonly Cardinal SouthEast = new Cardinal("southeast", "se", new Vec3i(1, 0, 1), 3, 7, true);
        public static readonly Cardinal South = new Cardinal("south", "s", new Vec3i(0, 0, 1), 4, 0, false);
        public static readonly Cardinal SouthWest = new Cardinal("southwest", "sw", new Vec3i(-1, 0, 1), 5, 1, true);
        public static readonly Cardinal West = new Cardinal("west", "w", new Vec3i(-1, 0, 0), 6, 2, false);
        public static readonly Cardinal NorthWest = new Cardinal("northwest", "nw", new Vec3i(-1, 0, -1), 7, 3, true);

        //public static readonly Cardinal Up = new Cardinal(new Vec3i(0, 1, 0));
        //public static readonly Cardinal Down = new Cardinal(new Vec3i(0, -1, 0));

        public static Cardinal[] ALL = new Cardinal[] { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest };

        public Vec3i Normali {get; private set;}
        public Cardinal Opposite { get { return ALL[OppositeIndex]; } }
        public int Index { get; private set; }
        public string Initial { get; private set; }
        public string Code { get; private set; }

        public bool IsDiagnoal { get; private set; }

        public int OppositeIndex { get; private set; }

        public Cardinal(string code, string initial, Vec3i normali, int index, int oppositeIndex, bool isDiagonal)
        {
            this.Code = code;
            this.Initial = initial;
            this.Normali = normali;
            this.Index = index;
            this.IsDiagnoal = isDiagonal;
            this.OppositeIndex = oppositeIndex;
            byNormali.Add(normali, this);
            byInitial.Add(initial, this);
        }

        public static Cardinal FromNormali(Vec3i normali)
        {
            Cardinal card;
            byNormali.TryGetValue(normali, out card);
            return card;
        }

        public static Cardinal FromInitial(string initials)
        {
            Cardinal card;
            byInitial.TryGetValue(initials, out card);
            return card;
        }
    }

    /// <summary>
    /// Represents one of the 6 faces of a cube and all it's properties. Uses a right Handed Coordinate System. See also http://www.matrix44.net/cms/notes/opengl-3d-graphics/coordinate-systems-in-opengl
    /// In short: 
    /// North: Negative Z
    /// East: Positive X
    /// South: Positive Z
    /// West: Negative X
    /// Up: Positive Y
    /// Down: Negative Y
    /// </summary>
    public class BlockFacing
    {
        public const int NumberOfFaces = 6;
        public const int indexNORTH = 0;
        public const int indexEAST = 1;
        public const int indexSOUTH = 2;
        public const int indexWEST = 3;
        public const int indexUP = 4;
        public const int indexDOWN = 5;

        /// <summary>
        /// All horizontal blockfacing flags combined
        /// </summary>
        public static readonly byte HorizontalFlags = 1 | 2 | 4 | 8;

        /// <summary>
        /// All vertical blockfacing flags combined
        /// </summary>
        public static readonly byte VerticalFlags = 16 | 32;

        
        /// <summary>
        /// Faces towards negative Z
        /// </summary>
        public static readonly BlockFacing NORTH = new BlockFacing("north", 1, 0, 2, 1, new Vec3i(0, 0, -1), new Vec3f(0.5f, 0.5f, 0f), EnumAxis.Z);
        /// <summary>
        /// Faces towards positive X
        /// </summary>
        public static readonly BlockFacing EAST = new BlockFacing("east",   2, 1, 3, 0, new Vec3i(1, 0, 0), new Vec3f(1f, 0.5f, 0.5f), EnumAxis.X);
        /// <summary>
        /// Faces towards positive Z
        /// </summary>
        public static readonly BlockFacing SOUTH = new BlockFacing("south", 4, 2, 0, 3, new Vec3i(0, 0, 1), new Vec3f(0.5f, 0.5f, 1f), EnumAxis.Z);
        /// <summary>
        /// Faces towards negative X
        /// </summary>
        public static readonly BlockFacing WEST = new BlockFacing("west",   8, 3, 1, 2, new Vec3i(-1, 0, 0), new Vec3f(0, 0.5f, 0.5f), EnumAxis.X);

        /// <summary>
        /// Faces towards positive Y
        /// </summary>
        public static readonly BlockFacing UP = new BlockFacing("up",      16, 4, 5, -1, new Vec3i(0, 1, 0), new Vec3f(0.5f, 1, 0.5f), EnumAxis.Y);
        /// <summary>
        /// Faces towards negative Y
        /// </summary>
        public static readonly BlockFacing DOWN = new BlockFacing("down",  32, 5, 4, -1, new Vec3i(0, -1, 0), new Vec3f(0.5f, 0, 0.5f), EnumAxis.Y);

        /// <summary>
        /// All block faces in the order of N, E, S, W, U, D
        /// </summary>
        public static readonly BlockFacing[] ALLFACES = new BlockFacing[] { NORTH, EAST, SOUTH, WEST, UP, DOWN };

        /// <summary>
        /// All block faces in the order of N, E, S, W, U, D
        /// </summary>
        public static readonly Vec3i[] ALLNORMALI = new Vec3i[] { NORTH.normali, EAST.normali, SOUTH.normali, WEST.normali, UP.normali, DOWN.normali };

        /// <summary>
        /// Packed ints representing the normal flags, left-shifted by 15 for easy inclusion in VertexFlags
        /// </summary>
        public static readonly int[] AllVertexFlagsNormals = new int[] { NORTH.normalPackedFlags, EAST.normalPackedFlags, SOUTH.normalPackedFlags, WEST.normalPackedFlags, UP.normalPackedFlags, DOWN.normalPackedFlags };

        /// <summary>
        /// Array of horizontal faces (N, E, S, W)
        /// </summary>
        public static readonly BlockFacing[] HORIZONTALS = new BlockFacing[] { NORTH, EAST, SOUTH, WEST };
        /// <summary>
        /// Array of vertical faces (U, D)
        /// </summary>
        public static readonly BlockFacing[] VERTICALS = new BlockFacing[] { UP, DOWN };

        /// <summary>
        /// Array of horizontal faces in angle order (0°, 90°, 180°, 270°) => (E, N, W, S)
        /// </summary>
        public static readonly BlockFacing[] HORIZONTALS_ANGLEORDER = new BlockFacing[] { EAST, NORTH, WEST, SOUTH };

        int index;
        byte meshDataIndex;
        int horizontalAngleIndex;
        byte flag;
        int oppositeIndex;
        Vec3i normali;
        Vec3f normalf;
        byte normalb;
        int normalPacked;
        int normalPackedFlags;
        Vec3f planeCenter;
        string code;
        EnumAxis axis;

        /// <summary>
        /// The faces byte flag
        /// </summary>
        public byte Flag { get { return flag; } }
        /// <summary>
        /// The index of the face (N=0, E=1, S=2, W=3, U=4, D=5)
        /// </summary>
        public int Index { get { return index; } }
        /// <summary>
        /// Index + 1
        /// </summary>
        public byte MeshDataIndex { get { return meshDataIndex; } }

        /// <summary>
        /// The angle index of the face (E = 0, N = 1, W = 2, S = 3)
        /// </summary>
        public int HorizontalAngleIndex { get { return horizontalAngleIndex; } }
        /// <summary>
        /// Returns a normal vector of this face
        /// </summary>
        public Vec3i Normali { get { return normali; } } 
        /// <summary>
        /// Returns a normal vector of this face
        /// </summary>
        public Vec3f Normalf { get { return normalf; } }

        /// <summary>
        /// Returns a normal vector of this face encoded in 6 bits/
        /// bit 0: 1 if south or west
        /// bit 1: sign bit 
        /// bit 2: 1 if up or down
        /// bit 3: sign bit 
        /// bit 4: 1 if north or south
        /// bit 5: sign bit 
        /// </summary>
        public byte NormalByte { get { return normalb; } }

        /// <summary>
        /// Normalized normal vector in format GL_INT_2_10_10_10_REV
        /// </summary>
        public int NormalPacked { get { return normalPacked; } }

        /// <summary>
        /// Normalized normal vector packed into 3x4=12 bytes total and bit shifted by 15 bits, for use in meshdata flags data
        /// </summary>
        public int NormalPackedFlags { get { return normalPackedFlags; } }

        /// <summary>
        /// Returns the center position of this face
        /// </summary>
        public Vec3f PlaneCenter { get { return planeCenter; } }
        /// <summary>
        /// Returns the string north, east, south, west, up or down
        /// </summary>
        public string Code { get { return code; } }
        /// <summary>
        /// True if this face is N,E,S or W
        /// </summary>
        public bool IsHorizontal { get { return index <= 3; } }
        /// <summary>
        /// True if this face is U or D
        /// </summary>
        public bool IsVertical { get { return index >= 4; } }
        /// <summary>
        /// True if this face is N or S
        /// </summary>
        public bool IsAxisNS { get { return index == 0 || index == 2; } }
        /// <summary>
        /// True if this face is N or S
        /// </summary>
        public bool IsAxisWE { get { return index == 1 || index == 3; } }
        /// <summary>
        /// The normal axis of this vector.
        /// </summary>
        public EnumAxis Axis { get { return axis; } }

        private BlockFacing(string code, byte flag, int index, int oppositeIndex, int horizontalAngleIndex, Vec3i facingVector, Vec3f planeCenter, EnumAxis axis)
        {
            this.index = index;
            this.meshDataIndex = (byte)(index + 1);
            this.horizontalAngleIndex = horizontalAngleIndex;
            this.flag = flag;
            this.code = code;
            this.oppositeIndex = oppositeIndex;
            this.normali = facingVector;
            this.normalf = new Vec3f(facingVector.X, facingVector.Y, facingVector.Z);

            normalPacked = NormalUtil.PackNormal(normalf.X, normalf.Y, normalf.Z);
            normalb = (byte)(
                (axis == EnumAxis.Z ? 1 : 0) << 0
                | (facingVector.Z < 0 ? 1 : 0) << 1

                | (axis == EnumAxis.Y ? 1 : 0) << 2
                | (facingVector.Y < 0 ? 1 : 0) << 3

                | (axis == EnumAxis.X ? 1 : 0) << 4
                | (facingVector.X < 0 ? 1 : 0) << 5
            );

            normalPackedFlags = VertexFlags.NormalToPackedInt(normalf) << 15;

            this.planeCenter = planeCenter;
            this.axis = axis;
        }

        /// <summary>
        /// Returns the opposing face
        /// </summary>
        /// <returns></returns>
        public BlockFacing Opposite => ALLFACES[oppositeIndex];

        [Obsolete("Use Opposite property instead")]
        public BlockFacing GetOpposite()
        {
            return ALLFACES[oppositeIndex];
        }

        /// <summary>
        /// Returns the face if current face would be horizontally counter-clockwise rotated, only works for horizontal faces
        /// </summary>
        /// <returns></returns>
        public BlockFacing GetCCW()
        {
            return HORIZONTALS_ANGLEORDER[(horizontalAngleIndex + 1) % 4];
        }

        /// <summary>
        /// Returns the face if current face would be horizontally clockwise rotated, only works for horizontal faces
        /// </summary>
        /// <returns></returns>
        public BlockFacing GetCW()
        {
            return HORIZONTALS_ANGLEORDER[GameMath.Mod(horizontalAngleIndex - 1, 4)];
        }

       
        /// <summary>
        /// Applies a 3d rotation on the face and returns the face thats closest to the rotated face
        /// </summary>
        /// <param name="radX"></param>
        /// <param name="radY"></param>
        /// <param name="radZ"></param>
        /// <returns></returns>     
        public BlockFacing FaceWhenRotatedBy(float radX, float radY, float radZ)
        {
            float[] matrix = Mat4f.Create();

            Mat4f.RotateX(matrix, matrix, radX);
            Mat4f.RotateY(matrix, matrix, radY);
            Mat4f.RotateZ(matrix, matrix, radZ);

            float[] pos = new float[] { Normalf.X, Normalf.Y, Normalf.Z, 1 };
            pos = Mat4f.MulWithVec4(matrix, pos);

            float smallestAngle = GameMath.PI;
            BlockFacing facing = null;

            for (int i = 0; i < ALLFACES.Length; i++)
            {
                BlockFacing f = ALLFACES[i];
                float angle = (float)Math.Acos(f.Normalf.Dot(pos));

                if (angle < smallestAngle)
                {
                    smallestAngle = angle;
                    facing = f;
                }
            }

            return facing;
        }

        /// <summary>
        /// Rotates the face by given angle and returns the interpolated brightness of this face.
        /// </summary>
        /// <param name="radX"></param>
        /// <param name="radY"></param>
        /// <param name="radZ"></param>
        /// <param name="BlockSideBrightnessByFacing">Array of brightness values between 0 and 1 per face. In index order (N, E, S, W, U, D)</param>
        /// <returns></returns>
        public float GetFaceBrightness(float radX, float radY, float radZ, float[] BlockSideBrightnessByFacing)
        {
            float[] matrix = Mat4f.Create();

            Mat4f.RotateX(matrix, matrix, radX);
            Mat4f.RotateY(matrix, matrix, radY);
            Mat4f.RotateZ(matrix, matrix, radZ);

            FastVec3f pos = Mat4f.MulWithVec3(matrix, Normalf.X, Normalf.Y, Normalf.Z);

            float brightness = 0;

            for (int i = 0; i < ALLFACES.Length; i++)
            {
                BlockFacing f = ALLFACES[i];
                float angle = (float)Math.Acos(f.Normalf.Dot(pos));

                if (angle >= GameMath.PIHALF) continue;

                brightness += (1 - angle / GameMath.PIHALF) * BlockSideBrightnessByFacing[f.Index];
            }

            return brightness;
        }




        /// <summary>
        /// Rotates the face by given angle and returns the interpolated brightness of this face.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="BlockSideBrightnessByFacing">Array of brightness values between 0 and 1 per face. In index order (N, E, S, W, U, D)</param>
        /// <returns></returns>
        public float GetFaceBrightness(double[] matrix, float[] BlockSideBrightnessByFacing)
        {
            double[] pos = new double[] { Normalf.X, Normalf.Y, Normalf.Z, 1 };
            matrix[12] = 0;
            matrix[13] = 0;
            matrix[14] = 0;
            pos = Mat4d.MulWithVec4(matrix, pos);


            float len = GameMath.Sqrt(pos[0] * pos[0] + pos[1] * pos[1] + pos[2] * pos[2]);
            pos[0] /= len;
            pos[1] /= len;
            pos[2] /= len;

            float brightness = 0;

            for (int i = 0; i < ALLFACES.Length; i++)
            {
                BlockFacing f = ALLFACES[i];
                float angle = (float)Math.Acos(f.Normalf.Dot(pos));

                if (angle >= GameMath.PIHALF) continue;

                brightness += (1 - angle / GameMath.PIHALF) * BlockSideBrightnessByFacing[f.Index];
            }

            return brightness;
        }

        
        public bool IsAdjacent(BlockFacing facing)
        {
            if (IsVertical)
            {
                return facing.IsHorizontal;
            }

            return
                (IsHorizontal && facing.IsVertical)
                || (axis == EnumAxis.X && facing.axis == EnumAxis.Z)
                || (axis == EnumAxis.Z && facing.axis == EnumAxis.X)
            ;
        }


        public override string ToString()
        {
            return code;
        }





        /// <summary>
        /// Returns the face if code is 'north', 'east', 'south', 'west', 'north', 'up' or 'down'. Otherwise null.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static BlockFacing FromCode(string code)
        {
            code = code?.ToLowerInvariant();

            switch (code)
            {
                case "north": return NORTH;
                case "south": return SOUTH;
                case "east": return EAST;
                case "west": return WEST;
                case "up": return UP;
                case "down": return DOWN;
            }

            return null;
        }

        public static BlockFacing FromFirstLetter(char code)
        {
            return FromFirstLetter("" + code);
        }

        /// <summary>
        /// Returns the face if code is 'n', 'e', 's', 'w', 'n', 'u' or 'd'. Otherwise null.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static BlockFacing FromFirstLetter(string code)
        {
            code = code.ToLowerInvariant();

            switch (code)
            {
                case "n": return NORTH;
                case "s": return SOUTH;
                case "e": return EAST;
                case "w": return WEST;
                case "u": return UP;
                case "d": return DOWN;
            }

            return null;
        }



        public static BlockFacing FromNormal(Vec3f vec)
        {
            float smallestAngle = GameMath.PI;
            BlockFacing facing = null;


            for (int i = 0; i < ALLFACES.Length; i++)
            {
                BlockFacing f = ALLFACES[i];
                float angle = (float)Math.Acos(f.Normalf.Dot(vec));

                if (angle < smallestAngle)
                {
                    smallestAngle = angle;
                    facing = f;
                }
            }

            return facing;
        }


        public static BlockFacing FromNormal(Vec3i vec)
        {
            Cardinal c = Cardinal.FromNormali(vec);
            if (c == null) return null;
            return FromFirstLetter(c.Initial);
        }


        public static BlockFacing FromVector(double x, double y, double z)
        {
            float smallestAngle = GameMath.PI;
            BlockFacing facing = null;

            double len = GameMath.Sqrt(x * x + y * y + z * z);
            x /= len;
            y /= len;
            z /= len;

            for (int i = 0; i < ALLFACES.Length; i++)
            {
                BlockFacing f = ALLFACES[i];
                float angle = (float)Math.Acos(f.Normalf.X * x + f.Normalf.Y * y + f.Normalf.Z * z);

                if (angle < smallestAngle)
                {
                    smallestAngle = angle;
                    facing = f;
                }
            }

            return facing;
        }

        public static BlockFacing FromFlag(int flag)
        {
            switch (flag)
            {
                case 1: return NORTH;
                case 4: return SOUTH;
                case 2: return EAST;
                case 8: return WEST;
                case 16: return UP;
                case 32: return DOWN;
            }

            return null;
        }



        /// <summary>
        /// Returns the closest horizontal face from given angle (0 degree = east). Uses HORIZONTALS_ANGLEORDER
        /// </summary>
        /// <param name="radiant"></param>
        /// <returns></returns>
        public static BlockFacing HorizontalFromAngle(float radiant)
        {
            int index = GameMath.Mod(((int)(Math.Round(radiant * GameMath.RAD2DEG / 90))), 4);

            return HORIZONTALS_ANGLEORDER[index];
        }


        /// <summary>
        /// Returns true if given byte flags contain given face 
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="facing"></param>
        /// <returns></returns>
        public static bool FlagContains(byte flag, BlockFacing facing)
        {
            return (flag & facing.flag) > 0;
        }


        /// <summary>
        /// Returns true if given byte flags contains a horizontal face
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool FlagContainsHorizontals(byte flag)
        {
            return (flag & HorizontalFlags) > 0;
        }

    }
}
