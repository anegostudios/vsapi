using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;

#nullable disable


// Rewrite contributed by Apache#8842 over discord 20th of October 2021. Edited by Tyron
// Apache — Today at 11:45 AM
// If you want to use it, it's under your license... I made it to be added to the game. ITranslationService is so that it can be mocked within your Test Suite, added to an IOC Container, extended via mods, etc. Interfaces should be used, in favour of concrete classes, in  most cases. Especially where you have volatile code such as IO.


namespace Vintagestory.API.Config
{
    /// <summary>
    /// Utility class for enabling i18n. Loads language entries from assets/[locale].json
    /// </summary>
    /// <remarks>
    /// Kept legacy code structure and arguments for backwards compatibility.
    /// </remarks>
    public static class Lang
    {
        public static Dictionary<string, ITranslationService> AvailableLanguages { get; } = new Dictionary<string, ITranslationService>();

        /// <summary>
        /// Gets the language code that this currently used to translate values.
        /// </summary>
        /// <value>A string, that contains the language code that this currently used to translate values.</value>
        public static string CurrentLocale { get; private set; }
        public static string DefaultLocale { get; set; } = "en";


        /// <summary>
        /// Loads all translations
        /// </summary>
        /// <param name="logger">The <see cref="ILogger" /> instance used within the sided API.</param>
        /// <param name="assetManager">The <see cref="IAssetManager" /> instance used within the sided API.</param>
        /// <param name="language">The desired language</param>
        public static void Load(ILogger logger, IAssetManager assetManager, string language = "en")
        {
            CurrentLocale = language;

            var languageFile = Path.Combine(GamePaths.AssetsPath, "game", "lang", "languages.json");
            var json = JsonObject.FromJson(File.ReadAllText(languageFile)).AsArray();

            foreach (var jsonObject in json)
            {
                var languageCode = jsonObject["code"].AsString();
                EnumLinebreakBehavior lbBehavior = (EnumLinebreakBehavior)Enum.Parse(typeof(EnumLinebreakBehavior), jsonObject["linebreakBehavior"].AsString("AfterWord"));

                LoadLanguage(logger, assetManager, languageCode, languageCode != CurrentLocale, lbBehavior);
            }

            if (!AvailableLanguages.ContainsKey(language))
            {
                logger.Error("Language '{0}' not found. Will default to english.", language);
                CurrentLocale = "en";
            }
        }

        /// <summary>
        /// Changes the current language for the game.
        /// </summary>
        /// <param name="languageCode">The language code to set as the language for the game.</param>
        public static void ChangeLanguage(string languageCode)
        {
            CurrentLocale = languageCode;
        }

        /// <summary>
        /// Loads translation key/value pairs from all relevant JSON files within the Asset Manager.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger" /> instance used within the sided API.</param>
        /// <param name="assetManager">The <see cref="IAssetManager" /> instance used within the sided API.</param>
        /// <param name="languageCode">The language code to use as the default language.</param>
        /// <param name="lazyLoad"></param>
        /// <param name="lbBehavior"></param>
        public static void LoadLanguage(ILogger logger, IAssetManager assetManager, string languageCode = "en", bool lazyLoad = false, EnumLinebreakBehavior lbBehavior = EnumLinebreakBehavior.AfterWord)
        {
            if (AvailableLanguages.ContainsKey(languageCode))
            {
                AvailableLanguages[languageCode].UseAssetManager(assetManager);
                AvailableLanguages[languageCode].Load(lazyLoad);
                return;
            }
            var translationService = new TranslationService(languageCode, logger, assetManager, lbBehavior);
            translationService.Load(lazyLoad);
            AvailableLanguages.Add(languageCode, translationService);
        }

