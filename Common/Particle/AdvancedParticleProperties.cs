using Newtonsoft.Json;
using System.IO;
using Vintagestory.API.MathTools;
using System;
using Vintagestory.API.Client;

namespace Vintagestory.API.Common
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AdvancedParticleProperties : IParticlePropertiesProvider
    {
        [JsonProperty]
        public AdvancedParticleProperties[] SecondaryParticles;

        [JsonProperty]
        public AdvancedParticleProperties[] DeathParticles;

        [JsonProperty]
        public NatFloat SecondarySpawnInterval = NatFloat.createUniform(0, 0);

        [JsonProperty]
        public bool DieInAir = false;
        [JsonProperty]
        public bool DieInLiquid = false;
        [JsonProperty]
        public NatFloat[] HsvaColor = new NatFloat[] {
            NatFloat.createUniform(128, 128),
            NatFloat.createUniform(128, 128),
            NatFloat.createUniform(128, 128),
            NatFloat.createUniform(255, 0)
        };
        [JsonProperty]
        public bool ColorByBlock = false;

        [JsonProperty]
        public EvolvingNatFloat OpacityEvolve = null;
        [JsonProperty]
        public EvolvingNatFloat RedEvolve = null;
        [JsonProperty]
        public EvolvingNatFloat GreenEvolve = null;
        [JsonProperty]
        public EvolvingNatFloat BlueEvolve = null;


        [JsonProperty]
        public NatFloat GravityEffect = NatFloat.createUniform(1, 0);
        [JsonProperty]
        public NatFloat LifeLength = NatFloat.createUniform(1, 0);

        /// <summary>
        /// Offset from the blocks hitboxes top middle position
        /// </summary>
        [JsonProperty]
        public NatFloat[] PosOffset = new NatFloat[]
        {
            NatFloat.createUniform(0, 0), NatFloat.createUniform(0, 0), NatFloat.createUniform(0, 0)
        };

        [JsonProperty]
        public NatFloat Quantity = NatFloat.createUniform(1, 0);

        [JsonProperty]
        public NatFloat Size = NatFloat.createUniform(1, 0);

        [JsonProperty]
        public EvolvingNatFloat SizeEvolve = EvolvingNatFloat.createIdentical(1);

        [JsonProperty]
        public NatFloat[] Velocity = new NatFloat[]
        {
            NatFloat.createUniform(0f, 0.5f), NatFloat.createUniform(0f, 0.5f), NatFloat.createUniform(0f, 0.5f)
        };

        [JsonProperty]
        public EvolvingNatFloat[] VelocityEvolve = null;

        [JsonProperty]
        public EnumParticleModel ParticleModel = EnumParticleModel.Cube;

        [JsonProperty]
        public byte GlowLevel = 0;

        [JsonProperty]
        public bool SelfPropelled = false;

        [JsonProperty]
        public bool TerrainCollision = true;


        public Vec3d basePos = new Vec3d();
        public Block block;

        public void Init(ICoreAPI api) { }

        bool IParticlePropertiesProvider.DieInAir()
        {
            return DieInAir;
        }

        bool IParticlePropertiesProvider.DieInLiquid()
        {
            return DieInLiquid;
        }

        public int GetRgbaColor(ICoreClientAPI capi)
        {
            int color = ColorUtil.HsvToRgba(
                (byte)GameMath.Clamp(HsvaColor[0].nextFloat(), 0, 255),
                (byte)GameMath.Clamp(HsvaColor[1].nextFloat(), 0, 255),
                (byte)GameMath.Clamp(HsvaColor[2].nextFloat(), 0, 255),
                (byte)GameMath.Clamp(HsvaColor[3].nextFloat(), 0, 255)
            );

            int r = color & 0xff;
            int g = (color >> 8) & 0xff;
            int b = (color >> 16) & 0xff;
            int a = (color >> 24) & 0xff;

            return (r << 16) | (g << 8) | (b << 0) | (a << 24);
        }

        public float GetGravityEffect()
        {
            return GravityEffect.nextFloat();
        }

        public float GetLifeLength()
        {
            return LifeLength.nextFloat();
        }

        public Vec3d GetPos()
        {
            return new Vec3d(
                basePos.X + PosOffset[0].nextFloat(),
                basePos.Y + PosOffset[1].nextFloat(),
                basePos.Z + PosOffset[2].nextFloat()
            );
        }

        public float GetQuantity()
        {
            return Quantity.nextFloat();
        }

        public float GetSize()
        {
            return Size.nextFloat();
        }

        public Vec3f GetVelocity(Vec3d pos)
        {
            return new Vec3f(Velocity[0].nextFloat(), Velocity[1].nextFloat(), Velocity[2].nextFloat());
        }

        public EvolvingNatFloat[] GetVelocityEvolve()
        {
            return VelocityEvolve;
        }

        EnumParticleModel IParticlePropertiesProvider.ParticleModel()
        {
            return ParticleModel;
        }

        public byte GetGlowLevel()
        {
            return GlowLevel;
        }


        public void ToBytes(BinaryWriter writer)
        {
            writer.Write(basePos.X);
            writer.Write(basePos.Y);
            writer.Write(basePos.Z);

            writer.Write(DieInAir);
            writer.Write(DieInLiquid);
            for (int i = 0; i < 4; i++)
            {
                HsvaColor[i].ToBytes(writer);
            }
            GravityEffect.ToBytes(writer);
            LifeLength.ToBytes(writer);
            for (int i = 0; i < 3; i++)
            {
                PosOffset[i].ToBytes(writer);
            }
            Quantity.ToBytes(writer);
            Size.ToBytes(writer);
            for (int i = 0; i < 3; i++)
            {
                Velocity[i].ToBytes(writer);
            }
            writer.Write((byte)ParticleModel);
            writer.Write(GlowLevel);


            writer.Write(OpacityEvolve == null);
            if (OpacityEvolve != null) OpacityEvolve.ToBytes(writer);

            writer.Write(RedEvolve == null);
            if (RedEvolve != null) RedEvolve.ToBytes(writer);

            writer.Write(GreenEvolve == null);
            if (GreenEvolve != null) GreenEvolve.ToBytes(writer);

            writer.Write(BlueEvolve == null);
            if (BlueEvolve != null) BlueEvolve.ToBytes(writer);


            SizeEvolve.ToBytes(writer);
            writer.Write(SelfPropelled);
            writer.Write(TerrainCollision);
            writer.Write(ColorByBlock);

            writer.Write(VelocityEvolve != null);
            if (VelocityEvolve != null)
            {
                for (int i = 0; i < 3; i++) VelocityEvolve[i].ToBytes(writer);
            }

            SecondarySpawnInterval.ToBytes(writer);
            if(SecondaryParticles == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(SecondaryParticles.Length);
                for (int i = 0; i < SecondaryParticles.Length; i++)
                {
                    SecondaryParticles[i].ToBytes(writer);
                }
            }


            if (DeathParticles == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(DeathParticles.Length);
                for (int i = 0; i < DeathParticles.Length; i++)
                {
                    DeathParticles[i].ToBytes(writer);
                }
            }
        }

        public void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
            basePos = new Vec3d(reader.ReadDouble(), reader.ReadDouble(), reader.ReadDouble());

            DieInAir = reader.ReadBoolean();
            DieInLiquid = reader.ReadBoolean();
            HsvaColor = new NatFloat[] { NatFloat.createFromBytes(reader), NatFloat.createFromBytes(reader), NatFloat.createFromBytes(reader), NatFloat.createFromBytes(reader) };
            GravityEffect = NatFloat.createFromBytes(reader);
            LifeLength = NatFloat.createFromBytes(reader);
            PosOffset = new NatFloat[] { NatFloat.createFromBytes(reader), NatFloat.createFromBytes(reader), NatFloat.createFromBytes(reader) };
            Quantity = NatFloat.createFromBytes(reader);
            Size = NatFloat.createFromBytes(reader);
            Velocity = new NatFloat[] { NatFloat.createFromBytes(reader), NatFloat.createFromBytes(reader), NatFloat.createFromBytes(reader) };
            ParticleModel = (EnumParticleModel)reader.ReadByte();
            GlowLevel = reader.ReadByte();

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

            SizeEvolve.FromBytes(reader);
            SelfPropelled = reader.ReadBoolean();
            TerrainCollision = reader.ReadBoolean();
            ColorByBlock = reader.ReadBoolean();

            if (reader.ReadBoolean())
            {
                VelocityEvolve = new EvolvingNatFloat[]
                {
                    EvolvingNatFloat.createIdentical(1),
                    EvolvingNatFloat.createIdentical(1),
                    EvolvingNatFloat.createIdentical(1),
                };

                VelocityEvolve[0].FromBytes(reader);
                VelocityEvolve[1].FromBytes(reader);
                VelocityEvolve[2].FromBytes(reader);
            }
            SecondarySpawnInterval = NatFloat.createFromBytes(reader);
            int secondaryPropCount = reader.ReadInt32();
            if(secondaryPropCount > 0)
            {
                SecondaryParticles = new AdvancedParticleProperties[secondaryPropCount];
                for(int i = 0; i < secondaryPropCount; i++)
                {
                    SecondaryParticles[i] = AdvancedParticleProperties.createFromBytes(reader, resolver);
                }
            }

            int deathPropCount = reader.ReadInt32();
            if (deathPropCount > 0)
            {
                DeathParticles = new AdvancedParticleProperties[deathPropCount];
                for (int i = 0; i < deathPropCount; i++)
                {
                    DeathParticles[i] = AdvancedParticleProperties.createFromBytes(reader, resolver);
                }
            }
        }

        public static AdvancedParticleProperties createFromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
            AdvancedParticleProperties temp = new AdvancedParticleProperties();
            temp.FromBytes(reader, resolver);
            return temp;
        }

        internal AdvancedParticleProperties Clone()
        {
            AdvancedParticleProperties cloned = new AdvancedParticleProperties();

            // Because I'm lazy
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(ms);
                ToBytes(writer);
                ms.Position = 0; // Dunno if needed
                cloned.FromBytes(new BinaryReader(ms), null);
            }

            return cloned;
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

        bool IParticlePropertiesProvider.SelfPropelled()
        {
            return SelfPropelled;
        }

        public void BeginParticle() { }

        public IParticlePropertiesProvider[] GetSecondaryParticles()
        {
            return SecondaryParticles;
        }

        public IParticlePropertiesProvider[] GetDeathParticles() { return DeathParticles; }

        public float GetSecondarySpawnInterval()
        {
            return SecondarySpawnInterval.nextFloat();
        }

        public void PrepareForSecondarySpawn(IParticleInstance particleInstance)
        {
            Vec3d particlePos = particleInstance.GetPosition();
            basePos.X = particlePos.X;
            basePos.Y = particlePos.Y;
            basePos.Z = particlePos.Z;
        }

        bool IParticlePropertiesProvider.TerrainCollision()
        {
            return TerrainCollision;
        }
    }
}
