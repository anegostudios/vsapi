using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class ExplosionParticles
    {

        public static AdvancedParticleProperties ExplosionFireTrailCubicles = new AdvancedParticleProperties()
        {
            HsvaColor = new NatFloat[] { NatFloat.createUniform(30, 20), NatFloat.createUniform(255, 50), NatFloat.createUniform(255, 50), NatFloat.createUniform(255, 0) },
            Size = NatFloat.createUniform(0.5f, 0.2f),
            GravityEffect = NatFloat.createUniform(0.3f, 0),
            Velocity = new NatFloat[] { NatFloat.createUniform(0, 0.6f), NatFloat.createUniform(0.4f, 0), NatFloat.createUniform(0, 0.6f) },
            Quantity = NatFloat.createUniform(30, 10),
            GlowLevel = 64,
            SizeEvolve = EvolvingNatFloat.create(EnumTransformFunction.QUADRATIC, -0.5f),
            DieInLiquid = true,
            PosOffset = new NatFloat[] { NatFloat.createUniform(0,0.5f), NatFloat.createUniform(0, 0.5f), NatFloat.createUniform(0, 0.5f) },

            SecondaryParticles = new AdvancedParticleProperties[]
            {
                new AdvancedParticleProperties() {

                    HsvaColor = new NatFloat[] { NatFloat.createUniform(0, 0), NatFloat.createUniform(0, 0), NatFloat.createUniform(100, 30), NatFloat.createUniform(220, 50) },
                    OpacityEvolve = new EvolvingNatFloat(EnumTransformFunction.QUADRATIC, -16),
                    GravityEffect = NatFloat.createUniform(0f, 0),
                    Size = NatFloat.createUniform(0.25f, 0.05f),
                    SizeEvolve = EvolvingNatFloat.create(EnumTransformFunction.LINEAR, 0.5f),
                    Quantity = NatFloat.createUniform(1, 1),
                    DieInLiquid = true,
                    SecondarySpawnInterval = NatFloat.createUniform(0.15f, 0),
                    Velocity = new NatFloat[] { NatFloat.createUniform(0, 0.025f), NatFloat.createUniform(0.15f, 0.1f), NatFloat.createUniform(0, 0.025f) },
                    ParticleModel = EnumParticleModel.Quad,
                    GlowLevel = 64,
                    
                }
            }
        };
            
            
            /*= new AdvancedParticleProperties(
            10, 20,
            ColorUtil.ColorFromArgb(255, 0, 255, 255),
            new Vec3d(),
            new Vec3d(),
            new Vec3f(-1.5f, -0.5f, -1.5f),
            new Vec3f(3f, 3f, 3f),
            1f,
            0.3f,
            0.3f,
            0.7f,
            EnumParticleModel.Cube
        );*/


        public static SimpleParticleProperties ExplosionFireParticles = new SimpleParticleProperties(
            10, 20,
            ColorUtil.ToRgba(150, 0, 255, 255),
            new Vec3d(),
            new Vec3d(),
            new Vec3f(-1.5f, -1.5f, -1.5f),
            new Vec3f(3f, 3f, 3f),
            0.12f,
            0,
            0.5f,
            1.5f,
            EnumParticleModel.Quad);
        
        static ExplosionParticles()
        {
            ExplosionFireParticles.RedEvolve = EvolvingNatFloat.create(EnumTransformFunction.LINEAR, -255f);
            ExplosionFireParticles.OpacityEvolve = EvolvingNatFloat.create(EnumTransformFunction.QUADRATIC, -12.4f);
            ExplosionFireParticles.addPos.Set(1, 1, 1);
        }
    }

    public class ExplosionSmokeParticles : IParticlePropertiesProvider
    {
        Random rand = new Random();
        public Vec3d basePos = new Vec3d();

        // Transferred data
        List<sbyte> offsets = new List<sbyte>();
        int quantityParticles = 0;

        // Temporary data
        SimpleParticleProperties[] providers;
        int curParticle = -1;
        int color;

        public ExplosionSmokeParticles()
        {
            color = ColorUtil.ToRgba(50, 220, 220, 220);

            providers = new SimpleParticleProperties[]
            {
                new SimpleParticleProperties(
                    1, 1,
                    ColorUtil.WhiteArgb,
                    new Vec3d(),
                    new Vec3d(),
                    new Vec3f(-0.25f, 0.1f, -0.25f),
                    new Vec3f(0.25f, 0.1f, 0.25f),
                    1.5f,
                    -0.075f,
                    0.5f,
                    1.5f,
                    EnumParticleModel.Quad
                )
            };

            providers[0].addPos.Set(0.1, 0.1, 0.1);

            quantityParticles += 30;
        }

        public void Init(ICoreAPI api) { }

        public void AddBlock(BlockPos pos)
        {
            offsets.Add((sbyte)(pos.X + 0.5 - basePos.X));
            offsets.Add((sbyte)(pos.Y + 0.5 - basePos.Y));
            offsets.Add((sbyte)(pos.Z + 0.5 - basePos.Z));

            quantityParticles += 4;
        }

        public void BeginParticle()
        {
            curParticle++;
        }
        
        public bool DieInAir() { return false; }
        public bool DieInLiquid() { return true; }
        public byte GetGlowLevel() { return 0; }
        public float GetGravityEffect() { return -0.04f; }
        public bool TerrainCollision() { return true; }
        public float GetLifeLength() { return 5f + (float)SimpleParticleProperties.rand.NextDouble(); }

        public EvolvingNatFloat GetOpacityEvolve() { return EvolvingNatFloat.create(EnumTransformFunction.LINEAR, -125f); }
        public EvolvingNatFloat GetRedEvolve() { return null; }
        public EvolvingNatFloat GetGreenEvolve() { return null; }
        public EvolvingNatFloat GetBlueEvolve() { return null; }

        public Vec3d GetPos()
        {
            int index = curParticle * 3 / 3;

            if (index + 2 >= offsets.Count)
            {
                return new Vec3d(
                    basePos.X + rand.NextDouble(),
                    basePos.Y + rand.NextDouble(),
                    basePos.Z + rand.NextDouble()
                );
            }

            return new Vec3d(
                basePos.X + offsets[index + 0] + rand.NextDouble(), 
                basePos.Y + offsets[index + 1] + rand.NextDouble(),
                basePos.Z + offsets[index + 2] + rand.NextDouble()
            );
        }

        public float GetQuantity()
        {
            return quantityParticles;
        }

        public int GetRgbaColor(ICoreClientAPI capi)
        {
            color = color & 0x00ffffff;
            color |= 50 + SimpleParticleProperties.rand.Next(100);

            return color;
        }

        public float GetSize()
        {
            return providers[0].GetSize();
        }

        public EvolvingNatFloat GetSizeEvolve()
        {
            return new EvolvingNatFloat(EnumTransformFunction.LINEAR, 8);
        }

        public Vec3f GetVelocity(Vec3d pos)
        {
            int index = 3 * curParticle / 3;

            if (index + 2 >= offsets.Count)
            {
                return new Vec3f(
                    (float)rand.NextDouble() * 16f - 8f,
                    (float)rand.NextDouble() * 16f - 8f,
                    (float)rand.NextDouble() * 16f - 8f
                );
            }

            float x = offsets[index + 0];
            float y = offsets[index + 1];
            float z = offsets[index + 2];

            float length = GameMath.FastSqrt(x * x + y * y + z + z);

            return new Vec3f(
                8f * x / length,
                8f * y / length,
                8f * z / length
            );
        }

        public EvolvingNatFloat[] GetVelocityEvolve()
        {
            return new EvolvingNatFloat[] {
                EvolvingNatFloat.create(EnumTransformFunction.INVERSELINEAR, 60f),
                EvolvingNatFloat.create(EnumTransformFunction.INVERSELINEAR, 60f),
                EvolvingNatFloat.create(EnumTransformFunction.INVERSELINEAR, 60f)
            };
        }


        public EnumParticleModel ParticleModel()
        {
            return EnumParticleModel.Quad;
        }

        public bool SelfPropelled()
        {
            return false;
        }


        public void ToBytes(BinaryWriter writer)
        {
            writer.Write(quantityParticles);
            writer.Write(offsets.Count);

            byte[] tmp = new byte[offsets.Count];
            for (int i = 0; i < offsets.Count; i++) tmp[i] = (byte)offsets[i];
            writer.Write(tmp);

            writer.Write(basePos.X);
            writer.Write(basePos.Y);
            writer.Write(basePos.Z);
        }


        public void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
            quantityParticles = reader.ReadInt32();
            int cnt = reader.ReadInt32();

            offsets.Clear();
            byte[] tmp = reader.ReadBytes(cnt);
            for (int i = 0; i < tmp.Length; i++)
            {
                offsets.Add((sbyte)tmp[i]);
            }

            basePos = new Vec3d(reader.ReadDouble(), reader.ReadDouble(), reader.ReadDouble());
        }

        public void AddBlocks(Dictionary<BlockPos, Block> explodedBlocks)
        {
            foreach (BlockPos pos in explodedBlocks.Keys)
            {
                AddBlock(pos);
            }
        }

        public float GetSecondarySpawnInterval()
        {
            return 0;
        }

        public IParticlePropertiesProvider[] GetSecondaryParticles()
        {
            return null;
        }
        public IParticlePropertiesProvider[] GetDeathParticles() { return null; }

        public void PrepareForSecondarySpawn(IParticleInstance particleInstance)
        {
        }
    }
}
