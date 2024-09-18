using System;
using System.IO;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common.Entities
{
    public class FuzzyEntityPos : EntityPos
    {
        public float Radius;
        public int UsesLeft;

        public FuzzyEntityPos(double x, double y, double z, float heading = 0, float pitch = 0, float roll = 0) : base(x, y, z, heading, pitch, roll)
        {
            
        }
    }

    /// <summary>
    /// Represents all positional information of an entity, such as coordinates, motion and angles
    /// </summary>
    public class EntityPos
    {
        protected double x;
        protected double y;
        protected double z;
        public int Dimension;
        protected float roll; // "rotX"
        protected float yaw; // "rotY"
        protected float pitch; // "rotZ"
        protected int stance;
        /// <summary>
        /// The yaw of the agents head
        /// </summary>
        public float HeadYaw;

        

        /// <summary>
        /// The pitch of the agents head
        /// </summary>
        public float HeadPitch;


        public Vec3d Motion = new Vec3d();

        /// <summary>
        /// The X position of the Entity.
        /// </summary>
        public virtual double X {
            get { return x; }
            set { x = value; }
        }
        
        /// <summary>
        /// The Y position of the Entity.
        /// </summary>
        public virtual double Y
        {
            get { return y; }
            set { y = value; }
        }
        
        public virtual double InternalY
        {
            get { return y + Dimension * BlockPos.DimensionBoundary; }
        }

        /// <summary>
        /// The Z position of the Entity.
        /// </summary>
        public virtual double Z
        {
            get { return z; }
            set { z = value; }
        }

        public virtual int DimensionYAdjustment
        {
            get { return Dimension * BlockPos.DimensionBoundary; }
        }

        /// <summary>
        /// The rotation around the X axis, in radians.
        /// </summary>
        public virtual float Roll
        {
            get { return roll; }
            set { roll = value; }
        }

        /// <summary>
        /// The rotation around the Y axis, in radians.
        /// </summary>
        public virtual float Yaw
        {
            get { return yaw; }
            set { yaw = value; } 
        }

        /// <summary>
        /// The rotation around the Z axis, in radians.
        /// </summary>
        public virtual float Pitch
        {
            get { return pitch; }
            set { pitch = value; }
        }


        #region Position

        /// <summary>
        /// Returns the position as BlockPos object
        /// </summary>
        public BlockPos AsBlockPos
        {
            get { return new BlockPos((int)X, (int)Y, (int)Z, Dimension);  }
        }

        /// <summary>
        /// Returns the position as a Vec3i object
        /// </summary>
        public Vec3i XYZInt
        {
            get { return new Vec3i(XInt, YInt, ZInt); }
        }

        /// <summary>
        /// Returns the position as a Vec3d object. Note, dimension aware
        /// </summary>
        public Vec3d XYZ
        {
            get { return new Vec3d(X, InternalY, Z); }
        }

        /// <summary>
        /// Returns the position as a Vec3f object
        /// </summary>
        public Vec3f XYZFloat
        {
            get { return new Vec3f((float)X, (float)Y, (float)Z); }
        }

        internal int XInt
        {
            get { return (int)X; }
        }
        internal int YInt
        {
            get { return (int)Y; }
        }
        internal int ZInt
        {
            get { return (int)Z; }
        }

        /// <summary>
        /// Sets this position to a Vec3d, including setting the dimension
        /// </summary>
        /// <param name="pos">The Vec3d to set to.</param>
        public void SetPosWithDimension(Vec3d pos)
        {
            this.x = pos.X;
            this.y = pos.Y % BlockPos.DimensionBoundary;
            this.z = pos.Z;
            this.Dimension = (int)pos.Y / BlockPos.DimensionBoundary;
        }

        /// <summary>
        /// Sets this position to a Vec3d, without dimension information - needed in some situations where no dimension change is intended
        /// </summary>
        /// <param name="pos">The Vec3d to set to.</param>
        public void SetPos(Vec3d pos)
        {
            this.x = pos.X;
            this.y = pos.Y;
            this.z = pos.Z;
        }


        #endregion

        public EntityPos()
        {

        }

        public EntityPos(double x, double y, double z, float heading = 0, float pitch = 0, float roll = 0)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Yaw = heading;
            this.Pitch = pitch;
            this.Roll = roll;
        }

        /// <summary>
        /// Adds given position offset
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns>Returns itself</returns>
        public EntityPos Add(double x, double y, double z)
        {
            this.X += x;
            this.Y += y;
            this.Z += z;
            return this;
        }

        /// <summary>
        /// Adds given position offset
        /// </summary>
        /// <param name="vec"></param>
        /// <returns>Returns itself</returns>
        public EntityPos Add(Vec3f vec)
        {
            this.X += vec.X;
            this.Y += vec.Y;
            this.Z += vec.Z;
            return this;
        }

        /// <summary>
        /// Sets the entity position.
        /// </summary>
        public EntityPos SetPos(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            return this;
        }
        /// <summary>
        /// Sets the entity position.
        /// </summary>
        public EntityPos SetPos(BlockPos pos)
        {
            this.X = pos.X;
            this.Y = pos.Y;
            this.Z = pos.Z;
            return this;
        }

        /// <summary>
        /// Sets the entity position.
        /// </summary>
        public EntityPos SetPos(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            return this;
        }

        /// <summary>
        /// Sets the entity position.
        /// </summary>
        public EntityPos SetPos(EntityPos pos)
        {
            this.X = pos.X;
            this.Y = pos.Y;
            this.Z = pos.Z;
            return this;
        }

        /// <summary>
        /// Sets the entity angles.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public EntityPos SetAngles(EntityPos pos)
        {
            this.Roll = pos.Roll;
            this.Yaw = pos.Yaw;
            this.Pitch = pos.Pitch;
            this.HeadPitch = pos.HeadPitch;
            this.HeadYaw = pos.HeadYaw;
            return this;
        }

        /// <summary>
        /// Sets the entity position.
        /// </summary>
        public EntityPos SetAngles(float roll, float yaw, float pitch)
        {
            this.Roll = roll;
            this.Yaw = yaw;
            this.Pitch = pitch;
            return this;
        }

        /// <summary>
        /// Sets the Yaw of this entity.
        /// </summary>
        /// <param name="yaw"></param>
        /// <returns></returns>
        public EntityPos SetYaw(float yaw)
        {
            this.Yaw = yaw;
            return this;
        }


        /*===== All below methods have been updated to use the x/y/z fields instead of the X/Y/Z properties for higher performance (verified by profiler to gain notable performance) =====*/

        /// <summary>
        /// Returns true if the entity is within given distance of the other entity
        /// </summary>
        /// <param name="position"></param>
        /// <param name="squareDistance"></param>
        /// <returns></returns>
        public bool InRangeOf(EntityPos position, int squareDistance)
        {
            double dx = this.x - position.X;
            double dy = this.InternalY - position.InternalY;
            double dz = this.z - position.Z;

            return dx*dx + dy*dy + dz*dz <= squareDistance;
        }

        /// <summary>
        /// Returns true if the entity is within given distance of given position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="squareDistance"></param>
        /// <returns></returns>
        public bool InRangeOf(int x, int y, int z, float squareDistance)
        {
            double dx = this.x - x;
            double dy = this.InternalY - y;
            double dz = this.z - z;

            return dx * dx + dy * dy + dz * dz <= squareDistance;
        }

        /// <summary>
        /// Returns true if the entity is within given distance of given position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="squareDistance"></param>
        /// <returns></returns>
        public bool InHorizontalRangeOf(int x, int z, float squareDistance)
        {
            double dx = this.x - x;
            double dz = this.z - z;

            return dx * dx + dz * dz <= squareDistance;
        }

        /// <summary>
        /// Returns true if the entity is within given distance of given position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="squareDistance"></param>
        /// <returns></returns>
        public bool InRangeOf(double x, double y, double z, float squareDistance)
        {
            double dx = this.x - x;
            double dy = this.InternalY - y;
            double dz = this.z - z;

            return dx * dx + dy * dy + dz * dz <= squareDistance;
        }

        /// <summary>
        /// Returns true if the entity is within given distance of given block position
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="squareDistance"></param>
        /// <returns></returns>
        public bool InRangeOf(BlockPos pos, float squareDistance)
        {
            double dx = this.x - pos.X;
            double dy = this.InternalY - pos.InternalY;
            double dz = this.z - pos.Z;

            return dx * dx + dy * dy + dz * dz <= squareDistance;
        }

        /// <summary>
        /// Returns true if the entity is within given distance of given position
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="squareDistance"></param>
        /// <returns></returns>
        public bool InRangeOf(Vec3f pos, float squareDistance)
        {
            double dx = x - pos.X;
            double dy = InternalY - pos.Y;
            double dz = z - pos.Z;

            return dx * dx + dy * dy + dz * dz <= squareDistance;
        }

        /// <summary>
        /// Returns true if the entity is within given distance of given position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="horRangeSq"></param>
        /// <param name="vertRange"></param>
        /// <returns></returns>
        public bool InRangeOf(Vec3d position, float horRangeSq, float vertRange)
        {
            double dx = x - position.X;
            double dz = z - position.Z;
            if (dx * dx + dz * dz > horRangeSq) return false;

            double dy = InternalY - position.Y;
            return Math.Abs(dy) <= vertRange;
        }

        /// <summary>
        /// Returns the squared distance of the entity to this position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public float SquareDistanceTo(float x, float y, float z)
        {
            double dx = this.x - x;
            double dy = this.InternalY - y;
            double dz = this.z - z;

            return (float)(dx * dx + dy * dy + dz * dz);
        }

        /// <summary>
        /// Returns the squared distance of the entity to this position. Note: dimension aware, this requires the parameter y coordinate also to be based on InternalY as it should be (like EntityPos.XYZ)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public float SquareDistanceTo(double x, double y, double z)
        {
            double dx = this.x - x;
            double dy = this.InternalY - y;
            double dz = this.z - z;

            return (float)(dx * dx + dy * dy + dz * dz);
        }

        /// <summary>
        /// Returns the squared distance of the entity to this position. Note: dimension aware, this requires the parameter Vec3d pos.Y coordinate also to be based on InternalY as it should be (like EntityPos.XYZ)
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public double SquareDistanceTo(Vec3d pos)
        {
            double dx = this.x - pos.X;
            double dy = this.InternalY - pos.Y;
            double dz = this.z - pos.Z;

            return (dx * dx + dy * dy + dz * dz);
        }

        /// <summary>
        /// Returns the horizontal squared distance of the entity to this position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public double SquareHorDistanceTo(Vec3d pos)
        {
            double dx = this.x - pos.X;
            double dz = this.z - pos.Z;

            return (dx * dx + dz * dz);
        }

        public double DistanceTo(Vec3d pos)
        {
            double dx = this.x - pos.X;
            double dy = this.InternalY - pos.Y;
            double dz = this.z - pos.Z;

            return GameMath.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public double DistanceTo(EntityPos pos)
        {
            double dx = this.x - pos.x;
            double dy = this.InternalY - pos.InternalY;
            double dz = this.z - pos.z;

            return GameMath.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public double HorDistanceTo(Vec3d pos)
        {
            double dx = this.x - pos.X;
            double dz = this.z - pos.Z;

            return GameMath.Sqrt(dx * dx + dz * dz);
        }

        public double HorDistanceTo(EntityPos pos)
        {
            double dx = this.x - pos.X;
            double dz = this.z - pos.Z;

            return GameMath.Sqrt(dx * dx + dz * dz);
        }


        /// <summary>
        /// Returns the squared distance of the entity to this position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public float SquareDistanceTo(EntityPos pos)
        {
            double dx = x - pos.X;
            double dy = InternalY - pos.InternalY;
            double dz = z - pos.Z;

            return (float)(dx * dx + dy * dy + dz * dz);
        }

        /// <summary>
        /// Creates a full copy
        /// </summary>
        /// <returns></returns>
        public EntityPos Copy()
        {
            EntityPos ret = new EntityPos()
            {
                X = X,
                Y = Y,
                Z = Z,
                Yaw = Yaw,
                Pitch = Pitch,
                Roll = Roll,
                HeadYaw = HeadYaw,
                HeadPitch = HeadPitch,
                Motion = new Vec3d(Motion.X, Motion.Y, Motion.Z)
            };
            
            return ret;
        }

        /// <summary>
        /// Same as AheadCopy(1) - AheadCopy(0)
        /// </summary>
        /// <returns></returns>
        public Vec3f GetViewVector()
        {
            return GetViewVector(pitch, yaw);
        }


        /// <summary>
        /// Same as AheadCopy(1) - AheadCopy(0)
        /// </summary>
        /// <returns></returns>
        public static Vec3f GetViewVector(float pitch, float yaw)
        {
            float cosPitch = GameMath.Cos(pitch);
            float sinPitch = GameMath.Sin(pitch);

            float cosYaw = GameMath.Cos(yaw);
            float sinYaw = GameMath.Sin(yaw);

            return new Vec3f(-cosPitch * sinYaw, sinPitch, -cosPitch * cosYaw);
        }

        /// <summary>
        /// Returns a new entity position that is in front of the position the entity is currently looking at
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public EntityPos AheadCopy(double offset)
        {
            float cosPitch = GameMath.Cos(Pitch);
            float sinPitch = GameMath.Sin(Pitch);

            float cosYaw = GameMath.Cos(Yaw);
            float sinYaw = GameMath.Sin(Yaw);

            EntityPos copy = new EntityPos(X - cosPitch * sinYaw * offset, Y + sinPitch * offset, Z - cosPitch * cosYaw * offset, Yaw, Pitch, Roll);
            copy.Dimension = Dimension;
            return copy;
        }

        /// <summary>
        /// Returns a new entity position that is in front of the position the entity is currently looking at using only the entities yaw, meaning the resulting coordinate will be always at the same y position.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public EntityPos HorizontalAheadCopy(double offset)
        {
            float cosYaw = GameMath.Cos(Yaw);
            float sinYaw = GameMath.Sin(Yaw);

            return new EntityPos(X + sinYaw * offset, Y, Z + cosYaw * offset, Yaw, Pitch, Roll);
        }

        /// <summary>
        /// Returns a new entity position that is behind of the position the entity is currently looking at
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public EntityPos BehindCopy(double offset)
        {
            float cosYaw = GameMath.Cos(-Yaw);
            float sinYaw = GameMath.Sin(-Yaw);

            return new EntityPos(X + sinYaw * offset, Y, Z + cosYaw * offset, Yaw, Pitch, Roll);
        }

        
        /// <summary>
        /// Makes a "basiclly equals" check on the position, motions and angles using a small tolerance of epsilon=0.0001f 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool BasicallySameAs(EntityPos pos, double epsilon = 0.0001)
        {
            double epsilonSquared = epsilon * epsilon;
            return
                GameMath.SumOfSquares(X - pos.x, Y - pos.y, Z - pos.z) < epsilonSquared &&            // Math.Abs involves conditional statements; instead of comparing absolutes, it is faster (and mathematically equivalent) to compare the squared values
                GameMath.Square(roll - pos.roll) < epsilonSquared &&
                GameMath.Square(yaw - pos.yaw) < epsilonSquared &&
                GameMath.Square(pitch - pos.pitch) < epsilonSquared &&
                GameMath.SumOfSquares(Motion.X - pos.Motion.X, Motion.Y - pos.Motion.Y, Motion.Z - pos.Motion.Z) < epsilonSquared
            ;
        }


        /// <summary>
        /// Makes a "basiclly equals" check on the position, motions and angles using a small tolerance of epsilon=0.0001f. Ignores motion
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool BasicallySameAsIgnoreMotion(EntityPos pos, double epsilon = 0.0001)
        {
            double epsilonSquared = epsilon * epsilon;
            return
                GameMath.Square(X - pos.x) < epsilonSquared &&
                GameMath.Square(Y - pos.y) < epsilonSquared &&
                GameMath.Square(Z - pos.z) < epsilonSquared &&
                GameMath.Square(roll - pos.roll) < epsilonSquared &&
                GameMath.Square(yaw - pos.yaw) < epsilonSquared &&
                GameMath.Square(pitch - pos.pitch) < epsilonSquared
            ;
        }

        /// <summary>
        /// Makes a "basiclly equals" check on position and motions using a small tolerance of epsilon=0.0001f. Ignores the entities angles.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool BasicallySameAsIgnoreAngles(EntityPos pos, double epsilon = 0.0001)
        {
            double epsilonSquared = epsilon * epsilon;
            return
                GameMath.SumOfSquares(X - pos.x, Y - pos.y, Z - pos.z) < epsilonSquared &&            // Math.Abs involves conditional statements; instead of comparing absolutes, it is faster (and mathematically equivalent) to compare the squared values
                GameMath.SumOfSquares(Motion.X - pos.Motion.X, Motion.Y - pos.Motion.Y, Motion.Z - pos.Motion.Z) < epsilonSquared
            ;
        }



        /// <summary>
        /// Loads the position and angles from given entity position.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>Returns itself</returns>
        public EntityPos SetFrom(EntityPos pos)
        {
            X = pos.X;
            Y = pos.Y;
            Z = pos.Z;
            Dimension = pos.Dimension;
            Roll = pos.Roll;
            Yaw = pos.Yaw;
            Pitch = pos.Pitch;
            Motion.X = pos.Motion.X;
            Motion.Y = pos.Motion.Y;
            Motion.Z = pos.Motion.Z;
            HeadYaw = pos.HeadYaw;
            HeadPitch = pos.HeadPitch;

            return this;
        }

        /// <summary>
        /// Loads the position from given position.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>Returns itself</returns>
        public EntityPos SetFrom(Vec3d pos)
        {
            X = pos.X;
            Y = pos.Y;
            Z = pos.Z;

            return this;
        }


        public override string ToString()
        {
            return "XYZ: " + X + "/" + Y + "/" + Z + ", YPR " + Yaw + "/" + Pitch + "/" + Roll;
        }

        public string OnlyPosToString()
        {
            return X.ToString("#.##", GlobalConstants.DefaultCultureInfo) + ", " + Y.ToString("#.##", GlobalConstants.DefaultCultureInfo) + ", " + Z.ToString("#.##", GlobalConstants.DefaultCultureInfo);
        }


        public string OnlyAnglesToString()
        {
            return roll.ToString("#.##", GlobalConstants.DefaultCultureInfo) + ", " + yaw.ToString("#.##", GlobalConstants.DefaultCultureInfo) + pitch.ToString("#.##", GlobalConstants.DefaultCultureInfo);
        }

        /// <summary>
        /// Serializes all positional information. Does not write HeadYaw and HeadPitch.
        /// </summary>
        /// <param name="writer"></param>
        public void ToBytes(BinaryWriter writer)
        {
            writer.Write(x);
            writer.Write(InternalY);
            writer.Write(z);
            writer.Write(roll);
            writer.Write(yaw);
            writer.Write(pitch);
            writer.Write(stance);
            writer.Write(Motion.X);
            writer.Write(Motion.Y);
            writer.Write(Motion.Z);
        }

        /// <summary>
        /// Deserializes all positional information. Does not read HeadYaw and HeadPitch
        /// </summary>
        /// <param name="reader"></param>
        public void FromBytes(BinaryReader reader)
        {
            x = reader.ReadDouble();
            y = reader.ReadDouble();
            Dimension = (int)y / BlockPos.DimensionBoundary;
            y -= Dimension * BlockPos.DimensionBoundary;
            z = reader.ReadDouble();
            roll = reader.ReadSingle();
            yaw = reader.ReadSingle();
            pitch = reader.ReadSingle();
            stance = reader.ReadInt32();
            Motion.X = reader.ReadDouble();
            Motion.Y = reader.ReadDouble();
            Motion.Z = reader.ReadDouble();
        }

    }
}
