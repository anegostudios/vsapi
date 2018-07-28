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

    }
}
