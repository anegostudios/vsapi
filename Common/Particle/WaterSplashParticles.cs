using System;
using System.IO;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class WaterSplashParticles : ParticlesProviderBase
    {
        Random rand = new Random();
        public Vec3d BasePos = new Vec3d();
        public Vec3f AddVelocity = new Vec3f();
        public float QuantityMul;

        public override bool DieInLiquid() { return true; }
        public override float GetGravityEffect() { return 1f; }
        public override float GetLifeLength() { return 0.25f; }

        public override Vec3d GetPos()
        {
            return new Vec3d(BasePos.X + rand.NextDouble() * 0.25 - 0.125, BasePos.Y + 0.1 + rand.NextDouble() * 0.2, BasePos.Z + rand.NextDouble() * 0.25 - 0.125);
        }

        public override float GetQuantity()
        {
            return 45 * QuantityMul;
        }

        public override byte[] GetRgbaColor()
        {
            return ColorUtil.HSVa2RGBaBytes(new byte[] {
                (byte)GameMath.Clamp(110, 0, 255),
                (byte)GameMath.Clamp(40 + rand.Next(50), 0, 255),
                (byte)GameMath.Clamp(200 + rand.Next(30), 0, 255),
                (byte)GameMath.Clamp(120 + rand.Next(50), 0, 255)
            });

        }

        public override float GetSize()
        {
            return 0.25f;
        }

        public override EvolvingNatFloat GetSizeEvolve()
        {
            return new EvolvingNatFloat(EnumTransformFunction.LINEAR, 0.5f);
        }

        public override EvolvingNatFloat GetOpacityEvolve()
        {
            return new EvolvingNatFloat(EnumTransformFunction.QUADRATIC, -16);
        }

        public override Vec3f GetVelocity(Vec3d pos)
        {
            return new Vec3f(2 * (float)rand.NextDouble() - 1f + AddVelocity.X, 3 * (float)rand.NextDouble() + 2f + AddVelocity.Y, 2 * (float)rand.NextDouble() - 1f + AddVelocity.Z);
        }
    }
}
