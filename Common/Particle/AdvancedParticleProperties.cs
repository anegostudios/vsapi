using Newtonsoft.Json;
using System.IO;
using Vintagestory.API.MathTools;
using Vintagestory.API.Client;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Used to add a set of particle properties to a collectible.
    /// </summary>
    [DocumentAsJson]
    [JsonObject(MemberSerialization.OptIn)]
    public class AdvancedParticleProperties : IParticlePropertiesProvider
    {
        public bool IgnoreUserConfig { get; set; }
        public bool Async => false;

        /// <summary>
        /// Allows each particle to randomly change its velocity over time.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "False")]
        public bool RandomVelocityChange { get; set; }

        /// <summary>
        /// If true, particle dies if it falls below the rain height at its given location
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "False")]
        public bool DieOnRainHeightmap { get; set; }

        /// <summary>
        /// More particles that spawn from this particle over time. See <see cref="SecondarySpawnInterval"/> to control rate.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "None")]
        public AdvancedParticleProperties[] SecondaryParticles { get; set; }

        /// <summary>
        /// More particles that spawn when this particle dies.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "None")]
        public AdvancedParticleProperties[] DeathParticles { get; set; }

        /// <summary>
        /// The inverval that the <see cref="SecondaryParticles"/> spawn.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "0")]
        public NatFloat SecondarySpawnInterval { get; set; } = NatFloat.createUniform(0, 0);

        /// <summary>
        /// The amount of velocity to be kept when this particle collides with something. Directional velocity is multipled by (-Bounciness * 0.65) on any collision.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "0")]
        public float Bounciness { get; set; }

        /// <summary>
        /// Whether or not the particle dies in air.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "False")]
        public bool DieInAir { get; set; } = false;

        /// <summary>
        /// Whether or not the particle dies in water.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "False")]
        public bool DieInLiquid { get; set; } = false;

        /// <summary>
        /// Whether or not the particle floats on liquids.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "False")]
        public bool SwimOnLiquid { get; set; } = false;

        /// <summary>
        /// The Hue/Saturation/Value/Alpha for the color of the particle.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "Random")]
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
        [DocumentAsJson("Optional", "False")]
        public bool ColorByBlock { get; set; } = false;

        /// <summary>
        /// A transforming opacity value.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "None")]
        public EvolvingNatFloat OpacityEvolve { get; set; } = EvolvingNatFloat.NoValueSet;

        /// <summary>
        /// A transforming Red value.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "None")]
        public EvolvingNatFloat RedEvolve { get; set; } = EvolvingNatFloat.NoValueSet;

        /// <summary>
        /// A transforming Green value.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "None")]
        public EvolvingNatFloat GreenEvolve { get; set; } = EvolvingNatFloat.NoValueSet;

        /// <summary>
        /// A transforming Blue value.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "None")]
        public EvolvingNatFloat BlueEvolve { get; set; } = EvolvingNatFloat.NoValueSet;

        /// <summary>
        /// The gravity effect on the particle.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "1")]
        public NatFloat GravityEffect { get; set; } = NatFloat.createUniform(1, 0);

        /// <summary>
        /// The life length, in seconds, of the particle.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "1")]
        public NatFloat LifeLength { get; set; } = NatFloat.createUniform(1, 0);

        /// <summary>
        /// Offset from the blocks hitboxes top middle position
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "0, 0, 0")]
        public NatFloat[] PosOffset = new NatFloat[]
        {
            NatFloat.createUniform(0, 0), NatFloat.createUniform(0, 0), NatFloat.createUniform(0, 0)
        };

        /// <summary>
        /// The quantity of the particles given.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "1")]
        public NatFloat Quantity { get; set; } = NatFloat.createUniform(1, 0);

        /// <summary>
        /// The size of the particles given.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "1")]
        public NatFloat Size { get; set; } = NatFloat.createUniform(1, 0);

        /// <summary>
        /// A transforming Size value.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "0")]
        public EvolvingNatFloat SizeEvolve { get; set; } = EvolvingNatFloat.createIdentical(0);

        /// <summary>
        /// The velocity of the particles.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "Random")]
        public NatFloat[] Velocity { get; set; } = new NatFloat[]
        {
            NatFloat.createUniform(0f, 0.5f), NatFloat.createUniform(0f, 0.5f), NatFloat.createUniform(0f, 0.5f)
        };

        /// <summary>
        /// A dynamic velocity value.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "None")]
        public EvolvingNatFloat[] VelocityEvolve { get; set; } = null;

        /// <summary>
        /// Sets the base model for the particle.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "Cube")]
        public EnumParticleModel ParticleModel { get; set; } = EnumParticleModel.Cube;

        /// <summary>
        /// The level of glow in the particle.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "0")]
        public int VertexFlags { get; set; } = 0;

        /// <summary>
        /// Whether or not the particle is self propelled.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "False")]
        public bool SelfPropelled { get; set; } = false;

        /// <summary>
        /// Whether or not the particle collides with the terrain.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "True")]
        public bool TerrainCollision { get; set; } = true;

        /// <summary>
        /// How much the particles are affected by wind.
        /// </summary>
        [JsonProperty]
        [DocumentAsJson("Optional", "0")]
        public float WindAffectednes { get; set; } = 0;


        /// <summary>
        /// The base position for the particles.
        /// </summary>
        public Vec3d basePos = new Vec3d();


        public Vec3f baseVelocity = new Vec3f();

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

        public int LightEmission => 0;
        bool IParticlePropertiesProvider.DieInAir => DieInAir;

        bool IParticlePropertiesProvider.DieInLiquid => DieInLiquid;

        bool IParticlePropertiesProvider.SwimOnLiquid => SwimOnLiquid;

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
        /// Gets the position of the particle in world.
        /// </summary>
        /// <returns></returns>
        Vec3d tmpPos = new Vec3d();
        public Vec3d Pos
        {
            get
            {
                tmpPos.Set(
                    basePos.X + PosOffset[0].nextFloat(),
                    basePos.Y + PosOffset[1].nextFloat(),
                    basePos.Z + PosOffset[2].nextFloat()
                );
                return tmpPos;
            }
        }


        /// <summary>
        /// gets the quantity released.
        /// </summary>
        /// <returns></returns>
        float IParticlePropertiesProvider.Quantity => Quantity.nextFloat();

        /// <summary>
        /// Gets the dynamic size of the particle.
        /// </summary>
        float IParticlePropertiesProvider.Size => Size.nextFloat();

        Vec3f tmpVelo = new Vec3f();
        /// <summary>
        /// Gets the velocity of the particle.
        /// </summary>
        public Vec3f GetVelocity(Vec3d pos)
        {
            tmpVelo.Set(baseVelocity.X + Velocity[0].nextFloat(), baseVelocity.Y + Velocity[1].nextFloat(), baseVelocity.Z + Velocity[2].nextFloat());
            return tmpVelo;
        }

        public Vec3f ParentVelocity { get; set; } = null;


        public float WindAffectednesAtPos { get; set; } = 0;

        public float ParentVelocityWeight { get; set; } = 0;

        EnumParticleModel IParticlePropertiesProvider.ParticleModel => ParticleModel;

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
            writer.Write(VertexFlags);


            if (OpacityEvolve != EvolvingNatFloat.NoValueSet)
            {
                writer.Write(false);
                OpacityEvolve.ToBytes(writer);
            } else writer.Write(true);

            if (RedEvolve != EvolvingNatFloat.NoValueSet)
            {
                writer.Write(false);
                RedEvolve.ToBytes(writer);
            }
            else writer.Write(true);

            if (GreenEvolve != EvolvingNatFloat.NoValueSet)
            {
                writer.Write(false);
                GreenEvolve.ToBytes(writer);
            }
            else writer.Write(true);

            if (BlueEvolve != EvolvingNatFloat.NoValueSet)
            {
                writer.Write(false);
                BlueEvolve.ToBytes(writer);
            }
            else writer.Write(true);


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
            if (SecondaryParticles == null)
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

            writer.Write(WindAffectednes);
            writer.Write(Bounciness);
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
            VertexFlags = reader.ReadInt32();

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

            SizeEvolve = EvolvingNatFloat.CreateFromBytes(reader);
            SelfPropelled = reader.ReadBoolean();
            TerrainCollision = reader.ReadBoolean();
            ColorByBlock = reader.ReadBoolean();

            if (reader.ReadBoolean())
            {
                VelocityEvolve[0] = EvolvingNatFloat.CreateFromBytes(reader);
                VelocityEvolve[1] = EvolvingNatFloat.CreateFromBytes(reader);
                VelocityEvolve[2] = EvolvingNatFloat.CreateFromBytes(reader);
            }
            SecondarySpawnInterval = NatFloat.createFromBytes(reader);
            int secondaryPropCount = reader.ReadInt32();
            if (secondaryPropCount > 0)
            {
                SecondaryParticles = new AdvancedParticleProperties[secondaryPropCount];
                for (int i = 0; i < secondaryPropCount; i++)
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

            WindAffectednes = reader.ReadSingle();
            Bounciness = reader.ReadSingle();
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


        bool IParticlePropertiesProvider.SelfPropelled => SelfPropelled;

        /// <summary>
        /// Begins the advanced particle.
        /// </summary>
        public void BeginParticle() {
            if (WindAffectednes > 0)
            {
                ParentVelocityWeight = WindAffectednesAtPos * WindAffectednes;
                ParentVelocity = GlobalConstants.CurrentWindSpeedClient;
            } else
            {
                // This makes it impossible to use ParentVelocity for other purposes. Why is this here?
                //ParentVelocityWeight = 0;
                //ParentVelocity = null;
            }
        }

        /// <summary>
        /// Gets the secondary spawn interval.
        /// </summary>
        /// <returns></returns>
        float IParticlePropertiesProvider.SecondarySpawnInterval => SecondarySpawnInterval.nextFloat();

        /// <summary>
        /// prepares the particle for secondary spawning.
        /// </summary>
        /// <param name="particleInstance"></param>
        public void PrepareForSecondarySpawn(ParticleBase particleInstance)
        {
            Vec3d particlePos = particleInstance.Position;
            basePos.X = particlePos.X;
            basePos.Y = particlePos.Y;
            basePos.Z = particlePos.Z;
        }

        bool IParticlePropertiesProvider.TerrainCollision => TerrainCollision;


        float IParticlePropertiesProvider.GravityEffect => GravityEffect.nextFloat();

        float IParticlePropertiesProvider.LifeLength => LifeLength.nextFloat();

        IParticlePropertiesProvider[] IParticlePropertiesProvider.SecondaryParticles => SecondaryParticles;

        IParticlePropertiesProvider[] IParticlePropertiesProvider.DeathParticles => DeathParticles;
    }
}
