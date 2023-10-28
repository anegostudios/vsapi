using System;
using System.Text;

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

    }
    public static class StringExtensions
    {
        public static bool CaseInsensitiveContains(this string text, string value, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
        {
            return text.IndexOf(value, stringComparison) >= 0;
        }
    }
}
