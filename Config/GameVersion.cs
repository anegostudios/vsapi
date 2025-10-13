using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Config
{
    /// <summary>
    /// Current branch of the game
    /// </summary>
    public enum EnumGameBranch
    {
        Stable,
        Unstable
    }

    public enum EnumReleaseType
    {
        Stable,
        Candidate,
        Preview,
        Development
    }

    /// <summary>
    /// The games current version
    /// </summary>
    public static class GameVersion
    {
        /// <summary>
        /// Assembly Info Version number in the format: major.minor.revision
        /// </summary>
        public const string OverallVersion = "1.21.4";

        /// <summary>
        /// Whether this is a stable or unstable version
        /// </summary>
        public const EnumGameBranch Branch = EnumGameBranch.Stable;

        /// <summary>
        /// Version number in the format: major.minor.revision[appendix]
        /// </summary>
        public const string ShortGameVersion = OverallVersion + "";

        public static EnumReleaseType ReleaseType => GetReleaseType(ShortGameVersion);

        /// <summary>
        /// Version number in the format: major.minor.revision [release title]
        /// </summary>
        public static string LongGameVersion = "v" + ShortGameVersion + " (" + Branch + ")";

        /// <summary>
        /// Assembly Info Version number in the format: major.minor.revision
        /// </summary>
        public const string AssemblyVersion = "1.0.0.0";




        /// <summary>
        /// Version of the Mod API
        /// </summary>
        public const string APIVersion = "1.21.0";

        /// <summary>
        /// Version of the Network Protocol
        /// </summary>
        public const string NetworkVersion = "1.21.9";

        /// <summary>
        /// Version of the world generator - a change in version will insert a smoothed chunk between old and new version
        /// </summary>
        public const int WorldGenVersion = 3;

        /// <summary>
        /// Version of the savegame database
        /// </summary>
        public static int DatabaseVersion = 2;

        /// <summary>
        /// Version of the chunkdata compression for individual WorldChunks (0 is Deflate; 1 is ZSTD and palettised)  Also affects compression of network packets sent
        /// </summary>
        public const int ChunkdataVersion = 2;

        /// <summary>
        /// "Version" of the block and item mapping. This number gets increased by 1 when remappings are needed
        /// </summary>
        public static int BlockItemMappingVersion = 1;


        /// <summary>
        /// Copyright notice
        /// </summary>
        public const string CopyRight = "Copyright © 2016-2024 Anego Studios";


        static string[] separators = new string[] { ".", "-" };
        public static int[] SplitVersionString(string version)
        {
            // Initial check to see if calling code (is a little borked) and using version string like 1.17-pre.1 instead of 1.17.0-pre.1, which would break stuff later in this method because later code assumes that "pre" is in parts[3]
            int hyphenIndex = version.IndexOf('-');
            string majorMinorVersion = hyphenIndex < 1 ? version : version.Substring(0, hyphenIndex);
            if (majorMinorVersion.CountChars('.') == 1)   // example, 1.17 instead of 1.17.0 has only one '.' separator
            {
                majorMinorVersion += ".0";     // Add the missing ".0"
                version = hyphenIndex < 1 ? majorMinorVersion : majorMinorVersion + version.Substring(hyphenIndex);   // Now version can be parsed consistently
            }

            string[] parts = version.Split(separators, StringSplitOptions.None);
            if (parts.Length <= 3)
            {
                parts = parts.Append("3");
            } else
            {
                if (parts[3] == "rc") parts[3] = "2"; // -rc
                else if (parts[3] == "pre") parts[3] = "1"; // -pre
                else parts[3] = "0"; // -dev
            }

            int[] versions = new int[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                int.TryParse(parts[i], out int ver);
                versions[i] = ver;
            }

            return versions;
        }

        public static EnumReleaseType GetReleaseType(string version)
        {
            switch(SplitVersionString(version)[3])
            {
                case 0:
                    return EnumReleaseType.Development;
                case 1:
                    return EnumReleaseType.Preview;
                case 2:
                    return EnumReleaseType.Candidate;
                case 3:
                    return EnumReleaseType.Stable;
            }

            throw new ArgumentException("Unknown release type");
        }

        /// <summary>
        /// Returns true if given version has the same major and minor version. Ignores revision.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static bool IsCompatibleApiVersion(string version)
        {
            int[] partsTheirs = SplitVersionString(version);
            int[] partsMine = SplitVersionString(APIVersion);

            if (partsTheirs.Length < 2) return false;

            return partsMine[0] == partsTheirs[0] && partsMine[1] == partsTheirs[1];
        }

        /// <summary>
        /// Returns true if given version has the same major and minor version. Ignores revision.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static bool IsCompatibleNetworkVersion(string version)
        {
            int[] partsTheirs = SplitVersionString(version);
            int[] partsMine = SplitVersionString(NetworkVersion);

            if (partsTheirs.Length < 2) return false;

            return partsMine[0] == partsTheirs[0] && partsMine[1] == partsTheirs[1];
        }

        /// <summary>
        /// Returns true if supplied version is the same or higher as the current version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static bool IsAtLeastVersion(string version)
        {
            return IsAtLeastVersion(version, ShortGameVersion);
        }


        /// <summary>
        /// Returns true if supplied version is the same or higher as the reference version
        /// </summary>
        /// <param name="version"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static bool IsAtLeastVersion(string version, string reference)
        {
            int[] partsMin = SplitVersionString(reference);
            int[] partsCur = SplitVersionString(version);

            for (int i = 0; i < partsMin.Length; i++)
            {
                if (i >= partsCur.Length) return false;

                if (partsMin[i] > partsCur[i]) return false;
                if (partsMin[i] < partsCur[i]) return true;
            }

            return true;
        }


        public static bool IsLowerVersionThan(string version, string reference)
        {
            return version != reference && !IsNewerVersionThan(version, reference);
        }

        /// <summary>
        /// Returns true if supplied version is the higher as the reference version
        /// </summary>
        /// <param name="version"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static bool IsNewerVersionThan(string version, string reference)
        {
            int[] partsMin = SplitVersionString(reference);
            int[] partsCur = SplitVersionString(version);

            for (int i = 0; i < partsMin.Length; i++)
            {
                if (i >= partsCur.Length) return false;

                if (partsMin[i] > partsCur[i]) return false;
                if (partsMin[i] < partsCur[i]) return true;
            }

            return false;
        }


        public static void EnsureEqualVersionOrKillExecutable(ICoreAPI api, string version, string reference, string modName)
        {
            if (version != reference)
            {
                if (api.Side == EnumAppSide.Server)
                {
                    Exception e = new Exception(Lang.Get("versionmismatch-server", modName + ".dll"));
                    ((ICoreServerAPI)api).Server.ShutDown();
                    throw e;
                } else
                {
                    Exception e = new Exception(Lang.Get("versionmismatch-client", modName + ".dll"));
                    ((ICoreClientAPI)api).Event.EnqueueMainThreadTask(() => throw e, "killgame");
                }
            }
        }

    }
}
