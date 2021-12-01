﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Util;


// Contributed by Apache#8842 over discord 20th of October 2021. Edited by Tyron
// Apache — Today at 11:45 AM
// If you want to use it, it's under your license... I made it to be added to the game. ITranslationService is so that it can be mocked within your Test Suite, added to an IOC Container, extended via mods, etc. Interfaces should be used, in favour of concrete classes, in  most cases. Especially where you have volatile code such as IO.


namespace Vintagestory.API.Config
{
    /// <summary>
    /// A service, which provides access to translated strings, based on key/value pairs read from JSON files.
    /// </summary>
    /// <seealso cref="ITranslationService" />
    public class TranslationService : ITranslationService
    {
        private readonly Dictionary<string, string> entryCache = new Dictionary<string, string>();

        // This is needed because in singleplayer the server and client access the same Lang dicts, so we need to prevent race conditions
        private object cacheLock = new object();

        private readonly Dictionary<string, KeyValuePair<Regex, string>> regexCache = new Dictionary<string, KeyValuePair<Regex, string>>();

        private readonly Dictionary<string, string> wildcardCache = new Dictionary<string, string>();

        private IAssetManager assetManager;

        private readonly ILogger logger;

        private bool loaded = false;
        private string preLoadAssetsPath = null;


        /// <summary>
        ///     Initialises a new instance of the <see cref="TranslationService" /> class.
        /// </summary>
        /// <param name="languageCode">The language code that this translation service caters for.</param>
        /// <param name="logger">The <see cref="ILogger" /> instance used within the sided API.</param>
        /// <param name="assetManager">The <see cref="IAssetManager" /> instance used within the sided API.</param>
        public TranslationService(string languageCode, ILogger logger, IAssetManager assetManager = null)
        {
            LanguageCode = languageCode;
            this.logger = logger;
            this.assetManager = assetManager;
        }

        /// <summary>
        ///     Gets the language code that this translation service caters for.
        /// </summary>
        /// <value>A string, that contains the language code that this translation service caters for.</value>
        public string LanguageCode { get; }

        /// <summary>
        ///     Loads translation key/value pairs from all relevant JSON files within the Asset Manager.
        /// </summary>
        public void Load(bool lazyLoad = false)
        {
            preLoadAssetsPath = null;
            if (lazyLoad) return;

            lock (cacheLock)
            {
                loaded = true;
                entryCache.Clear();
                regexCache.Clear();
                wildcardCache.Clear();
                var origins = assetManager.Origins;
                foreach (var asset in origins.SelectMany(p => p.GetAssets(AssetCategory.lang).Where(a => a.Name.Equals($"{LanguageCode}.json"))))

                    try
                    {
                        var json = asset.ToText();
                        LoadEntries(JsonConvert.DeserializeObject<Dictionary<string, string>>(json));
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Failed to load language file: {asset.Name} \n\n\t {ex}");
                    }
            }
        }

        /// <summary>
        ///     Loads only the vanilla JSON files, without dealing with mods, or resource-packs.
        /// </summary>
        /// <param name="assetsPath">The root assets path to load the vanilla files from.</param>
        public void PreLoad(string assetsPath, bool lazyLoad = false)
        {
            preLoadAssetsPath = assetsPath;
            if (lazyLoad) return;

            lock (cacheLock)
            {
                loaded = true;
                var assetsDirectory = new DirectoryInfo(Path.Combine(assetsPath, GlobalConstants.DefaultDomain, "lang"));
                var files = assetsDirectory.EnumerateFiles($"{LanguageCode}.json", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    try
                    {
                        var json = File.ReadAllText(file.FullName);
                        LoadEntries(JsonConvert.DeserializeObject<Dictionary<string, string>>(json));
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Failed to load language file: {file.Name} \n\n\t {ex}");
                    }
                }
            }
        }

        protected void EnsureLoaded()
        {
            if (!loaded)
            {
                if (preLoadAssetsPath != null) PreLoad(preLoadAssetsPath);
                else Load();
            }
        }

        /// <summary>
        ///     Gets a translation for a given key, if any matching wildcarded keys are found within the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The arguments to interpolate into the resulting string.</param>
        /// <returns>
        ///     Returns the key as a default value, if no results are found; otherwise returns the pre-formatted, translated
        ///     value.
        /// </returns>
        public string GetIfExists(string key, params object[] args)
        {
            lock (cacheLock)
            {
                EnsureLoaded();
                return entryCache.TryGetValue(KeyWithDomain(key), out var value)
                    ? string.Format(value, args)
                    : null;
            }
        }

