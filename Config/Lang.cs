using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace Vintagestory.API.Config
{
    /// <summary>
    /// Utility class for enabling i18n. Loads language entries from assets/[currentlanguage].json
    /// </summary>
    public class Lang
    {

        /// <summary>
        /// Loads all lang entries including this from mods
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="manager"></param>
        /// <param name="language"></param>
        public static void Load(ILogger logger, IAssetManager manager, string language = "en")
        {
            Inst.LangEntries.Clear();
            Inst.LangRegexes.Clear();
            Inst.LangStartsWith.Clear();

            if (language != "en")
            {
                Load(logger, manager, "en"); // Default to english phrases for missing translated ones
            }

            List<IAssetOrigin> origins = manager.Origins;
            for (int i = 0; i < origins.Count; i++)
            {
                List<IAsset> assets = origins[i].GetAssets(AssetCategory.lang);
                foreach (var asset in assets)
                {
                    if (asset.Name.Equals(language.ToLowerInvariant() + ".json"))
                    {
                        string text = asset.ToText();

                        try
                        {
                            LoadEntries(asset.Location.Domain, JsonConvert.DeserializeObject<Dictionary<string, string>>(text));
                        }
                        catch (Exception e)
                        {
                            logger.Error("Failed to load lang file {0}: {1}", asset.Location, e);
                        }
                    }
                }
            }
        }

        public Dictionary<string, string> LangEntries = new Dictionary<string, string>();
        public Dictionary<string, KeyValuePair<Regex, string>> LangRegexes = new Dictionary<string, KeyValuePair<Regex, string>>();
        // Because c# regexes are slooooow
        public Dictionary<string, string> LangStartsWith = new Dictionary<string, string>();

        /// <summary>
        /// Yes this means in a singleplayer situdation server and client share the same lang inst, but thats okay, since they use the same file anyway?
        /// </summary>
        public static Lang Inst;

        /// <summary>
        /// This will load the vanilla json file, without taking care of mods or resourcepacks.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="assetsPath"></param>
        /// <param name="language"></param>
        public static void PreLoad(ILogger logger, string assetsPath, string language = "en")
        {
            Inst = new Lang();

            if (language != "en") PreLoad(logger, assetsPath, "en"); // Default to english phrases for missing translated ones


            string filePath = Path.Combine(assetsPath, "game", "lang", language + ".json");

            Dictionary<string, string> langEntries = new Dictionary<string, string>();

            if (File.Exists(filePath))
            {
                string langtexts = File.ReadAllText(filePath);

                try
                {
                    LoadEntries(GlobalConstants.DefaultDomain, JsonConvert.DeserializeObject<Dictionary<string, string>>(langtexts));
                } catch (Exception e)
                {
                    logger.Error("Failed to load lang file {0}: {1}", filePath, e);
                }
                
            }

        }


        static void LoadEntries(string domain, Dictionary<string, string> entries)
        {
            foreach (var val in entries)
            {
                string key = val.Key;
                if (!val.Key.Contains(":")) key = domain + AssetLocation.LocationSeparator + key;

                int wildCardCount = key.CountChars('*');

                if (wildCardCount > 0)
                {
                    if (wildCardCount == 1 && key.EndsWith("*"))
                    {
                        Inst.LangStartsWith[key.TrimEnd('*')] = val.Value;
                    }
                    else
                    {
                        Regex regex = new Regex(key.Replace("*", "(.*)"), RegexOptions.Compiled);
                        Inst.LangRegexes[key] = new KeyValuePair<Regex, string>(regex, val.Value);
                    }

                    
                } else
                {
                    Inst.LangEntries[key] = val.Value;
                }
            }

        }


        /// <summary>
        /// Returns null if the entry does not exist
        /// </summary>
        /// <param name="key"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string GetIfExists(string key, params object[] param)
        {
            string value;
            string domainandkey = key.Contains(":") ? key : GlobalConstants.DefaultDomain + AssetLocation.LocationSeparator + key;


            if (Inst.LangEntries.TryGetValue(domainandkey, out value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// Returns the key itself it the entry does not exist
        /// </summary>
        /// <param name="key"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string Get(string key, params object[] param)
        {
            return string.Format(GetUnformatted(key), param);
        }

        public static string GetUnformatted(string key)
        {
            string value;
            string domainandkey = key.Contains(":") ? key : GlobalConstants.DefaultDomain + AssetLocation.LocationSeparator + key;


            if (Inst.LangEntries.TryGetValue(domainandkey, out value))
            {
                return value;
            }

            return key;
        }

        public static string GetMatching(string key, params object[] param)
        {
            string value;
            string domainandkey = key.Contains(":") ? key : GlobalConstants.DefaultDomain + AssetLocation.LocationSeparator + key;

            if (Inst.LangEntries.TryGetValue(domainandkey, out value))
            {
                return string.Format(value, param);
            }

            foreach (var pair in Inst.LangStartsWith)
            {
                if (StringUtil.FastStartsWith(domainandkey, pair.Key))
                {
                    return string.Format(pair.Value, param);
                }
            }

            foreach (var pair in Inst.LangRegexes.Values)
            {
                if (pair.Key.IsMatch(domainandkey))
                {
                    return string.Format(pair.Value, param);
                }
            }


            return string.Format(key, param);
        }

        public static bool HasTranslation(string key, bool findWildcarded = true)
        {
            string domainandkey = key.Contains(":") ? key : GlobalConstants.DefaultDomain + AssetLocation.LocationSeparator + key;

            if (Inst.LangEntries.ContainsKey(domainandkey)) return true;

            if (findWildcarded)
            {
                foreach (var pair in Inst.LangStartsWith)
                {
                    if (StringUtil.FastStartsWith(key, pair.Key))
                    {
                        return true;
                    }
                }

                foreach (var pair in Inst.LangRegexes.Values)
                {
                    if (pair.Key.IsMatch(domainandkey))
                    {
                        return true;
                    }
                }
            }

            return false;
        }




    }
}
