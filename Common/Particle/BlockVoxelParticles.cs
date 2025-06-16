using System.IO;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    public class BlockCubeParticles : CollectibleParticleProperties
    {
        /// <summary>
        /// The position of the particle
        /// </summary>
        public Vec3d particlePos;

        /// <summary>
        /// The amount of particles.
        /// </summary>
        public int quantity;

        /// <summary>
        /// The radius of the particle emission.
        /// </summary>
        public float radius;

        /// <summary>
        /// The scale of the particles.
        /// </summary>
        public float scale;

        Block block;
        BlockPos blockpos;
        public Vec3f velocity;

        public override bool DieInLiquid => block.LiquidCode != null;

        public BlockCubeParticles() { }

        public BlockCubeParticles(IWorldAccessor world, BlockPos blockpos, Vec3d particlePos, float radius, int quantity, float scale, Vec3f velocity = null)
        {
            this.particlePos = particlePos;
            this.blockpos = blockpos;
            this.quantity = quantity;
            this.radius = radius;
            this.scale = scale;
            this.velocity = velocity;
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

        public override Vec3d Pos => new Vec3d(particlePos.X + rand.NextDouble() * radius - radius / 2, particlePos.Y + 0.1f, particlePos.Z + rand.NextDouble() * radius - radius / 2);

        public override Vec3f GetVelocity(Vec3d pos)
        {
            if (velocity != null) return velocity;

            Vec3f distanceVector = new Vec3f(1.5f - 3 * (float)rand.NextDouble(), 1.5f - 3 * (float)rand.NextDouble(), 1.5f - 3 * (float)rand.NextDouble());

            if (block.IsLiquid())
            {
                distanceVector.Y += 4f;
            }

            return distanceVector;

        }


        public override float Size => scale;

        public override EnumParticleModel ParticleModel => EnumParticleModel.Cube;

        public override float Quantity => quantity;

        public override float LifeLength => 0.5f + (float)api.World.Rand.NextDouble() / 4f;

        public override int VertexFlags => block.VertexFlags.GlowLevel;

        public override IParticlePropertiesProvider[] SecondaryParticles => null;
        public override void ToBytes(BinaryWriter writer)
        {
            particlePos.ToBytes(writer);
            blockpos.ToBytes(writer);
            writer.Write(quantity);
            writer.Write(radius);
            writer.Write(scale);
            writer.Write(velocity != null);
            if (velocity != null) velocity.ToBytes(writer);
        }

        public override void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
            particlePos = Vec3d.CreateFromBytes(reader);
            blockpos = BlockPos.CreateFromBytes(reader);
            quantity = reader.ReadInt32();
            radius = reader.ReadSingle();
            scale = reader.ReadSingle();
            if (reader.ReadBoolean())
            {
                velocity = Vec3f.CreateFromBytes(reader);
            }
        }

    }



    public class StackCubeParticles : CollectibleParticleProperties
    {
        /// <summary>
        /// The position of the collision to create these particles.
        /// </summary>
        public Vec3d collisionPos;

        /// <summary>
        /// The contents that the particles are built off of.
        /// </summary>
        public ItemStack stack;

        /// <summary>
        /// The amount of particles to be released.
        /// </summary>
        public int quantity;

        /// <summary>
        /// The radius to release the particles.
        /// </summary>
        public float radius;

        /// <summary>
        /// The scale of the particles.
        /// </summary>
        public float scale;

        public Vec3f velocity = new Vec3f();

        public override bool DieInLiquid => false;
        public override bool SwimOnLiquid => stack.Collectible.MaterialDensity < 1000;

        public StackCubeParticles() { }

        public StackCubeParticles(Vec3d collisionPos, ItemStack stack, float radius, int quantity, float scale, Vec3f velocity = null)
        {
            this.collisionPos = collisionPos;
            this.stack = stack;
            this.quantity = quantity;
            this.radius = radius;
            this.scale = scale;
            this.velocity = velocity;
        }

        public override int GetRgbaColor(ICoreClientAPI capi)
        {
            return stack.Collectible.GetRandomColor(capi, stack);
        }

        public override Vec3d Pos => new Vec3d(collisionPos.X + rand.NextDouble() * radius - radius / 2, collisionPos.Y + 0.1f, collisionPos.Z + rand.NextDouble() * radius - radius / 2);

        public override Vec3f GetVelocity(Vec3d pos)
        {
            if (velocity != null)
            {
                return new Vec3f(1.5f - 3 * (float)rand.NextDouble() + velocity.X, 1.5f - 3 * (float)rand.NextDouble() + velocity.Y, 1.5f - 3 * (float)rand.NextDouble() + velocity.Z);
            } else
            {
                return new Vec3f(1.5f - 3 * (float)rand.NextDouble(), 1.5f - 3 * (float)rand.NextDouble(), 1.5f - 3 * (float)rand.NextDouble());
            }
        }


        public override float Size => scale;

        public override EnumParticleModel ParticleModel => EnumParticleModel.Cube;

        public override float Quantity => quantity;

        public override float LifeLength => 1f + (float)api.World.Rand.NextDouble() / 2f;

        public override int VertexFlags => stack.Class == EnumItemClass.Block ? stack.Block.VertexFlags.GlowLevel : (byte)0;

        public override IParticlePropertiesProvider[] SecondaryParticles => null;
        public override void ToBytes(BinaryWriter writer)
        {
            collisionPos.ToBytes(writer);
            stack.ToBytes(writer);
            writer.Write(quantity);
            writer.Write(radius);
            writer.Write(scale);
            writer.Write(velocity != null);
            if (velocity != null) velocity.ToBytes(writer);
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
            if (reader.ReadBoolean())
            {
                velocity = Vec3f.CreateFromBytes(reader);
            }
        }


    }
}

