using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class GeneratedStructure
    {
        /// <summary>
        /// Block position of the structure
        /// </summary>
        public Cuboidi Location;

        /// <summary>
        /// Code as defined in the WorldGenStructure object
        /// </summary>
        public string Code;

        /// <summary>
        /// Group as defined in the WorldGenStructure object
        /// </summary>
        public string Group;
    }

    /// <summary>
    /// 2D Map data for a 8x8 area of chunk columns. Holds a few maps for the chunk generation.
    /// </summary>
    public interface IMapRegion
    {
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
        /// Holds a beach strength map
        /// </summary>
        IntMap BeachMap { get; set; }
        
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
        /// Holds the rock strata noise maps
        /// </summary>
        IntMap[] RockStrata { get; set; }
        

        /// <summary>
        /// Holds the raw mod data.
        /// </summary>
        Dictionary<string, byte[]> ModData { get; }

        /// <summary>
        /// Holds the mod mappings.
        /// </summary>
        Dictionary<string, IntMap> ModMaps { get; }

        /// <summary>
        /// Gets the ore map for the given item.
        /// </summary>
        Dictionary<string, IntMap> OreMaps { get; }

        IntMap OreMapVerticalDistortTop { get; }
        IntMap OreMapVerticalDistortBottom { get; }


        /// <summary>
        /// List of structures that were generated in this region
        /// </summary>
        List<GeneratedStructure> GeneratedStructures { get; }


        bool DirtyForSaving { get; set; }
    }
}
