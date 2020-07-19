using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Config;

namespace Vintagestory.API.Util
{
    public static class StringUtil
    {

        /// <summary>
        /// Convert the first character to an uppercase one
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string UcFirst(this string text)
        {
            return text.Substring(0, 1).ToUpperInvariant() + text.Substring(1);
        }


        public static bool ToBool(this string text, bool defaultValue = false)
        {
            string val = text?.ToLowerInvariant();
            if (val == "true" || val == "yes" || val == "1") return true;
            if (val == "false" || val == "no" || val == "0") return false;
            return defaultValue;
        }
        public static int ToInt(this string text, int defaultValue = 0)
        {
            int value;
            if (!int.TryParse(text, NumberStyles.Any, GlobalConstants.DefaultCultureInfo, out value))
            {
                return defaultValue;
            }
            return value;
        }

        public static float ToFloat(this string text, float defaultValue = 0)
        {
            float value;
            if (!float.TryParse(text, NumberStyles.Any, GlobalConstants.DefaultCultureInfo, out value))
            {
                return defaultValue;
            }
            return value;
        }

        public static double ToDouble(this string text, double defaultValue = 0)
        {
            double value;
            if (!double.TryParse(text, NumberStyles.Any, GlobalConstants.DefaultCultureInfo, out value))
            {
                return defaultValue;
            }
            return value;
        }

        public static double? ToDoubleOrNull(this string text, double? defaultValue = 0)
        {
            double value;

            if (!double.TryParse(text, NumberStyles.Any, GlobalConstants.DefaultCultureInfo, out value))
            {
                return defaultValue;
            }
            return value;
        }


        public static float? ToFloatOrNull(this string text, float? defaultValue = 0)
        {
            float value;

            if (!float.TryParse(text, NumberStyles.Any, GlobalConstants.DefaultCultureInfo, out value))
            {
                return defaultValue;
            }

            return value;
        }


        public static int CountChars(this string text, char c)
        {
            int cnt = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == c) cnt++;
            }
            return cnt;
        }


        public static bool FastStartsWith(string value, string reference)
        {
            if (reference.Length > value.Length) return false;

            for (int i = 0; i < reference.Length; i++)
            {
                if (value[i] != reference[i]) return false;
            }

            return true;
        }


        public static bool StartsWithFast(this string value, string reference)
        {
            if (reference.Length > value.Length) return false;

            for (int i = 0; i < reference.Length; i++)
            {
                if (value[i] != reference[i]) return false;
            }

            return true;
        }


        public static bool EqualsFast(this string value, string reference)
        {
            if (reference.Length != value.Length) return false;

            for (int i = 0; i < reference.Length; i++)
            {
                if (value[i] != reference[i]) return false;
            }

            return true;
        }

        public static bool FastStartsWith(string value, string reference, int len)
        {
            if (len > reference.Length) throw new ArgumentException("reference must be longer than len");
            if (len > value.Length) return false;

            for (int i = 0; i < len; i++)
            {
                if (value[i] != reference[i]) return false;
            }

            return true;
        }

    }
}
