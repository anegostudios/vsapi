using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    public struct ColorMapData
    {
        // 8 bit season map index
        // 8 bits climate map index
        // 8 bits temperature
        // 8 bits rainfall
        public int Value;
        
        public byte SeasonMapIndex => (byte)(Value & 0xff);
        public byte ClimateMapIndex => (byte)((Value >> 8) & 0xff);
        public byte Temperature => (byte)((Value >> 16) & 0xff);
        public byte Rainfall => (byte)((Value >> 24) & 0xff);


        public ColorMapData(int value)
        {
            Value = value;
        }

        public ColorMapData(byte seasonMapIndex, byte climateMapIndex, byte temperature, byte rainFall)
        {
            Value = (int)(seasonMapIndex | (climateMapIndex << 8) | (temperature << 16) | (rainFall << 24));
        }

        public static int FromValues(byte seasonMapIndex, byte climateMapIndex, byte temperature, byte rainFall)
        {
            return (int)(seasonMapIndex | (climateMapIndex << 8) | (temperature << 16) | (rainFall << 24));
        }
    }
}
