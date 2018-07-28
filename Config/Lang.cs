using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vintagestory.API.Common;

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

            List<IAssetOrigin> origins = manager.GetOrigins();
            for (int i = 0; i < origins.Count; i++)
            {
                List<IAsset> assets = origins[i].GetAssets(AssetCategory.lang);
                foreach (var asset in assets)
                {
                    if (asset.Name.Equals(language + ".json"))
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

        Dictionary<string, string> LangEntries = new Dictionary<string, string>();
        Dictionary<Regex, string> LangRegexes = new Dictionary<Regex, string>();

        /// <summary>
        /// Yes this means in a singleplayer situdation server and client share the same lang inst, but thats okay, since they use the same file anyway?
        /// </summary>
        public static Lang Inst;

        /// <summary>
        /// This will load the vanilla json file, without taking care of mods or resourcepacks.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="basePath"></param>
        /// <param name="language"></param>
        public static void PreLoad(ILogger logger, string basePath, string language = "en")
        {
            Inst = new Lang();
            string filePath = Path.Combine(basePath, "assets", "lang", language + ".json");

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

                if (key.Contains("*"))
                {
                    Regex regex = new Regex(key.Replace("*", "(.*)"), RegexOptions.Compiled);
                    Inst.LangRegexes[regex] = val.Value;
                } else
                {
                    Inst.LangEntries[key] = val.Value;
                }
            }
        }
        


        public static string Get(string key, params object[] param)
        {
            string value;
            string domainandkey = key.Contains(":") ? key : GlobalConstants.DefaultDomain + AssetLocation.LocationSeparator + key;
            

            if (Inst.LangEntries.TryGetValue(domainandkey, out value))
            {
                return string.Format(value, param);
            }

            return string.Format(key, param);
        }

        public static string GetMatching(string key, params object[] param)
        {
            string value;
            string domainandkey = key.Contains(":") ? key : GlobalConstants.DefaultDomain + AssetLocation.LocationSeparator + key;

            if (Inst.LangEntries.TryGetValue(domainandkey, out value))
            {
                return string.Format(value, param);
            }

            foreach (var val in Inst.LangRegexes)
            {
                if (val.Key.IsMatch(domainandkey))
                {
                    return string.Format(val.Value, param);
                }
            }


            return string.Format(key, param);
        }


        public static bool HasTranslation(string key)
        {
            string domainandkey = key.Contains(":") ? key : GlobalConstants.DefaultDomain + AssetLocation.LocationSeparator + key;

            if (Inst.LangEntries.ContainsKey(domainandkey)) return true;

            foreach (var val in Inst.LangRegexes)
            {
                if (val.Key.IsMatch(domainandkey))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
