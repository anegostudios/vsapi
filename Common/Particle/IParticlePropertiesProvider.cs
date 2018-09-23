using System.IO;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Represents a provider of particle properties to be used when generating a particle
    /// </summary>
    public interface IParticlePropertiesProvider
    {
        /// <summary>
        /// Called before the particle provider is used for particle creation
        /// </summary>
        /// <param name="api"></param>
        void Init(ICoreAPI api);

        /// <summary>
        /// Called just before a new particle is being created. You can use this to produce e.g. alternating kinds of particles
        /// </summary>
        void BeginParticle();

        /// <summary>
        /// Whether the particle should despawn when in contact with liquids
        /// </summary>
        /// <returns></returns>
        bool DieInLiquid();

        /// <summary>
        /// Whether the particle should despawn when in contact with air (e.g. for water bubbles)
        /// </summary>
        /// <returns></returns>
        bool DieInAir();

        /// <summary>
        /// How many particles should spawn? For every particle spawned, all of belows methods are called once. E.g. if quantity is 10, GetPos(), GetVelocity(),... is called 10 times. 
        /// </summary>
        /// <returns></returns>
        float GetQuantity();

        /// <summary>
        /// Position in the world where the particle should spawn
        /// </summary>
        /// <returns></returns>
        Vec3d GetPos();

        /// <summary>
        /// In what direction should the particle fly/fall
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        Vec3f GetVelocity(Vec3d pos);

        /// <summary>
        /// The particles Rgba Color
        /// </summary>
        /// <returns></returns>
        int GetRgbaColor(ICoreClientAPI capi);

        /// <summary>
        /// Return null or 1 if opacity should remain unchanged over time. lifetimeLeft is always a value between 0 and 1
        /// </summary>
        /// <param name="lifetimeLeft"></param>
        /// <returns></returns>
        EvolvingNatFloat GetOpacityEvolve();

        /// <summary>
        /// Return null or 1 if opacity should remain unchanged over time. lifetimeLeft is always a value between 0 and 1
        /// </summary>
        /// <param name="lifetimeLeft"></param>
        /// <returns></returns>
        EvolvingNatFloat GetRedEvolve();

        /// <summary>
        /// Return null or 1 if opacity should remain unchanged over time. lifetimeLeft is always a value between 0 and 1
        /// </summary>
        /// <param name="lifetimeLeft"></param>
        /// <returns></returns>
        EvolvingNatFloat GetGreenEvolve();

        /// <summary>
        /// Return null or 1 if opacity should remain unchanged over time. lifetimeLeft is always a value between 0 and 1
        /// </summary>
        /// <param name="lifetimeLeft"></param>
        /// <returns></returns>
        EvolvingNatFloat GetBlueEvolve();



        /// <summary>
        /// Cube or Quad?
        /// </summary>
        /// <returns></returns>
        EnumParticleModel ParticleModel();

        /// <summary>
        /// Size of the particle
        /// </summary>
        /// <returns></returns>
        float GetSize();

        /// <summary>
        /// Size change over time
        /// </summary>
        /// <returns></returns>
        EvolvingNatFloat GetSizeEvolve();

        /// <summary>
        /// Velocity change over time (acts as a multiplier to the velocity)
        /// </summary>
        /// <returns></returns>
        EvolvingNatFloat[] GetVelocityEvolve();

        /// <summary>
        /// How strongly the particle is affected by gravity (0 = no gravity applied)
        /// </summary>
        /// <returns></returns>
        float GetGravityEffect();

        /// <summary>
        /// How long the particle should live (default = 1)
        /// </summary>
        /// <returns></returns>
        float GetLifeLength();

        /// <summary>
        /// Value between 0 and 16 to determine glowiness of the particle
        /// </summary>
        /// <returns></returns>
        byte GetGlowLevel();

        /// <summary>
        /// If true, a particle will restore it's initial velocity once it's obstruction has been cleared
        /// e.g. Smokes will start flying upwards again if is currently stuck under a block and the block is removed
        /// </summary>
        /// <returns></returns>
        bool SelfPropelled();

        /// <summary>
        /// If true, the particle will collide with the terrain
        /// </summary>
        /// <returns></returns>
        bool TerrainCollision();

        /// <summary>
        /// For sending over the network
        /// </summary>
        /// <param name="writer"></param>
        void ToBytes(BinaryWriter writer);

        /// <summary>
        /// For reading from the network
        /// </summary>
        /// <param name="reader"></param>
        void FromBytes(BinaryReader reader, IWorldAccessor resolver);
        
        /// <summary>
        /// Determines the interval of time that must elapse during it's parent particle's lifetime before this one will spawn.
        /// This is only honored if this particle is defined as a secondary particle.
        /// </summary>
        /// <returns></returns>
        float GetSecondarySpawnInterval();

        /// <summary>
        /// The secondary particle properties. Secondary particles are particles that are emitted from an in-flight particle.
        /// </summary>
        /// <returns></returns>
        IParticlePropertiesProvider[] GetSecondaryParticles();

        /// <summary>
        /// The particle to spawn upon the particle death.
        /// </summary>
        /// <returns></returns>
        IParticlePropertiesProvider[] GetDeathParticles();

        /// <summary>
        /// Updates instance related state for secondary particles based on the given parent particle instance
        /// </summary>
        /// <param name="particleInstance">The parent IParticleInstance from which this secondary particle is being spawned</param>
        void PrepareForSecondarySpawn(IParticleInstance particleInstance);
        
    }
}