        /// <summary>
        /// Loads only the vanilla JSON files, without dealing with mods, or resource-packs.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger" /> instance used within the sided API.</param>
        /// <param name="assetsPath">The root assets path to load the vanilla files from.</param>
        /// <param name="defaultLanguage">The language code to use as the default language.</param>
        public static void PreLoad(ILogger logger, string assetsPath, string defaultLanguage = "en")
        {
            CurrentLocale = defaultLanguage;
            var languageFile = Path.Combine(GamePaths.AssetsPath, "game", "lang", "languages.json");
            var json = JsonObject.FromJson(File.ReadAllText(languageFile)).AsArray();
            bool found = false;

            foreach (var jsonObject in json)
            {
                var languageCode = jsonObject["code"].AsString();
                EnumLinebreakBehavior lbBehavior = (EnumLinebreakBehavior)Enum.Parse(typeof(EnumLinebreakBehavior), jsonObject["linebreakBehavior"].AsString("AfterWord"));

                var translationService = new TranslationService(languageCode, logger, null, lbBehavior);
                bool lazyLoad = languageCode != defaultLanguage;
                translationService.PreLoad(assetsPath, lazyLoad);
                AvailableLanguages[languageCode] = translationService;

                if (languageCode == defaultLanguage) found = true;
            }

            if (defaultLanguage != "en" && !found)
            {
                logger.Error("Language '{0}' not found. Will default to english.", defaultLanguage);
                AvailableLanguages["en"].PreLoad(assetsPath);
                CurrentLocale = "en";
            }
        }

        /// <summary>
        /// Loads the mod worldconfig language JSON files only.
        /// </summary>
        /// <param name="modPath">The assets path to load the mod files from.</param>
        /// <param name="modDomain">The mod domain to use when loading the files.</param>
        /// <param name="defaultLanguage">The language code to use as the default language.</param>
        public static void PreLoadModWorldConfig(string modPath, string modDomain, string defaultLanguage = "en")
        {
            var languageFile = Path.Combine(GamePaths.AssetsPath, "game", "lang", "languages.json");
            var json = JsonObject.FromJson(File.ReadAllText(languageFile)).AsArray();

            foreach (var jsonObject in json)
            {
                var languageCode = jsonObject["code"].AsString();
                EnumLinebreakBehavior lbBehavior = (EnumLinebreakBehavior)Enum.Parse(typeof(EnumLinebreakBehavior), jsonObject["linebreakBehavior"].AsString("AfterWord"));

                bool lazyLoad = languageCode != defaultLanguage;
                AvailableLanguages[languageCode].PreLoadModWorldConfig(modPath, modDomain, lazyLoad);
            }
        }

        /// <summary>
        /// Gets a translation entry for given key, if any matching wildcarded keys are found within the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The arguments to interpolate into the resulting string.</param>
        /// <returns>Returns the key as a default value, if no results are found; otherwise returns the pre-formatted, translated value.</returns>
        public static string GetIfExists(string key, params object[] args)
        {
            return HasTranslation(key)
                ? AvailableLanguages[CurrentLocale].GetIfExists(key, args)
                : AvailableLanguages[DefaultLocale].GetIfExists(key, args);
        }

        /// <summary>
        /// Gets a translation entry for given key using given locale
        /// </summary>
        /// <param name="langcode"></param>
        /// <param name="key"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GetL(string langcode, string key, params object[] args)
        {
            return AvailableLanguages.TryGetValue(langcode, out var language) && language.HasTranslation(key, false)
                ? language.Get(key, args)
                : AvailableLanguages[DefaultLocale].Get(key, args);
        }

        public static string GetMatchingL(string langcode, string key, params object[] args)
        {
            return AvailableLanguages.TryGetValue(langcode, out var language) && language.HasTranslation(key)
                ? language.GetMatching(key, args)
                : AvailableLanguages[DefaultLocale].GetMatching(key, args);
        }

        /// <summary>
        /// Gets a translation entry for given key using the current locale
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The arguments to interpolate into the resulting string.</param>
        /// <returns>Returns the key as a default value, if no results are found; otherwise returns the pre-formatted, translated value.</returns>
        public static string Get(string key, params object[] args)
        {
            return HasTranslation(key, false)
                ? AvailableLanguages[CurrentLocale].Get(key, args)
                : AvailableLanguages[DefaultLocale].Get(key, args);
        }

        /// <summary>
        /// Gets a translation entry for given key using the current locale. Also tries the fallback key - useful if we change a key but not all languages updated yet
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fallbackKey"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GetWithFallback(string key, string fallbackKey, params object[] args)
        {
            return HasTranslation(key, false) ? AvailableLanguages[CurrentLocale].Get(key, args) :
                HasTranslation(fallbackKey, false) ? AvailableLanguages[CurrentLocale].Get(fallbackKey, args) :
                AvailableLanguages[DefaultLocale].Get(key, args);
        }

