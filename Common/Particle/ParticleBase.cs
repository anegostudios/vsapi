using System;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

#nullable disable

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
        public float Bounciness;

        public float prevPosDeltaX, prevPosDeltaY, prevPosDeltaZ;
        public float prevPosAdvance;

        protected Vec3f motion = new Vec3f();

        public int lightrgbs;
        public float accum;
        protected byte tickCount = 0;

        protected float tdragnow;
        protected float rnddrag = 1;

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

        protected EnumCollideFlags flags;

        protected void updatePositionWithCollision(float pdt, ICoreClientAPI api, ParticlePhysics physicsSim, float height)
        {
            flags = physicsSim.UpdateMotion(Position, motion, height);

            // Adding the *0.999 seems to completely fix particles glitching through blocks galore
            Position.X += motion.X * 0.999;
            Position.Y += motion.Y * 0.999;
            Position.Z += motion.Z * 0.999;


            if (flags > 0)
            {
                tdragnow = (1 - GameMath.Clamp(0.2f / (33f / 1000f) * pdt, 0.001f, 1)) * rnddrag;
                if (api.World.Rand.NextDouble() < 0.001) rnddrag = Math.Max(0, (float)api.World.Rand.NextDouble() - 0.1f);

                if ((flags & EnumCollideFlags.CollideX) != 0)
                {
                    Velocity.X *= -Bounciness * 0.65f;
                    Velocity.Y *= Math.Min(1, tdragnow * 5f);
                    Velocity.Z *= tdragnow;
                }

                if ((flags & EnumCollideFlags.CollideY) != 0)
                {
                    Velocity.X *= tdragnow;
                    Velocity.Y *= -Bounciness * 0.65f;
                    Velocity.Z *= tdragnow;
                }

                if ((flags & EnumCollideFlags.CollideZ) != 0)
                {
                    Velocity.X *= tdragnow;
                    Velocity.Y *= Math.Min(1, tdragnow * 5f);
                    Velocity.Z *= -Bounciness * 0.65f;
                }
            }
            else
            {
                tdragnow = 1;
            }
        }



        public abstract void TickNow(float dt, float physicsdt, ICoreClientAPI api, ParticlePhysics physicsSim);

        public abstract void UpdateBuffers(MeshData buffer, Vec3d cameraPos, ref int posPosition, ref int rgbaPosition, ref int flagPosition);

    }
}
