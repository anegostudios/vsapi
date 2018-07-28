using System;

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
        public const string OverallVersion = "1.5.6";

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
        public static string LongGameVersion = ShortGameVersion + " Survival Mode Edition (" + Branch + ")";

        /// <summary>
        /// Assembly Info Version number in the format: major.minor.revision
        /// </summary>
        public const string AssemblyVersion = "1.0.0.0";
        
 


        /// <summary>
        /// Version of the Mod API
        /// </summary>
        public const string APIVersion = "1.5.3";

        /// <summary>
        /// Version of the Network Protocol
        /// </summary>
        public static string NetworkVersion = "1.5.4";

        /// <summary>
        /// Version of the savegame database
        /// </summary>
        public static int DatabaseVersion = 2;


        static string[] separators = new string[] { ".", "pre", " ", "-" };
        static string[] splitVersionString(string version)
        {
            return version.Split(separators, StringSplitOptions.None);
        }

        /// <summary>
        /// Returns true if given version has the same major and minor version. Ignores revision.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static bool IsCompatibleApiVersion(string version)
        {
            string[] partsTheirs = splitVersionString(version);
            string[] partsMine = splitVersionString(APIVersion);

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
            string[] partsTheirs = splitVersionString(version);
            string[] partsMine = splitVersionString(NetworkVersion);

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
            string[] partsMin = splitVersionString(reference);
            string[] partsCur = splitVersionString(version);

            for (int i = 0; i < partsMin.Length; i++)
            {
                if (i >= partsCur.Length) return false;

                int partMin = 0;
                int.TryParse(partsMin[i], out partMin);

                int partCur = 0;
                int.TryParse(partsCur[i], out partCur);


                if (partMin > partCur) return false;

                if (partMin < partCur) return true;
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
            string[] partsMin = splitVersionString(reference);
            string[] partsCur = splitVersionString(version);

            for (int i = 0; i < partsMin.Length; i++)
            {
                if (i >= partsCur.Length) return false;

                int partMin = 0;
                int.TryParse(partsMin[i], out partMin);

                int partCur = 0;
                int.TryParse(partsCur[i], out partCur);

                if (partMin > partCur) return false;
                if (partMin < partCur) return true;
            }

            return false;
        }

    }
}
