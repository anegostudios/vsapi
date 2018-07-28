using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API;
using Vintagestory.API.MathTools;

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

        }

        public SyncedEntityPos(double x, double y, double z, float heading = 0, float pitch = 0) : base(x, y, z, heading, pitch)
        {
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

        public override int Stance
        {
            get { return stance; }
            set { stance = value; Dirty = true; }
        }



        public double XInternal
        {
            set { x = value; }
        }

        public double YInternal
        {
            set { y = value; }
        }

        public double ZInternal
        {
            set { z = value; }
        }

        public float RollInternal
        {
            set { roll = value; }
        }

        public float YawInternal
        {
            set { yaw = value; }
        }

        public float PitchInternal
        {
            set { pitch = value; }
        }

        public int StanceInternal
        {
            set { stance = value; }
        }




        public bool Dirty
        {
            get { return dirty; }
            set { dirty = value; }
        }

        public void MarkClean()
        {
            dirty = false;
        }

    }
}
