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
            if (facing == null)
            {
                Cuboidf[] selectionBoxes = block.GetSelectionBoxes(blockAccess, pos);
                Cuboidf box = (selectionBoxes != null && selectionBoxes.Length > 0) ? selectionBoxes[0] : Block.DefaultCollisionBox;

                return new Vec3d(
                    pos.X + box.X1 + 1 / 32f + rand.NextDouble() * (box.X2 - box.X1 - 1 / 16f),
                    pos.Y + box.Y1 + 1 / 32f + rand.NextDouble() * (box.Y2 - box.Y1 - 1 / 16f),
                    pos.Z + box.Z1 + 1 / 32f + rand.NextDouble() * (box.Z2 - box.Z1 - 1 / 16f)
                );
            }
            else
            {
                Vec3i face = facing.Normali;

                Cuboidf[] boxes = block.GetCollisionBoxes(blockAccess, pos);
                if (boxes == null || boxes.Length == 0) boxes = block.GetSelectionBoxes(blockAccess, pos);
                

                bool haveCollisionBox = boxes != null && boxes.Length > 0;

                Vec3d basepos = new Vec3d(
                    pos.X + 0.5f + face.X / 1.9f + (haveCollisionBox && facing.Axis == EnumAxis.X ? (face.X > 0 ? boxes[0].X2 - 1 : boxes[0].X1) : 0),
                    pos.Y + 0.5f + face.Y / 1.9f + (haveCollisionBox && facing.Axis == EnumAxis.Y ? (face.Y > 0 ? boxes[0].Y2 - 1 : boxes[0].Y1) : 0),
                    pos.Z + 0.5f + face.Z / 1.9f + (haveCollisionBox && facing.Axis == EnumAxis.Z ? (face.Z > 0 ? boxes[0].Z2 - 1 : boxes[0].Z1) : 0)
                );

                Vec3d posVariance = new Vec3d(
                    1f * (1 - Math.Abs(face.X)),
                    1f * (1 - Math.Abs(face.Y)),
                    1f * (1 - Math.Abs(face.Z))
                );

                return new Vec3d(
                    basepos.X + (rand.NextDouble() - 0.5) * posVariance.X,
                    basepos.Y + (rand.NextDouble() - 0.5) * posVariance.Y - (facing == BlockFacing.DOWN ? 0.1f : 0f),
                    basepos.Z + (rand.NextDouble() - 0.5) * posVariance.Z
                );
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
