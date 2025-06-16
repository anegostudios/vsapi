using Newtonsoft.Json;
using Vintagestory.API.Client;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Represents a loaded asset from the assets folder
    /// </summary>
    public interface IAsset
    {

        /// <summary>
        /// The assets Filename 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The AssetLocation of the asset.
        /// </summary>
        AssetLocation Location { get; }

        /// <summary>
        /// The origin informaton of the asset.
        /// </summary>
        IAssetOrigin Origin { get; set; }

        /// <summary>
        /// The file contents in binary format
        /// </summary>
        byte[] Data { get; set; }

        /// <summary>
        /// If the asset is a json file you can use this convenience method to turn it into an object
        /// </summary>
        /// <typeparam name="T">Attempts to convert the asset into the given type.</typeparam>
        /// <param name="settings">Settings for the Json Serializer.</param>
        /// <returns></returns>
        T ToObject<T>(JsonSerializerSettings settings = null);

        /// <summary>
        /// Turns the binary data into a UTF-8 string. Use for text files.
        /// </summary>
        /// <returns></returns>
        string ToText();

        /// <summary>
        /// Turns the binary data into a Bitmap. Use for .png images. Does not work on other image formats.
        /// </summary>
        /// <param name="capi"></param>
        /// <returns></returns>
        BitmapRef ToBitmap(ICoreClientAPI capi);

        /// <summary>
        /// Whether or not the asset is currently loaded.
        /// </summary>
        /// <returns></returns>
        bool IsLoaded();

        /// <summary>
        /// Set to true if the asset has been patched by JsonPatchLoader - if so, we don't want to unload it
        /// </summary>
        bool IsPatched { get; set; }
    }
}
