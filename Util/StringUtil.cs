using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
