using System.Collections.Generic;
using System.Globalization;
using System.Text;

#nullable disable

namespace Vintagestory.API.Client
{
    internal static class ComplexTextLayout
    {
        enum TextDirection
        {
            Neutral,
            LeftToRight,
            RightToLeft
        }

        readonly struct ArabicForms
        {
            public readonly char Isolated;
            public readonly char Final;
            public readonly char Initial;
            public readonly char Medial;

            public ArabicForms(char isolated, char final, char initial = '\0', char medial = '\0')
            {
                Isolated = isolated;
                Final = final;
                Initial = initial;
                Medial = medial;
            }

            public bool ConnectsToPrevious => Final != '\0' || Medial != '\0';
            public bool ConnectsToNext => Initial != '\0' || Medial != '\0';

            public char GetShapedChar(bool connectsToPrevious, bool connectsToNext)
            {
                if (connectsToPrevious && connectsToNext && Medial != '\0') return Medial;
                if (connectsToPrevious && Final != '\0') return Final;
                if (connectsToNext && Initial != '\0') return Initial;
                return Isolated;
            }
        }

        readonly struct DirectionalRun
        {
            public readonly TextDirection Direction;
            public readonly List<string> Clusters;

            public DirectionalRun(TextDirection direction, List<string> clusters)
            {
                Direction = direction;
                Clusters = clusters;
            }
        }

