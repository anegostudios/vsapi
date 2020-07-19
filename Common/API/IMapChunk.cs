using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

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
        ConcurrentDictionary<BlockPos, float> SnowAccum { get; }

        /// <summary>
        /// The map region this map chunk resides in
        /// </summary>
        IMapRegion MapRegion { get; }

        /// <summary>
        /// The current world generation pass this chunk column is in
        /// </summary>
        EnumWorldGenPass CurrentPass { get; set; }

        /// <summary>
        /// Can be used to store custom data along with the map chunk
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        void SetData(string key, byte[] data);

        /// <summary>
        /// Can be used to retrieve custom data from the map chunk (as previously set by SetModdata)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        byte[] GetData(string key);


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
