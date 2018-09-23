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
    public class BlockCubeParticles : CollectibleParticleProperties
    {
        public Vec3d particlePos;
        public int quantity;
        public float radius;
        public float scale;

        Block block;
        BlockPos blockpos;

        public override bool DieInLiquid() { return true; }


        public BlockCubeParticles() { }

        public BlockCubeParticles(IWorldAccessor world, BlockPos blockpos, Vec3d particlePos, float radius, int quantity, float scale)
        {
            this.particlePos = particlePos;
            this.blockpos = blockpos;
            this.quantity = quantity;
            this.radius = radius;
            this.scale = scale;
            block = world.BlockAccessor.GetBlock(blockpos);
        }

        public override void Init(ICoreAPI api)
        {
            base.Init(api);

            if (block == null) block = api.World.BlockAccessor.GetBlock(blockpos);
        }

        public override int GetRgbaColor(ICoreClientAPI capi)
        {
            return block.GetRandomColor(capi, blockpos, BlockFacing.UP);
        }

        public override Vec3d GetPos()
        {
            return new Vec3d(particlePos.X + rand.NextDouble() * radius - radius / 2, particlePos.Y + 0.1f, particlePos.Z + rand.NextDouble() * radius - radius / 2);
        }

        public override Vec3f GetVelocity(Vec3d pos)
        {
            Vec3f distanceVector = new Vec3f(1.5f - 3 * (float)rand.NextDouble(), 1.5f - 3 * (float)rand.NextDouble(), 1.5f - 3 * (float)rand.NextDouble());

            if (block.IsLiquid())
            {
                distanceVector.Y += 5f;
            }

            return distanceVector;

        }


        public override float GetSize()
        {
            return scale;
        }

        public override EnumParticleModel ParticleModel()
        {
            return EnumParticleModel.Cube;
        }

        public override float GetQuantity()
        {
            return quantity;
        }

        public override float GetLifeLength()
        {
            return 0.5f + (float)api.World.Rand.NextDouble() / 4f;
        }

        public override byte GetGlowLevel()
        {
            return block.VertexFlags.GlowLevel;
        }

        public override IParticlePropertiesProvider[] GetSecondaryParticles() { return null; }

        public override void ToBytes(BinaryWriter writer)
        {
            particlePos.ToBytes(writer);
            blockpos.ToBytes(writer);
            writer.Write(quantity);
            writer.Write(radius);
            writer.Write(scale);
        }

        public override void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
            particlePos = Vec3d.CreateFromBytes(reader);
            blockpos = BlockPos.CreateFromBytes(reader);
            quantity = reader.ReadInt32();
            radius = reader.ReadSingle();
            scale = reader.ReadSingle();
        }

    }



    public class StackCubeParticles : CollectibleParticleProperties
    {
        public Vec3d collisionPos;
        public ItemStack stack;
        public int quantity;
        public float radius;
        public float scale;

        public override bool DieInLiquid() { return true; }


        public StackCubeParticles() { }

        public StackCubeParticles(Vec3d collisionPos, ItemStack stack, float radius, int quantity, float scale)
        {
            this.collisionPos = collisionPos;
            this.stack = stack;
            this.quantity = quantity;
            this.radius = radius;
            this.scale = scale;
        }

        public override int GetRgbaColor(ICoreClientAPI capi)
        {
            return stack.Collectible.GetRandomColor(capi, stack);
        }

        public override Vec3d GetPos()
        {
            return new Vec3d(collisionPos.X + rand.NextDouble() * radius - radius / 2, collisionPos.Y + 0.1f, collisionPos.Z + rand.NextDouble() * radius - radius / 2);
        }

        public override Vec3f GetVelocity(Vec3d pos)
        {
            Vec3f distanceVector = new Vec3f(1.5f - 3 * (float)rand.NextDouble(), 1.5f - 3 * (float)rand.NextDouble(), 1.5f - 3 * (float)rand.NextDouble());

            return distanceVector;

        }


        public override float GetSize()
        {
            return scale;
        }

        public override EnumParticleModel ParticleModel()
        {
            return EnumParticleModel.Cube;
        }

        public override float GetQuantity()
        {
            return quantity;
        }

        public override float GetLifeLength()
        {
            return 0.5f + (float)api.World.Rand.NextDouble() / 4f;
        }

        public override byte GetGlowLevel()
        {
            return stack.Class == EnumItemClass.Block ? stack.Block.VertexFlags.GlowLevel : (byte)0;
        }

        public override IParticlePropertiesProvider[] GetSecondaryParticles() { return null; }

        public override void ToBytes(BinaryWriter writer)
        {
            collisionPos.ToBytes(writer);
            stack.ToBytes(writer);
            writer.Write(quantity);
            writer.Write(radius);
            writer.Write(scale);
        }

        public override void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
            collisionPos = Vec3d.CreateFromBytes(reader);
            stack = new ItemStack();
            stack.FromBytes(reader);
            stack.ResolveBlockOrItem(resolver);
            quantity = reader.ReadInt32();
            radius = reader.ReadSingle();
            scale = reader.ReadSingle();
        }


    }
}