        static readonly Dictionary<char, ArabicForms> ArabicPresentationForms = new Dictionary<char, ArabicForms>
        {
            ['\u0621'] = new ArabicForms('\uFE80', '\0'),
            ['\u0622'] = new ArabicForms('\uFE81', '\uFE82'),
            ['\u0623'] = new ArabicForms('\uFE83', '\uFE84'),
            ['\u0624'] = new ArabicForms('\uFE85', '\uFE86'),
            ['\u0625'] = new ArabicForms('\uFE87', '\uFE88'),
            ['\u0626'] = new ArabicForms('\uFE89', '\uFE8A', '\uFE8B', '\uFE8C'),
            ['\u0627'] = new ArabicForms('\uFE8D', '\uFE8E'),
            ['\u0628'] = new ArabicForms('\uFE8F', '\uFE90', '\uFE91', '\uFE92'),
            ['\u0629'] = new ArabicForms('\uFE93', '\uFE94'),
            ['\u062A'] = new ArabicForms('\uFE95', '\uFE96', '\uFE97', '\uFE98'),
            ['\u062B'] = new ArabicForms('\uFE99', '\uFE9A', '\uFE9B', '\uFE9C'),
            ['\u062C'] = new ArabicForms('\uFE9D', '\uFE9E', '\uFE9F', '\uFEA0'),
            ['\u062D'] = new ArabicForms('\uFEA1', '\uFEA2', '\uFEA3', '\uFEA4'),
            ['\u062E'] = new ArabicForms('\uFEA5', '\uFEA6', '\uFEA7', '\uFEA8'),
            ['\u062F'] = new ArabicForms('\uFEA9', '\uFEAA'),
            ['\u0630'] = new ArabicForms('\uFEAB', '\uFEAC'),
            ['\u0631'] = new ArabicForms('\uFEAD', '\uFEAE'),
            ['\u0632'] = new ArabicForms('\uFEAF', '\uFEB0'),
            ['\u0633'] = new ArabicForms('\uFEB1', '\uFEB2', '\uFEB3', '\uFEB4'),
            ['\u0634'] = new ArabicForms('\uFEB5', '\uFEB6', '\uFEB7', '\uFEB8'),
            ['\u0635'] = new ArabicForms('\uFEB9', '\uFEBA', '\uFEBB', '\uFEBC'),
            ['\u0636'] = new ArabicForms('\uFEBD', '\uFEBE', '\uFEBF', '\uFEC0'),
            ['\u0637'] = new ArabicForms('\uFEC1', '\uFEC2', '\uFEC3', '\uFEC4'),
            ['\u0638'] = new ArabicForms('\uFEC5', '\uFEC6', '\uFEC7', '\uFEC8'),
            ['\u0639'] = new ArabicForms('\uFEC9', '\uFECA', '\uFECB', '\uFECC'),
            ['\u063A'] = new ArabicForms('\uFECD', '\uFECE', '\uFECF', '\uFED0'),
            ['\u0640'] = new ArabicForms('\u0640', '\u0640', '\u0640', '\u0640'),
            ['\u0641'] = new ArabicForms('\uFED1', '\uFED2', '\uFED3', '\uFED4'),
            ['\u0642'] = new ArabicForms('\uFED5', '\uFED6', '\uFED7', '\uFED8'),
            ['\u0643'] = new ArabicForms('\uFED9', '\uFEDA', '\uFEDB', '\uFEDC'),
            ['\u0644'] = new ArabicForms('\uFEDD', '\uFEDE', '\uFEDF', '\uFEE0'),
            ['\u0645'] = new ArabicForms('\uFEE1', '\uFEE2', '\uFEE3', '\uFEE4'),
            ['\u0646'] = new ArabicForms('\uFEE5', '\uFEE6', '\uFEE7', '\uFEE8'),
            ['\u0647'] = new ArabicForms('\uFEE9', '\uFEEA', '\uFEEB', '\uFEEC'),
            ['\u0648'] = new ArabicForms('\uFEED', '\uFEEE'),
            ['\u0649'] = new ArabicForms('\uFEEF', '\uFEF0'),
            ['\u064A'] = new ArabicForms('\uFEF1', '\uFEF2', '\uFEF3', '\uFEF4'),
            ['\u0671'] = new ArabicForms('\uFB50', '\uFB51'),
            ['\u067E'] = new ArabicForms('\uFB56', '\uFB57', '\uFB58', '\uFB59'),
            ['\u0686'] = new ArabicForms('\uFB7A', '\uFB7B', '\uFB7C', '\uFB7D'),
            ['\u0688'] = new ArabicForms('\uFB88', '\uFB89'),
            ['\u0691'] = new ArabicForms('\uFB8C', '\uFB8D'),
            ['\u0698'] = new ArabicForms('\uFB8A', '\uFB8B'),
            ['\u06A4'] = new ArabicForms('\uFB6A', '\uFB6B', '\uFB6C', '\uFB6D'),
            ['\u06A9'] = new ArabicForms('\uFB8E', '\uFB8F', '\uFB90', '\uFB91'),
            ['\u06AF'] = new ArabicForms('\uFB92', '\uFB93', '\uFB94', '\uFB95'),
            ['\u06BA'] = new ArabicForms('\uFB9E', '\uFB9F'),
            ['\u06BE'] = new ArabicForms('\uFBAA', '\uFBAB', '\uFBAC', '\uFBAD'),
            ['\u06C1'] = new ArabicForms('\uFBA6', '\uFBA7', '\uFBA8', '\uFBA9'),
            ['\u06CC'] = new ArabicForms('\uFBFC', '\uFBFD', '\uFBFE', '\uFBFF'),
            ['\u06D2'] = new ArabicForms('\uFBAE', '\uFBAF')
        };

        static readonly Dictionary<char, char> MirroredChars = new Dictionary<char, char>
        {
            ['('] = ')',
            [')'] = '(',
            ['['] = ']',
            [']'] = '[',
            ['{'] = '}',
            ['}'] = '{',
            ['<'] = '>',
            ['>'] = '<'
        };

        static readonly HashSet<char> LtrFriendlyNeutrals = new HashSet<char>
        {
            '.', ',', ':', ';', '/', '\\', '-', '_', '+', '@', '#', '&', '=', '%', '*'
        };

