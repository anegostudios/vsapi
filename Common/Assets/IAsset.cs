using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Client;

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

        
        AssetLocation Location { get; }

        IAssetOrigin Origin { get; set; }

        /// <summary>
        /// The file contents in binary format
        /// </summary>
        byte[] Data { get; set; }

        /// <summary>
        /// If the asset is a json file you can use this convenience method to turn it into an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settings"></param>
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
        /// <param name="platform"></param>
        /// <returns></returns>
        BitmapRef ToBitmap(ICoreClientAPI capi);

        bool IsLoaded();
    }
}
