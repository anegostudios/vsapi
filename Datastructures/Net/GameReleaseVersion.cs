using System.Collections.Generic;

#nullable disable

namespace Vintagestory.API.Net
{
    public class GameReleaseVersion
    {
        public GameBuild Windows;
        public GameBuild Windowsserver;
        public GameBuild Linuxserver;
        public GameBuild Linux;
        public GameBuild Mac;
    }

    public class GameBuild
    {
        public string filename;
        public string filesize;
        public string md5;
        public Dictionary<string, string> urls;
        public bool latest = false;
    }
}
