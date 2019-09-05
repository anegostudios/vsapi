using System;
using System.IO;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// A configurable implementation of IParticlePropertiesProvider
    /// </summary>
    public class SimpleParticleProperties : IParticlePropertiesProvider
    {
        public static Random rand = new Random();

        public float minQuantity;
        public float addQuantity;

        public Vec3d minPos;
        public Vec3d addPos = new Vec3d();
        public Vec3f minVelocity = new Vec3f();
        public Vec3f addVelocity = new Vec3f();

        public float lifeLength;
        public float addLifeLength;
        public float gravityEffect;

        public float minSize = 1f;
        public float maxSize = 1f;

        public int color;
        public byte glowLevel;
        public EnumParticleModel model = EnumParticleModel.Cube;

        public bool ShouldDieInAir;
        public bool ShouldDieInLiquid;
        public bool ShouldSwimOnLiquid;
        public bool WithTerrainCollision = true;

        public EvolvingNatFloat OpacityEvolve;
        public EvolvingNatFloat RedEvolve;
        public EvolvingNatFloat GreenEvolve;
        public EvolvingNatFloat BlueEvolve;

        public EvolvingNatFloat SizeEvolve;

        public bool SelfPropelled;

        public Block ColorByBlock;
        public int TintIndex;

        public SimpleParticleProperties()
        {
        }

//        ICoreAPI api;

        public void Init(ICoreAPI api) {  }

        public SimpleParticleProperties(float minQuantity, float maxQuantity, int color, Vec3d minPos, Vec3d maxPos, Vec3f minVelocity, Vec3f maxVelocity, float lifeLength = 1f, float gravityEffect = 1f, float minSize = 1f, float maxSize = 1f, EnumParticleModel model = EnumParticleModel.Cube)
        {
            this.minQuantity = minQuantity;
            this.addQuantity = maxQuantity - minQuantity;
            this.color = color;
            this.minPos = minPos;
            this.addPos = maxPos - minPos;
            this.minVelocity = minVelocity;
            this.addVelocity = maxVelocity - minVelocity;
            this.lifeLength = lifeLength;
            this.gravityEffect = gravityEffect;
            this.minSize = minSize;
            this.maxSize = maxSize;
            this.model = model;
        }

        public bool DieInAir()
        {
            return ShouldDieInAir;
        }

        public bool DieInLiquid()
        {
            return ShouldDieInLiquid;
        }

        public bool SwimOnLiquid()
        {
            return ShouldSwimOnLiquid;
        }

        public float GetQuantity()
        {
            return minQuantity + (float)rand.NextDouble() * addQuantity;
        }

        public Vec3d GetPos()
        {
            return new Vec3d(
                minPos.X + addPos.X * rand.NextDouble(),
                minPos.Y + addPos.Y * rand.NextDouble(),
                minPos.Z + addPos.Z * rand.NextDouble()
            );
        }


        public Vec3f GetVelocity(Vec3d pos)
        {
            return new Vec3f(
                 minVelocity.X + addVelocity.X * (float)rand.NextDouble(),
                 minVelocity.Y + addVelocity.Y * (float)rand.NextDouble(),
                 minVelocity.Z + addVelocity.Z * (float)rand.NextDouble()
            );
        }


        public float GetSize()
        {
            return minSize + (float)rand.NextDouble() * (maxSize - minSize);
        }

        public int GetRgbaColor(ICoreClientAPI capi)
        {
            if (ColorByBlock != null) return ColorByBlock.GetRandomColor(capi, new ItemStack(ColorByBlock));
            if (TintIndex > 0)
            {
                return capi.ApplyColorTintOnRgba((int)TintIndex, color, (int)minPos.X, (int)minPos.Y, (int)minPos.Z);
            }
            return color;
        }

        public byte GetGlowLevel()
        {
            return glowLevel;
        }

        public float GetGravityEffect()
        {
            return gravityEffect;
        }

        public float GetLifeLength()
        {
            return lifeLength + addLifeLength * (float)rand.NextDouble();
        }

        public EnumParticleModel ParticleModel()
        {
            return model;
        }

        public bool UseLighting()
        {
            return true;
        }

        public EvolvingNatFloat GetOpacityEvolve()
        {
            return OpacityEvolve;
        }


        public virtual EvolvingNatFloat GetRedEvolve()
        {
            return RedEvolve;
        }

        public virtual EvolvingNatFloat GetGreenEvolve()
        {
            return GreenEvolve;
        }

        public virtual EvolvingNatFloat GetBlueEvolve()
        {
            return BlueEvolve;
        }

        public EvolvingNatFloat GetSizeEvolve()
        {
            return SizeEvolve;
        }

        public EvolvingNatFloat[] GetVelocityEvolve()
        {
            return null;
        }


        bool IParticlePropertiesProvider.SelfPropelled()
        {
            return SelfPropelled;
        }

        public bool TerrainCollision() { return WithTerrainCollision; }
        



        public void ToBytes(BinaryWriter writer)
        {
            writer.Write(minQuantity);
            writer.Write(addQuantity);
            minPos.ToBytes(writer);
            addPos.ToBytes(writer);
            minVelocity.Write(writer);
            addVelocity.Write(writer);
            writer.Write(lifeLength);
            writer.Write(gravityEffect);
            writer.Write(minSize);
            writer.Write(maxSize);
            writer.Write(color);
            writer.Write(glowLevel);
            writer.Write((int)model);
            writer.Write(ShouldDieInAir);
            writer.Write(ShouldDieInLiquid);

            writer.Write(OpacityEvolve == null);
            if (OpacityEvolve != null) OpacityEvolve.ToBytes(writer);

            writer.Write(RedEvolve == null);
            if (RedEvolve != null) RedEvolve.ToBytes(writer);

            writer.Write(GreenEvolve == null);
            if (GreenEvolve != null) GreenEvolve.ToBytes(writer);

            writer.Write(BlueEvolve == null);
            if (BlueEvolve != null) BlueEvolve.ToBytes(writer);

            writer.Write(SizeEvolve == null);
            if (SizeEvolve != null) SizeEvolve.ToBytes(writer);

            writer.Write(SelfPropelled);

            writer.Write(ColorByBlock == null);
            if (ColorByBlock != null) writer.Write(ColorByBlock.BlockId);

            writer.Write((ushort)TintIndex);
        }

        public void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
            minQuantity = reader.ReadSingle();
            addQuantity = reader.ReadSingle();
            minPos = Vec3d.CreateFromBytes(reader);
            addPos = Vec3d.CreateFromBytes(reader);
            minVelocity = Vec3f.CreateFromBytes(reader);
            addVelocity = Vec3f.CreateFromBytes(reader);
            lifeLength = reader.ReadSingle();
            gravityEffect = reader.ReadSingle();
            minSize = reader.ReadSingle();
            maxSize = reader.ReadSingle();
            color = reader.ReadInt32();
            glowLevel = reader.ReadByte();
            model = (EnumParticleModel)reader.ReadInt32();
            ShouldDieInAir = reader.ReadBoolean();
            ShouldDieInLiquid = reader.ReadBoolean();

            if (!reader.ReadBoolean())
            {
                OpacityEvolve = EvolvingNatFloat.CreateFromBytes(reader);
            }

            if (!reader.ReadBoolean())
            {
                RedEvolve = EvolvingNatFloat.CreateFromBytes(reader);
            }

            if (!reader.ReadBoolean())
            {
                GreenEvolve = EvolvingNatFloat.CreateFromBytes(reader);
            }

            if (!reader.ReadBoolean())
            {
                BlueEvolve = EvolvingNatFloat.CreateFromBytes(reader);
            }


            if (!reader.ReadBoolean())
            {
                SizeEvolve = EvolvingNatFloat.CreateFromBytes(reader);
            }
            
            
            SelfPropelled = reader.ReadBoolean();

            if (!reader.ReadBoolean())
            {
                ColorByBlock = resolver.Blocks[reader.ReadInt16()];
            }

            TintIndex = reader.ReadInt16();
        }

        public void BeginParticle() { }

        public IParticlePropertiesProvider[] GetSecondaryParticles() { return null; }
        public IParticlePropertiesProvider[] GetDeathParticles() { return null; }

        public float GetSecondarySpawnInterval()
        {
            return 0.0f;
        }

        public void PrepareForSecondarySpawn(IParticleInstance particleInstance)
        {
        }
    }
}
