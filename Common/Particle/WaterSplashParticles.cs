using System;
using System.IO;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    public class FloatingSedimentParticles : ParticlesProviderBase
    {
        Random rand = new Random();
        public Vec3d BasePos = new Vec3d();
        public Vec3d AddPos = new Vec3d();

        public Vec3f AddVelocity = new Vec3f();

        public Block SedimentBlock;
        public BlockPos SedimentPos = new BlockPos(Config.Dimensions.WillSetLater);
        public float quantity;

        public int waterColor;

        public override EnumParticleModel ParticleModel => EnumParticleModel.Quad;
        public override bool DieInLiquid => false;
        public override bool DieInAir => true;
        public override float GravityEffect => 0f; 
        public override float LifeLength => 9f; 
        public override bool SwimOnLiquid => false;
        public override Vec3d Pos => new Vec3d(BasePos.X + rand.NextDouble() * AddPos.X, BasePos.Y + rand.NextDouble() * AddPos.Y, BasePos.Z + AddPos.Z * rand.NextDouble());
        public override float Quantity => quantity;

        public override float Size => 0.25f;

        public override int VertexFlags => 1<<9; // Adds a 0.2 multiplier to the WBOIT weight so that it renders behind water
        public override EvolvingNatFloat SizeEvolve => new EvolvingNatFloat(EnumTransformFunction.LINEAR, 3f);
        public override EvolvingNatFloat OpacityEvolve => new EvolvingNatFloat(EnumTransformFunction.QUADRATIC, -50);

        public override Vec3f GetVelocity(Vec3d pos)
        {
            return new Vec3f(
                ((float)rand.NextDouble() - 0.5f) / 8f + AddVelocity.X, 
                ((float)rand.NextDouble() - 0.5f) / 8f + AddVelocity.Y, 
                ((float)rand.NextDouble() - 0.5f) / 8f + AddVelocity.Z
            );
        }

        public override int GetRgbaColor(ICoreClientAPI capi)
        {
            var color = SedimentBlock.GetRandomColor(capi, SedimentPos, BlockFacing.UP);

            int wCol = ((waterColor & 0xff) << 16) | (waterColor & (0xff << 8)) | ((waterColor >> 16) & 0xff) | (255 << 24); // (waterColor & (0xff<<24));

            color = ColorUtil.ColorOverlay(color, wCol, 0.1f);

            return color;
        }

    }

    public class WaterSplashParticles : ParticlesProviderBase
    {
        Random rand = new Random();
        public Vec3d BasePos = new Vec3d();
        public Vec3d AddPos = new Vec3d();

        public Vec3f AddVelocity = new Vec3f();
        public float QuantityMul;

        public override bool DieInLiquid => false;
        public override float GravityEffect => 1f;
        public override float LifeLength => 1.25f;
        public override bool SwimOnLiquid => true;
        public override Vec3d Pos => new Vec3d(BasePos.X + rand.NextDouble() * AddPos.X, BasePos.Y + rand.NextDouble() * AddPos.Y, BasePos.Z + AddPos.Z * rand.NextDouble());

        public override float Quantity => 30 * QuantityMul;

        public override int GetRgbaColor(ICoreClientAPI capi)
        {
            return ColorUtil.HsvToRgba(110, 40 + rand.Next(50), 200 + rand.Next(30), 50 + rand.Next(40));
        }

        public override float Size => 0.15f;

        public override EvolvingNatFloat SizeEvolve => new EvolvingNatFloat(EnumTransformFunction.LINEAR, 0.5f);

        public override EvolvingNatFloat OpacityEvolve => new EvolvingNatFloat(EnumTransformFunction.QUADRATIC, -16);

        public override Vec3f GetVelocity(Vec3d pos)
        {
            return new Vec3f(1f * (float)rand.NextDouble() - 0.5f + AddVelocity.X, 3 * (float)rand.NextDouble() + 2f + AddVelocity.Y, 1f * (float)rand.NextDouble() - 0.5f + AddVelocity.Z);
        }



        public override void ToBytes(BinaryWriter writer)
        {
            BasePos.ToBytes(writer);
            AddPos.ToBytes(writer);
            AddVelocity.ToBytes(writer);
            writer.Write(QuantityMul);
        }

        public override void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
            BasePos = Vec3d.CreateFromBytes(reader);
            AddPos = Vec3d.CreateFromBytes(reader);
            AddVelocity = Vec3f.CreateFromBytes(reader);
            QuantityMul = reader.ReadSingle();
        }


    }
}
