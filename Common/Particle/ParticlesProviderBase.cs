using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public abstract class ParticlesProviderBase : IParticlePropertiesProvider
    {

        public virtual bool DieInLiquid()
        {
            return false;
        }

        public virtual bool DieInAir()
        {
            return false;
        }

        public virtual float GetQuantity()
        {
            return 1;
        }

        public virtual Vec3d GetPos()
        {
            return Vec3d.Zero;
        }

        public virtual Vec3f GetVelocity(Vec3d pos)
        {
            return Vec3f.Zero;
        }

        public virtual byte[] GetRgbaColor()
        {
            return ColorUtil.WhiteArgbBytes;
        }

        public virtual Block ColorByBlock()
        {
            return null;
        }

        public virtual EvolvingNatFloat GetOpacityEvolve()
        {
            return null;
        }

        public virtual EvolvingNatFloat GetRedEvolve()
        {
            return null;
        }

        public virtual EvolvingNatFloat GetGreenEvolve()
        {
            return null;
        }

        public virtual EvolvingNatFloat GetBlueEvolve()
        {
            return null;
        }

        public virtual EnumParticleModel ParticleModel()
        {
            return EnumParticleModel.Quad;
        }

        public virtual float GetSize()
        {
            return 1f;
        }

        public virtual EvolvingNatFloat GetSizeEvolve()
        {
            return null;
        }

        public virtual EvolvingNatFloat[] GetVelocityEvolve()
        {
            return null;
        }

        public virtual float GetGravityEffect()
        {
            return 1;
        }

        public virtual float GetLifeLength()
        {
            return 1;
        }

        public virtual byte GetGlowLevel()
        {
            return 0;
        }

        public virtual bool SelfPropelled()
        {
            return false;
        }

        public bool TerrainCollision() { return true; }

        public virtual void ToBytes(BinaryWriter writer)
        {
            
        }

        public virtual void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
            
        }

        public virtual float GetSecondarySpawnInterval()
        {
            return 0;
        }

        public virtual IParticlePropertiesProvider[] GetSecondaryParticles()
        {
            return null;
        }

        public virtual void BeginParticle() { }
        public virtual void PrepareForSecondarySpawn(IParticleInstance particleInstance) { }
        public virtual void Init(ICoreAPI api) { }

    }
}
