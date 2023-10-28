using System;
using System.Globalization;
using System.Text;
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

        public static long ToLong(this string text, long defaultValue = 0)
        {
            long value;
            if (!long.TryParse(text, NumberStyles.Any, GlobalConstants.DefaultCultureInfo, out value))
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

        public static bool ContainsFast(this string value, string reference)
        {
            if (reference.Length > value.Length) return false;
            
            int j = 0;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == reference[j]) j++;
                else j = 0;

                if (j >= reference.Length) return true;
            }

            return false;
        }


        public static bool StartsWithFast(this string value, string reference)
        {
            if (reference.Length > value.Length) return false;

            // search from the right end of the reference string, as the right end is more likely to be unique
            for (int i = reference.Length - 1; i >= 0; i--)
            {
                if (value[i] != reference[i]) return false;
            }

            return true;
        }


        public static bool StartsWithFast(this string value, string reference, int offset)
        {
            if (reference.Length + offset > value.Length) return false;

            // search from the right end of the reference string, as the right end is more likely to be unique
            for (int i = reference.Length + offset - 1; i >= offset; i--)
            {
                if (value[i] != reference[i - offset]) return false;
            }

            return true;
        }


        public static bool EqualsFast(this string value, string reference)
        {
            if (reference.Length != value.Length) return false;

            // search from the right end of the reference string, as the right end is more likely to be unique
            for (int i = reference.Length - 1; i >= 0; i--)
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


        public static string RemoveDiacritics(this string stIn)
        {
            string stFormD = stIn.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

    }
}
