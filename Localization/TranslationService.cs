using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

#nullable disable


// Contributed by Apache#8842 over discord 20th of October 2021. Edited by Tyron
// Apache — Today at 11:45 AM
// If you want to use it, it's under your license... I made it to be added to the game. ITranslationService is so that it can be mocked within your Test Suite, added to an IOC Container, extended via mods, etc. Interfaces should be used, in favour of concrete classes, in  most cases. Especially where you have volatile code such as IO.

[assembly: InternalsVisibleTo("VSTests")]
namespace Vintagestory.API.Config
{
    /// <summary>
    /// A service, which provides access to translated strings, based on key/value pairs read from JSON files.
    /// </summary>
    /// <seealso cref="ITranslationService" />
    public class TranslationService : ITranslationService
    {
        internal Dictionary<string, string> entryCache = new Dictionary<string, string>();
        private Dictionary<string, KeyValuePair<Regex, string>> regexCache = new Dictionary<string, KeyValuePair<Regex, string>>();
        private Dictionary<string, string> wildcardCache = new Dictionary<string, string>();
        private HashSet<string> notFound = new HashSet<string>();

        private IAssetManager assetManager;
        private readonly ILogger logger;
        internal bool loaded = false;
        private string preLoadAssetsPath = null;
        private Dictionary<string, string> preLoadModPaths = new Dictionary<string, string>();
        private bool modWorldConfig = false;
        public EnumLinebreakBehavior LineBreakBehavior { get; set; }


        /// <summary>
        /// Initialises a new instance of the <see cref="TranslationService" /> class.
        /// </summary>
        /// <param name="languageCode">The language code that this translation service caters for.</param>
        /// <param name="logger">The <see cref="ILogger" /> instance used within the sided API.</param>
        /// <param name="assetManager">The <see cref="IAssetManager" /> instance used within the sided API.</param>
        /// <param name="lbBehavior"></param>
        public TranslationService(string languageCode, ILogger logger, IAssetManager assetManager = null, EnumLinebreakBehavior lbBehavior = EnumLinebreakBehavior.AfterWord)
        {
            LanguageCode = languageCode;
            this.logger = logger;
            this.assetManager = assetManager;
            this.LineBreakBehavior = lbBehavior;
        }

        /// <summary>
        /// Gets the language code that this translation service caters for.
        /// </summary>
        /// <value>A string, that contains the language code that this translation service caters for.</value>
        public string LanguageCode { get; }

        /// <summary>
        /// Loads translation key/value pairs from all relevant JSON files within the Asset Manager.
        /// </summary>
        public void Load(bool lazyLoad = false)
        {
            preLoadAssetsPath = null;
            if (lazyLoad) return;
            loaded = true;

            // Don't work on dicts directly for thread safety (client and local server access the same dict)
            var entryCache = new Dictionary<string, string>();
            var regexCache = new Dictionary<string, KeyValuePair<Regex, string>>();
            var wildcardCache = new Dictionary<string, string>();

            var origins = assetManager.Origins;

            foreach (var asset in origins.SelectMany(p => p.GetAssets(AssetCategory.lang).Where(a => a.Name.Equals($"{LanguageCode}.json") || a.Name.Equals($"worldconfig-{LanguageCode}.json"))))
            {

                try
                {
                    var json = asset.ToText();
                    LoadEntries(entryCache, regexCache, wildcardCache, JsonConvert.DeserializeObject<Dictionary<string, string>>(json), asset.Location.Domain);
                }
                catch (Exception ex)
                {
                    logger.Error($"Failed to load language file: {asset.Name}");
                    logger.Error(ex);
                }
            }

            this.entryCache = entryCache;
            this.regexCache = regexCache;
            this.wildcardCache = wildcardCache;
        }

