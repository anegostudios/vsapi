using System;
using System.IO;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class AirBubbleParticles : IParticlePropertiesProvider
    {
        Random rand = new Random();
        /// <summary>
        /// The base position of the bubble particle.
        /// </summary>
        public Vec3d BasePos = new Vec3d();

        /// <summary>
        /// The velocity to add to the bubble particle.
        /// </summary>
        public Vec3f AddVelocity = new Vec3f();

        /// <summary>
        /// Initializes the bubble particle.
        /// </summary>
        /// <param name="api">The core API.</param>
        public void Init(ICoreAPI api) { }

        /// <summary>
        /// Gets the secondary particles
        /// </summary>
        /// <returns>null, the bubble particles don't have secondary particles.</returns>
        public IParticlePropertiesProvider[] GetSecondaryParticles() { return null;  }

        /// <summary>
        /// Gets the death particles.
        /// </summary>
        /// <returns>There are no death particles for bubbles.</returns>
        public IParticlePropertiesProvider[] GetDeathParticles() { return null; }

        /// <summary>
        /// Whether or not the bubbles die in air.
        /// </summary>
        /// <returns>Yes they die in air.</returns>
        public bool DieInAir() { return true; }

        /// <summary>
        /// Whether or not the bubbles die in liquid.
        /// </summary>
        /// <returns>They don't.</returns>
        public bool DieInLiquid() { return false; }

        /// <summary>
        /// Gets the glow level of the bubbles.
        /// </summary>
        /// <returns>No glow.</returns>
        public byte GetGlowLevel() { return 0; }

        /// <summary>
        /// Gets the gravity applied to the particle.
        /// </summary>
        /// <returns>None.</returns>
        public float GetGravityEffect() { return 0f; }

        /// <summary>
        /// Whether or not the bubble collides with the terrain.
        /// </summary>
        /// <returns></returns>
        public bool TerrainCollision() { return true; }

        /// <summary>
        /// Gets the length of life for the particle.
        /// </summary>
        /// <returns>0.25f</returns>
        public float GetLifeLength() { return 0.25f; }

        public EvolvingNatFloat GetOpacityEvolve() { return null; }
        public EvolvingNatFloat GetRedEvolve() { return null; }
        public EvolvingNatFloat GetGreenEvolve() { return null; }
        public EvolvingNatFloat GetBlueEvolve() { return null; }

        public Vec3d GetPos()
        {
            return new Vec3d(BasePos.X + rand.NextDouble() * 0.25 - 0.125, BasePos.Y + 0.1 + rand.NextDouble() * 0.2, BasePos.Z + rand.NextDouble() * 0.25 - 0.125);
        }

        public float GetQuantity()
        {
            return 30;
        }

        public int GetRgbaColor(ICoreClientAPI capi)
        {
            return ColorUtil.HsvToRgba(
                (byte)GameMath.Clamp(110, 0, 255),
                (byte)GameMath.Clamp(20 + rand.Next(20), 0, 255),
                (byte)GameMath.Clamp(220 + rand.Next(30), 0, 255),
                (byte)GameMath.Clamp(120 + rand.Next(50), 0, 255)
            );

        }

        public float GetSize()
        {
            return (float)rand.NextDouble() * 0.2f + 0.2f;
        }

        public EvolvingNatFloat GetSizeEvolve()
        {
            return new EvolvingNatFloat(EnumTransformFunction.LINEAR, 0.25f);
        }

        public Vec3f GetVelocity(Vec3d pos)
        {
            return new Vec3f(
                1 * (float)rand.NextDouble() - 0.5f + AddVelocity.X, 
                0.1f * (float)rand.NextDouble() + 0.4f + AddVelocity.Y, 
                1 * (float)rand.NextDouble() - 0.5f + AddVelocity.Z
            );

        }


        public EvolvingNatFloat[] GetVelocityEvolve()
        {
            return null;
        }

        /// <summary>
        /// The base model of the particle.
        /// </summary>
        /// <returns>It's a cube.</returns>
        public EnumParticleModel ParticleModel()
        {
            return EnumParticleModel.Cube;
        }
        
        /// <summary>
        /// This particle is not self propelled.
        /// </summary>
        /// <returns></returns>
        public bool SelfPropelled()
        {
            return false;
        }

        /// <summary>
        /// This particle does not save to file.
        /// </summary>
        /// <param name="writer"></param>
        public void ToBytes(BinaryWriter writer)
        {
        }

        /// <summary>
        /// This particle does not load from file.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="resolver"></param>
        public void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
        }

        public void BeginParticle() { }

        public float GetSecondarySpawnInterval()
        {
            return 0.0f;
        }

        public void PrepareForSecondarySpawn(IParticleInstance particleInstance)
        {
        }
    }
}
