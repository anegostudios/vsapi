using System;
using System.Text.RegularExpressions;
using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.API.Util
{
    public static class WildcardUtil
    {
        /// <summary>
        /// Returns a new AssetLocation with the wildcards (*) being filled with the blocks other Code parts, if the wildcard matches. 
        /// Example this block is trapdoor-up-north. search is *-up-*, replace is *-down-*, in this case this method will return trapdoor-down-north.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="search"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static AssetLocation WildCardReplace(this AssetLocation code, AssetLocation search, AssetLocation replace)
        {
            if (search == code) return search;

            if (code == null || (search.Domain != "*" && search.Domain != code.Domain)) return null;

            string pattern = Regex.Escape(search.Path).Replace(@"\*", @"(.*)");

            Match match = Regex.Match(code.Path, @"^" + pattern + @"$");
            if (!match.Success) return null;

            string outCode = replace.Path;

            for (int i = 1; i < match.Groups.Count; i++)
            {
                Group g = match.Groups[i];
                CaptureCollection cc = g.Captures;
                for (int j = 0; j < cc.Count; j++)
                {
                    Capture c = cc[j];

                    int pos = outCode.IndexOf('*');
                    outCode = outCode.Remove(pos, 1).Insert(pos, c.Value);
                }
            }

            return new AssetLocation(code.Domain, outCode);
        }

        public static bool Match(string needle, string haystack)
        {
            return fastMatch(needle, haystack);
        }
        public static bool Match(string[] needles, string haystack)
        {
            for (int i = 0; i < needles.Length; i++)
            {
                if (fastMatch(needles[i], haystack)) return true;
            }

            return false;
        }

        public static bool Match(AssetLocation needle, AssetLocation haystack)
        {
            if (needle.Domain != "*" && needle.Domain != haystack.Domain) return false;
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
            if (wildCard.Equals(inCode)) return true;

            int wildCardIndex;
            if (inCode == null || (wildCard.Domain != "*" && !wildCard.Domain.Equals(inCode.Domain)) || ((wildCardIndex = wildCard.Path.IndexOf('*')) == -1 && wildCard.Path.IndexOf('(') == -1)) return false;
            

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
                if (!MatchesVariants(wildCard, inCode, allowedVariants)) return false;
            }

            return true;
        }

        public static bool MatchesVariants(AssetLocation wildCard, AssetLocation inCode, string[] allowedVariants)
        {
            int wildcardStartLen = wildCard.Path.IndexOf('*');
            int wildcardEndLen = wildCard.Path.Length - wildcardStartLen - 1;
            if (inCode.Path.Length <= wildcardStartLen) return false;

            string code = inCode.Path.Substring(wildcardStartLen);
            if (code.Length - wildcardEndLen <= 0) return false;
            string codepart = code.Substring(0, code.Length - wildcardEndLen);
            
            return allowedVariants.Contains(codepart);
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
            if (inCode == null || (wildCard.Domain != "*" && !wildCard.Domain.Equals(inCode.Domain))) return null;
            if (!wildCard.Path.Contains('*')) return null;

            string pattern = Regex.Escape(wildCard.Path).Replace(@"\*", @"(.*)");

            Match match = Regex.Match(inCode.Path, @"^" + pattern + @"$", RegexOptions.None);

            return match.Success ? match.Groups[1].Captures[0].Value : null;
        }




        private static bool fastMatch(string needle, string haystack)
        {
            if (haystack == null) throw new ArgumentNullException("Text cannot be null");
            if (needle.Length == 0) return false;

            if (needle[0] == '@')
            {
                return Regex.IsMatch(haystack, @"^" + needle.Substring(1) + @"$", RegexOptions.None);
            }

            int needleLength = needle.Length;
            for (int i = 0; i < needleLength; i++)
            {
                char ch = needle[i];
                if (ch == '*')
                {
                    // Oh, * was at the end of the pattern? We no longer need to match the rest of the text
                    int remainingChars = needleLength - 1 - i;
                    if (remainingChars == 0) return true;

                    int secondAsterisk = needle.IndexOf('*', i + 1);
                    if (secondAsterisk >= 0)
                    {
                        if (needle.IndexOf('*', secondAsterisk + 1) >= 0)
                        {
                            // Three *? Ok, that really does need a regex!!
                            needle = Regex.Escape(needle).Replace(@"\*", @".*");
                            return Regex.IsMatch(haystack, @"^" + needle + @"$", RegexOptions.IgnoreCase);
                        }
                        if (haystack.Length < needle.Length - 2) return false;
                        int countTailpiece = needleLength - (secondAsterisk + 1);
                        if (!EndsWith(haystack, needle, countTailpiece)) return false;

                        // First part and last part of haystack matched, middle section of haystack needs to contain the needle section between the two wildcards
                        string needleCentreSection = needle.Substring(i + 1, secondAsterisk - (i + 1)).ToLowerInvariant();
                        if (i == 0 && countTailpiece == 0)
                        {
                            return haystack.ToLowerInvariant().Contains(needleCentreSection);
                        }
                        return haystack.Substring(i, haystack.Length - i - countTailpiece).ToLowerInvariant().Contains(needleCentreSection);
                    }

                    // Otherwise see if pattern matches after the *
                    return haystack.Length >= needle.Length - 1 && EndsWith(haystack, needle, remainingChars);
                }
                else
                {
                    // No * yet? Lets make sure the string starts with all those chars
                    if (haystack.Length <= i) return false;
                    char h = haystack[i];
                    if (ch != h && char.ToLowerInvariant(ch) != char.ToLowerInvariant(h)) return false;
                }
            }

            // No * wildcard? Then we're good if needle is of equal length than haystack
            return needle.Length == haystack.Length;
        }

        private static bool EndsWith(string haystack, string needle, int endCharsCount)
        {
            int hEnd = haystack.Length - 1;
            int nEnd = needle.Length - 1;

            // Length of haystack has been pre-checked
            for (int i = 0; i < endCharsCount; i++)
            {
                char h = haystack[hEnd - i];
                char ch = needle[nEnd - i];
                if (ch != h && char.ToLowerInvariant(ch) != char.ToLowerInvariant(h)) return false;
            }

            return true;
        }

        internal static bool fastExactMatch(string needle, string haystack)
        {
            // No wildcard: match all needle characters with haystack - we start at the end because end is least likely to match

            if (haystack.Length != needle.Length) return false;

            int lastChar = needle.Length - 1;
            for (int i = lastChar; i >= 0; i--)
            {
                char ch = needle[i];
                char h = haystack[i];
                if (ch != h && char.ToLowerInvariant(ch) != char.ToLowerInvariant(h)) return false;
            }

            // Everything matched
            return true;
        }

        /// <summary>
        /// Requires a pre-check that needle.Length is at least 1, and needleAsRegex has been pre-prepared
        /// </summary>
        /// <param name="needle"></param>
        /// <param name="haystack"></param>
        /// <param name="needleAsRegex">If it starts with '^' interpret as a regex search string; otherwise special case, it represents the tailpiece of the needle following a single asterisk</param>
        /// <returns></returns>
        internal static bool fastMatch(string needle, string haystack, string needleAsRegex)
        {
            // Pre-prepared regex string
            int tailPieceLength = needleAsRegex.Length;
            if (tailPieceLength > 0 && needleAsRegex[0] == '^')
            {
                return Regex.IsMatch(haystack, needleAsRegex, RegexOptions.IgnoreCase);   // None of our vanilla code ever actually uses this regex approach (would need two wildcards in a SearchBlocks call)
            }

            // Special case: one asterisk present, and needleAsRegex is actually the needle substring following that asterisk (which may be "" if the asterisk was at the end)
            if (haystack.Length < needle.Length - 1) return false;

            // Check the part after the asterisk
            if (tailPieceLength != 0 && !EndsWith(haystack, needle, tailPieceLength)) return false;

            // Check the part before the asterisk
            int lengthFirstPart = needle.Length - tailPieceLength - 1;
            for (int i = 0; i < lengthFirstPart; i++)
            {
                char ch = needle[i];
                char h = haystack[i];
                if (ch != h && char.ToLowerInvariant(ch) != char.ToLowerInvariant(h)) return false;
            }

            // Everything matched
            return true;
        }


        /// <summary>
        /// Returns the needle as a Regex string, if we are going to need to do a Regex search; alternatively returns some special case values
        /// <br/>Special case: return value of null signifies no wildcard, look for exact matches only
        /// <br/>Special case: return value of a non-regex string (not starting '^') represents the tailpiece part of the needle (the part following a single wildcard)
        /// </summary>
        /// <param name="needle"></param>
        /// <returns></returns>
        internal static string Prepare(string needle)
        {
            if (needle[0] == '@')
            {
                return @"^" + needle.Substring(1) + @"$";
            }
            int wildIndex = needle.IndexOf('*');
            if (wildIndex == -1) return null;   // null signifies no regex required

            if (needle[0] != '^')
            {
                // return a simple string (which may be "") to signify exactly one asterisk present; the string is the rest of the needle after that asterisk
                if (needle.IndexOf('*', wildIndex + 1) < 0) return needle.Substring(wildIndex + 1);
            }

            needle = Regex.Escape(needle).Replace(@"\*", @".*");
            return @"^" + needle + @"$";
        }
    }
    
}
