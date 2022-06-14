using System;
using System.Collections.Generic;
using System.IO.Compression;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;

namespace Vintagestory.API.Common
{
    /*/// <summary>
    /// Callback for when assets have been loaded. Can be registered with through the IAssetManager.
    /// </summary>
    /// <param name="assets"></param>
    public delegate void AfterAssetFolderLoaded(List<IAsset> assets);*/

    /// <summary>
    /// Takes care loading, reloading and managing all files inside the assets folder. All asset names and paths are always converted to lower case.
    /// </summary>
    public interface IAssetManager
    {
        /// <summary>
        /// All assets found in the assets folder
        /// </summary>
        Dictionary<AssetLocation, IAsset> AllAssets { get; }

        /// <summary>
        /// Returns true if given asset exists in the list of loaded assets
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        bool Exists(AssetLocation location);

        /// <summary>
        /// Retrieves an asset from given path within the assets folder. Throws an exception when the asset does not exist. Remember to use lower case paths.
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        IAsset Get(string Path);

        /// <summary>
        /// Retrieves an asset from given path within the assets folder. Throws an exception when the asset does not exist. Remember to use lower case paths.
        /// </summary>
        /// <param name="Location"></param>
        /// <returns></returns>
        IAsset Get(AssetLocation Location);

        /// <summary>
        /// Retrieves an asset from given path within the assets folder. Returns null when the asset does not exist. Remember to use lower case paths.
        /// <br/><br/> Mods must not call TryGet to get assets before AssetsLoaded stage in a ModSystem - do not load assets in the Start() method!  In StartClientSide() is OK though.  (Or if you absolutely have to load assets in Start(), use IAssetManager.Get(), but it will throw an exception for anything except a base asset.)
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="loadAsset"></param>
        /// <returns></returns>
        IAsset TryGet(string Path, bool loadAsset = true);

        /// <summary>
        /// Retrieves an asset from given path within the assets folder. Returns null when the asset does not exist. Remember to use lower case paths.
        /// <br/><br/> Mods must not call TryGet to get assets before AssetsLoaded stage in a ModSystem - do not load assets in the Start() method!  (Or if you absolutely have to load assets in Start(), use IAssetManager.Get(), but it will throw an exception for anything except a base asset.)
        /// </summary>
        /// <param name="Location"></param>
        /// <param name="loadAsset"></param>
        /// <returns></returns>
        IAsset TryGet(AssetLocation Location, bool loadAsset = true);

        /// <summary>
        /// Returns all assets inside given category with the given path. If no domain is specified, all domains will be searched. The returned list is considered unsorted.
        /// </summary>
        /// <param name="pathBegins"></param>
        /// <param name="domain"></param>
        /// <param name="loadAsset">Whether it should load the contents of this asset</param>
        /// <returns></returns>
        List<IAsset> GetMany(string pathBegins, string domain = null, bool loadAsset = true);

        List<IAsset> GetManyInCategory(string categoryCode, string pathBegins, string domain = null, bool loadAsset = true);

        /// <summary>
        /// Searches for all assets in given basepath and uses JSON.NET to automatically turn them into objects. Will log an error to given ILogger if it can't parse the json file and continue with the next asset. Remember to use lower case paths. If no domain is specified, all domains will be searched.
        /// The returned list is considered unsorted.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="pathBegins"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        Dictionary<AssetLocation, T> GetMany<T>(ILogger logger, string pathBegins, string domain = null);



        /// <summary>
        /// Returns all asset locations that begins with given path and domain. If no domain is specified, all domains will be searched. The returned list is considered unsorted.
        /// </summary>
        /// <param name="pathBegins"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        List<AssetLocation> GetLocations(string pathBegins, string domain = null);


        /// <summary>
        /// Retrieves an asset from given path within the assets folder and uses JSON.NET to automatically turn them into objects. Throws an exception when the asset does not exist or the conversion failed. Remember to use lower case paths.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="location"></param>
        /// <returns></returns>
        T Get<T>(AssetLocation location);

        /// <summary>
        /// Reloads all assets in given base location path. It returns the amount of the found locations.
        /// </summary>
        /// <param name="basePath"></param>
        /// <returns></returns>
        int Reload(AssetLocation baseLocation);

        /// <summary>
        /// Reloads all assets in given base location path. It returns the amount of the found locations.
        /// </summary>
        /// <param name="basePath"></param>
        /// <returns></returns>
        int Reload(AssetCategory category);

        /// <summary>
        /// Returns all origins in the priority order. Highest (First) to Lowest (Last)
        /// </summary>
        /// <returns></returns>
        List<IAssetOrigin> Origins { get; }


        [Obsolete("Use AddModOrigin")]
        void AddPathOrigin(string domain, string fullPath);
        void AddModOrigin(string domain, string fullPath);
    }
}
