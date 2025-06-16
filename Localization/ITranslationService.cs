using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

#nullable disable


// Contributed by Apache#8842 over discord 20th of October 2021. Edited by Tyron
// Apache — Today at 11:45 AM
// If you want to use it, it's under your license... I made it to be added to the game. ITranslationService is so that it can be mocked within your Test Suite, added to an IOC Container, extended via mods, etc. Interfaces should be used, in favour of concrete classes, in  most cases. Especially where you have volatile code such as IO.

namespace Vintagestory.API.Config
{
    /// <summary>
    ///     Represents a service, which provides access to translated strings, based on key/value pairs read from JSON files.
    /// </summary>
    public interface ITranslationService
    {
        EnumLinebreakBehavior LineBreakBehavior { get; }

        /// <summary>
        ///     Gets the language code that this translation service caters for.
        /// </summary>
        /// <value>A string, that contains the language code that this translation service caters for.</value>
        string LanguageCode { get; }

        /// <summary>
        ///     Loads translation key/value pairs from all relevant JSON files within the Asset Manager.
        /// </summary>
        void Load(bool lazyload = false);

        /// <summary>
        ///     Loads only the vanilla JSON files, without dealing with mods, or resource-packs.
        /// </summary>
        /// <param name="assetsPath">The root assets path to load the vanilla files from.</param>
        /// <param name="lazyLoad"></param>
        void PreLoad(string assetsPath, bool lazyLoad = false);

        /// <summary>
        ///     Loads the mod worldconfig language JSON files only.
        /// </summary>
        /// <param name="modPath">The assets path to load the mod files from.</param>
        /// <param name="modDomain">The mod domain to use when loading the files.</param>
        /// <param name="lazyLoad"></param>
        void PreLoadModWorldConfig(string modPath, string modDomain, bool lazyLoad = false);

        /// <summary>
        ///     Gets a translation for a given key, if any matching wildcarded keys are found within the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The arguments to interpolate into the resulting string.</param>
        /// <returns>Returns null if no results are found; otherwise returns the pre-formatted, translated value.</returns>
        string GetIfExists(string key, params object[] args);

        /// <summary>
        ///     Gets a translation for a given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The arguments to interpolate into the resulting string.</param>
        /// <returns>Returns the key if no results are found; otherwise returns the pre-formatted, translated value.</returns>
        string Get(string key, params object[] args);

        /// <summary>
        ///     Retrieves a list of all translation entries within the cache.
        /// </summary>
        /// <returns>A dictionary of localisation entries.</returns>
        IDictionary<string, string> GetAllEntries();

        /// <summary>
        ///     Gets the raw, unformatted translated value for the key provided.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns the key if no results are found; otherwise returns the unformatted, translated value.</returns>
        string GetUnformatted(string key);

        /// <summary>
        ///     Gets a translation for a given key, if any matching wildcarded keys are found within the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The arguments to interpolate into the resulting string.</param>
        /// <returns>Returns the key if no results are found; otherwise returns the pre-formatted, translated value.</returns>
        string GetMatching(string key, params object[] args);

        /// <summary>
        ///     Gets a translation for a given key, if any matching wildcarded keys are found within the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The arguments to interpolate into the resulting string.</param>
        /// <returns>Returns <c>null</c> if no results are found; otherwise returns the pre-formatted, translated value.</returns>
        string GetMatchingIfExists(string key, params object[] args);

        /// <summary>
        ///     Determines whether the specified key has a translation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="findWildcarded">if set to <c>true</c>, the scan will include any wildcarded values.</param>
        /// <returns><c>true</c> if the specified key has a translation; otherwise, <c>false</c>.</returns>
        bool HasTranslation(string key, bool findWildcarded = true);

        /// <summary>
        ///     Determines whether the specified key has a translation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="findWildcarded">if set to <c>true</c>, the scan will include any wildcarded values.</param>
        /// <param name="logErrors">if set to <c>true</c>, will add "Lang key not found" logging</param>
        /// <returns><c>true</c> if the specified key has a translation; otherwise, <c>false</c>.</returns>
        bool HasTranslation(string key, bool findWildcarded, bool logErrors);

        /// <summary>
        ///     Specifies an asset manager to use, when the service has been lazy-loaded.
        /// </summary>
        /// <param name="assetManager">The <see cref="IAssetManager"/> instance used within the sided API.</param>
        void UseAssetManager(IAssetManager assetManager);

        /// <summary>
        ///      Used to compile the regexes, to save time on the first 'actual' wildcard search - saves about 300ms
        /// </summary>
        void InitialiseSearch();

        /// <summary>
        /// Sets the loaded flag to false, so that the next lookup causes it to reload all translation entries
        /// </summary>
        void Invalidate();
    }
}
