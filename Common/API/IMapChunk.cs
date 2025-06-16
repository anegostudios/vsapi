using System;
using System.Collections.Concurrent;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

#nullable disable

namespace Vintagestory.API.Common
{
    public interface IServerMapChunk : IMapChunk
    {
        
    }

    /// <summary>
    /// Holds 2 dimensional data for one chunk column
    /// </summary>
    public interface IMapChunk
    {
        ConcurrentDictionary<Vec2i, float> SnowAccum { get; }

        /// <summary>
        /// The map region this map chunk resides in
        /// </summary>
        IMapRegion MapRegion { get; }

        /// <summary>
        /// The current world generation pass this chunk column is in
        /// </summary>
        EnumWorldGenPass CurrentPass { get; set; }


        /// <summary>
        /// Server: Can be used to store custom data along with the map chunk
        /// Client: Not implemented. Map chunk Moddata is not synced from server to client
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        [Obsolete("Use SetModData instead")]
        void SetData(string key, byte[] data);


        /// <summary>
        /// Server: Can be used to retrieve custom data from the map chunk (as previously set by SetModdata)
        /// Client: Not implemented. Map chunk Moddata is not synced from server to client
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Obsolete("Use GetModData instead")]
        byte[] GetData(string key);


        /// <summary>
        /// Server: Allows setting of arbitrary, permanently stored moddata of this map chunk.
        /// Client: Not implemented. Map chunk Moddata is not synced from server to client
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        void SetModdata(string key, byte[] data);

        /// <summary>
        /// Server: Removes the permanently stored data. 
        /// Client: Not implemented. Map chunk Moddata is not synced from server to client
        /// </summary>
        /// <param name="key"></param>
        void RemoveModdata(string key);

        /// <summary>
        /// Server: Retrieve arbitrary, permanently stored mod data
        /// Client: Not implemented. Map chunk Moddata is not synced from server to client
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        byte[] GetModdata(string key);

        /// <summary>
        /// Server: Allows setting of arbitrary, permanantly stored moddata of this map chunk.
        /// Client: Not implemented. Map chunk Moddata is not synced from server to client
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>

        void SetModdata<T>(string key, T data);
        /// <summary>
        /// Server: Retrieve arbitrary, permantly stored mod data
        /// Client: Not implemented. Map chunk Moddata is not synced from server to client
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue">Default value</param>
        /// <returns></returns>
        T GetModdata<T>(string key, T defaultValue = default(T));


        byte[] CaveHeightDistort { get; set; }
        
        /// <summary>
        /// The position of the last block that is not rain permeable before the first airblock
        /// </summary>
        ushort[] RainHeightMap { get; }

        /// <summary>
        /// The position of the last block before the first airblock before world gen pass Vegetation
        /// </summary>
        ushort[] WorldGenTerrainHeightMap { get; }

        /// <summary>
        /// The rock block id of the topmost rock layer
        /// </summary>
        int[] TopRockIdMap { get; }

        ushort[] SedimentaryThicknessMap { get; }


        /// <summary>
        /// The highest position of any non-air block
        /// </summary>
        ushort YMax { get; set; }

        /// <summary>
        /// Causes the TTL counter to reset so that it the mapchunk does not unload. No effect when called client side.
        /// </summary>
        void MarkFresh();

        /// <summary>
        /// Tells the server that it has to save the changes of this chunk to disk. No effect when called client side.
        /// </summary>
        void MarkDirty();
    }
}
