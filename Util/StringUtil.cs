using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Text;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.Util
{
    public static class StringUtil
    {
        // Use this if and only if 'Denial of Service' attacks are not a concern (i.e. never used for free-form user input),
        // or are otherwise mitigated
        public static unsafe int GetNonRandomizedHashCode(this string str)
        {
            fixed (char* src = str)
            {
                Debug.Assert(src[str.Length] == '\0', "src[this.Length] == '\\0'");
                Debug.Assert(((int)src) % 4 == 0, "Managed string should start at 4 bytes boundary");

                uint hash1 = (5381 << 16) + 5381;
                uint hash2 = hash1;

                uint* ptr = (uint*)src;
                int length = str.Length;

                while (length > 2)
                {
                    length -= 4;
                    // Where length is 4n-1 (e.g. 3,7,11,15,19) this additionally consumes the null terminator
                    hash1 = (BitOperations.RotateLeft(hash1, 5) + hash1) ^ ptr[0];
                    hash2 = (BitOperations.RotateLeft(hash2, 5) + hash2) ^ ptr[1];
                    ptr += 2;
                }

                if (length > 0)
                {
                    // Where length is 4n-3 (e.g. 1,5,9,13,17) this additionally consumes the null terminator
                    hash2 = (BitOperations.RotateLeft(hash2, 5) + hash2) ^ ptr[0];
                }

                return (int)(hash1 + (hash2 * 1566083941));
            }
        }

        /// <summary>
        /// IMPORTANT!   This method should be used for every IndexOf operation in our code (except possibly in localised output to the user). This is important in order to avoid any
        /// culture-specific different results even when indexing GLSL shader code or other code strings, etc., or other strings in English, when the current culture is a different language
        /// (Known issue in the Thai language which has no spaces and treats punctuation marks as invisible, see https://github.com/dotnet/runtime/issues/59120)
        /// <br/>See also: https://learn.microsoft.com/en-us/dotnet/standard/base-types/best-practices-strings
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int IndexOfOrdinal(this String a, String b)
        {
            return a.IndexOf(b, StringComparison.Ordinal);
        }

        /// <summary>
        /// IMPORTANT!   This method should be used for every StartsWith operation in our code (except possibly in localised output to the user). This is important in order to avoid any
        /// culture-specific different results even when examining strings in English, when the user machine's current culture is a different language
        /// (Known issue in the Thai language which has no spaces and treats punctuation marks as invisible, see https://github.com/dotnet/runtime/issues/59120)
        /// <br/>See also: https://learn.microsoft.com/en-us/dotnet/standard/base-types/best-practices-strings
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool StartsWithOrdinal(this String a, String b)
        {
            return a.StartsWith(b, StringComparison.Ordinal);
        }

        /// <summary>
        /// IMPORTANT!   This method should be used for every EndsWith operation in our code (except possibly in localised output to the user). This is important in order to avoid any
        /// culture-specific different results even when examining strings in English, when the user machine's current culture is a different language
        /// (Known issue in the Thai language which has no spaces and treats punctuation marks as invisible, see https://github.com/dotnet/runtime/issues/59120)
        /// <br/>See also: https://learn.microsoft.com/en-us/dotnet/standard/base-types/best-practices-strings
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool EndsWithOrdinal(this String a, String b)
        {
            return a.EndsWith(b, StringComparison.Ordinal);
        }

        /// <summary>
        /// This should be used for every string comparison when ordering strings (except possibly in localised output to the user) in order to avoid any
        /// culture specific string comparison issues in certain languages (worst in the Thai language which has no spaces and treats punctuation marks as invisible)
        /// <br/>See also: https://learn.microsoft.com/en-us/dotnet/standard/base-types/best-practices-strings
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int CompareOrdinal(this String a, String b)
        {
            return String.CompareOrdinal(a, b);
        }

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

        public static string RemoveFileEnding(this string text)
        {
            return text.Substring(0, text.IndexOf('.'));
        }

        public static int ToInt(this string text, int defaultValue = 0)
        {
            if (!int.TryParse(text, NumberStyles.Any, GlobalConstants.DefaultCultureInfo, out int value))
            {
                return defaultValue;
            }
            return value;
        }

        public static long ToLong(this string text, long defaultValue = 0)
        {
            if (!long.TryParse(text, NumberStyles.Any, GlobalConstants.DefaultCultureInfo, out long value))
            {
                return defaultValue;
            }
            return value;
        }


        public static float ToFloat(this string text, float defaultValue = 0)
        {
            if (!float.TryParse(text, NumberStyles.Any, GlobalConstants.DefaultCultureInfo, out float value))
            {
                return defaultValue;
            }
            return value;
        }

        public static double ToDouble(this string text, double defaultValue = 0)
        {
            if (!double.TryParse(text, NumberStyles.Any, GlobalConstants.DefaultCultureInfo, out double value))
            {
                return defaultValue;
            }
            return value;
        }

        public static double? ToDoubleOrNull(this string text, double? defaultValue = 0)
        {

            if (!double.TryParse(text, NumberStyles.Any, GlobalConstants.DefaultCultureInfo, out double value))
            {
                return defaultValue;
            }
            return value;
        }


        public static float? ToFloatOrNull(this string text, float? defaultValue = 0)
        {

            if (!float.TryParse(text, NumberStyles.Any, GlobalConstants.DefaultCultureInfo, out float value))
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

        public static bool ContainsFast(this string value, char reference)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == reference) return true;
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

        /// <summary>
        /// A fast case-insensitive string comparison for "ordinal" culture i.e. plain ASCII comparison used for internal strings such as asset paths
        /// </summary>
        /// <param name="value"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static bool EqualsFastIgnoreCase(this string value, string reference)
        {
            if (reference.Length != value.Length) return false;

            char a, b;
            // search from the right end of the reference string, as the right end is more likely to be unique
            for (int i = reference.Length - 1; i >= 0; i--)
            {
                if ((a = value[i]) != (b = reference[i]))
                {
                    if ((a & 0xFFDF) == (b & 0xFFDF))   // Rough "toUppercase()" comparison
                    {
                        if ((a & 0xFFDF) >= 'A' && (a & 0xFFDF) <= 'Z') continue;  // Precise "toUppercase()" comparison
                    }
                    return false;
                }
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


        /// <summary>
        /// Removes diacritics and replaces quotation marks, guillemets and brackets with a blank space. Used to create a search friendly term
        /// </summary>
        /// <param name="stIn"></param>
        /// <returns></returns>
        public static string ToSearchFriendly(this string stIn)
        {
            string stFormD = stIn.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                var chr = stFormD[ich];
                if (chr == '«' || chr == '»' || chr == '"' || chr == '(' || chr == ')')
                {
                    sb.Append(' ');
                    continue;
                }
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(chr);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(chr);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }


        /// <summary>
        /// Remove username from paths printed in log files
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string SanitizePath(string path)
        {
            if (Environment.UserName == null || Environment.UserName.Length == 0) return path;

            switch (RuntimeEnv.OS)
            {
                case OS.Windows:
                {
                    int j = path.IndexOf("\\Users\\");
                    if (j >= 0)
                    {
                        int k = path.IndexOf("\\AppData\\Roaming\\");
                        if (k > j)
                        {
                            path = "%appdata%" + path.Substring(k + 16);
                        }
                        else
                        {
                            path = path.Replace(Environment.UserName, "username");
                        }

                    }
                    break;
                }
                case OS.Linux:
                {
                    int j = path.IndexOf("/home/");
                    if (j >= 0)
                    {
                        path = path.Replace(Environment.UserName, "username");
                    }
                    break;
                }
                case OS.Mac:
                {
                    int j = path.IndexOf("/Users/");
                    if (j >= 0)
                    {
                        path = path.Replace(Environment.UserName, "username");
                    }
                    break;
                }
            }

            return path;
        }
    }
}
