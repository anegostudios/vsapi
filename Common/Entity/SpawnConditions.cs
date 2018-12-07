using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common.Entities
{
    public class SpawnConditions
    {
        public RuntimeSpawnConditions Runtime;
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
        public double Chance = 1f;
        public int MaxQuantity = 20;
        public int MinDistanceToPlayer = 18;

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
        public string Group;

        public int MinLightLevel = 0;
        public int MaxLightLevel = 32;
        public EnumLightLevelType LightLevelType = EnumLightLevelType.MaxLight;

        public NatFloat GroupSize = NatFloat.createUniform(1, 0);

        public AssetLocation[] Companions = new AssetLocation[0];

        public AssetLocation[] InsideBlockCodes = new AssetLocation[] { new AssetLocation("air") };

        public bool RequireSolidGround = true;
        public bool TryOnlySurface = false;

        public float MinTemp = -40;
        public float MaxTemp = 40;
        public float MinRain = 0f;
        public float MaxRain = 1f;
        public float MinForest = 0;
        public float MaxForest = 1;
        public float MinShrubs = 0;
        public float MaxShrubs = 1;

        public float MinY = 0;
        public float MaxY = 1;

        public float MinForestOrShrubs = 0;

    }
}