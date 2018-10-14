using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.MathTools
{
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
        /// <summary>
        /// All horizontal blockfacing flags combined
        /// </summary>
        public static byte HorizontalFlags = 1 | 2 | 4 | 8;

        /// <summary>
        /// All vertical blockfacing flags combined
        /// </summary>
        public static byte VerticalFlags = 16 | 32;

        
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
        int horizontalAngleIndex;
        byte flag;
        int oppositeIndex;
        Vec3i normali;
        Vec3f normalf;
        byte normalb;
        int normalPacked;
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
        public byte NormalByte { get { return NormalByte; } }

        /// <summary>
        /// Normalized normal vector in format GL_INT_2_10_10_10_REV
        /// </summary>
        public int NormalPacked { get { return normalPacked; } }

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
        /// The normal axis of this vector.
        /// </summary>
        public EnumAxis Axis { get { return axis; } }

        private BlockFacing(string code, byte flag, int index, int oppositeIndex, int horizontalAngleIndex, Vec3i facingVector, Vec3f planeCenter, EnumAxis axis)
        {
            this.index = index;
            this.horizontalAngleIndex = horizontalAngleIndex;
            this.flag = flag;
            this.code = code;
            this.oppositeIndex = oppositeIndex;
            this.normali = facingVector;
            this.normalf = new Vec3f(facingVector.X, facingVector.Y, facingVector.Z);

            normalPacked = NormalUtil.PackNormal(new float[] { normalf.X, normalf.Y, normalf.Z, 0 });
            normalb = (byte)(
                (axis == EnumAxis.Z ? 1 : 0) << 0
                | (facingVector.Z < 0 ? 1 : 0) << 1

                | (axis == EnumAxis.Y ? 1 : 0) << 2
                | (facingVector.Y < 0 ? 1 : 0) << 3

                | (axis == EnumAxis.X ? 1 : 0) << 4
                | (facingVector.X < 0 ? 1 : 0) << 5
            );
            this.planeCenter = planeCenter;
            this.axis = axis;
        }

        /// <summary>
        /// Returns the opposing face
        /// </summary>
        /// <returns></returns>
        public BlockFacing GetOpposite()
        {
            return ALLFACES[oppositeIndex];
        }

        /// <summary>
        /// Returns the face if current face would be horizontally clockwise rotated, only works for horizontal faces
        /// </summary>
        /// <returns></returns>
        public BlockFacing GetCW()
        {
            return HORIZONTALS_ANGLEORDER[(horizontalAngleIndex + 1) % 4];
        }

        /// <summary>
        /// Returns the face if current face would be horizontally counter-clockwise rotated, only works for horizontal faces
        /// </summary>
        /// <returns></returns>
        public BlockFacing GetCCW()
        {
            return HORIZONTALS_ANGLEORDER[GameMath.Mod(horizontalAngleIndex - 1, 4)];
        }

        /// <summary>
        /// Returns the face if code is 'north', 'east', 'south', 'west', 'north', 'up' or 'down'. Otherwise null.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static BlockFacing FromCode(string code)
        {
            code = code?.ToLowerInvariant();

            switch(code)
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
        


        public static BlockFacing FromVector(Vec3f vec)
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


        public static BlockFacing FromVector(double x, double y, double z)
        {
            float smallestAngle = GameMath.PI;
            BlockFacing facing = null;

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

            float[] pos = new float[] { Normalf.X, Normalf.Y, Normalf.Z, 1 };
            pos = Mat4f.MulWithVec4(matrix, pos);

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


        public override string ToString()
        {
            return code;
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
