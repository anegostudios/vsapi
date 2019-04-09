using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    public class ModWorldConfiguration
    {
        public PlayStyle[] PlayStyles;
        public WorldConfigurationAttribute[] WorldConfigAttributes;
    }

    public enum EnumDataType
    {
        Bool, IntInput, DoubleInput, IntRange, String
    }

    public class WorldConfigurationAttribute
    {
        public EnumDataType DataType;

        public JsonObject Data;

        public string Code;
        public double Min;
        public double Max;

        public object Default;
    }

    public interface IWorldConfigurationEntry
    {
        string Code { get; }
        JsonObject Data { get; }
        object Value { get; }
    }
    
}
