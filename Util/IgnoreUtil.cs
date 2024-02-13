using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Vintagestory.API.Util
{
    public class IgnoreFile
    {
        public readonly string filename;
        public readonly string fullpath;
        List<string> ignored = new List<string>();
        List<string> ignoredFiles = new List<string>();

        public IgnoreFile(string filename, string fullpath)
        {
            this.filename = filename;
            this.fullpath = fullpath;
            foreach(var line in File.ReadAllLines(filename))
            {
                if (String.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWithOrdinal("!"))
                    ignoredFiles.Add(WildCardToRegular(line.Substring(1)));
                else
                {
                    bool folder = line.EndsWith('/');
                    var path = cleanUpPath(line.Replace('/', Path.DirectorySeparatorChar));
                    if (folder)
                        path += Path.DirectorySeparatorChar + "*";
                    
                    ignored.Add(WildCardToRegular(path));
                }
            }
        }

        string cleanUpPath(string path)
        {
            string[] splitted = path.Split('/', '\\');
            return Path.Combine(splitted);
        }

        private static string WildCardToRegular(string value)
        {
            return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        }

        public bool Available(string path)
        {
            if(ignoredFiles.Count > 0 && File.Exists(path))
            {
                string name = Path.GetFileName(path);
                foreach (var ignore in ignoredFiles)
                    if (Regex.IsMatch(name, ignore))
                        return false;
            }

            path = cleanUpPath(path.Replace(fullpath, ""));
            foreach (var ignore in ignored)
                if (Regex.IsMatch(path, ignore))
                    return false;

            return true;
        }

        bool IsPathDirectory(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            path = path.Trim();

            if (Directory.Exists(path))
                return true;

            if (File.Exists(path))
                return false;

            // neither file nor directory exists. guess intention

            // if has trailing slash then it's a directory
            if (new[] { '\\', '/' }.Any(x => path.EndsWith(x)))
                return true; // ends with slash

            // if has extension then its a file; directory otherwise
            return string.IsNullOrWhiteSpace(Path.GetExtension(path));
        }
    }
}
