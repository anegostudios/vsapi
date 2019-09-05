using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// A base class for all particle providers.
    /// </summary>
    public abstract class ParticlesProviderBase : IParticlePropertiesProvider
    {
        /// <summary>
        /// Determines whether or not the particle dies in liquid
        /// </summary>
        /// <returns></returns>
        public virtual bool DieInLiquid()
        {
            return false;
        }

        public virtual bool SwimOnLiquid()
        {
            return false;
        }

        /// <summary>
        /// Whether or not the particle dies in air.
        /// </summary>
        /// <returns></returns>
        public virtual bool DieInAir()
        {
            return false;
        }

        /// <summary>
        /// Gets the quantity of particles.
        /// </summary>
        /// <returns></returns>
        public virtual float GetQuantity()
        {
            return 1;
        }

        /// <summary>
        /// Gets the position of particles.
        /// </summary>
        /// <returns></returns>
        public virtual Vec3d GetPos()
        {
            return Vec3d.Zero;
        }

        /// <summary>
        /// Gets the velocity of the particles.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public virtual Vec3f GetVelocity(Vec3d pos)
        {
            return Vec3f.Zero;
        }

        /// <summary>
        /// Gets the color of the particle.
        /// </summary>
        /// <param name="capi"></param>
        /// <returns></returns>
        public virtual int GetRgbaColor(ICoreClientAPI capi)
        {
            return ColorUtil.WhiteArgb;
        }
        
        /// <summary>
        /// Gets the evolving opacity value of the particle.
        /// </summary>
        /// <returns>An evolving value based on opacity.</returns>
        public virtual EvolvingNatFloat GetOpacityEvolve()
        {
            return null;
        }

        /// <summary>
        /// Gets the evolving red value of the particle.
        /// </summary>
        /// <returns></returns>
        public virtual EvolvingNatFloat GetRedEvolve()
        {
            return null;
        }

        /// <summary>
        /// Gets the evolving green value of the particle.
        /// </summary>
        /// <returns></returns>
        public virtual EvolvingNatFloat GetGreenEvolve()
        {
            return null;
        }

        /// <summary>
        /// Gets the evolving blue value of the particle.
        /// </summary>
        /// <returns></returns>
        public virtual EvolvingNatFloat GetBlueEvolve()
        {
            return null;
        }

        /// <summary>
        /// Gets the model type of the particle.
        /// </summary>
        /// <returns></returns>
        public virtual EnumParticleModel ParticleModel()
        {
            return EnumParticleModel.Quad;
        }

        /// <summary>
        /// gets the size of the particle.
        /// </summary>
        /// <returns></returns>
        public virtual float GetSize()
        {
            return 1f;
        }

        /// <summary>
        /// gets the dynamic size of the particle.
        /// </summary>
        /// <returns></returns>
        public virtual EvolvingNatFloat GetSizeEvolve()
        {
            return null;
        }

        /// <summary>
        /// Get the dynamic speeds of the particle.
        /// </summary>
        /// <returns></returns>
        public virtual EvolvingNatFloat[] GetVelocityEvolve()
        {
            return null;
        }

        /// <summary>
        /// Gets the gravity effect on the particle.
        /// </summary>
        /// <returns></returns>
        public virtual float GetGravityEffect()
        {
            return 1;
        }

        /// <summary>
        /// gets the life length of the particle.
        /// </summary>
        /// <returns></returns>
        public virtual float GetLifeLength()
        {
            return 1;
        }

        /// <summary>
        /// gets the glow level of the particle.
        /// </summary>
        /// <returns></returns>
        public virtual byte GetGlowLevel()
        {
            return 0;
        }

        /// <summary>
        /// Whether or not the particle is self-propelled.
        /// </summary>
        /// <returns></returns>
        public virtual bool SelfPropelled()
        {
            return false;
        }

        /// <summary>
        /// Whether or not the particle collides with the terrain or not.
        /// </summary>
        /// <returns></returns>
        public bool TerrainCollision() { return true; }

        /// <summary>
        /// How the particle is written to the save. (if it is)
        /// </summary>
        /// <param name="writer"></param>
        public virtual void ToBytes(BinaryWriter writer)
        {
            
        }

        /// <summary>
        /// How the particle is read from the save.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="resolver"></param>
        public virtual void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
            
        }

        /// <summary>
        /// Gets the secondary particle spawn interval.
        /// </summary>
        /// <returns></returns>
        public virtual float GetSecondarySpawnInterval()
        {
            return 0;
        }

        /// <summary>
        /// Gets the secondary particle type for this particle.
        /// </summary>
        /// <returns></returns>
        public virtual IParticlePropertiesProvider[] GetSecondaryParticles()
        {
            return null;
        }

        /// <summary>
        /// Gets the death particle for this type of particle.
        /// </summary>
        /// <returns></returns>
        public IParticlePropertiesProvider[] GetDeathParticles() { return null; }

        public virtual void BeginParticle() { }
        public virtual void PrepareForSecondarySpawn(IParticleInstance particleInstance) { }
        public virtual void Init(ICoreAPI api) { }

    }
}
