using System;
using System.IO;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    public class BlockDamage
    {
        public IPlayer ByPlayer;

        public int DecalId;

        public BlockPos Position;
        public BlockFacing Facing;
        public Block Block;

        public float RemainingResistance;

        //public long LastBreakMilliseconds;
        public long LastBreakEllapsedMs;

        public long BeginBreakEllapsedMs;

        public EnumTool? Tool;

        public int BreakingCounter;
    }

    public class BlockBreakingParticleProps : CollectibleParticleProperties
    {
        public BlockDamage blockdamage;

        public bool boyant;

        EvolvingNatFloat sizeEvolve = new EvolvingNatFloat(EnumTransformFunction.LINEAR, -0.5f);

        public override int GetRgbaColor(ICoreClientAPI capi)
        {
            return blockdamage.Block.GetRandomColor(capi, blockdamage.Position, blockdamage.Facing);
        }

        public override Vec3d Pos => RandomBlockPos(api.World.BlockAccessor, blockdamage.Position, blockdamage.Block, blockdamage.Facing);

        public override float Size => 0.5f + (float)rand.NextDouble() * 0.8f;

        public override EvolvingNatFloat SizeEvolve => sizeEvolve;

        public override bool SwimOnLiquid => boyant;

        public override Vec3f GetVelocity(Vec3d pos)
        {
            Vec3i face = blockdamage.Facing.Normali;

            return new Vec3f(
                (float)(face.X == 0 ? (rand.NextDouble() - 0.5f) : (0.25f + rand.NextDouble()) * face.X),
                (float)(face.Y == 0 ? (rand.NextDouble() - 0.25f) : (0.75f + rand.NextDouble()) * face.Y),
                (float)(face.Z == 0 ? (rand.NextDouble() - 0.5f) : (0.25f + rand.NextDouble()) * face.Z)
            ) * (1 + (float)rand.NextDouble() / 2);
        }

        public override EnumParticleModel ParticleModel => EnumParticleModel.Cube;

        public override float Quantity => 0.5f;

        public override int VertexFlags => blockdamage.Block.VertexFlags.GlowLevel;

        public override float LifeLength => base.LifeLength + 0.5f + (float)rand.NextDouble() / 4f;

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);

            writer.Write(blockdamage.Position.X);
            writer.Write(blockdamage.Position.InternalY);
            writer.Write(blockdamage.Position.Z);
            writer.Write(blockdamage.Facing.Index);
            writer.Write(blockdamage.Block.Id);
        }

        public override void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
            base.FromBytes(reader, resolver);

            blockdamage = new BlockDamage();
            blockdamage.Position = new BlockPos(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
            blockdamage.Facing = BlockFacing.ALLFACES[reader.ReadInt32()];
            blockdamage.Block = resolver.GetBlock(reader.ReadInt32());
        }
    }

    public class BlockBrokenParticleProps : BlockBreakingParticleProps
    {
        EvolvingNatFloat sizeEvolve = new EvolvingNatFloat(EnumTransformFunction.LINEAR, -0.5f);

        public override Vec3d Pos => RandomBlockPos(api.World.BlockAccessor, blockdamage.Position, blockdamage.Block, null);

        public override float Size => 0.5f + (float)rand.NextDouble() * 0.8f;

        public override bool SwimOnLiquid => boyant;

        public override EvolvingNatFloat SizeEvolve => sizeEvolve;

        public override Vec3f GetVelocity(Vec3d pos)
        {
            Vec3f velocity = new Vec3f(3 * (float)rand.NextDouble() - 1.5f, 4 * (float)rand.NextDouble(), 3 * (float)rand.NextDouble() - 1.5f);

            return velocity * (1 + (float)rand.NextDouble() / 2);
        }

        public override float Quantity => 16 + rand.Next(32);

        public override int VertexFlags => blockdamage.Block.VertexFlags.GlowLevel;

        public override float LifeLength => base.LifeLength + (float)rand.NextDouble();


    }


    /// <summary>
    /// Abstract class used for BlockVoxelParticles and ItemVoxelParticles
    /// </summary>
    public abstract class CollectibleParticleProperties : IParticlePropertiesProvider
    {
        public Random rand = new Random();

        public bool Async => false;
        public float Bounciness { get; set; }
        public bool DieOnRainHeightmap { get; set; }
        public virtual bool RandomVelocityChange { get; set; }
        public virtual bool DieInLiquid => false; public virtual bool SwimOnLiquid => false; public virtual bool DieInAir => false; public abstract float Quantity { get; }

        public abstract Vec3d Pos { get; }

        public abstract Vec3f GetVelocity(Vec3d pos);
        public abstract int GetRgbaColor(ICoreClientAPI capi);
        public int LightEmission { get; set; }
        public abstract int VertexFlags { get; }
        public abstract EnumParticleModel ParticleModel { get; }

        public ICoreAPI api;

        public virtual bool SelfPropelled => false;

        public virtual bool TerrainCollision => true;
        public virtual float Size => 1f;

        public virtual float GravityEffect => 1f;

        public virtual float LifeLength => 1.5f;

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
                    pos.X + box.X1 + 1 / 32f + rand.NextDouble() * (box.XSize - 1 / 16f),
                    pos.InternalY + box.Y1 + 1 / 32f + rand.NextDouble() * (box.YSize - 1 / 16f),
                    pos.Z + box.Z1 + 1 / 32f + rand.NextDouble() * (box.ZSize - 1 / 16f)
                );
            }
            else
            {
                bool haveBox = box != null;
                Vec3i facev = facing.Normali;

                Vec3d outpos = new Vec3d(
                    pos.X + 0.5f + facev.X / 1.9f + (haveBox && facing.Axis == EnumAxis.X ? (facev.X > 0 ? box.X2 - 1 : box.X1) : 0),
                    pos.InternalY + 0.5f + facev.Y / 1.9f + (haveBox && facing.Axis == EnumAxis.Y ? (facev.Y > 0 ? box.Y2 - 1 : box.Y1) : 0),
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

        public virtual EvolvingNatFloat OpacityEvolve => null;

        public virtual EvolvingNatFloat RedEvolve => null; public virtual EvolvingNatFloat GreenEvolve => null; public virtual EvolvingNatFloat BlueEvolve => null;

        public virtual EvolvingNatFloat SizeEvolve => null;

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

        public virtual EvolvingNatFloat[] VelocityEvolve => null;

        public virtual IParticlePropertiesProvider[] SecondaryParticles => null;

        public IParticlePropertiesProvider[] DeathParticles => null;
        public virtual float SecondarySpawnInterval => 0.0f;

        public virtual void PrepareForSecondarySpawn(ParticleBase particleInstance)
        {
        }

        public virtual void Init(ICoreAPI api)
        {
            this.api = api;
        }


        public Vec3f ParentVelocity { get; set; } = null;

        public float ParentVelocityWeight { get; set; } = 0;
    }
}
