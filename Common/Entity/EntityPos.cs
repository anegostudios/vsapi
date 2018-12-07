using System;
using System.Globalization;
using System.IO;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common.Entities
{
    /// <summary>
    /// Represents all positional information of an entity, such as coordinates, motion and angles
    /// </summary>
    public class EntityPos
    {
        protected double x;
        protected double y;
        protected double z;
        protected float roll; // "rotX"
        protected float yaw; // "rotY"
        protected float pitch; // "rotZ"
        protected int stance;



        public Vec3d Motion = new Vec3d();

        public virtual double X {
            get { return x; }
            set { x = value; }
        }
        
        public virtual double Y
        {
            get { return y; }
            set { y = value; }
        }
        
        public virtual double Z
        {
            get { return z; }
            set { z = value; }
        }

        public virtual float Roll
        {
            get { return roll; }
            set { roll = value; }
        }
        public virtual float Yaw
        {
            get { return yaw; }
            set { yaw = value; } 
        }
        public virtual float Pitch
        {
            get { return pitch; }
            set { pitch = value; }
        }
        public virtual int Stance
        {
            get { return stance; }
            set { stance = value; }
        }

        #region Position

        /// <summary>
        /// Returns the position as BlockPos object
        /// </summary>
        public BlockPos AsBlockPos
        {
            get { return new BlockPos((int)X, (int)Y, (int)Z);  }
        }

        /// <summary>
        /// Returns the position as a Vec3i object
        /// </summary>
        public Vec3i XYZInt
        {
            get { return new Vec3i(XInt, YInt, ZInt); }
        }

        /// <summary>
        /// Returns the position as a Vec3d object
        /// </summary>
        public Vec3d XYZ
        {
            get { return new Vec3d(X, Y, Z); }
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


        public EntityPos SetPos(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            return this;
        }

        public EntityPos SetPos(BlockPos pos)
        {
            this.X = pos.X;
            this.Y = pos.Y;
            this.Z = pos.Z;
            return this;
        }


        public EntityPos SetPos(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            return this;
        }

        public EntityPos SetPos(EntityPos pos)
        {
            this.X = pos.X;
            this.Y = pos.Y;
            this.Z = pos.Z;
            return this;
        }

        public EntityPos SetAngles(EntityPos pos)
        {
            this.Roll = pos.Roll;
            this.Yaw = pos.Yaw;
            this.Pitch = pos.Pitch;
            return this;
        }

        public EntityPos SetAngles(float roll, float yaw, float pitch)
        {
            this.Roll = roll;
            this.Yaw = yaw;
            this.Pitch = pitch;
            return this;
        }

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
            double dy = this.y - position.Y;
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
            double dy = this.y - y;
            double dz = this.z - z;

            return dx * dx + dy * dy + dz * dz <= squareDistance;
        }

        /// <summary>
        /// Returns true if the entity is within given distance of given position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
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
            double dy = this.y - y;
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
            double dy = this.y - pos.Y;
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
            double dy = y - pos.Y;
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
            double dy = y - position.Y;
            double dz = z - position.Z;

            return dx * dx + dz * dz <= horRangeSq && Math.Abs(dy) <= vertRange;
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
            double dy = this.y - y;
            double dz = this.z - z;

            return (float)(dx * dx + dy * dy + dz * dz);
        }

        /// <summary>
        /// Returns the squared distance of the entity to this position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public float SquareDistanceTo(double x, double y, double z)
        {
            double dx = this.x - x;
            double dy = this.y - y;
            double dz = this.z - z;

            return (float)(dx * dx + dy * dy + dz * dz);
        }

        /// <summary>
        /// Returns the squared distance of the entity to this position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public double SquareDistanceTo(Vec3d pos)
        {
            double dx = this.x - pos.X;
            double dy = this.y - pos.Y;
            double dz = this.z - pos.Z;

            return (dx * dx + dy * dy + dz * dz);
        }


        /// <summary>
        /// Returns the squared distance of the entity to this position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public float SquareDistanceTo(EntityPos pos)
        {
            double dx = x - pos.X;
            double dy = y - pos.Y;
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
                Stance = Stance,
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

            float cosYaw = GameMath.Cos(yaw + GameMath.PI / 2);
            float sinYaw = GameMath.Sin(yaw + GameMath.PI / 2);

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

            float cosYaw = GameMath.Cos(Yaw + GameMath.PI / 2);
            float sinYaw = GameMath.Sin(Yaw + GameMath.PI / 2);

            return new EntityPos(X - cosPitch * sinYaw * offset, Y + sinPitch * offset, Z - cosPitch * cosYaw * offset, Yaw, Pitch, Roll);
        }

        /// <summary>
        /// Returns a new entity position that is in front of the position the entity is currently looking at using only the entities yaw, meaning the resulting coordinate will be always at the same y position.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public EntityPos HorizontalAheadCopy(double offset)
        {
            float cosYaw = GameMath.Cos(Yaw + GameMath.PI / 2);
            float sinYaw = GameMath.Sin(Yaw + GameMath.PI / 2);

            return new EntityPos(X + sinYaw * offset, Y, Z + cosYaw * offset, Yaw, Pitch, Roll);
        }

        /// <summary>
        /// Returns a new entity position that is behind of the position the entity is currently looking at
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public EntityPos BehindCopy(double offset)
        {
            float cosYaw = GameMath.Cos(Yaw - GameMath.PI / 2);
            float sinYaw = GameMath.Sin(Yaw - GameMath.PI / 2);

            return new EntityPos(X + sinYaw * offset, Y, Z + cosYaw * offset, Yaw, Pitch, Roll);
        }

        
        /// <summary>
        /// Makes a "basiclly equals" check on the position, motions and angles using a small tolerance of epsilon=0.0001f 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool BasicallySameAs(EntityPos pos, float epsilon = 0.0001f)
        {
            return
                Math.Abs(X - pos.x) < epsilon &&
                Math.Abs(Y - pos.y) < epsilon &&
                Math.Abs(Z - pos.z) < epsilon &&
                Math.Abs(roll - pos.roll) < epsilon &&
                Math.Abs(yaw - pos.yaw) < epsilon &&
                Math.Abs(pitch - pos.pitch) < epsilon &&
                Math.Abs(Motion.X - pos.Motion.X) < epsilon &&
                Math.Abs(Motion.Y - pos.Motion.Y) < epsilon &&
                Math.Abs(Motion.Z - pos.Motion.Z) < epsilon
            ;
        }

        /// <summary>
        /// Makes a "basiclly equals" check on position and motions using a small tolerance of epsilon=0.0001f. Ignores the entities angles.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool BasicallySameAsIgnoreAngles(EntityPos pos, float epsilon = 0.0001f)
        {
            return
                Math.Abs(X - pos.x) < epsilon &&
                Math.Abs(Y - pos.y) < epsilon &&
                Math.Abs(Z - pos.z) < epsilon &&
                Math.Abs(Motion.X - pos.Motion.X) < epsilon &&
                Math.Abs(Motion.Y - pos.Motion.Y) < epsilon &&
                Math.Abs(Motion.Z - pos.Motion.Z) < epsilon
            ;
        }


        /// <summary>
        /// Loads the position and angles from given entity position.
        /// </summary>
        /// <param name="pos"></param>
        public void SetFrom(EntityPos pos)
        {
            X = pos.X;
            Y = pos.Y;
            Z = pos.Z;
            Roll = pos.Roll;
            Yaw = pos.Yaw;
            Pitch = pos.Pitch;
            Motion.X = pos.Motion.X;
            Motion.Y = pos.Motion.Y;
            Motion.Z = pos.Motion.Z;
        }

        /// <summary>
        /// Loads the position from given position.
        /// </summary>
        /// <param name="pos"></param>
        public void SetFrom(Vec3d pos)
        {
            X = pos.X;
            Y = pos.Y;
            Z = pos.Z;
        }


        public override string ToString()
        {
            return "XYZ: " + X + "/" + Y + "/" + Z + ", YPR " + Yaw + "/" + Pitch + "/" + Roll;
        }

        public string OnlyPosToString()
        {
            return X.ToString("#.##", CultureInfo.InvariantCulture) + ", " + Y.ToString("#.##", CultureInfo.InvariantCulture) + ", " + Z.ToString("#.##", CultureInfo.InvariantCulture);
        }


        public string OnlyAnglesToString()
        {
            return roll.ToString("#.##", CultureInfo.InvariantCulture) + ", " + yaw.ToString("#.##", CultureInfo.InvariantCulture) + pitch.ToString("#.##", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Serializes all positional information
        /// </summary>
        /// <param name="writer"></param>
        public void ToBytes(BinaryWriter writer)
        {
            writer.Write(x);
            writer.Write(y);
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
        /// Deserializes all positional information
        /// </summary>
        /// <param name="reader"></param>
        public void FromBytes(BinaryReader reader)
        {
            x = reader.ReadDouble();
            y = reader.ReadDouble();
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