        public static string PrepareForRendering(string text)
        {
            if (string.IsNullOrEmpty(text) || !ContainsRightToLeftText(text))
            {
                return text;
            }

            if (ContainsPresentationForms(text) || ContainsDirectionalControlCharacters(text))
            {
                return text;
            }

            StringBuilder builder = new StringBuilder(text.Length);
            int lineStart = 0;

            while (lineStart < text.Length)
            {
                int lineEnd = lineStart;
                while (lineEnd < text.Length && text[lineEnd] != '\r' && text[lineEnd] != '\n')
                {
                    lineEnd++;
                }

                builder.Append(PrepareLine(text.Substring(lineStart, lineEnd - lineStart), true));

                if (lineEnd < text.Length)
                {
                    if (text[lineEnd] == '\r' && lineEnd + 1 < text.Length && text[lineEnd + 1] == '\n')
                    {
                        builder.Append("\r\n");
                        lineStart = lineEnd + 2;
                    }
                    else
                    {
                        builder.Append(text[lineEnd]);
                        lineStart = lineEnd + 1;
                    }
                }
                else
                {
                    lineStart = lineEnd;
                }
            }

            return builder.ToString();
        }

        public static string PrepareForGlyphRendering(string text)
        {
            if (string.IsNullOrEmpty(text) || !ContainsRightToLeftText(text))
            {
                return text;
            }

            if (ContainsDirectionalControlCharacters(text))
            {
                return text;
            }

            StringBuilder builder = new StringBuilder(text.Length);
            int lineStart = 0;

            while (lineStart < text.Length)
            {
                int lineEnd = lineStart;
                while (lineEnd < text.Length && text[lineEnd] != '\r' && text[lineEnd] != '\n')
                {
                    lineEnd++;
                }

                builder.Append(PrepareLine(text.Substring(lineStart, lineEnd - lineStart), false));

                if (lineEnd < text.Length)
                {
                    if (text[lineEnd] == '\r' && lineEnd + 1 < text.Length && text[lineEnd + 1] == '\n')
                    {
                        builder.Append("\r\n");
                        lineStart = lineEnd + 2;
                    }
                    else
                    {
                        builder.Append(text[lineEnd]);
                        lineStart = lineEnd + 1;
                    }
                }
                else
                {
                    lineStart = lineEnd;
                }
            }

            return builder.ToString();
        }

        public static bool RequiresRightToLeftLayout(string text)
        {
            return !string.IsNullOrEmpty(text) && ContainsRightToLeftText(text);
        }

        static string PrepareLine(string line, bool shapeArabic)
        {
            if (string.IsNullOrEmpty(line) || !ContainsRightToLeftText(line))
            {
                return line;
            }

            string shapedLine = shapeArabic ? ShapeArabic(line) : line;
            List<string> clusters = CreateClusters(shapedLine);
            if (clusters.Count == 0)
            {
                return shapedLine;
            }

            TextDirection baseDirection = GetBaseDirection(line);
            List<DirectionalRun> runs = CreateRuns(clusters, baseDirection);

            StringBuilder builder = new StringBuilder(shapedLine.Length);

            if (baseDirection == TextDirection.RightToLeft)
            {
                for (int i = runs.Count - 1; i >= 0; i--)
                {
                    AppendRun(builder, runs[i]);
                }
            }
            else
            {
                for (int i = 0; i < runs.Count; i++)
                {
                    AppendRun(builder, runs[i]);
                }
            }

            return builder.ToString();
        }

        static void AppendRun(StringBuilder builder, DirectionalRun run)
        {
            if (run.Direction == TextDirection.RightToLeft)
            {
                for (int i = run.Clusters.Count - 1; i >= 0; i--)
                {
                    builder.Append(MirrorCluster(run.Clusters[i]));
                }

                return;
            }

            for (int i = 0; i < run.Clusters.Count; i++)
            {
                builder.Append(run.Clusters[i]);
            }
        }

