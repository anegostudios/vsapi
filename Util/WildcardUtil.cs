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
        public static bool Match(string needle, string haystack)
        {
            return fastMatch(needle, haystack);
        }



        public static bool Match(AssetLocation needle, AssetLocation haystack)
        {
            if (needle.Domain != haystack.Domain) return false;
            return fastMatch(needle.Path, haystack.Path);
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
            // Todo: Adapt some of the fastMatch methods to save some cpu cycles here

            if (wildCard.Equals(inCode)) return true;

            int wildCardIndex;
            if (inCode == null || !wildCard.Domain.Equals(inCode.Domain) || ((wildCardIndex = wildCard.Path.IndexOf("*")) == -1 && wildCard.Path.IndexOf("(") == -1)) return false;
            

            // Some faster/pre checks before doing a regex, because regexes are so, sooooo sloooooow
            if (wildCardIndex == wildCard.Path.Length - 1)
            {
                if (!StringUtil.FastStartsWith(inCode.Path, wildCard.Path, wildCardIndex)) return false;
            } else
            {
                if (!StringUtil.FastStartsWith(inCode.Path, wildCard.Path, wildCardIndex)) return false;

                string pattern = Regex.Escape(wildCard.Path).Replace(@"\*", @"(.*)");
                if (!Regex.IsMatch(inCode.Path, @"^" + pattern + @"$", RegexOptions.None)) return false;
            }

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

            Match match = Regex.Match(inCode.Path, @"^" + pattern + @"$", RegexOptions.None);

            return match.Success ? match.Groups[1].Captures[0].Value : null;
        }




        private static bool fastMatch(string pattern, string text)
        {
            if (text == null) throw new ArgumentNullException("Text cannot be null");
            if (pattern.Length == 0) return false;

            if (pattern[0] == '@')
            {
                return Regex.IsMatch(text, @"^" + pattern.Substring(1) + @"$", RegexOptions.None);
            }

            int wildCardIndex = -1;
            int i;
            for (i = 0; i < pattern.Length; i++)
            {
                char ch = pattern[i];
                if (ch == '*')
                {
                    // Two *? Ok, that needs a regex :<
                    if (wildCardIndex >= 0)
                    {
                        pattern = Regex.Escape(pattern).Replace(@"\*", @"(.*)");
                        return Regex.IsMatch(text, @"^" + pattern + @"$", RegexOptions.None);
                    }

                    wildCardIndex = i;
                } else
                {
                    // No * yet? Lets make sure the string starts with all those chars
                    if (wildCardIndex < 0 && (text.Length <=     i || char.ToLowerInvariant(ch) != char.ToLowerInvariant(text[i]))) return false;
                }
            }

            // No *? Nice, we're done here
            if (wildCardIndex == -1) return true;

            // Oh, * was at the end of the pattern? We no longer need to match the rest of the text
            if (wildCardIndex == pattern.Length - 1) return true;
            

            // Otherwise fallback to full on regex matching again :<
            pattern = Regex.Escape(pattern).Replace(@"\*", @"(.*)");
            return Regex.IsMatch(text, @"^" + pattern + @"$", RegexOptions.None);
        }
    }
    
}
