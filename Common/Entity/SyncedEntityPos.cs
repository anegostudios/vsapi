using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common.Entities
{
    /// <summary>
    /// Represents an EntityPos thats synced over the network using a dirty flag and regular is-dirty checks
    /// </summary>
    public class SyncedEntityPos : EntityPos
    {
        bool dirty;
        public long LastReceivedClientPosition;

        public SyncedEntityPos()
        {

        }

        public SyncedEntityPos(Vec3d position) : base(position.X, position.Y, position.Z, 0, 0)
        {
            dirty = true;
        }

        public SyncedEntityPos(double x, double y, double z, float heading = 0, float pitch = 0) : base(x, y, z, heading, pitch)
        {
            dirty = true;
        }

        public override double X
        {
            get { return x; }
            set { x = value; Dirty = true; }
        }

        public override double Y
        {
            get { return y; }
            set { y = value; Dirty = true; }
        }

        public override double Z
        {
            get { return z; }
            set { z = value; Dirty = true; }
        }

        public override float Roll
        {
            get { return roll; }
            set { roll = value; Dirty = true; }
        }

        public override float Yaw
        {
            get { return yaw; }
            set { yaw = value; Dirty = true; }
        }

        public override float Pitch
        {
            get { return pitch; }
            set { pitch = value; Dirty = true; }
        }


        /// <summary>
        /// Internally sets the value of X.  This may cause desync.
        /// </summary>
        public double XInternal
        {
            set { x = value; }
        }

        /// <summary>
        /// Internally sets the value of Y.  This may cause desync.
        /// </summary>
        public double YInternal
        {
            set { y = value; }
        }

        /// <summary>
        /// Internally sets the value of Z.  This may cause desync.
        /// </summary>
        public double ZInternal
        {
            set { z = value; }
        }

        /// <summary>
        /// Sets the roll of the Entity Position.  This may cause desync.
        /// </summary>
        public float RollInternal
        {
            set { roll = value; }
        }

        /// <summary>
        /// Sets the yaw of the Entity Position.  This may cause desync.
        /// </summary>
        public float YawInternal
        {
            set { yaw = value; }
        }

        /// <summary>
        /// Sets the pitch of the Entity Position.  This may cause desync.
        /// </summary>
        public float PitchInternal
        {
            set { pitch = value; }
        }

        /// <summary>
        /// Sets the stance of the Entity Position.  This may cause desync.
        /// </summary>
        public int StanceInternal
        {
            set { stance = value; }
        }



        /// <summary>
        /// Marks the position as dirty- requiring a refresh from the server.
        /// </summary>
        public bool Dirty
        {
            get { return dirty; }
            set { dirty = value; }
        }

        /// <summary>
        /// Marks the position as clean- and not requiring a refresh from the server.
        /// </summary>
        public void MarkClean()
        {
            dirty = false;
        }

    }
}
