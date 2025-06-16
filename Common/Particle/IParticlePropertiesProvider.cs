using System.IO;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Represents a provider of particle properties to be used when generating a particle
    /// </summary>
    public interface IParticlePropertiesProvider
    {
        /// <summary>
        /// If true, will be be spawned in the async particle system, which does not affect main game performance, recommended for large quantities of particles, slightly less optimal for particles that spawn very often
        /// </summary>
        bool Async { get; }

        /// <summary>
        /// Called before the particle provider is used for particle creation
        /// </summary>
        /// <param name="api"></param>
        void Init(ICoreAPI api);

        /// <summary>
        /// Called just before a new particle is being created. You can use this to produce e.g. alternating kinds of particles
        /// </summary>
        void BeginParticle();

        float ParentVelocityWeight { get; }

        /// <summary>
        /// Whether the particle should despawn when in contact with liquids
        /// </summary>
        /// <returns></returns>
        bool DieInLiquid { get; }

        bool SwimOnLiquid { get; }

        float Bounciness { get; }

        /// <summary>
        /// Whether the particle should despawn when in contact with air (e.g. for water bubbles)
        /// </summary>
        /// <returns></returns>
        bool DieInAir { get; }

        /// <summary>
        /// If true, particle dies if it falls below the rain height at its given location
        /// </summary>
        bool DieOnRainHeightmap { get; }

        /// <summary>
        /// How many particles should spawn? For every particle spawned, all of belows methods are called once. E.g. if quantity is 10, GetPos(), GetVelocity(),... is called 10 times. 
        /// </summary>
        /// <returns></returns>
        float Quantity { get; }

        

        /// <summary>
        /// Position in the world where the particle should spawn
        /// </summary>
        /// <returns></returns>
        Vec3d Pos { get; }

        /// <summary>
        /// In what direction should the particle fly/fall
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        Vec3f GetVelocity(Vec3d pos);

        Vec3f ParentVelocity { get; }


        /// <summary>
        /// The particles Rgba Color
        /// </summary>
        /// <returns></returns>
        int GetRgbaColor(ICoreClientAPI capi);

        /// <summary>
        /// If this particle emits light, this is its RGBA. Does not actually brighten the scene around it, but prevents incorrect lighting of particles when everything else around it is dark
        /// </summary>
        int LightEmission { get; }


        /// <summary>
        /// Return null or 1 if opacity should remain unchanged over time. lifetimeLeft is always a value between 0 and 1
        /// </summary>
        /// <returns></returns>
        EvolvingNatFloat OpacityEvolve { get; }

        /// <summary>
        /// Return null or 1 if opacity should remain unchanged over time. lifetimeLeft is always a value between 0 and 1
        /// </summary>
        /// <returns></returns>
        EvolvingNatFloat RedEvolve { get; }

        /// <summary>
        /// Return null or 1 if opacity should remain unchanged over time. lifetimeLeft is always a value between 0 and 1
        /// </summary>
        /// <returns></returns>
        EvolvingNatFloat GreenEvolve { get; }

        /// <summary>
        /// Return null or 1 if opacity should remain unchanged over time. lifetimeLeft is always a value between 0 and 1
        /// </summary>
        /// <returns></returns>
        EvolvingNatFloat BlueEvolve { get; }



        /// <summary>
        /// Cube or Quad?
        /// </summary>
        /// <returns></returns>
        EnumParticleModel ParticleModel { get; }

        /// <summary>
        /// Size of the particle
        /// </summary>
        /// <returns></returns>
        float Size { get; }

        /// <summary>
        /// Size change over time
        /// </summary>
        /// <returns></returns>
        EvolvingNatFloat SizeEvolve { get; }

        /// <summary>
        /// Velocity change over time (acts as a multiplier to the velocity)
        /// </summary>
        /// <returns></returns>
        EvolvingNatFloat[] VelocityEvolve { get; }

        /// <summary>
        /// How strongly the particle is affected by gravity (0 = no gravity applied)
        /// </summary>
        /// <returns></returns>
        float GravityEffect { get; }

        /// <summary>
        /// How long the particle should live (default = 1)
        /// </summary>
        /// <returns></returns>
        float LifeLength { get; }

        /// <summary>
        /// See also <see cref="VertexFlags"/>
        /// </summary>
        /// <returns></returns>
        int VertexFlags { get; }

        /// <summary>
        /// If true, a particle will restore it's initial velocity once it's obstruction has been cleared
        /// e.g. Smokes will start flying upwards again if is currently stuck under a block and the block is removed
        /// </summary>
        /// <returns></returns>
        bool SelfPropelled { get; }

        /// <summary>
        /// If true, the particle will collide with the terrain
        /// </summary>
        /// <returns></returns>
        bool TerrainCollision { get; }

        /// <summary>
        /// For sending over the network
        /// </summary>
        /// <param name="writer"></param>
        void ToBytes(BinaryWriter writer);

        /// <summary>
        /// For reading from the network
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="resolver"></param>
        void FromBytes(BinaryReader reader, IWorldAccessor resolver);

        /// <summary>
        /// Determines the interval of time that must elapse during it's parent particle's lifetime before this one will spawn.
        /// This is only honored if this particle is defined as a secondary particle.
        /// </summary>
        /// <returns></returns>
        float SecondarySpawnInterval { get; }

        /// <summary>
        /// The secondary particle properties. Secondary particles are particles that are emitted from an in-flight particle.
        /// </summary>
        /// <returns></returns>
        IParticlePropertiesProvider[] SecondaryParticles { get; }

        /// <summary>
        /// The particle to spawn upon the particle death.
        /// </summary>
        /// <returns></returns>
        IParticlePropertiesProvider[] DeathParticles { get; }

        /// <summary>
        /// Updates instance related state for secondary particles based on the given parent particle instance
        /// </summary>
        /// <param name="particleInstance">The parent IParticleInstance from which this secondary particle is being spawned</param>
        void PrepareForSecondarySpawn(ParticleBase particleInstance);
        

        bool RandomVelocityChange { get; }
        
    }
}
