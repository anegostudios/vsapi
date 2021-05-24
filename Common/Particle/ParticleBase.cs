using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{

    /// <summary>
    /// Represents a particle that has been spawned
    /// </summary>
    public abstract class ParticleBase
    {
        public ParticleBase Next;
        public ParticleBase Prev;

        public byte ColorRed;
        public byte ColorGreen;
        public byte ColorBlue;
        public byte ColorAlpha;
        public int VertexFlags;
        public float LifeLength = 0;

        public float SecondsAlive = 0;
        public bool Alive;

        /// <summary>
        /// Returns the current position of the particle
        /// </summary>
        /// <returns></returns>
        public Vec3d Position = new Vec3d();
        public Vec3d PrevPosition = new Vec3d();
        protected double dx, dy, dz;

        /// <summary>
        /// Returns the current velocity of the particle
        /// </summary>
        /// <returns></returns>
        public Vec3d Velocity = new Vec3d();


        public int lightrgbs;
        protected float accum;
        protected int slowTick;


        /// <summary>
        /// Dt will never be above PhysicsTickTime
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="api"></param>
        /// <param name="physicsSim"></param>
        public virtual void TickFixedStep(float dt, ICoreClientAPI api, ParticlePhysics physicsSim)
        {
            accum += dt;

            float step = dt / physicsSim.PhysicsTickTime;
            PrevPosition.X += dx * step;
            PrevPosition.Y += dy * step;
            PrevPosition.Z += dz * step;

            if (accum > 1)
            {
                accum = 1;
            }

            if (accum >= physicsSim.PhysicsTickTime)
            {
                dt = physicsSim.PhysicsTickTime;
                accum = 0;

                PrevPosition.Set(Position);
                TickNow(dt, dt, api, physicsSim);

                SecondsAlive += dt - physicsSim.PhysicsTickTime;

                dx = Position.X - PrevPosition.X;
                dy = Position.Y - PrevPosition.Y;
                dz = Position.Z - PrevPosition.Z;
            }
            else
            {
                SecondsAlive += dt;
            }
        }


        public abstract void TickNow(float dt, float physicsdt, ICoreClientAPI api, ParticlePhysics physicsSim);

        public abstract void UpdateBuffers(MeshData buffer, Vec3d cameraPos, ref int posPosition, ref int rgbaPosition, ref int flagPosition);

    }
}