        /// <summary>
        ///     Gets a translation for a given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The arguments to interpolate into the resulting string.</param>
        /// <returns>
        ///     Returns the key as a default value, if no results are found; otherwise returns the pre-formatted, translated
        ///     value.
        /// </returns>
        public string Get(string key, params object[] args)
        {
            lock (cacheLock)
            {
                EnsureLoaded();
                return string.Format(GetUnformatted(key), args);
            }
        }

        /// <summary>
        ///     Retrieves a list of all translation entries within the cache.
        /// </summary>
        /// <returns>A dictionary of localisation entries.</returns>
        public IDictionary<string, string> GetAllEntries()
        {
            lock (cacheLock)
            {
                EnsureLoaded();
                return entryCache;
            }
        }

        /// <summary>
        ///     Gets the raw, unformatted translated value for the key provided.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///     Returns the key as a default value, if no results are found; otherwise returns the unformatted, translated
        ///     value.
        /// </returns>
        public string GetUnformatted(string key)
        {
            lock (cacheLock)
            {
                EnsureLoaded();
                return entryCache.TryGetValue(KeyWithDomain(key), out var value) ? value : key;
            }
        }

        /// <summary>
        ///     Gets a translation for a given key, if any matching wildcarded keys are found within the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The arguments to interpolate into the resulting string.</param>
        /// <returns>
        ///     Returns the key as a default value, if no results are found; otherwise returns the pre-formatted, translated
        ///     value.
        /// </returns>
        public string GetMatching(string key, params object[] args)
        {
            lock (cacheLock)
            {
                EnsureLoaded();
                var value = GetMatchingIfExists(KeyWithDomain(key), args);
                return string.IsNullOrEmpty(value)
                    ? string.Format(key, args)
                    : value;
            }
        }

        /// <summary>
        ///     Determines whether the specified key has a translation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="findWildcarded">if set to <c>true</c>, the scan will include any wildcarded values.</param>
        /// <returns><c>true</c> if the specified key has a translation; otherwise, <c>false</c>.</returns>
        public bool HasTranslation(string key, bool findWildcarded = true)
        {
            lock (cacheLock)
            {
                EnsureLoaded();
                var validKey = KeyWithDomain(key);
                if (entryCache.ContainsKey(validKey)) return true;
                if (findWildcarded)
                    return wildcardCache.Any(pair => StringUtil.FastStartsWith(key, pair.Key))
                           || regexCache.Values.Any(pair => pair.Key.IsMatch(validKey));
                return false;
            }
        }

        /// <summary>
        ///     Specifies an asset manager to use, when the service has been lazy-loaded.
        /// </summary>
        /// <param name="assetManager">The <see cref="IAssetManager" /> instance used within the sided API.</param>
        public void UseAssetManager(IAssetManager assetManager)
        {
            this.assetManager = assetManager;
        }

        /// <summary>
        ///     Gets a translation for a given key, if any matching wildcarded keys are found within the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The arguments to interpolate into the resulting string.</param>
        /// <returns>
        ///     Returns <c>null</c> as a default value, if no results are found; otherwise returns the pre-formatted,
        ///     translated value.
        /// </returns>
        public string GetMatchingIfExists(string key, params object[] args)
        {
            EnsureLoaded();
            var validKey = KeyWithDomain(key);

            if (entryCache.TryGetValue(validKey, out var value)) return string.Format(value, args);

            foreach (var pair in wildcardCache
                .Where(pair => StringUtil.FastStartsWith(validKey, pair.Key)))
                return string.Format(pair.Value, args);

            return regexCache.Values
                .Where(pair => pair.Key.IsMatch(validKey))
                .Select(pair => string.Format(pair.Value, args))
                .FirstOrDefault();
        }

        private void LoadEntries(Dictionary<string, string> entries, string domain = GlobalConstants.DefaultDomain)
        {
            foreach (var entry in entries) LoadEntry(entry, domain);
        }

        private void LoadEntry(KeyValuePair<string, string> entry, string domain = GlobalConstants.DefaultDomain)
        {
            var key = KeyWithDomain(entry.Key, domain);
            switch (key.CountChars('*'))
            {
                case 0:
                    entryCache[key] = entry.Value;
                    break;
                case 1 when key.EndsWith("*"):
                    wildcardCache[key.TrimEnd('*')] = entry.Value;
                    break;
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
    }
}