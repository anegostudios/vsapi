using ProtoBuf;
using System;
using System.Collections.Generic;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

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

        /// <summary>
        /// If this flag is set, trees and shrubs will not generate inside the structure's bounding box 
        /// </summary>
        public bool SuppressTreesAndShrubs;

        /// <summary>
        /// If this flag is set, rivulets will not generate inside the structure's bounding box 
        /// </summary>
        public bool SuppressRivulets;
    }

    /// <summary>
    /// 2D Map data for a 16x16 area of chunk columns. Holds a few maps for the chunk generation.
    /// </summary>
    public interface IMapRegion
    {
        /// <summary>
        /// Density maps for block patches
        /// </summary>
        Dictionary<string, IntDataMap2D> BlockPatchMaps { get; set; }

        /// <summary>
        /// Currently unuseds
        /// </summary>
        IntDataMap2D FlowerMap { get; set; }

        /// <summary>
        /// Holds a shrub density map
        /// </summary>
        IntDataMap2D ShrubMap { get; set; }

        /// <summary>
        /// Holds a forest density map
        /// </summary>
        IntDataMap2D ForestMap { get; set; }

        /// <summary>
        /// Holds a beach strength map
        /// </summary>
        IntDataMap2D BeachMap { get; set; }

        IntDataMap2D OceanMap { get; set; }

        IntDataMap2D UpheavelMap { get; set; }

        /// <summary>
        /// Holds the landform indices
        /// </summary>
        IntDataMap2D LandformMap { get; set; }

        /// <summary>
        /// Holds temperature and rain fall.<br/>
        /// 16-23 bits = Red = temperature<br/>
        /// 8-15 bits = Green = rain<br/>
        /// 0-7 bits = Blue = unused 
        /// </summary>
        IntDataMap2D ClimateMap { get; set; }

        /// <summary>
        /// Holds the geologic province indices
        /// </summary>
        IntDataMap2D GeologicProvinceMap { get; set; }


        /// <summary>
        /// Holds the rock strata noise maps
        /// </summary>
        IntDataMap2D[] RockStrata { get; set; }
        

        /// <summary>
        /// Holds the raw mod data.
        /// </summary>
        [Obsolete("Use Get/Set/RemoveModData instead")]
        Dictionary<string, byte[]> ModData { get; }

        /// <summary>
        /// Holds the mod mappings.
        /// </summary>
        Dictionary<string, IntDataMap2D> ModMaps { get; }

        /// <summary>
        /// Gets the ore map for the given item.
        /// </summary>
        Dictionary<string, IntDataMap2D> OreMaps { get; }

        IntDataMap2D OreMapVerticalDistortTop { get; }
        IntDataMap2D OreMapVerticalDistortBottom { get; }


        /// <summary>
        /// List of structures that were generated in this region
        /// </summary>
        List<GeneratedStructure> GeneratedStructures { get; }


        bool DirtyForSaving { get; set; }


        /// <summary>
        /// Server: Allows setting of arbitrary, permanently stored moddata of this map region.
        /// Client: Not implemented. Map chunk Moddata is not synced from server to client
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data">Use SerializerUtil to encode your data to bytes</param>
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
        /// Server: Allows setting of arbitrary, permanantly stored moddata of this map region.
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
        /// <returns></returns>
        T GetModdata<T>(string key);
        /// <summary>
        /// A thread-safe way to add a new GeneratedStructure, also marks DirtyForSaving = true
        /// </summary>
        void AddGeneratedStructure(GeneratedStructure generatedStructure);
    }
}
