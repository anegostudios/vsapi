using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Special class to handle the vertex flagging in a very nicely compressed space.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class VertexFlags
    {
        public const int GlowLevelBitMask = 0xFF;
        public const int ZOffsetBitMask = 0x7 << 8;
        public const int FoliageWindWaveBitMask = 1 << 11;
        public const int WaterWaveBitMask = 1 << 12;

        public const int ReflectiveBitMask = 1 << 13;
        public const int WeakWaveBitMask = 1 << 14;

        public const int NormalBitMask = 0xFFF << 15;
        public const int LeavesWindWaveBitMask = 1 << 27;   // if both Foliage and Leaves Wind Wave are 1, it means Wind Sway

        public const int GroundDistanceBitsShift = 28;
        public const int GroundDistanceBitMask = 0x7 << GroundDistanceBitsShift; // 3 bits - means Ground Distance for Leaves wind wave, Special Wave for Foliage wind wave (e.g. for specific crops such as Pineapple)
        public const int Lod0BitMask = 1 << 31;

        public const int clearWaveBits = (~FoliageWindWaveBitMask) & (~LeavesWindWaveBitMask) & (~GroundDistanceBitMask);
        public const int clearWaveFlagsOnly = (~FoliageWindWaveBitMask) & (~LeavesWindWaveBitMask);
        public const int clearZOffset = ~0x700;

        public const int clearNormalBits = ~NormalBitMask;


        int all;

        [JsonProperty]
        public int All
        {
            get
            {
                return all;
            }
            set
            {
                glowLevel = (byte)(value & 0xFF);
                zOffset = (byte)((value >> 8) & 0x7);
                grassWindWave = ((value >> 11) & 1) != 0;
                waterWave = ((value >> 12) & 1) != 0;
                shiny = ((value >> 13) & 1) != 0;
                weakWave = ((value >> 14) & 1) != 0;
                normal = (byte)((value >> 15) & 0xFFF);
                leavesWindWave = ((value >> 27) & 1) != 0;
                foliageWaveSpecial = ((value >> GroundDistanceBitsShift) & 7);
                all = value;
            }
        }

        public int AllWithoutWaveFlags
        {
            get { return all & clearWaveBits; }
        }

        byte glowLevel, zOffset;
        int normal;
        bool grassWindWave, leavesWindWave, waterWave, shiny, weakWave;
        int foliageWaveSpecial;

        // Bit 15: x-sign
        // Bit 16, 17, 18: x-value

        // Bit 19: y-sign
        // Bit 20, 21, 22: y-value

        // Bit 23: z-sign
        // Bit 24, 25, 26: z-value
        public static int NormalToPackedInt(Vec3d normal)
        {
            return NormalToPackedInt(normal.X, normal.Y, normal.Z);
        }

        public static int NormalToPackedInt(double x, double y, double z)
        {
            int xN = (int)(x * 7.000001) * 2;  //a small bump up for rounding, as the x, y, z values may be 1/7th fractions rounded down in binary
            int yN = (int)(y * 7.000001) * 2;
            int zN = (int)(z * 7.000001) * 2;  //can be any even number between -14 and +14

            return
                (xN < 0 ? 1 - xN : xN) |
                (yN < 0 ? 1 - yN : yN) << 4 |
                (zN < 0 ? 1 - zN : zN) << 8
            ;
        }

        public static int NormalToPackedInt(Vec3f normal)
        {
            int xN = (int)(normal.X * 7.000001f) * 2;  //a small bump up for rounding, as the x, y, z values may be 1/7th fractions rounded down in binary
            int yN = (int)(normal.Y * 7.000001f) * 2;
            int zN = (int)(normal.Z * 7.000001f) * 2;  //can be any even number between -14 and +14

            return
                (xN < 0 ? 1 - xN : xN) |
                (yN < 0 ? 1 - yN : yN) << 4 |
                (zN < 0 ? 1 - zN : zN) << 8
            ;
        }

        public static int NormalToPackedInt(Vec3i normal)
        {
            int xN = (int)(normal.X * 7.000001f) * 2;  //a small bump up for rounding, as the x, y, z values may be 1/7th fractions rounded down in binary
            int yN = (int)(normal.Y * 7.000001f) * 2;
            int zN = (int)(normal.Z * 7.000001f) * 2;  //can be any even number between -14 and +14

            return
                (xN < 0 ? 1 - xN : xN) |
                (yN < 0 ? 1 - yN : yN) << 4 |
                (zN < 0 ? 1 - zN : zN) << 8
            ;
        }

        public static void PackedIntToNormal(int packedNormal, float[] intoFloats)
        {
            int x = packedNormal & 0x00E;
            int y = packedNormal & 0x0E0;
            int z = packedNormal & 0xE00;

            int signx = 1 - ((packedNormal << 1) & 2);
            int signy = 1 - ((packedNormal >> 3) & 2);
            int signz = 1 - ((packedNormal >> 7) & 2);

            intoFloats[0] = signx * x / 14f;   // 7 * 2
            intoFloats[1] = signy * y / 224f;  // this is 7 * 2 * 16
            intoFloats[2] = signz * z / 3584f; // this is 7 * 2 * 256
        }

        public static void PackedIntToNormal(int packedNormal, double[] intoDouble)
        {
            int x = packedNormal & 0x00E;
            int y = packedNormal & 0x0E0;
            int z = packedNormal & 0xE00;

            int signx = 1 - ((packedNormal << 1) & 2);
            int signy = 1 - ((packedNormal >> 3) & 2);
            int signz = 1 - ((packedNormal >> 7) & 2);

            intoDouble[0] = signx * x / 14.0;   // 7 * 2
            intoDouble[1] = signy * y / 224.0;  // this is 7 * 2 * 16
            intoDouble[2] = signz * z / 3584.0; // this is 7 * 2 * 256
        }


        // Bits 0..7
        [JsonProperty]
        public byte GlowLevel
        {
            get
            {
                return glowLevel;
            }
            set
            {
                glowLevel = value;
                UpdateAll();
            }
        }

        // Bits 8, 9 and 10
        [JsonProperty]
        public byte ZOffset
        {
            get
            {
                return zOffset;
            }
            set
            {
                zOffset = value;
                UpdateAll();
            }
        }

        // Bit 11
        [JsonProperty]
        public bool GrassWindWave
        {
            get
            {
                return grassWindWave;
            }
            set
            {
                grassWindWave = value;
                UpdateAll();
            }
        }

        // Bit 12
        [JsonProperty]
        public bool WaterWave
        {
            get
            {
                return waterWave;
            }
            set
            {
                waterWave = value;
                UpdateAll();
            }
        }

        // Bit 13
        [JsonProperty]
        public bool Reflective
        {
            get
            {
                return shiny;
            }
            set
            {
                shiny = value;
                UpdateAll();
            }
        }

        // Bit 14
        [JsonProperty]
        public bool WeakWave
        {
            get
            {
                return weakWave;
            }
            set
            {
                weakWave = value;
                UpdateAll();
            }
        }

        // Bit 15: x-sign
        // Bit 16, 17, 18: x-value

        // Bit 19: y-sign
        // Bit 20, 21, 22: y-value

        // Bit 23: z-sign
        // Bit 24, 25, 26: z-value
        [JsonProperty]
        public int Normal
        {
            get
            {
                return normal;
            }
            set
            {
                normal = value;
                UpdateAll();
            }
        }

        // Bit 27
        [JsonProperty]
        public bool LeavesWindWave
        {
            get
            {
                return leavesWindWave;
            }
            set
            {
                leavesWindWave = value;
                UpdateAll();
            }
        }

        [JsonProperty]
        public bool WindSway
        {
            get
            {
                return leavesWindWave && grassWindWave;
            }
            set
            {
                leavesWindWave |= value;
                grassWindWave |= value;
                UpdateAll();
            }
        }

        /// <summary>
        /// 0 = default
        /// 1 = On weak wave, also have only low frequency jiggle
        /// 2 = unused
        /// 3 = Solid fruit and Stalk, rotate with origin
        /// 4 = Fruit underleaves
        /// </summary>
        [JsonProperty]
        public short FoliageWaveSpecial
        {
            get
            {
                return (short) foliageWaveSpecial;
            }
            set
            {
                foliageWaveSpecial = value;
                UpdateAll();
            }
        }

        public bool Lod0Fade
        {
            get
            {
                return (all & Lod0BitMask) != 0;
            }
        }


        public VertexFlags()
        {

        }

        public VertexFlags(int flags)
        {
            All = flags;
        }

        public VertexFlags(byte glowLevel, byte zOffset, bool grassWindWave, bool leavesWindWave, bool waterWave, bool lowContrast, bool weakWave, int normal)
        {
            this.glowLevel = glowLevel;
            this.zOffset = zOffset;
            this.grassWindWave = grassWindWave;
            this.waterWave = waterWave;
            this.shiny = lowContrast;
            this.normal = normal;
            this.weakWave = weakWave;
            this.leavesWindWave = leavesWindWave;

            UpdateAll();
        }

        void UpdateAll()
        {
            all = glowLevel
                  | ((zOffset & 0x7) << 8)
                  | (grassWindWave ? 1 : 0) << 11
                  | (waterWave ? 1 : 0) << 12
                  | (shiny ? 1 : 0) << 13
                  | (weakWave ? 1 : 0) << 14
                  | ((normal & 0xFFF) << 15)
                  | (leavesWindWave ? 1 : 0) << 27
                  | foliageWaveSpecial << GroundDistanceBitsShift;
            ;
        }

        /// <summary>
        /// Clones this set of vertex flags.  
        /// </summary>
        /// <returns></returns>
        public VertexFlags Clone()
        {
            return new VertexFlags(All);
        }
    }
}
