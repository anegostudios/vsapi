using System.Globalization;
using Vintagestory.API.Config;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Common
{
    public class ModWorldConfiguration
    {
        public PlayStyle[] PlayStyles;
        public WorldConfigurationAttribute[] WorldConfigAttributes;
    }

    public enum EnumDataType
    {
        Bool, IntInput, DoubleInput, IntRange, String, DropDown, DoubleRange, StringRange
    }

    public class WorldConfigurationAttribute
    {
        public EnumDataType DataType;

        public string Category;
        public string Code;
        public double Min;
        public double Max;
        public double Step;
        public double Alarm = int.MaxValue;
        public decimal Multiplier = 1;
        public decimal DisplayUnit = 1;

        public bool OnCustomizeScreen = true;

        public string Default;
        public string[] Values;
        public string[] Names;
        public string[] SkipValues = null;

        public bool OnlyDuringWorldCreate = false;

        [Newtonsoft.Json.JsonIgnore] // This is set during startup and should not be deserialized from worldsettings.json.
        public ModInfo ModInfo { get; set; }

        public object stringToValue(string text)
        {
            switch (DataType)
            {
                case EnumDataType.Bool:
                    bool.TryParse(text, out bool on);
                    return on;
                case EnumDataType.DoubleInput:
                case EnumDataType.DoubleRange:
                    double.TryParse(text, NumberStyles.Float, GlobalConstants.DefaultCultureInfo, out double dval);
                    return dval;
                case EnumDataType.IntInput:
                case EnumDataType.IntRange:
                    int.TryParse(text, NumberStyles.Integer, GlobalConstants.DefaultCultureInfo, out int val);
                    return val;
                case EnumDataType.String:
                case EnumDataType.DropDown:
                case EnumDataType.StringRange:
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
                case EnumDataType.DropDown:
                case EnumDataType.StringRange:
                    int index = Values.IndexOf(value);
                    return index >= 0 ? Lang.Get("worldconfig-" + Code + "-" + Names[index]) : value+"";
                case EnumDataType.DoubleInput:
                case EnumDataType.DoubleRange:
                case EnumDataType.IntInput:
                case EnumDataType.IntRange:
                case EnumDataType.String:
                    return value.ToString();
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
