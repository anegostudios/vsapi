using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace Vintagestory.API.Util
{
    public static class WildcardUtil
    {
        public static bool Match(AssetLocation needle, AssetLocation haystack)
        {
            if (needle == haystack) return true;

            if (haystack == null || needle.Domain != haystack.Domain) return false;

            string pattern = Regex.Escape(needle.Path).Replace(@"\*", @"(.*)");

            return Regex.IsMatch(haystack.Path, @"^" + pattern + @"$");
        }

        /// <summary>
        /// Checks whether or not the wildcard matches for inCode, for example, returns true for wildcard rock-* and inCode rock-granite
        /// </summary>
        /// <param name="wildCard"></param>
        /// <param name="inCode"></param>
        /// <param name="allowedVariants"></param>
        /// <returns></returns>
        public static bool Match(AssetLocation wildCard, AssetLocation inCode, string[] allowedVariants)
        {
            if (inCode == null || !wildCard.Domain.Equals(inCode.Domain)) return false;
            if (wildCard.Equals(inCode)) return true;

            string pattern = Regex.Escape(wildCard.Path).Replace(@"\*", @"(.*)");

            if (!Regex.IsMatch(inCode.Path, @"^" + pattern + @"$")) return false;

            if (allowedVariants != null)
            {
                int wildcardStartLen = wildCard.Path.IndexOf("*");
                int wildcardEndLen = wildCard.Path.Length - wildcardStartLen - 1;
                string code = inCode.Path.Substring(wildcardStartLen);
                string codepart = code.Substring(0, code.Length - wildcardEndLen);
                if (!allowedVariants.Contains(codepart)) return false;
            }

            return true;
        }

        /// <summary>
        /// Extract the value matched by the wildcard. For exammple for rock-* and inCode rock-granite, this method will return 'granite'
        /// Returns null if the wildcard does not match
        /// </summary>
        /// <param name="wildCard"></param>
        /// <param name="inCode"></param>
        /// <returns></returns>
        public static string GetWildcardValue(AssetLocation wildCard, AssetLocation inCode)
        {
            if (inCode == null || !wildCard.Domain.Equals(inCode.Domain)) return null;
            if (!wildCard.Path.Contains("*")) return null;

            string pattern = Regex.Escape(wildCard.Path).Replace(@"\*", @"(.*)");

            Match match = Regex.Match(inCode.Path, @"^" + pattern + @"$");

            return match.Success ? match.Groups[1].Captures[0].Value : null;
        }

    }
}
