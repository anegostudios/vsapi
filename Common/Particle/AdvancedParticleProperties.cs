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
        /// <summary>
        /// The Secondary particles for the JsonObject.
        /// </summary>
        [JsonProperty]
        public AdvancedParticleProperties[] SecondaryParticles;

        /// <summary>
        /// The death particles for the JsonObject.
        /// </summary>
        [JsonProperty]
        public AdvancedParticleProperties[] DeathParticles;

        /// <summary>
        /// The inverval that the secondary particles spawn.
        /// </summary>
        [JsonProperty]
        public NatFloat SecondarySpawnInterval = NatFloat.createUniform(0, 0);

        /// <summary>
        /// Whether or not the entity dies in air.
        /// </summary>
        [JsonProperty]
        public bool DieInAir = false;

        /// <summary>
        /// Whether or not the entity dies in water.
        /// </summary>
        [JsonProperty]
        public bool DieInLiquid = false;

        [JsonProperty]
        public bool SwimOnLiquid = false;

        /// <summary>
        /// The Hue/Saturation/Value/Alpha for the color of the particle.
        /// </summary>
        [JsonProperty]
        public NatFloat[] HsvaColor = new NatFloat[] {
            NatFloat.createUniform(128, 128),
            NatFloat.createUniform(128, 128),
            NatFloat.createUniform(128, 128),
            NatFloat.createUniform(255, 0)
        };

        /// <summary>
        /// Whether or not to color the particle by the block it's on.
        /// </summary>
        [JsonProperty]
        public bool ColorByBlock = false;

        /// <summary>
        /// a transforming opacity value.
        /// </summary>
        [JsonProperty]
        public EvolvingNatFloat OpacityEvolve = null;

        /// <summary>
        /// A transforming Red value.
        /// </summary>
        [JsonProperty]
        public EvolvingNatFloat RedEvolve = null;

        /// <summary>
        /// A transforming Green value.
        /// </summary>
        [JsonProperty]
        public EvolvingNatFloat GreenEvolve = null;

        /// <summary>
        /// A transforming Blue value.
        /// </summary>
        [JsonProperty]
        public EvolvingNatFloat BlueEvolve = null;

        /// <summary>
        /// The gravity effect on the particle.
        /// </summary>
        [JsonProperty]
        public NatFloat GravityEffect = NatFloat.createUniform(1, 0);

        /// <summary>
        /// The life length of the particle.
        /// </summary>
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

        /// <summary>
        /// The quantity of the particles given.
        /// </summary>
        [JsonProperty]
        public NatFloat Quantity = NatFloat.createUniform(1, 0);

        /// <summary>
        /// The size of the particles given.
        /// </summary>
        [JsonProperty]
        public NatFloat Size = NatFloat.createUniform(1, 0);

        /// <summary>
        /// a transforming Size value.
        /// </summary>
        [JsonProperty]
        public EvolvingNatFloat SizeEvolve = EvolvingNatFloat.createIdentical(1);

        /// <summary>
        /// The velocity of the particles.
        /// </summary>
        [JsonProperty]
        public NatFloat[] Velocity = new NatFloat[]
        {
            NatFloat.createUniform(0f, 0.5f), NatFloat.createUniform(0f, 0.5f), NatFloat.createUniform(0f, 0.5f)
        };

        /// <summary>
        /// A dynamic velocity value.
        /// </summary>
        [JsonProperty]
        public EvolvingNatFloat[] VelocityEvolve = null;

        /// <summary>
        /// Sets the base model for the particle.
        /// </summary>
        [JsonProperty]
        public EnumParticleModel ParticleModel = EnumParticleModel.Cube;

        /// <summary>
        /// The level of glow in the particle.
        /// </summary>
        [JsonProperty]
        public byte GlowLevel = 0;

        /// <summary>
        /// Whether or not the particle is self propelled.
        /// </summary>
        [JsonProperty]
        public bool SelfPropelled = false;

        /// <summary>
        /// Whether or not the particle collides with the terrain.
        /// </summary>
        [JsonProperty]
        public bool TerrainCollision = true;

        /// <summary>
        /// The base position for the particles.
        /// </summary>
        public Vec3d basePos = new Vec3d();

        /// <summary>
        /// The base block for the particle.
        /// </summary>
        public Block block;

        /// <summary>
        /// Initializes the particle.
        /// </summary>
        /// <param name="api">The core API.</param>
        public void Init(ICoreAPI api) { }

        /// <summary>
        /// When HsvaColor is null, this is used
        /// </summary>
        public int Color;

        bool IParticlePropertiesProvider.DieInAir()
        {
            return DieInAir;
        }

        bool IParticlePropertiesProvider.DieInLiquid()
        {
            return DieInLiquid;
        }

        bool IParticlePropertiesProvider.SwimOnLiquid()
        {
            return SwimOnLiquid;
        }

        /// <summary>
        /// Converts the color to RGBA.
        /// </summary>
        /// <param name="capi">The Core Client API.</param>
        /// <returns>The set RGBA color.</returns>
        public int GetRgbaColor(ICoreClientAPI capi)
        {
            if (HsvaColor == null) return Color;

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

        /// <summary>
        /// Gets the next gravity effect.
        /// </summary>
        /// <returns></returns>
        public float GetGravityEffect()
        {
            return GravityEffect.nextFloat();
        }

        /// <summary>
        /// Gets the life length of the particle.
        /// </summary>
        /// <returns></returns>
        public float GetLifeLength()
        {
            return LifeLength.nextFloat();
        }

        /// <summary>
        /// Gets the position of the particle in world.
        /// </summary>
        /// <returns></returns>
        public Vec3d GetPos()
        {
            return new Vec3d(
                basePos.X + PosOffset[0].nextFloat(),
                basePos.Y + PosOffset[1].nextFloat(),
                basePos.Z + PosOffset[2].nextFloat()
            );
        }

        /// <summary>
        /// gets the quantity released.
        /// </summary>
        /// <returns></returns>
        public float GetQuantity()
        {
            return Quantity.nextFloat();
        }

        /// <summary>
        /// Gets the dynamic size of the particle.
        /// </summary>
        public float GetSize()
        {
            return Size.nextFloat();
        }

        /// <summary>
        /// Gets the velocity of the particle.
        /// </summary>
        public Vec3f GetVelocity(Vec3d pos)
        {
            return new Vec3f(Velocity[0].nextFloat(), Velocity[1].nextFloat(), Velocity[2].nextFloat());
        }

        /// <summary>
        /// Gets the dynamic velocity of the particle.
        /// </summary>
        /// <returns></returns>
        public EvolvingNatFloat[] GetVelocityEvolve()
        {
            return VelocityEvolve;
        }

        EnumParticleModel IParticlePropertiesProvider.ParticleModel()
        {
            return ParticleModel;
        }

        /// <summary>
        /// Gets the glow level of the particle.
        /// </summary>
        /// <returns></returns>
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
            writer.Write(SwimOnLiquid);
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
            SwimOnLiquid = reader.ReadBoolean();
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

        public AdvancedParticleProperties Clone()
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

        /// <summary>
        /// Gets the dynamic opacity of the particle.
        /// </summary>
        /// <returns></returns>
        public EvolvingNatFloat GetOpacityEvolve()
        {
            return OpacityEvolve;
        }

        /// <summary>
        /// Gets the dynamic red of the particle.
        /// </summary>
        /// <returns></returns>
        public virtual EvolvingNatFloat GetRedEvolve()
        {
            return RedEvolve;
        }

        /// <summary>
        /// Gets the dynamic green of the particle.
        /// </summary>
        /// <returns></returns>
        public virtual EvolvingNatFloat GetGreenEvolve()
        {
            return GreenEvolve;
        }

        /// <summary>
        /// Gets the dynamic blue of the particle.
        /// </summary>
        /// <returns></returns>
        public virtual EvolvingNatFloat GetBlueEvolve()
        {
            return BlueEvolve;
        }

        /// <summary>
        /// Gets the dynamic size of the particle.
        /// </summary>
        /// <returns></returns>
        public EvolvingNatFloat GetSizeEvolve()
        {
            return SizeEvolve;
        }

        bool IParticlePropertiesProvider.SelfPropelled()
        {
            return SelfPropelled;
        }

        /// <summary>
        /// Begins the advanced particle.
        /// </summary>
        public void BeginParticle() { }

        /// <summary>
        /// Gets the secondary particles.
        /// </summary>
        /// <returns></returns>
        public IParticlePropertiesProvider[] GetSecondaryParticles()
        {
            return SecondaryParticles;
        }

        /// <summary>
        /// Gets the death particles.
        /// </summary>
        /// <returns></returns>
        public IParticlePropertiesProvider[] GetDeathParticles() { return DeathParticles; }

        /// <summary>
        /// Gets the secondary spawn interval.
        /// </summary>
        /// <returns></returns>
        public float GetSecondarySpawnInterval()
        {
            return SecondarySpawnInterval.nextFloat();
        }

        /// <summary>
        /// prepares the particle for secondary spawning.
        /// </summary>
        /// <param name="particleInstance"></param>
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