        /// <summary>
        /// Gets the raw, unformatted translated value for the key provided.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns the key as a default value, if no results are found; otherwise returns the unformatted, translated value.</returns>
        public static string GetUnformatted(string key)
        {
            return HasTranslation(key)
                ? AvailableLanguages[CurrentLocale].GetUnformatted(key)
                : AvailableLanguages[DefaultLocale].GetUnformatted(key);
        }

        /// <summary>
        /// Gets a translation for a given key, if any matching wildcarded keys are found within the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The arguments to interpolate into the resulting string.</param>
        /// <returns>Returns the key as a default value, if no results are found; otherwise returns the pre-formatted, translated value.</returns>
        public static string GetMatching(string key, params object[] args)
        {
            return HasTranslation(key)
                ? AvailableLanguages[CurrentLocale].GetMatching(key, args)
                : AvailableLanguages[DefaultLocale].GetMatching(key, args);
        }

        /// <summary>
        /// Gets a translation entry for given key, if any matching wildcarded keys are found within the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The arguments to interpolate into the resulting string.</param>
        /// <returns>Returns <c>null</c> as a default value, if no results are found; otherwise returns the pre-formatted, translated value.</returns>
        public static string GetMatchingIfExists(string key, params object[] args)
        {
            return HasTranslation(key, true, false)
                ? AvailableLanguages[CurrentLocale].GetMatchingIfExists(key, args)
                : AvailableLanguages[DefaultLocale].GetMatchingIfExists(key, args);
        }

        /// <summary>
        /// Retrieves a list of all translation entries within the cache.
        /// </summary>
        /// <returns>A dictionary of localisation entries.</returns>
        public static IDictionary<string, string> GetAllEntries()
        {
            var defaultEntries = AvailableLanguages[DefaultLocale].GetAllEntries().ToDictionary(p => p.Key, p => p.Value);
            var currentEntries = AvailableLanguages[CurrentLocale].GetAllEntries().ToDictionary(p => p.Key, p => p.Value);
            foreach (var entry in defaultEntries.Where(entry => !currentEntries.ContainsKey(entry.Key)))
            {
                currentEntries.Add(entry.Key, entry.Value);
            }
            return currentEntries;
        }

        /// <summary>
        /// Determines whether the specified key has a translation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="findWildcarded">if set to <c>true</c>, the scan will include any wildcarded values.</param>
        /// <param name="logErrors"></param>
        /// <returns><c>true</c> if the specified key has a translation; otherwise, <c>false</c>.</returns>
        public static bool HasTranslation(string key, bool findWildcarded = true, bool logErrors = true)
        {
            return AvailableLanguages[CurrentLocale].HasTranslation(key, findWildcarded, logErrors);
        }

        public static void InitialiseSearch()
        {
            AvailableLanguages[CurrentLocale].InitialiseSearch();
        }


        public static string GetNamePlaceHolder(AssetLocation code)
        {
            string hint = "";

            string[] parts = code.Path.Split('-');
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] == "north" || parts[i] == "east" || parts[i] == "west" || parts[i] == "south" || parts[i] == "up" || parts[i] == "down") continue;

                if (i > 0) hint += " ";

                if (i > 0 && i == parts.Length - 1) hint += "(";
                if (i == 0)
                {
                    hint += parts[i].First().ToString().ToUpper() + parts[i].Substring(1);
                } else
                {
                    hint += parts[i];
                }

                if (i > 0 && i == parts.Length - 1) hint += ")";
            }

            return hint;
        }

        public static bool UsesNonLatinCharacters(string lang)
        {
            return 
                lang == "ar" ||                                     // Arabic
                lang.StartsWithOrdinal("zh-") ||                    // Chinese, both kinds
                lang == "ja" || lang == "ko" || lang == "th" ||     // Other Asian languages (Japanese, Korean, Thai)
                lang == "uk" || lang == "ru";                       // Cyrillic   (Ukrainian, Russian)

            // 1.20.0: This test will need to be expanded in future if we add more languages including Greek, Bulgarian, etc.
            // It's relevant, for example, to the localization of the room plaques in the Resonance Archives - in non-Latin characters these don't work in the saved font, Almendra: Github #4748
        }
    }
}
