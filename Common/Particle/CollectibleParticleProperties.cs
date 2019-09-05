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
    /// Abstract class used for BlockVoxelParticles and ItemVoxelParticles
    /// </summary>
    public abstract class CollectibleParticleProperties : IParticlePropertiesProvider
    {
        public Random rand = new Random();

        public virtual bool DieInLiquid() { return false; }
        public virtual bool SwimOnLiquid() { return false; }
        public virtual bool DieInAir() { return false; }
        public abstract float GetQuantity();
        public abstract Vec3d GetPos();

        public abstract Vec3f GetVelocity(Vec3d pos);
        public abstract int GetRgbaColor(ICoreClientAPI capi);
        public abstract byte GetGlowLevel();
        public abstract EnumParticleModel ParticleModel();

        public ICoreAPI api;

        public virtual bool SelfPropelled()
        {
            return false;
        }

        public virtual bool TerrainCollision() { return true; }

        public virtual float GetSize()
        {
            return 1f;
        }

        public virtual float GetGravityEffect()
        {
            return 1f;
        }

        public virtual float GetLifeLength()
        {
            return 1.5f;
        }

        public virtual bool UseLighting()
        {
            return true;
        }


        

        public Vec3d RandomBlockPos(IBlockAccessor blockAccess, BlockPos pos, Block block, BlockFacing facing = null)
        {
            Cuboidf box = block.GetParticleBreakBox(blockAccess, pos, facing);

            if (facing == null)
            {
                return new Vec3d(
                    pos.X + box.X1 + 1 / 32f + rand.NextDouble() * (box.X2 - box.X1 - 1 / 16f),
                    pos.Y + box.Y1 + 1 / 32f + rand.NextDouble() * (box.Y2 - box.Y1 - 1 / 16f),
                    pos.Z + box.Z1 + 1 / 32f + rand.NextDouble() * (box.Z2 - box.Z1 - 1 / 16f)
                );
            }
            else
            {
                bool haveBox = box != null;
                Vec3i facev = facing.Normali;

                Vec3d outpos = new Vec3d(
                    pos.X + 0.5f + facev.X / 1.9f + (haveBox && facing.Axis == EnumAxis.X ? (facev.X > 0 ? box.X2 - 1 : box.X1) : 0),
                    pos.Y + 0.5f + facev.Y / 1.9f + (haveBox && facing.Axis == EnumAxis.Y ? (facev.Y > 0 ? box.Y2 - 1 : box.Y1) : 0),
                    pos.Z + 0.5f + facev.Z / 1.9f + (haveBox && facing.Axis == EnumAxis.Z ? (facev.Z > 0 ? box.Z2 - 1 : box.Z1) : 0)
                );

                outpos.Add(
                    (rand.NextDouble() - 0.5) * (1 - Math.Abs(facev.X)),
                    (rand.NextDouble() - 0.5) * (1 - Math.Abs(facev.Y)) - (facing == BlockFacing.DOWN ? 0.1f : 0f),
                    (rand.NextDouble() - 0.5) * (1 - Math.Abs(facev.Z))
                );

                return outpos;
            }
        }

        public virtual EvolvingNatFloat GetOpacityEvolve()
        {
            return null;
        }

        public virtual EvolvingNatFloat GetRedEvolve() { return null; }
        public virtual EvolvingNatFloat GetGreenEvolve() { return null; }
        public virtual EvolvingNatFloat GetBlueEvolve() { return null; }


        public virtual EvolvingNatFloat GetSizeEvolve()
        {
            return null;
        }

        public virtual Block ColorByBlock()
        {
            return null;
        }

        public virtual void ToBytes(BinaryWriter writer)
        {

        }

        public virtual void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {

        }

        public void BeginParticle() { }

        public virtual EvolvingNatFloat[] GetVelocityEvolve()
        {
            return null;
        }

        public virtual IParticlePropertiesProvider[] GetSecondaryParticles()
        {
            return null;
        }

        public IParticlePropertiesProvider[] GetDeathParticles() { return null; }

        public virtual float GetSecondarySpawnInterval()
        {
            return 0.0f;
        }

        public virtual void PrepareForSecondarySpawn(IParticleInstance particleInstance)
        {
        }

        public virtual void Init(ICoreAPI api)
        {
            this.api = api;
        }
    }
}
