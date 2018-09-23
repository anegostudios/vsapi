using System;
using System.IO;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class AirBubbleParticles : IParticlePropertiesProvider
    {
        Random rand = new Random();
        public Vec3d BasePos = new Vec3d();
        public Vec3f AddVelocity = new Vec3f();

        public void Init(ICoreAPI api) { }

        public IParticlePropertiesProvider[] GetSecondaryParticles() { return null;  }
        public IParticlePropertiesProvider[] GetDeathParticles() { return null; }
        public bool DieInAir() { return true; }
        public bool DieInLiquid() { return false; }
        public byte GetGlowLevel() { return 0; }
        public float GetGravityEffect() { return 0f; }
        public bool TerrainCollision() { return true; }

        public float GetLifeLength() { return 0.25f; }

        public EvolvingNatFloat GetOpacityEvolve() { return null; }
        public EvolvingNatFloat GetRedEvolve() { return null; }
        public EvolvingNatFloat GetGreenEvolve() { return null; }
        public EvolvingNatFloat GetBlueEvolve() { return null; }

        public Vec3d GetPos()
        {
            return new Vec3d(BasePos.X + rand.NextDouble() * 0.25 - 0.125, BasePos.Y + 0.1 + rand.NextDouble() * 0.2, BasePos.Z + rand.NextDouble() * 0.25 - 0.125);
        }

        public float GetQuantity()
        {
            return 30;
        }

        public int GetRgbaColor(ICoreClientAPI capi)
        {
            return ColorUtil.HsvToRgba(
                (byte)GameMath.Clamp(110, 0, 255),
                (byte)GameMath.Clamp(20 + rand.Next(20), 0, 255),
                (byte)GameMath.Clamp(220 + rand.Next(30), 0, 255),
                (byte)GameMath.Clamp(120 + rand.Next(50), 0, 255)
            );

        }

        public float GetSize()
        {
            return (float)rand.NextDouble() * 0.2f + 0.2f;
        }

        public EvolvingNatFloat GetSizeEvolve()
        {
            return new EvolvingNatFloat(EnumTransformFunction.LINEAR, 0.25f);
        }

        public Vec3f GetVelocity(Vec3d pos)
        {
            return new Vec3f(
                1 * (float)rand.NextDouble() - 0.5f + AddVelocity.X, 
                0.1f * (float)rand.NextDouble() + 0.4f + AddVelocity.Y, 
                1 * (float)rand.NextDouble() - 0.5f + AddVelocity.Z
            );

        }

        public EvolvingNatFloat[] GetVelocityEvolve()
        {
            return null;
        }


        public EnumParticleModel ParticleModel()
        {
            return EnumParticleModel.Cube;
        }

        public bool SelfPropelled()
        {
            return false;
        }


        public void ToBytes(BinaryWriter writer)
        {
        }

        public void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
        }

        public void BeginParticle() { }

        public float GetSecondarySpawnInterval()
        {
            return 0.0f;
        }

        public void PrepareForSecondarySpawn(IParticleInstance particleInstance)
        {
        }
    }
}