        /// <summary>
        /// Loads only the vanilla JSON files, without dealing with mods, or resource-packs.
        /// </summary>
        /// <param name="assetsPath">The root assets path to load the vanilla files from.</param>
        /// <param name="lazyLoad"></param>
        public void PreLoad(string assetsPath, bool lazyLoad = false)
        {
            preLoadAssetsPath = assetsPath;
            if (lazyLoad) return;
            
            loaded = true;

            // Don't work on dicts directly for thread safety (client and local server access the same dict)
            var entryCache = new Dictionary<string, string>();
            var regexCache = new Dictionary<string, KeyValuePair<Regex, string>>();
            var wildcardCache = new Dictionary<string, string>();


            var assetsDirectory = new DirectoryInfo(Path.Combine(assetsPath, GlobalConstants.DefaultDomain, "lang"));
            var files = assetsDirectory.EnumerateFiles($"{LanguageCode}.json", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                try
                {
                    var json = File.ReadAllText(file.FullName);
                    LoadEntries(entryCache, regexCache, wildcardCache, JsonConvert.DeserializeObject<Dictionary<string, string>>(json));
                }
                catch (Exception ex)
                {
                    logger.Error($"Failed to load language file: {file.Name}");
                    logger.Error(ex);
                }
            }

            this.entryCache = entryCache;
            this.regexCache = regexCache;
            this.wildcardCache = wildcardCache;
            
        }

        /// <summary>
        /// Loads the mod worldconfig language JSON files only.
        /// </summary>
        /// <param name="modPath">The assets path to load the mod files from.</param>
        /// <param name="modDomain">The mod domain to use when loading the files.</param>
        /// <param name="lazyLoad"></param>
        public void PreLoadModWorldConfig(string modPath = null, string modDomain = null, bool lazyLoad = false)
        {
            modWorldConfig = true;
            if (modPath != null && modDomain != null && !preLoadModPaths.ContainsKey(modDomain))
            {
                preLoadModPaths.Add(modDomain, modPath);
            }
            if (lazyLoad || modPath == null || modDomain == null) return;

            // Don't work on dicts directly for thread safety (client and local server access the same dict)
            var entryCache = new Dictionary<string, string>();
            var regexCache = new Dictionary<string, KeyValuePair<Regex, string>>();
            var wildcardCache = new Dictionary<string, string>();

            var loadPaths = new Dictionary<string, string>();
            if (modPath == null && modDomain == null) loadPaths = preLoadModPaths;
            else loadPaths.Add(modDomain, modPath);

            loadPaths.Foreach(mod =>
            {
                var assetsDirectory = new DirectoryInfo(Path.Combine(mod.Value, "assets", "game", "lang"));
                try
                {
                    var files = assetsDirectory.EnumerateFiles($"worldconfig-{LanguageCode}.json", SearchOption.AllDirectories);

                    foreach (var file in files)
                    {
                        try
                        {
                            var json = File.ReadAllText(file.FullName);
                            LoadEntries(entryCache, regexCache, wildcardCache, JsonConvert.DeserializeObject<Dictionary<string, string>>(json));
                        }
                        catch (Exception ex)
                        {
                            logger.Error($"Failed to load language file: {file.Name}");
                            logger.Error(ex);
                        }
                    }
                }
                catch { logger.Error($"Failed to find language folder: {assetsDirectory.FullName}"); }
            });

            this.entryCache.AddRange(entryCache);
            this.regexCache.AddRange(regexCache);
            this.wildcardCache.AddRange(wildcardCache);

        }

        protected void EnsureLoaded()
        {
            if (!loaded)
            {
                if (preLoadAssetsPath != null)
                {
                    PreLoad(preLoadAssetsPath);
                    if (modWorldConfig) PreLoadModWorldConfig();
                }
                else Load();
            }
        }

        /// <summary>
        /// Sets the loaded flag to false, so that the next lookup causes it to reload all translation entries
        /// </summary>
        public void Invalidate()
        {
            loaded = false;
        }

        protected string Format(string value, params object[] args)
        {
            if (value.ContainsFast("{p")) return PluralFormat(value, args);
            return TryFormat(value, args);
        }

