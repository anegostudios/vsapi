using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Net
{
    public class GameReleaseVersion
    {
        public GameBuild Windows;
        public GameBuild Server;
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
