using System;
using System.IO;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    public class AirBubbleParticles : IParticlePropertiesProvider
    {
        public bool Async => false;

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
        public IParticlePropertiesProvider[] SecondaryParticles => null;
        /// <summary>
        /// Gets the death particles.
        /// </summary>
        /// <returns>There are no death particles for bubbles.</returns>
        public IParticlePropertiesProvider[] DeathParticles => null;

        public float Bounciness { get; set; }

        /// <summary>
        /// Whether or not the bubbles die in air.
        /// </summary>
        /// <returns>Yes they die in air.</returns>
        public bool DieInAir => false;
        /// <summary>
        /// Whether or not the bubbles die in liquid.
        /// </summary>
        /// <returns>They don't.</returns>
        public bool DieInLiquid => false;
        /// <summary>
        /// Whether or not the bubbles die in liquid.
        /// </summary>
        /// <returns>They don't.</returns>
        public bool SwimOnLiquid { get; set; } = true;

        /// <summary>
        /// Gets the glow level of the bubbles.
        /// </summary>
        /// <returns>No glow.</returns>
        public int VertexFlags => 0;
        /// <summary>
        /// Gets the gravity applied to the particle.
        /// </summary>
        /// <returns>None.</returns>
        public float GravityEffect => 0.1f;
        /// <summary>
        /// Whether or not the bubble collides with the terrain.
        /// </summary>
        /// <returns></returns>
        public bool TerrainCollision => true;
        /// <summary>
        /// Gets the length of life for the particle.
        /// </summary>
        /// <returns>0.25f</returns>
        public float LifeLength { get; set; }=0.25f;
        public EvolvingNatFloat OpacityEvolve => null; public EvolvingNatFloat RedEvolve => null; public EvolvingNatFloat GreenEvolve => null; public EvolvingNatFloat BlueEvolve => null;
        public bool RandomVelocityChange { get; set; }
        public Vec3d Pos => new Vec3d(BasePos.X + rand.NextDouble() * Range - Range/2f, BasePos.Y + 0.1 + rand.NextDouble() * 0.2, BasePos.Z + rand.NextDouble() * Range - Range/2f);

        public float Range = 0.25f;

        public float Quantity => quantity;

        public float quantity = 30f;
        public float horVelocityMul = 1f;

        public int GetRgbaColor(ICoreClientAPI capi)
        {
            return ColorUtil.HsvToRgba(110, 20 + rand.Next(20), 220 + rand.Next(30), 120 + rand.Next(50));
        }

        public int LightEmission => 0;

        public float Size => (float)rand.NextDouble() * 0.2f + 0.2f;

        public EvolvingNatFloat SizeEvolve => new EvolvingNatFloat(EnumTransformFunction.LINEAR, 0.25f);

        public Vec3f GetVelocity(Vec3d pos)
        {
            return new Vec3f(
                horVelocityMul * ((float)rand.NextDouble() - 0.5f + AddVelocity.X), 
                0.1f * (float)rand.NextDouble() + 0.4f + AddVelocity.Y,
                horVelocityMul * ((float)rand.NextDouble() - 0.5f + AddVelocity.Z)
            );

        }


        public EvolvingNatFloat[] VelocityEvolve => null;

        /// <summary>
        /// The base model of the particle.
        /// </summary>
        /// <returns>It's a cube.</returns>
        public EnumParticleModel ParticleModel => EnumParticleModel.Cube;

        /// <summary>
        /// This particle is not self propelled.
        /// </summary>
        /// <returns></returns>
        public bool SelfPropelled => false;

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

        public float SecondarySpawnInterval => 0.0f;

        public bool DieOnRainHeightmap => false;

        public void PrepareForSecondarySpawn(ParticleBase particleInstance)
        {
        }

        public Vec3f ParentVelocity => null;
        
        float IParticlePropertiesProvider.ParentVelocityWeight => 0;
    }
}
