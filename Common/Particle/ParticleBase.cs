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

        public Vec3f Velocity = new Vec3f();
        public Vec3d Position = new Vec3d();

        public float prevPosDeltaX, prevPosDeltaY, prevPosDeltaZ;
        public float prevPosAdvance;


        public int lightrgbs;
        public float accum;
        protected byte slowTick;


        /// <summary>
        /// Advances the physics simulation of the particle if "physicsSim.PhysicsTickTime" seconds have passed by, otherwise
        /// it will only advance PrevPosition by the particles velocity.
        /// </summary>
        /// <param name="dt">Will never be above PhysicsTickTime</param>
        /// <param name="api"></param>
        /// <param name="physicsSim"></param>
        public virtual void TickFixedStep(float dt, ICoreClientAPI api, ParticlePhysics physicsSim)
        {
            accum += dt;
            prevPosAdvance += dt / physicsSim.PhysicsTickTime;

            if (accum >= physicsSim.PhysicsTickTime)
            {
                dt = physicsSim.PhysicsTickTime;
                accum = 0;
                prevPosAdvance = 0;

                double x = Position.X;
                double y = Position.Y;
                double z = Position.Z;

                TickNow(dt, dt, api, physicsSim);

                SecondsAlive += dt - physicsSim.PhysicsTickTime;

                prevPosDeltaX = (float)(Position.X - x);
                prevPosDeltaY = (float)(Position.Y - y);
                prevPosDeltaZ = (float)(Position.Z - z);
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