        static List<DirectionalRun> CreateRuns(List<string> clusters, TextDirection baseDirection)
        {
            TextDirection[] directions = new TextDirection[clusters.Count];

            for (int i = 0; i < clusters.Count; i++)
            {
                directions[i] = ClassifyCluster(clusters[i]);
            }

            for (int i = 0; i < directions.Length; i++)
            {
                if (directions[i] == TextDirection.Neutral)
                {
                    directions[i] = ResolveNeutralDirection(directions, i, baseDirection);
                }
            }

            List<DirectionalRun> runs = new List<DirectionalRun>();
            List<string> currentClusters = new List<string>();
            TextDirection currentDirection = directions[0];

            for (int i = 0; i < clusters.Count; i++)
            {
                if (currentClusters.Count > 0 && directions[i] != currentDirection)
                {
                    runs.Add(new DirectionalRun(currentDirection, currentClusters));
                    currentClusters = new List<string>();
                    currentDirection = directions[i];
                }

                currentClusters.Add(clusters[i]);
            }

            if (currentClusters.Count > 0)
            {
                runs.Add(new DirectionalRun(currentDirection, currentClusters));
            }

            return runs;
        }

        static TextDirection ClassifyCluster(string cluster)
        {
            if (string.IsNullOrEmpty(cluster))
            {
                return TextDirection.Neutral;
            }

            for (int i = 0; i < cluster.Length; i++)
            {
                char chr = cluster[i];

                if (IsRightToLeftChar(chr))
                {
                    return TextDirection.RightToLeft;
                }

                if (char.IsDigit(chr) || IsLeftToRightLetter(chr))
                {
                    return TextDirection.LeftToRight;
                }
            }

            return TextDirection.Neutral;
        }

        static TextDirection ResolveNeutralDirection(TextDirection[] directions, int index, TextDirection baseDirection)
        {
            TextDirection previousDirection = FindStrongDirection(directions, index - 1, -1);
            TextDirection nextDirection = FindStrongDirection(directions, index + 1, 1);

            if (previousDirection != TextDirection.Neutral && previousDirection == nextDirection)
            {
                return previousDirection;
            }

            if (previousDirection != TextDirection.Neutral && nextDirection == TextDirection.Neutral)
            {
                return previousDirection;
            }

            if (nextDirection != TextDirection.Neutral && previousDirection == TextDirection.Neutral)
            {
                return nextDirection;
            }

            return baseDirection == TextDirection.Neutral ? TextDirection.LeftToRight : baseDirection;
        }

        static TextDirection FindStrongDirection(TextDirection[] directions, int index, int step)
        {
            while (index >= 0 && index < directions.Length)
            {
                if (directions[index] != TextDirection.Neutral)
                {
                    return directions[index];
                }

                index += step;
            }

            return TextDirection.Neutral;
        }

        static List<string> CreateClusters(string text)
        {
            List<string> clusters = new List<string>();
            StringBuilder cluster = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                char chr = text[i];

                if (cluster.Length > 0 && IsTransparentChar(chr))
                {
                    cluster.Append(chr);
                    continue;
                }

                if (cluster.Length > 0)
                {
                    clusters.Add(cluster.ToString());
                    cluster.Clear();
                }

                cluster.Append(chr);
            }

            if (cluster.Length > 0)
            {
                clusters.Add(cluster.ToString());
            }

            return clusters;
        }

        static string MirrorCluster(string cluster)
        {
            if (string.IsNullOrEmpty(cluster))
            {
                return cluster;
            }

            char chr = cluster[0];
            if (!MirroredChars.TryGetValue(chr, out char mirrored))
            {
                return cluster;
            }

            if (cluster.Length == 1)
            {
                return mirrored.ToString();
            }

            char[] chars = cluster.ToCharArray();
            chars[0] = mirrored;
            return new string(chars);
        }

