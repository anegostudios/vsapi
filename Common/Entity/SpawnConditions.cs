using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common.Entities
{
    /// <summary>
    /// The spawn conditions assigned to various things.
    /// </summary>
    public class SpawnConditions
    {
        /// <summary>
        /// Runtime requirements for the object to spawn.
        /// </summary>
        public RuntimeSpawnConditions Runtime;

        /// <summary>
        /// Worldgen/region requirements for the object to spawn.
        /// </summary>
        public WorldGenSpawnConditions Worldgen;

        public SpawnConditions Clone()
        {
            return new SpawnConditions()
            {
                Runtime = Runtime?.Clone(),
                Worldgen = Worldgen?.Clone()
            };
        }
    }

    public class RuntimeSpawnConditions : BaseSpawnConditions
    {
        /// <summary>
        /// The chance for the object to spawn.
        /// </summary>
        public double Chance = 1f;

        /// <summary>
        /// The max quantity of objects to spawn.
        /// </summary>
        public int MaxQuantity = 20;

        /// <summary>
        /// The minimum distance from the player that an object will spawn.
        /// </summary>
        public int MinDistanceToPlayer = 18;

        /// <summary>
        /// Creates a deep copy of this set of spawn conditions.
        /// </summary>
        /// <returns></returns>
        public RuntimeSpawnConditions Clone()
        {
            return new RuntimeSpawnConditions()
            {
                Group = Group,
                MinLightLevel = MinLightLevel,
                MaxLightLevel = MaxLightLevel,
                LightLevelType = LightLevelType,
                GroupSize = GroupSize?.Clone(),
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
                MinForestOrShrubs = MinForestOrShrubs,
                Chance = Chance,
                MaxQuantity = MaxQuantity,
                MinDistanceToPlayer = MinDistanceToPlayer
            };
        }
    }


    public class WorldGenSpawnConditions : BaseSpawnConditions
    {
        /// <summary>
        /// The amount of time the object will attempt to spawn per chunk.
        /// </summary>
        public NatFloat TriesPerChunk = NatFloat.Zero;

        public WorldGenSpawnConditions Clone()
        {
            return new WorldGenSpawnConditions()
            {
                Group = Group,
                MinLightLevel = MinLightLevel,
                MaxLightLevel = MaxLightLevel,
                LightLevelType = LightLevelType,
                GroupSize = GroupSize?.Clone(),
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
                MinForestOrShrubs = MinForestOrShrubs,
                TriesPerChunk = TriesPerChunk?.Clone()
            };
        }
    }

    public class BaseSpawnConditions
    {
        /// <summary>
        /// The group of the spawn conditions.
        /// </summary>
        public string Group;

        /// <summary>
        /// The minimum light level for an object to spawn.
        /// </summary>
        public int MinLightLevel = 0;

        /// <summary>
        /// The maximum light level for an object to spawn.
        /// </summary>
        public int MaxLightLevel = 32;

        /// <summary>
        /// The type of light counted for spawning purposes.
        /// </summary>
        public EnumLightLevelType LightLevelType = EnumLightLevelType.MaxLight;

        /// <summary>
        /// the group size for the spawn.
        /// </summary>
        public NatFloat GroupSize = NatFloat.createUniform(1, 0);

        /// <summary>
        /// Additional companions for the spawn.
        /// </summary>
        public AssetLocation[] Companions = new AssetLocation[0];

        /// <summary>
        /// The blocks that the object will spawn in.  (default: air)
        /// </summary>
        public AssetLocation[] InsideBlockCodes = new AssetLocation[] { new AssetLocation("air") };

        /// <summary>
        /// Checks to see if the object requires solid ground.
        /// </summary>
        public bool RequireSolidGround = true;

        /// <summary>
        /// checks to see if the object can only spawn in the surface.
        /// </summary>
        public bool TryOnlySurface = false;

        /// <summary>
        /// The minimum tempurature for the object to spawn.
        /// </summary>
        public float MinTemp = -40;

        /// <summary>
        /// The maximum tempurature for the object to spawn.
        /// </summary>
        public float MaxTemp = 40;

        /// <summary>
        /// The minimum amount of rain for the object to spawn.
        /// </summary>
        public float MinRain = 0f;

        /// <summary>
        /// The maximum amount of rain for the object to spawn.
        /// </summary>
        public float MaxRain = 1f;

        /// <summary>
        /// The minimum amount of forest cover needed for the object to spawn.
        /// </summary>
        public float MinForest = 0;

        /// <summary>
        /// The maximum amount of forest cover needed for the object to spawn.
        /// </summary>
        public float MaxForest = 1;

        /// <summary>
        /// The minimum amount of shrubbery needed for the object to spawn.
        /// </summary>
        public float MinShrubs = 0;

        /// <summary>
        /// The maximum amount of shrubbery needed for the object to spawn.
        /// </summary>
        public float MaxShrubs = 1;

        /// <summary>
        /// Won't span below minY. 0...1 is world bottom to sea level, 1...2 is sea level to world top
        /// </summary>
        public float MinY = 0;

        /// <summary>
        /// Won't span above maxY. 0...1 is world bottom to sea level, 1...2 is sea level to world top
        /// </summary>
        public float MaxY = 1;

        /// <summary>
        /// The minimum amount of forest or shrubs for the object to spawn.
        /// </summary>
        public float MinForestOrShrubs = 0;

    }
}