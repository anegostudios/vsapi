using System;
using System.Text;

#nullable disable

namespace Vintagestory.API.Util
{
    public static class StringBuilderExtensions
    {

        public static void AppendLineOnce(this StringBuilder sb)
        {
            if (sb.Length > 0)
            {
                if (sb[sb.Length - 1] != '\n') sb.AppendLine();
            }
        }

        /// <summary>
        /// Prints a single byte in hexadecimal.  Potentially useful for logging issues relating to serialization, packets contents, etc
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="b"></param>
        public static void AppendHex(this StringBuilder sb, byte b)
        {
            sb.Append((char)('0' + (b / 16) + (b / 160 * 7)));
            b %= 16;
            sb.Append((char)('0' + b + (b / 10 * 7)));
        }

        /// <summary>
        /// Prints a byte array in hexadecimal.  Potentially useful for logging issues relating to serialization, packets contents, etc
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="b"></param>
        public static void AppendHex(this StringBuilder sb, byte[] bb)
        {
            foreach (byte b in bb) AppendHex(sb, b);
        }


    }
    public static class StringExtensions
    {
        public static bool CaseInsensitiveContains(this string text, string value, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
        {
            return text.IndexOf(value, stringComparison) >= 0;
        }

        public static string DeDuplicate(this string str)
        {
            return str == null ? null : string.Intern(str);
        }
    }
}