        private string TryFormat(string value, params object[] args)
        {
            string result;
            try
            {
                result = string.Format(value, args);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                result = value;
                if (logger != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Translation string format exception thrown for: \"");
                    for (int i = 0; i < value.Length; i++)
                    {
                        char c = value[i];
                        sb.Append(c);
                        if (c == '{' || c == '}') sb.Append(c);   // Need to escape {0} because the logger itself uses string.Format()!!!!
                    }
                    sb.Append("\"\n   Args were: ");
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (i > 0) sb.Append(", ");
                        sb.Append(args[i].ToString());
                    }

                    try
                    {
                        logger.Warning(sb.ToString());
                    }
                    catch (Exception e)
                    {
                        logger.Error("Exception thrown when trying to print exception message for an incorrect translation entry. Exception: ");
                        logger.Error(e);

                    }
                }
            }
            return result;
        }

        // General format: {p#:string0|string1|string2...} where # is a parameter index similar to using {0}, so zero for the first parameter etc.  That parameter should be a number, N
        // The strings string0, string1, string2 etc. will be the actual desired output for different values of N.
        // Most languages have different grammar rules for writing different numbers of objects, e.g. zero, one, two, more than two
        // These strings are separated by | with no spaces (any spaces you type will be in the output)
        // Left to right, these will be the language strings for N=0, N=1, N=2, N=3, N=4 etc.  The last one given continues to be repeated for all higher N.
        // The number N can be itself output in the string using a standard number format, for example #.00 
        //
        // Examples:
        // {p3:fish}                                                                   args[3] is N, output for different N is:  0+: fish
        // {p0:no apples|# apple|# apples}                                             args[0] is N, output for different N is:  0: no apples, 1: 1 apple, 2: 2 apples, 3: 3 apples, ... etc
        // {p9:no cake|a cake|a couple of cakes|a few cakes|a few cakes|many cakes}    args[9] is N, output for different N is:  0: no cake, 1: a cake, 2: a couple of cakes, 3-4: a few cakes, 5+: many cakes
        //
        private string PluralFormat(string value, object[] args)
        {
            int start = value.IndexOfOrdinal("{p");
            if (value.Length < start + 5) return TryFormat(value, args);   // Fail: too short to even allow sense checks without error
            int pluralOffset = start + 4;
            int end = value.IndexOf('}', pluralOffset);

            // Sense checks
            char c = value[start + 2];
            if (c < '0' || c > '9') return TryFormat(value, args);   // Fail: no argument number specified
            if (end < 0) return TryFormat(value, args);   // Fail: no closing curly brace
            int argNum = c - '0';
            if ((c = value[start + 3]) != ':')
            {
                if (value[start + 4] == ':' && c >= '0' && c <= '9')
                {
                    argNum = argNum * 10 + c - '0';
                    pluralOffset++;
                }
                else return TryFormat(value, args);   // Fail: no colon in position 3 or 4
            }
            if (argNum >= args.Length) throw new IndexOutOfRangeException("Index out of range: Plural format {p#:...} referenced an argument " + argNum + " but only " + args.Length + " arguments were available in the code");
            float N = 0;
            try
            {
                N = float.Parse(args[argNum].ToString());
            }
            catch (Exception)
            {
                // ignored
            }

            // Separate out the different elements of this string

            string before = value.Substring(0, start);
            string plural = value.Substring(pluralOffset, end - pluralOffset);
            string after = value.Substring(end + 1);

            StringBuilder sb = new StringBuilder();
            sb.Append(TryFormat(before, args));
            sb.Append(BuildPluralFormat(plural, N));
            sb.Append(Format(after, args));   // there could be further instances of {p#:...} after this
            return sb.ToString();
        }

        internal static string BuildPluralFormat(string input, float n)
        {
            string[] plurals = input.Split('|');
            int round = 3;
            if (plurals.Length >= 2)
            {
                // Attempt to guess the number of digits best to round to before choosing which plural form to use, by examining the displayed format of the number
                // If no number formatting found in plurals[1], try the last one instead (which may be the same ...)
                if (!TryGuessRounding(plurals[1], out round)) TryGuessRounding(plurals[plurals.Length - 1], out round);

                if (round < 0 || round > 15) round = 3;    // Prevent invalid rounding if the above code failed for any reason
            }

            int index = (int)Math.Ceiling(Math.Round(n, round));   // this implments a rule: 0 -> 0;  0.5 -> 1;  1 -> 1;  1.5 -> 2  etc.   This may not be appropriate for all languages e.g. French.  A future extension can allow more customisation by specifying math formulae
            if (index < 0 || index >= plurals.Length)
                index = plurals.Length - 1;
           
            if (plurals.Length >= 2)
            {
                // if we have unit == 1 we use singular, if it is smaller than 1 we use plural ex. 0.25 liters or 0.25 hours
                // only applies to plurals that have 3 plurals defined
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if(n == 1)
                    index = 1;
                else if(n < 1)
                {
                    index = 0;
                }
            }
            string rawResult = plurals[index];
            return WithNumberFormatting(rawResult, n);
        }

        private static bool TryGuessRounding(string entry, out int round)
        {
            string numberFormatting = GetNumberFormattingFrom(entry, out string _);
            if (numberFormatting.IndexOf('.') > 0)
            {
                round = numberFormatting.Length - numberFormatting.IndexOf('.') - 1;    // Number with decimal places: round to that number of decimal places
                return true;
            }
            else if (numberFormatting.Length > 0)        // Number with no decimal places: round to integer value
            {
                round = 0;
                return true;
            }

            round = 3;    // Default value
            return false;
        }

        internal static string GetNumberFormattingFrom(string rawResult, out string partB)
        {
            var j = GetStartIndexOfNumberFormat(rawResult);
            if (j >= 0)
            {
                int k = j;
                while (++k < rawResult.Length)
                {
                    char c = rawResult[k];
                    if (c != '#' && c != '.' && c != '0' && c != ',') break;
                }
                partB = rawResult.Substring(k);
                return rawResult.Substring(j, k - j);
            }
            partB = rawResult;
            return "";
        }

        private static int GetStartIndexOfNumberFormat(string rawResult)
        {
            var indexHash = rawResult.IndexOf('#');
            var indexZero = rawResult.IndexOf('0');
            var j = -1;

            if (indexHash >= 0 && indexZero >= 0)
            {
                j = Math.Min(indexHash, indexZero);
            }
            else if (indexHash >= 0)
            {
                j = indexHash;
            }
            else if (indexZero >= 0)
            {
                j = indexZero;
            }

            return j;
        }

        internal static string WithNumberFormatting(string rawResult, float n)
        {
            var j = GetStartIndexOfNumberFormat(rawResult);
            if (j < 0)
            {
                return rawResult;
            } 

            string partA = rawResult.Substring(0, j);
            string numberFormatting = GetNumberFormattingFrom(rawResult, out string partB);

            string number;
            try
            {
                if (numberFormatting.Length == 1 && n == 0) number = "0";
                else number = n.ToString(numberFormatting, GlobalConstants.DefaultCultureInfo);
            }
            catch (Exception) { number = n.ToString(GlobalConstants.DefaultCultureInfo); }      // Fallback if the translators gave us a badly formatted number string

            return partA + number + partB;
        }

        /// <summary>
        /// Gets a translation for a given key, if any matching wildcarded keys are found within the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The arguments to interpolate into the resulting string.</param>
        /// <returns>
        ///     Returns the key as a default value, if no results are found; otherwise returns the pre-formatted, translated
        ///     value.
        /// </returns>
        public string GetIfExists(string key, params object[] args)
        {
            EnsureLoaded();
            return entryCache.TryGetValue(KeyWithDomain(key), out var value)
                ? Format(value, args)
                : null;
        }

        /// <summary>
        /// Gets a translation for a given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The arguments to interpolate into the resulting string.</param>
        /// <returns>
        ///     Returns the key as a default value, if no results are found; otherwise returns the pre-formatted, translated
        ///     value.
        /// </returns>
        public string Get(string key, params object[] args)
        {
              return Format(GetUnformatted(key), args);   // There will be a cacheLock and EnsureLoaded inside the called method GetUnformatted
        }

        /// <summary>
        /// Retrieves a list of all translation entries within the cache.
        /// </summary>
        /// <returns>A dictionary of localisation entries.</returns>
        public IDictionary<string, string> GetAllEntries()
        {
            EnsureLoaded();
            return entryCache;
        }

        /// <summary>
        /// Gets the raw, unformatted translated value for the key provided.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///     Returns the key as a default value, if no results are found; otherwise returns the unformatted, translated
        ///     value.
        /// </returns>
        public string GetUnformatted(string key)
        {
            EnsureLoaded();
            bool found = entryCache.TryGetValue(KeyWithDomain(key), out var value);
            return found ? value : key;
        }

        /// <summary>
        /// Gets a translation for a given key, if any matching wildcarded keys are found within the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The arguments to interpolate into the resulting string.</param>
        /// <returns>
        /// Returns the key as a default value, if no results are found; otherwise returns the pre-formatted, translated
        /// value.
        /// </returns>
        public string GetMatching(string key, params object[] args)
        {
            EnsureLoaded();
            var value = GetMatchingIfExists(KeyWithDomain(key), args);
            return string.IsNullOrEmpty(value)
                ? Format(key, args)
                : value;
        }

        /// <summary>
        /// Determines whether the specified key has a translation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="findWildcarded">if set to <c>true</c>, the scan will include any wildcarded values.</param>
        /// <returns><c>true</c> if the specified key has a translation; otherwise, <c>false</c>.</returns>
        public bool HasTranslation(string key, bool findWildcarded = true)
        {
            return HasTranslation(key, findWildcarded, true);
        }

        /// <summary>
        /// Determines whether the specified key has a translation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="findWildcarded">if set to <c>true</c>, the scan will include any wildcarded values.</param>
        /// <param name="logErrors">if set to <c>true</c>, will add "Lang key not found" logging</param>
        /// <returns><c>true</c> if the specified key has a translation; otherwise, <c>false</c>.</returns>
        public bool HasTranslation(string key, bool findWildcarded, bool logErrors)
        {
            EnsureLoaded();
            var validKey = KeyWithDomain(key);
            if (entryCache.ContainsKey(validKey)) return true;
            if (findWildcarded)
            {
                if (!key.Contains(":")) key = "game:" + key;
                bool result = wildcardCache.Any(pair => key.StartsWithFast(pair.Key));
                if (!result) result = regexCache.Values.Any(pair => pair.Key.IsMatch(validKey));
                if (!result && logErrors && !key.Contains("desc-") && notFound.Add(key)) logger.VerboseDebug("Lang key not found: " + key.Replace("{", "{{").Replace("}", "}}"));
                return result;
            }
            return false;
        }

        /// <summary>
        /// Specifies an asset manager to use, when the service has been lazy-loaded.
        /// </summary>
        /// <param name="assetManager">The <see cref="IAssetManager" /> instance used within the sided API.</param>
        public void UseAssetManager(IAssetManager assetManager)
        {
            this.assetManager = assetManager;
        }

        /// <summary>
        /// Gets a translation for a given key, if any matching wildcarded keys are found within the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The arguments to interpolate into the resulting string.</param>
        /// <returns>
        /// Returns <c>null</c> as a default value, if no results are found; otherwise returns the pre-formatted, translated value.
        /// </returns>
        public string GetMatchingIfExists(string key, params object[] args)
        {
            EnsureLoaded();
            var validKey = KeyWithDomain(key);

            if (entryCache.TryGetValue(validKey, out var value)) return Format(value, args);

            foreach (var pair in wildcardCache
                .Where(pair => validKey.StartsWithFast(pair.Key)))
                return Format(pair.Value, args);

            return regexCache.Values
                .Where(pair => pair.Key.IsMatch(validKey))
                .Select(pair => Format(pair.Value, args))
                .FirstOrDefault();
        }

        private void LoadEntries(Dictionary<string, string> entryCache, Dictionary<string, KeyValuePair<Regex, string>> regexCache, Dictionary<string, string> wildcardCache, Dictionary<string, string> entries, string domain = GlobalConstants.DefaultDomain)
        {
            foreach (var entry in entries)
            {
                LoadEntry(entryCache, regexCache, wildcardCache, entry, domain);
            }
        }

        private void LoadEntry(Dictionary<string, string> entryCache, Dictionary<string, KeyValuePair<Regex, string>> regexCache, Dictionary<string, string> wildcardCache, KeyValuePair<string, string> entry, string domain = GlobalConstants.DefaultDomain)
        {
            var key = KeyWithDomain(entry.Key, domain);
            switch (key.CountChars('*'))
            {
                case 0:
                    entryCache[key] = entry.Value;
                    break;
                case 1 when key.EndsWith('*'):
                    wildcardCache[key.TrimEnd('*')] = entry.Value;
                    break;
                    // we can probably do better here, as we have our own wildcardsearch now
                default:
                {
                    var regex = new Regex("^" + key.Replace("*", "(.*)") + "$", RegexOptions.Compiled);
                    regexCache[key] = new KeyValuePair<Regex, string>(regex, entry.Value);
                    break;
                }
            }
        }

        private static string KeyWithDomain(string key, string domain = GlobalConstants.DefaultDomain)
        {
            if (key.Contains(AssetLocation.LocationSeparator)) return key;
            return new StringBuilder(domain)
                .Append(AssetLocation.LocationSeparator)
                .Append(key)
                .ToString();
        }

        public void InitialiseSearch()
        {
            regexCache.Values.Any(pair => pair.Key.IsMatch("nonsense_value_and_fairly_longgg"));   // Force compilation of all the regexCache keys on first use
        }
    }
}
