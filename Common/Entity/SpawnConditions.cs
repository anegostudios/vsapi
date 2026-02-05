using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Common.Entities
{
    /// <summary>
    /// A list of conditions based on climate.
    /// </summary>
    [DocumentAsJson]
    public class ClimateSpawnCondition
    {
        /// <summary>
        /// The minimum tempurature for the object to spawn.
        /// </summary>
        [DocumentAsJson("Optional", "-40")]
        public float MinTemp = -40;

        /// <summary>
        /// The maximum tempurature for the object to spawn.
        /// </summary>
        [DocumentAsJson("Optional", "40")]
        public float MaxTemp = 40;

        /// <summary>
        /// The minimum amount of rain for the object to spawn.
        /// </summary>
        [DocumentAsJson("Optional", "0")]
        public float MinRain = 0f;

        /// <summary>
        /// The maximum amount of rain for the object to spawn.
        /// </summary>
        [DocumentAsJson("Optional", "1")]
        public float MaxRain = 1f;

        /// <summary>
        /// The minimum amount of forest cover needed for the object to spawn.
        /// </summary>
        [DocumentAsJson("Optional", "0")]
        public float MinForest = 0;

        /// <summary>
        /// The maximum amount of forest cover needed for the object to spawn.
        /// </summary>
        [DocumentAsJson("Optional", "1")]
        public float MaxForest = 1;

        /// <summary>
        /// The minimum amount of shrubbery needed for the object to spawn.
        /// </summary>
        [DocumentAsJson("Optional", "0")]
        public float MinShrubs = 0;

        /// <summary>
        /// The maximum amount of shrubbery needed for the object to spawn.
        /// </summary>
        [DocumentAsJson("Optional", "1")]
        public float MaxShrubs = 1;

        /// <summary>
        /// Won't span below minY. 0...1 is world bottom to sea level, 1...2 is sea level to world top
        /// </summary>
        [DocumentAsJson("Optional", "0")]
        public float MinY = 0;

        /// <summary>
        /// Won't span above maxY. 0...1 is world bottom to sea level, 1...2 is sea level to world top
        /// </summary>
        [DocumentAsJson("Optional", "2")]
        public float MaxY = 2;

        /// <summary>
        /// The minimum amount of forest or shrubs for the object to spawn.
        /// </summary>
        [DocumentAsJson("Optional", "0")]
        public float MinForestOrShrubs = 0;

        /// <summary>
        /// If defined, further constrains the spawning areas by a large scale 2D density map
        /// </summary>
        [DocumentAsJson("Optional", "null")]
        public string[] RandomMapCodePool;

        [JsonIgnore]
        public string MapCode = null;

        /// <summary>
        /// If true, will check for suitable habitat size (e.g. when underwater and size of 2 will check that there is also water 2 blocks away in all directions
        /// For habitat land: Not implemented yet
        /// For habitat air: Not implemented yet
        /// </summary>
        [DocumentAsJson("Optional", "true")]
        public int HabitatSize = 2;

        public void SetFrom(ClimateSpawnCondition conds)
        {
            this.MinTemp = conds.MinTemp;
            this.MaxTemp = conds.MaxTemp;
            this.MinRain = conds.MinRain;
            this.MaxRain = conds.MaxRain;
            this.MinForest = conds.MinForest;
            this.MaxForest = conds.MaxForest;
            this.MinShrubs = conds.MinShrubs;
            this.MaxShrubs = conds.MaxShrubs;
            this.MinY = conds.MinY;
            this.MaxY = conds.MaxY;
            this.MinForestOrShrubs = conds.MinForestOrShrubs;
            this.MapCode = conds.MapCode;
            if (conds.RandomMapCodePool != null)
            {
                this.RandomMapCodePool = (string[])conds.RandomMapCodePool?.Clone();
            }
            this.HabitatSize= conds.HabitatSize;
        }

        public bool MatchesClimate(ClimateCondition climate)
        {
            if (MinTemp > climate.WorldGenTemperature || MaxTemp < climate.WorldGenTemperature) return false;
            if (MinRain > climate.Rainfall || MaxRain < climate.Rainfall) return false;
            return true;
        }

        public bool MatchesForestation(ClimateCondition climate)
        {
            if (MinForest > climate.ForestDensity || MaxForest < climate.ForestDensity) return false;
            if (MinShrubs > climate.ShrubDensity || MaxShrubs < climate.ShrubDensity) return false;
            if (MinForestOrShrubs > Math.Max(climate.ForestDensity, climate.ShrubDensity)) return false;
            return true;
        }

        public ClimateSpawnCondition Clone()
        {
            var csc = new ClimateSpawnCondition();
            csc.SetFrom(this);
            return csc;
        }
    }

    /// <summary>
    /// The spawn conditions assigned to various things.
    /// </summary>
    /// <example>
    /// <code language="json">
    ///"spawnconditions": {
	///	"worldgen": {
	///		"TriesPerChunk": {
	///			"avg": 0.1,
	///			"var": 0
	///		},
	///		"tryOnlySurface": true,
	///		"minLightLevel": 10,
	///		"groupSize": {
	///			"dist": "verynarrowgaussian",
	///			"avg": 3,
	///			"var": 4
	///		},
	///		"insideBlockCodes": [ "air", "tallgrass-*" ],
	///		"minTemp": 5,
	///		"maxTemp": 28,
	///		"minRain": 0.45,
	///		"minForest": 0.35,
	///		"companions": [ "pig-wild-female", "pig-wild-piglet" ]
	///	},
	///	"runtime": {
	///		"group": "neutral",
	///		"tryOnlySurface": true,
	///		"chance": 0.0006,
	///		"maxQuantity": 4,
	///		"minLightLevel": 10,
	///		"groupSize": {
	///			"dist": "verynarrowgaussian",
	///			"avg": 3,
	///			"var": 4
	///		},
	///		"insideBlockCodes": [ "air", "tallgrass-*" ],
	///		"minTemp": 5,
	///		"maxTemp": 28,
	///		"minRain": 0.45,
	///		"minForestOrShrubs": 0.35,
	///		"companions": [ "pig-wild-female", "pig-wild-piglet" ]
	///	}
	///}
    /// </code>
    /// </example>
    [DocumentAsJson]
    public class SpawnConditions
    {
        /// <summary>
        /// Control specific spawn conditions based on climate.
        /// Note that this will override any climate values set in <see cref="Runtime"/> and <see cref="Worldgen"/>.
        /// It is recommended to specify climate values here rather than setting them in the other spawn conditions.
        /// </summary>
        [DocumentAsJson("Recommended", "None")]
        public ClimateSpawnCondition Climate;

        /// <summary>
        /// Runtime requirements for the object to spawn.
        /// </summary>
        [DocumentAsJson("Recommended", "None")]
        public RuntimeSpawnConditions Runtime;

        /// <summary>
        /// Worldgen/region requirements for the object to spawn.
        /// </summary>
        [DocumentAsJson("Recommended", "None")]
        public WorldGenSpawnConditions Worldgen;

        public SpawnConditions Clone()
        {
            return new SpawnConditions()
            {
                Runtime = Runtime?.Clone(),
                Worldgen = Worldgen?.Clone()
            };
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (Climate != null)
            {
                Runtime?.SetFrom(Climate);
                Worldgen?.SetFrom(Climate);
            }
        }

    }

    /// <summary>
    /// Allows you to control spawn limits based on a set of entity codes using a wildcard.
    /// </summary>
    [DocumentAsJson]
    public class QuantityByGroup
    {
        /// <summary>
        /// The maximum quantity for all entities that match the <see cref="Code"/> wildcard.
        /// </summary>
        [DocumentAsJson("Required")]
        public int MaxQuantity;

        /// <summary>
        /// A wildcard asset location which can group many entities together.
        /// </summary>
        [DocumentAsJson("Required")]
        public AssetLocation Code;
    }

    /// <summary>
    /// A set of spawn conditions for chunks that have already been generated. Most properties are got from <see cref="BaseSpawnConditions"/>.
    /// </summary>
    [DocumentAsJson]
    public class RuntimeSpawnConditions : BaseSpawnConditions
    {
        /// <summary>
        /// The chance, usually between 0 (0% chance) and 1 (100% chance), for the entity to spawn during the spawning round. 
        /// </summary>
        [DocumentAsJson("Recommended", "1")]
        public double Chance = 1f;

        /// <summary>
        /// The max number of this entity that can ever exist in the world for a single player.
        /// With more than one player, the max number is actually (this)x(current player count)x(<see cref="SpawnCapPlayerScaling"/>).
        /// Consider using <see cref="MaxQuantityByGroup"/> to allow a max quantity based from many entities.
        /// </summary>
        [DocumentAsJson("Recommended", "20")]
        public int MaxQuantity = 20;

        /// <summary>
        /// The max quantity of objects to spawn based on a wildcard group of entities.<br/>
        /// For example, using <see cref="MaxQuantity"/> will allow a max of 20 pig-wild-male instances.
        /// Using this with a group of "pig-*" will allow a max of 20 pig entities, regardless if male, female, or piglet.
        /// </summary>
        [DocumentAsJson("Optional", "None")]
        public QuantityByGroup MaxQuantityByGroup = null;

        /// <summary>
        /// The maximum number of this entity that can exist in the world is <see cref="MaxQuantity"/> x (current player count) x (this).
        /// </summary>
        [DocumentAsJson("Optional", "1")]
        public float SpawnCapPlayerScaling = 1f;

        /// <summary>
        /// The minimum distance from the player that an object will spawn.
        /// </summary>
        [DocumentAsJson("Optional", "18")]
        public int MinDistanceToPlayer = 18;

        /// <summary>
        /// Set server-side after this has been loaded once, used only for error logging purposes
        /// </summary>
        public bool doneInitialLoad;

        /// <summary>
        /// Creates a deep copy of this set of spawn conditions.
        /// </summary>
        /// <returns></returns>
        public new RuntimeSpawnConditions Clone()
        {
            return new RuntimeSpawnConditions()
            {
                Group = Group,
                MinLightLevel = MinLightLevel,
                MaxLightLevel = MaxLightLevel,
                LightLevelType = LightLevelType,
                HerdSize = HerdSize?.Clone(),
                Companions = Companions?.Clone() as AssetLocation[],
                InsideBlockCodes = InsideBlockCodes?.Clone() as AssetLocation[],
                RequireSolidGround = RequireSolidGround,
                TryOnlySurface = TryOnlySurface,
                MinTemp = MinTemp,
                MaxTemp = MaxTemp,
                MinRain = MinRain,
                MaxRain = MaxRain,
                MinForest = MinForest,
                MaxForest = MaxForest,
                MinShrubs = MinShrubs,
                MaxShrubs = MaxShrubs,
                ClimateValueMode = ClimateValueMode,
                MinForestOrShrubs = MinForestOrShrubs,
                Chance = Chance,
                MaxQuantity = MaxQuantity,
                MinDistanceToPlayer = MinDistanceToPlayer,
                MaxQuantityByGroup = MaxQuantityByGroup,
                SpawnCapPlayerScaling = SpawnCapPlayerScaling,
                RandomMapCodePool = (string[])RandomMapCodePool?.Clone(),
                MapCode = MapCode,
                HabitatSize = HabitatSize
            };
        }


    }

    /// <summary>
    /// A set of spawn conditions for when chunks are generated. Most properties are got from <see cref="BaseSpawnConditions"/>.
    /// </summary>
    [DocumentAsJson]
    public class WorldGenSpawnConditions : BaseSpawnConditions
    {
        /// <summary>
        /// The amount of times the object will attempt to spawn per chunk.
        /// </summary>
        [DocumentAsJson("Recommended", "0")]
        public NatFloat TriesPerChunk = NatFloat.Zero;

        public new WorldGenSpawnConditions Clone()
        {
            return new WorldGenSpawnConditions()
            {
                Group = Group,
                MinLightLevel = MinLightLevel,
                MaxLightLevel = MaxLightLevel,
                LightLevelType = LightLevelType,
                HerdSize = HerdSize?.Clone(),
                Companions = Companions?.Clone() as AssetLocation[],
                InsideBlockCodes = InsideBlockCodes?.Clone() as AssetLocation[],
                RequireSolidGround = RequireSolidGround,
                TryOnlySurface = TryOnlySurface,
                MinTemp = MinTemp,
                MaxTemp = MaxTemp,
                MinRain = MinRain,
                MaxRain = MaxRain,
                ClimateValueMode = ClimateValueMode,
                MinForest = MinForest,
                MaxForest = MaxForest,
                MinShrubs = MinShrubs,
                MaxShrubs = MaxShrubs,
                MinForestOrShrubs = MinForestOrShrubs,
                RandomMapCodePool = (string[])RandomMapCodePool?.Clone(),
                MapCode = MapCode,
                TriesPerChunk = TriesPerChunk?.Clone(),
                HabitatSize = HabitatSize
            };
        }
    }

    /// <summary>
    /// A base class for entities spawning conditions.
    /// </summary>
    [DocumentAsJson]
    public class BaseSpawnConditions : ClimateSpawnCondition
    {
        /// <summary>
        /// The group of the spawn conditions. Vanilla groups are:<br/>
        /// - hostile<br/>
        /// - neutral<br/>
        /// - passive<br/>
        /// Hostile creatures should be defined as such here.
        /// This will automatically stop them spawning with a grace timer,
        ///     and in locations where hostiles should not spawn.
        /// </summary>
        [DocumentAsJson("Recommended", "None")]
        public string Group;

        /// <summary>
        /// The minimum light level for an object to spawn.
        /// </summary>
        [DocumentAsJson("Optional", "0")]
        public int MinLightLevel = 0;

        /// <summary>
        /// The maximum light level for an object to spawn.
        /// </summary>
        [DocumentAsJson("Optional", "32")]
        public int MaxLightLevel = 32;

        /// <summary>
        /// The type of light counted for spawning purposes.
        /// </summary>
        [DocumentAsJson("Optional", "MaxLight")]
        public EnumLightLevelType LightLevelType = EnumLightLevelType.MaxLight;

        /// <summary>
        /// the group size for the spawn.
        /// </summary>
        [DocumentAsJson("Recommended", "1")]
        public NatFloat HerdSize = NatFloat.createUniform(1, 0);

        /// <summary>
        /// Obsolete. Use <see cref="HerdSize"/> instead.
        /// </summary>
        [DocumentAsJson("Obsolete")] 
        [Obsolete("Use HerdSize instead")]
        public NatFloat GroupSize { get => HerdSize; set => HerdSize = value; }

        /// <summary>
        /// Additional companions for the spawn.
        /// </summary>
        [DocumentAsJson("Optional", "None")]
        public AssetLocation[] Companions = Array.Empty<AssetLocation>();

        /// <summary>
        /// The blocks that the object will spawn in.
        /// </summary>
        [DocumentAsJson("Optional", "air")]
        public AssetLocation[] InsideBlockCodes = new AssetLocation[] { new AssetLocation("air") };

        /// <summary>
        /// Checks to see if the object requires solid ground.
        /// </summary>
        [DocumentAsJson("Optional", "True")]
        public bool RequireSolidGround = true;

        /// <summary>
        /// checks to see if the object can only spawn on the surface.
        /// </summary>
        [DocumentAsJson("Optional", "False")]
        public bool TryOnlySurface = false;

        /// <summary>
        /// Whether the rain and temperature values are referring to the worldgen values (i.e. yearly averages) or the current values at the moment of spawning.
        /// </summary>
        [DocumentAsJson("Optional", "WorldGenValues")]
        public EnumGetClimateMode ClimateValueMode = EnumGetClimateMode.WorldGenValues;


        protected HashSet<Block> InsideBlockCodesResolved = null;
        protected string[] InsideBlockCodesBeginsWith;
        protected string[] InsideBlockCodesExact;
        protected string InsideBlockFirstLetters = "";

        public bool CanSpawnInside(Block testBlock)
        {
            if (testBlock.Code == null) return false;
            string testPath = testBlock.Code.Path;
            if (testPath.Length < 1) return false;
            if (InsideBlockFirstLetters.IndexOf(testPath[0]) < 0) return false;   // early exit if we don't have the first letter

            if (PathMatchesInsideBlockCodes(testPath))
            {
                return InsideBlockCodesResolved.Contains(testBlock);    // Here we do a hashset check just to make sure, because so far we only checked the path not the domain
            }
            return false;
        }

        private bool PathMatchesInsideBlockCodes(string testPath)
        {
            for (int i = 0; i < InsideBlockCodesExact.Length; i++)
            {
                if (testPath == InsideBlockCodesExact[i]) return true;
            }

            for (int i = 0; i < InsideBlockCodesBeginsWith.Length; i++)
            {
                if (testPath.StartsWithOrdinal(InsideBlockCodesBeginsWith[i])) return true;
            }

            return false;
        }

        public void Initialise(LCGRandom lcgrnd, IServerWorldAccessor server, string entityName, Dictionary<AssetLocation, Block[]> searchCache)
        {
            if (RandomMapCodePool != null && RandomMapCodePool.Length > 0)
            {
                lcgrnd.InitPositionSeed(entityName.GetHashCode(), 0);
                MapCode = RandomMapCodePool[lcgrnd.NextInt(RandomMapCodePool.Length)];
            }

            if (InsideBlockCodes != null && InsideBlockCodes.Length > 0)
            {
                bool anyBlockOk = false;
                foreach (var val in InsideBlockCodes)
                {
                    if (!searchCache.TryGetValue(val, out Block[] foundBlocks))
                    {
                        foundBlocks = server.SearchBlocks(val);
                        searchCache[val] = foundBlocks;
                    }

                    foreach (Block b in foundBlocks)
                    {
                        if (InsideBlockCodesResolved == null) InsideBlockCodesResolved = new HashSet<Block>();
                        InsideBlockCodesResolved.Add(b);
                    }
                    anyBlockOk |= foundBlocks.Length > 0;
                }
                if (!anyBlockOk)
                {
                    server.Logger.Warning("Entity with code {0} has defined InsideBlockCodes for its spawn conditions, but none of these blocks exists, entity is unlikely to spawn.", entityName);
                }

                List<string> targetEntityCodesList = new List<string>();
                List<string> beginswith = new List<string>();
                var codes = InsideBlockCodes;
                for (int i = 0; i < codes.Length; i++)
                {
                    string code = codes[i].Path;
                    if (code.EndsWith('*'))
                    {
                        beginswith.Add(code.Substring(0, code.Length - 1));
                    }
                    else targetEntityCodesList.Add(code);
                }

                InsideBlockCodesBeginsWith = beginswith.ToArray();

                InsideBlockCodesExact = new string[targetEntityCodesList.Count];
                int j = 0;
                foreach (string code in targetEntityCodesList)
                {
                    if (code.Length == 0) continue;
                    InsideBlockCodesExact[j++] = code;
                    char c = code[0];
                    if (InsideBlockFirstLetters.IndexOf(c) < 0) InsideBlockFirstLetters += c;
                }

                foreach (string code in InsideBlockCodesBeginsWith)
                {
                    if (code.Length == 0) continue;
                    char c = code[0];
                    if (InsideBlockFirstLetters.IndexOf(c) < 0) InsideBlockFirstLetters += c;
                }
            }
        }
    }
}
