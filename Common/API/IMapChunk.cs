using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Server;

namespace Vintagestory.API.Common
{
    public interface IMapChunk
    {
        IMapRegion MapRegion { get; }

        EnumWorldGenPass CurrentPass { get; set; }

        void SetModdata(string key, byte[] data);
        byte[] GetModdata(string key);

        
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
        ushort[] TopRockIdMap { get; }

        void MarkFresh();
    }
}
