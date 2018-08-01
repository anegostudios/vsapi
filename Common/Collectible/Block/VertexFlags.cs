using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    [JsonObject(MemberSerialization.OptIn)]
    public class VertexFlags
    {
        public const int GlowLevelBitMask = 0xFF;
        public const int ZOffsetBitMask = 0x3 << 8;
        public const int WindWaveBitMask = 1 << 11;
        public const int WaterWaveBitMask = 1 << 12;

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
                ZOffset = (byte)((value >> 8) & 0x3);
                WindWave = ((value >> 11) & 1) != 0;
                WaterWave = ((value >> 12) & 1) != 0;
            }
        }

        byte glowLevel, zOffset;
        bool windWave, waterWave;

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

        // Bits 8..10
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
        public bool WindWave
        {
            get
            {
                return windWave;
            }
            set
            {
                windWave = value;
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

        public VertexFlags()
        {

        }

        public VertexFlags(int flags)
        {
            All = flags;
        }

        public VertexFlags(byte glowLevel, byte zOffset, bool windWave, bool waterWave)
        {
            this.glowLevel = glowLevel;
            this.zOffset = zOffset;
            this.windWave = windWave;
            this.waterWave = waterWave;
            UpdateAll();
        }

        void UpdateAll()
        {
            all = glowLevel
                  | ((zOffset & 0x3) << 8)
                  | (windWave ? 1 : 0) << 11
                  | (waterWave ? 1 : 0) << 12;
        }

        public VertexFlags Clone()
        {
            return new VertexFlags(All);
        }
    }
}
