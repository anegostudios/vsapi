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
    public class ItemVoxelParticles : BlockVoxelParticles
    {
        public Item item;

        public ItemVoxelParticles() { }

        public ItemVoxelParticles(Vec3d collisionPos, Item item, float radius, int quantity, float scale) : base(collisionPos, null, radius, quantity, scale)
        {
            this.item = item;
        }


        public override byte[] GetRgbaColor()
        {
            return RandomItemPixel((ICoreClientAPI)api, item, BlockFacing.UP, collisionPos.AsBlockPos);
        }

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);
            writer.Write(item.Id);
        }

        public override void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
            base.FromBytes(reader, resolver);

            item = resolver.Items[reader.ReadInt32()];
        }

        public override byte GetGlowLevel()
        {
            return 0;
        }
    }

    public class BlockVoxelParticles : CollectibleParticleProperties
    {
        public Vec3d collisionPos;
        public Block block;
        public int quantity;
        public float radius;
        public float scale;

        public override bool DieInLiquid() { return true; }


        public BlockVoxelParticles() { }

        public BlockVoxelParticles(Vec3d collisionPos, Block block, float radius, int quantity, float scale)
        {
            this.collisionPos = collisionPos;
            this.block = block;
            this.quantity = quantity;
            this.radius = radius;
            this.scale = scale;
        }

        public override byte[] GetRgbaColor()
        {
            return RandomBlockPixel((ICoreClientAPI)api, block, BlockFacing.UP, collisionPos.AsBlockPos);
        }

        public override Vec3d GetPos()
        {
            return new Vec3d(collisionPos.X + rand.NextDouble() * radius - radius / 2, collisionPos.Y + 0.1f, collisionPos.Z + rand.NextDouble() * radius - radius / 2);
        }

        public override Vec3f GetVelocity(Vec3d pos)
        {
            Vec3f distanceVector = new Vec3f(1.5f - 3 * (float)rand.NextDouble(), 1.5f - 3 * (float)rand.NextDouble(), 1.5f - 3 * (float)rand.NextDouble());

            if (block != null && block.IsLiquid())
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
            collisionPos.ToBytes(writer);
            writer.Write(block == null ? 0 : block.Id);
            writer.Write(quantity);
            writer.Write(radius);
            writer.Write(scale);
        }

        public override void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
            collisionPos = Vec3d.CreateFromBytes(reader);
            block = resolver.Blocks[reader.ReadInt32()];
            quantity = reader.ReadInt32();
            radius = reader.ReadSingle();
            scale = reader.ReadSingle();
        }


    }
}
