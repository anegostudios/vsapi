using System.Globalization;
using Vintagestory.API.Config;
using Vintagestory.API.Util;

namespace Vintagestory.API.Common
{
    public class ModWorldConfiguration
    {
        public PlayStyle[] PlayStyles;
        public WorldConfigurationAttribute[] WorldConfigAttributes;
    }

    public enum EnumDataType
    {
        Bool, IntInput, DoubleInput, IntRange, String, DropDown
    }

    public class WorldConfigurationAttribute
    {
        public EnumDataType DataType;

        public string Category;
        public string Code;
        public double Min;
        public double Max;
        public double Step;

        public bool OnCustomizeScreen = true;

        public string Default;
        public string[] Values;
        public string[] Names;

        public bool OnlyDuringWorldCreate = false;

        public object stringToValue(string text)
        {
            switch (DataType)
            {

                case EnumDataType.Bool:
                    bool on;
                    bool.TryParse(text, out on);
                    return on;

                case EnumDataType.DoubleInput:
                    float fval;
                    float.TryParse(text, NumberStyles.Any, GlobalConstants.DefaultCultureInfo, out fval);
                    return fval;
                case EnumDataType.DropDown:
                    return text;

                case EnumDataType.IntInput:
                case EnumDataType.IntRange:
                    int val;
                    int.TryParse(text, out val);
                    return val;
                case EnumDataType.String:
                    return text;
            }

            return null;
        }

        public string valueToHumanReadable(string value)
        {
            switch (DataType)
            {
                case EnumDataType.Bool:
                    return value.ToLowerInvariant() == "true" ? Lang.Get("On") : Lang.Get("Off");

                case EnumDataType.DoubleInput:
                    return value+"";
                case EnumDataType.DropDown:
                    int index = Values.IndexOf((string)value);
                    return index >= 0 ? Names[index] : value+"";

                case EnumDataType.IntInput:
                case EnumDataType.IntRange:
                    return value + "";
                case EnumDataType.String:
                    return value + "";
            }

            return null;
        }


        public object TypedDefault
        {
            get
            {
                return stringToValue(Default);
            }
        }
    }



    public class WorldConfigurationValue
    {
        public WorldConfigurationAttribute Attribute;
        public string Code;
        public object Value;
    }
}
