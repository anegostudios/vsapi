using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// 2D Map data for a 8x8 area of chunk columns. Holds a few maps for the chunk generation.
    /// </summary>
    public interface IMapRegion
    {
        /// <summary>
        /// Holds some simple perlin noise to be applied as a 2d distortion when placing deposits so they are not perfectly round
        /// </summary>
        IntMap DepositDistortionMap { get; set; }

        /// <summary>
        /// Currently unuseds
        /// </summary>
        IntMap FlowerMap { get; set; }

        /// <summary>
        /// Holds a shrub density map
        /// </summary>
        IntMap ShrubMap { get; set; }

        /// <summary>
        /// Holds a forest density map
        /// </summary>
        IntMap ForestMap { get; set; }

        /// <summary>
        /// Holds the landform indices
        /// </summary>
        IntMap LandformMap { get; set; }

        /// <summary>
        /// Holds temperature and rain fall.
        /// 16-23 bits = Red = temperature
        /// 8-15 bits = Green = rain
        /// 0-7 bits = Blue = unused 
        /// </summary>
        IntMap ClimateMap { get; set; }

        /// <summary>
        /// Holds the geologic province indices
        /// </summary>
        IntMap GeologicProvinceMap { get; set; }

        /// <summary>
        /// Holds the raw mod data.
        /// </summary>
        Dictionary<string, byte> ModData { get; }

        /// <summary>
        /// Holds the mod mappings.
        /// </summary>
        Dictionary<string, IntMap> ModMaps { get; }

        /// <summary>
        /// Gets the ore map for the given item.
        /// </summary>
        Dictionary<string, IntMap> OreMaps { get; }



        bool DirtyForSaving { get; set; }
    }
}
