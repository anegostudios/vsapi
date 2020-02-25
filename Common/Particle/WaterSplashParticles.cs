using System;
using System.IO;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class WaterSplashParticles : ParticlesProviderBase
    {
        Random rand = new Random();
        public Vec3d BasePos = new Vec3d();
        public Vec3d AddPos = new Vec3d();

        public Vec3f AddVelocity = new Vec3f();
        public float QuantityMul;

        public override bool DieInLiquid => false; public override float GravityEffect => 1f; public override float LifeLength => 1.25f; public override bool SwimOnLiquid => true;
        public override Vec3d Pos => new Vec3d(BasePos.X + rand.NextDouble() * AddPos.X, BasePos.Y + rand.NextDouble() * AddPos.Y, BasePos.Z + AddPos.Z * rand.NextDouble());

        public override float Quantity => 30 * QuantityMul;

        public override int GetRgbaColor(ICoreClientAPI capi)
        {
            return ColorUtil.HsvToRgba(
                (byte)GameMath.Clamp(110, 0, 255),
                (byte)GameMath.Clamp(40 + rand.Next(50), 0, 255),
                (byte)GameMath.Clamp(200 + rand.Next(30), 0, 255),
                (byte)GameMath.Clamp(50 + rand.Next(40), 0, 255)
            );
        }

        public override float Size => 0.15f;

        public override EvolvingNatFloat SizeEvolve => new EvolvingNatFloat(EnumTransformFunction.LINEAR, 0.5f);

        public override EvolvingNatFloat OpacityEvolve => new EvolvingNatFloat(EnumTransformFunction.QUADRATIC, -16);

        public override Vec3f GetVelocity(Vec3d pos)
        {
            return new Vec3f(1f * (float)rand.NextDouble() - 0.5f + AddVelocity.X, 3 * (float)rand.NextDouble() + 2f + AddVelocity.Y, 1f * (float)rand.NextDouble() - 0.5f + AddVelocity.Z);
        }
    }
}
