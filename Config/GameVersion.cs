using System;
using System.Reflection;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

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

    /// <summary>
    /// The games current version
    /// </summary>
    public static class GameVersion
    {
        /// <summary>
        /// Assembly Info Version number in the format: major.minor.revision
        /// </summary>
        public const string OverallVersion = "1.16.5";

        /// <summary>
        /// Whether this is a stable or unstable version
        /// </summary>
        public const EnumGameBranch Branch = EnumGameBranch.Stable;

        /// <summary>
        /// Version number in the format: major.minor.revision[appendix]
        /// </summary>
        public const string ShortGameVersion = OverallVersion + "";
          
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
        public const string APIVersion = "1.7.0";

        /// <summary>
        /// Version of the Network Protocol
        /// </summary>
        public const string NetworkVersion = "1.16.5";

        /// <summary>
        /// Version of the savegame database
        /// </summary>
        public static int DatabaseVersion = 2;

        /// <summary>
        /// Version of the chunkdata compression for individual WorldChunks (0 is Deflate; 1 is ZSTD)
        /// </summary>
        public const int ChunkdataVersion = 0;

        /// <summary>
        /// "Version" of the block and item mapping. This number gets increased by 1 when remappings are needed
        /// </summary>
        public static int BlockItemMappingVersion = 1;


        /// <summary>
        /// Copyright notice
        /// </summary>
        public const string CopyRight = "Copyright © 2016-2022 Anego Studios";


        static string[] separators = new string[] { ".", "-" };
        static int[] splitVersionString(string version)
        {
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
                int ver;
                int.TryParse(parts[i], out ver);
                versions[i] = ver;
            }

            return versions;
        }

        /// <summary>
        /// Returns true if given version has the same major and minor version. Ignores revision.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static bool IsCompatibleApiVersion(string version)
        {
            int[] partsTheirs = splitVersionString(version);
            int[] partsMine = splitVersionString(APIVersion);

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
            int[] partsTheirs = splitVersionString(version);
            int[] partsMine = splitVersionString(NetworkVersion);

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
            int[] partsMin = splitVersionString(reference);
            int[] partsCur = splitVersionString(version);

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
            int[] partsMin = splitVersionString(reference);
            int[] partsCur = splitVersionString(version);

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
