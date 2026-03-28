using System;
using System.Runtime.CompilerServices;
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
            if (wildCard.Domain.Length * wildCard.Path.Length == 1 && wildCard.Domain == "*" && wildCard.Path == "*") return true;

            if (wildCard.Equals(inCode)) return true;

            int wildCardIndex;
            if (inCode == null || (wildCard.Domain != "*" && !wildCard.Domain.Equals(inCode.Domain)) || ((wildCardIndex = wildCard.Path.IndexOf('*')) == -1 && wildCard.Path.IndexOf('(') == -1))
            {
                return false;
            }


            // Some faster/pre checks before doing a regex, because regexes are so, sooooo sloooooow
            if (wildCardIndex == wildCard.Path.Length - 1)
            {
                if (!StringUtil.FastStartsWith(inCode.Path, wildCard.Path, wildCardIndex)) return false;
            }
            else
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
            if (haystack == null) throw new ArgumentNullException(nameof(haystack));
            if (string.IsNullOrEmpty(needle)) return false;

            // Special case: regular expression
            if (needle[0] == '@')
            {
                // Cache compiled regular expressions for better performance
                return RegexCache.IsMatch(needle, haystack);
            }

            // Use indices instead of Substring
            int needlePos = 0;
            int haystackPos = 0;
            int needleLen = needle.Length;
            int haystackLen = haystack.Length;
            int lastWildcardPos = -1;
            int lastHaystackPos = -1;

            while (haystackPos < haystackLen)
            {
                if (needlePos < needleLen)
                {
                    char n = needle[needlePos];
                    if (n == '*')
                    {
                        // Found wildcard
                        lastWildcardPos = needlePos;
                        lastHaystackPos = haystackPos;
                        needlePos++;

                        // If wildcard at end of needle, return true immediately
                        if (needlePos == needleLen) return true;

                        // Skip consecutive wildcards, and here also return true if our needle ends with a wildcard
                        // radfast note: we omit an initial needlePos < needleLen safety check, because we literally just tested that in the line above
                        while (needle[needlePos] == '*')
                        {
                            if (++needlePos == needleLen) return true;
                        }

                        continue;
                    }
                    else if (SameCharIgnoreCase(n, haystack[haystackPos]))
                    {
                        // Characters match
                        needlePos++;
                        haystackPos++;
                        continue;
                    }
                }

                // Now either needlePos == needleLen, or we don't have a wildcard at the end of the needle and we don't have the same character

                // Code path if there was a wildcard in the middle of the needle
                if (lastWildcardPos >= 0)
                {
                    // Backtrack to last wildcard in the needle, so that we try again to match the rest against the haystack
                    needlePos = lastWildcardPos + 1;
                    haystackPos = ++lastHaystackPos;                      // In effect if the needle is aaa*bbb, and the haystack is aaaZZZbbb, we skip one of the Z here
                }
                else
                {
                    // No match and no wildcard to backtrack
                    return false;
                }
            }

            // Skip remaining wildcards
            while (needlePos < needleLen)
            {
                if (needle[needlePos++] != '*') return false;
            }

            // If reached end of both strings - match
            return true;
        }

        /// <summary>
        /// This returns true if the characters compare as equal apart from the case, using a similar rule to string comparison with OrdinalIgnoreCase
        /// Intended for matching AssetLocation, attributes and other JSON-type data, not full language strings or user input
        /// Note: not guaranteed to match full string OrdinalIgnoreCase comparison for Unicode surrogate pairs etc. (as this method only sees single characters at a time)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SameCharIgnoreCase(char a, char b)
        {
            if (a == b) return true;

            // Fast "ignorecase" check for standard ASCII characters, e.g. letters a-zA-Z, numerals, punctuation like '-' or '_'
            if ((a | b) < 0x80)
            {
                uint diff = (uint)(a ^ b);              // Case-only letter differences produce diff == 0x20
                return diff == 0x20 && (uint)((a & 0x5F) - 'A') < 26;
                // radfast note: "(uint)((a & 0x5F) - 'A') < 26" is a fast single-branching check for whether char a is in the range A-Z or a-z.
            }

            // Non-ASCII fallback for accented characters, foreign languages etc - Microsoft says OrdinalIgnoreCase converts to UpperInvariant
            return char.ToUpperInvariant(a) == char.ToUpperInvariant(b);
        }

        // Class for caching regular expressions
        private static class RegexCache
        {
            private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Regex> cache = new();
            private static readonly TimeSpan timeout = TimeSpan.FromSeconds(1);

            public static bool IsMatch(string pattern, string input)
            {
                // Note, the Dictionary keys are the pattern starting with the @ characters
                var regex = cache.GetOrAdd(pattern, p => new Regex(string.Concat("^", p.AsSpan(1), "$"), RegexOptions.CultureInvariant, timeout));
                return regex.IsMatch(input);
            }

            public static void Clear() => cache.Clear();
        }


        /// <summary>
        /// Clears the regex cache
        /// Best to clean it up when exiting the world
        /// </summary>
        public static void ClearRegexCache() => RegexCache.Clear();


        private static bool EndsWith(string haystack, string needle, int endCharsCount)
        {
            int hEnd = haystack.Length - 1;
            int nEnd = needle.Length - 1;

            // Length of haystack has been pre-checked
            for (int i = 0; i < endCharsCount; i++)
            {
                if (!SameCharIgnoreCase(needle[nEnd - i], haystack[hEnd - i])) return false;
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
                if (!SameCharIgnoreCase(needle[i], haystack[i])) return false;
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
                if (!SameCharIgnoreCase(needle[i], haystack[i])) return false;
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

