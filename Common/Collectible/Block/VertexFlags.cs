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
        public const int LeavesWindWaveBitMask = 1 << 27;

        public const int GroundDistanceBitMask = 0x7 << 28; // 3 bits
        public const int Lod0BitMask = 1 << 31;

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
                all = value;
                GlowLevel = (byte)(value & 0xFF);
                ZOffset = (byte)((value >> 8) & 0x7);
                GrassWindWave = ((value >> 11) & 1) != 0;
                WaterWave = ((value >> 12) & 1) != 0;
                Reflective = ((value >> 13) & 1) != 0;
                WeakWave = ((value >> 14) & 1) != 0;
                Normal = (byte)((value >> 15) & 0xFFF);
                LeavesWindWave = ((value >> 27) & 1) != 0;
            }
        }

        public int AllWithoutWaveFlags
        {
            get { return all & ~FoliageWindWaveBitMask & ~LeavesWindWaveBitMask; }
        }

        byte glowLevel, zOffset;
        int normal;
        bool grassWindWave, leavesWindWave, waterWave, shiny, weakWave;

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
            int xN = (int)Math.Abs(x * 7);
            int yN = (int)Math.Abs(y * 7);
            int zN = (int)Math.Abs(z * 7);

            return
                (x < 0 ? 1 : 0) |
                (xN << 1) |
                ((y < 0 ? 1 : 0) << 4) |
                (yN << 5) |
                ((z < 0 ? 1 : 0) << 8) |
                (zN << 9)
            ;
        }

        public static int NormalToPackedInt(Vec3f normal)
        {
            int xN = (int)Math.Abs(normal.X * 7);
            int yN = (int)Math.Abs(normal.Y * 7);
            int zN = (int)Math.Abs(normal.Z * 7);

            return
                (normal.X < 0 ? 1 : 0) |
                (xN << 1) |
                ((normal.Y < 0 ? 1 : 0) << 4) |
                (yN << 5) |
                ((normal.Z < 0 ? 1 : 0) << 8) |
                (zN << 9)
            ;
        }

        public static void PackedIntToNormal(int packedNormal, float[] intoFloats)
        {
            int x = (packedNormal >> 1) & 0x7;
            int y = (packedNormal >> 5) & 0x7;
            int z = (packedNormal >> 9) & 0x7;

            int signx = packedNormal & 1;
            int signy = (packedNormal >> 4) & 1;
            int signz = (packedNormal >> 8) & 1;

            intoFloats[0] = (1.0f - signx * 2) * x / 7.0f;
            intoFloats[1] = (1.0f - signy * 2) * y / 7.0f;
            intoFloats[2] = (1.0f - signz * 2) * z / 7.0f;
        }

        public static void PackedIntToNormal(int packedNormal, double[] intoFloats)
        {
            int x = (packedNormal >> 1) & 0x7;
            int y = (packedNormal >> 5) & 0x7;
            int z = (packedNormal >> 9) & 0x7;

            int signx = packedNormal & 1;
            int signy = (packedNormal >> 4) & 1;
            int signz = (packedNormal >> 8) & 1;

            intoFloats[0] = (1.0f - signx * 2) * x / 7.0f;
            intoFloats[1] = (1.0f - signy * 2) * y / 7.0f;
            intoFloats[2] = (1.0f - signz * 2) * z / 7.0f;
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