        static string ShapeArabic(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            char[] sourceChars = text.ToCharArray();
            StringBuilder builder = new StringBuilder(text.Length);

            for (int i = 0; i < sourceChars.Length; i++)
            {
                char currentChar = sourceChars[i];
                if (!ArabicPresentationForms.TryGetValue(currentChar, out ArabicForms currentForms))
                {
                    builder.Append(currentChar);
                    continue;
                }

                int previousIndex = FindPreviousJoiningIndex(sourceChars, i - 1);
                int nextIndex = FindNextJoiningIndex(sourceChars, i + 1);

                bool connectsToPrevious =
                    previousIndex >= 0 &&
                    ArabicPresentationForms.TryGetValue(sourceChars[previousIndex], out ArabicForms previousForms) &&
                    previousForms.ConnectsToNext &&
                    currentForms.ConnectsToPrevious;

                if (currentChar == '\u0644' && TryGetLamAlefLigature(sourceChars, i, nextIndex, connectsToPrevious, out char ligature))
                {
                    builder.Append(ligature);
                    i = nextIndex;
                    continue;
                }

                bool connectsToNext =
                    nextIndex >= 0 &&
                    ArabicPresentationForms.TryGetValue(sourceChars[nextIndex], out ArabicForms nextForms) &&
                    currentForms.ConnectsToNext &&
                    nextForms.ConnectsToPrevious;

                builder.Append(currentForms.GetShapedChar(connectsToPrevious, connectsToNext));
            }

            return builder.ToString();
        }

        static bool TryGetLamAlefLigature(char[] chars, int lamIndex, int nextIndex, bool connectsToPrevious, out char ligature)
        {
            ligature = '\0';

            if (nextIndex != lamIndex + 1)
            {
                return false;
            }

            switch (chars[nextIndex])
            {
                case '\u0622':
                    ligature = connectsToPrevious ? '\uFEF6' : '\uFEF5';
                    return true;
                case '\u0623':
                    ligature = connectsToPrevious ? '\uFEF8' : '\uFEF7';
                    return true;
                case '\u0625':
                    ligature = connectsToPrevious ? '\uFEFA' : '\uFEF9';
                    return true;
                case '\u0627':
                    ligature = connectsToPrevious ? '\uFEFC' : '\uFEFB';
                    return true;
            }

            return false;
        }

        static int FindPreviousJoiningIndex(char[] chars, int index)
        {
            while (index >= 0)
            {
                if (!IsTransparentChar(chars[index]))
                {
                    return index;
                }

                index--;
            }

            return -1;
        }

        static int FindNextJoiningIndex(char[] chars, int index)
        {
            while (index < chars.Length)
            {
                if (!IsTransparentChar(chars[index]))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        static TextDirection GetBaseDirection(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                char chr = text[i];

                if (IsRightToLeftChar(chr))
                {
                    return TextDirection.RightToLeft;
                }

                if (IsLeftToRightLetter(chr))
                {
                    return TextDirection.LeftToRight;
                }
            }

            return TextDirection.Neutral;
        }

        static bool ContainsRightToLeftText(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (IsRightToLeftChar(text[i]))
                {
                    return true;
                }
            }

            return false;
        }

        static bool IsTransparentChar(char chr)
        {
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(chr);
            return category == UnicodeCategory.NonSpacingMark
                || category == UnicodeCategory.SpacingCombiningMark
                || category == UnicodeCategory.EnclosingMark;
        }

        static bool IsLeftToRightLetter(char chr)
        {
            return char.IsLetter(chr) && !IsRightToLeftChar(chr);
        }

        static bool ContainsPresentationForms(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                char chr = text[i];
                if ((chr >= '\uFB50' && chr <= '\uFDFF') || (chr >= '\uFE70' && chr <= '\uFEFF'))
                {
                    return true;
                }
            }

            return false;
        }

        static bool ContainsDirectionalControlCharacters(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '\u200E':
                    case '\u200F':
                    case '\u202A':
                    case '\u202B':
                    case '\u202C':
                    case '\u202D':
                    case '\u202E':
                    case '\u2066':
                    case '\u2067':
                    case '\u2068':
                    case '\u2069':
                        return true;
                }
            }

            return false;
        }

        static bool IsRightToLeftChar(char chr)
        {
            return
                (chr >= '\u0590' && chr <= '\u08FF') ||
                (chr >= '\uFB1D' && chr <= '\uFDFF') ||
                (chr >= '\uFE70' && chr <= '\uFEFF');
        }
    }
}
