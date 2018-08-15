using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common.Entities
{
    /// <summary>
    /// Describes a entity type
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class EntityType
    {
        [JsonProperty]
        public AssetLocation Code;
        [JsonProperty]
        public bool Enabled = true;
        /// <summary>
        /// The class as registered in the ClassRegistry
        /// </summary>
        [JsonProperty]
        public string Class;
        [JsonProperty]
        public EnumHabitat Habitat = EnumHabitat.Land;
        [JsonProperty]
        public Vec2f HitBoxSize = new Vec2f(0.25f, 0.5f);
        [JsonProperty]
        public double EyeHeight = 0.1;
        [JsonProperty]
        public bool CanClimb = false;
        [JsonProperty]
        public bool CanClimbAnywhere = false;
        [JsonProperty]
        public bool FallDamage = true;
        [JsonProperty]
        public float ClimbTouchDistance = 0.5f;
        [JsonProperty]
        public bool RotateModelOnClimb = false;
        [JsonProperty]
        public float KnockbackResistance = 0f;

        [JsonProperty, JsonConverter(typeof(JsonAttributesConverter))]
        public JsonObject Attributes;

        [JsonProperty]
        public ClientEntityConfig Client;

        [JsonProperty]
        public ServerEntityConfig Server;

        [JsonProperty]
        public Dictionary<string, AssetLocation> Sounds;

        [JsonProperty]
        public float IdleSoundChance = 0.3f;

        [JsonProperty]
        public float IdleSoundRange = 24;

        [JsonProperty]
        public BlockDropItemStack[] Drops;

        


        public Dictionary<string, AssetLocation[]> ResolvedSounds = new Dictionary<string, AssetLocation[]>();

        public void InitSounds(IAssetManager assetManager)
        {
            if (Sounds != null)
            {
                foreach (var val in Sounds)
                {
                    if (val.Value.Path.EndsWith("*"))
                    {
                        List<IAsset> assets = assetManager.GetMany("sounds/" + val.Value.Path.Substring(0, val.Value.Path.Length - 1), val.Value.Domain);
                        AssetLocation[] sounds = new AssetLocation[assets.Count];
                        int i = 0;

                        foreach (IAsset asset in assets)
                        {
                            sounds[i++] = asset.Location;
                        }

                        ResolvedSounds[val.Key] = sounds;
                    }
                    else
                    {
                        ResolvedSounds[val.Key] = new AssetLocation[] { val.Value.Clone().WithPathPrefix("sounds/") };
                    }
                }
            }

        }

        public void InitClass(IAssetManager assetManager)
        {
            InitSounds(assetManager);
        }
    }


    public class ClientEntityConfig
    {
        [JsonProperty]
        public string Renderer;
        [JsonProperty]
        public Dictionary<string, CompositeTexture> Textures = new Dictionary<string, CompositeTexture>();
        [JsonProperty]
        protected CompositeTexture Texture;
        [JsonProperty]
        public int GlowLevel = 0;
        [JsonProperty]
        public CompositeShape Shape;
        [JsonProperty(ItemConverterType = typeof(JsonAttributesConverter))]
        public JsonObject[] Behaviors;
        [JsonProperty]
        public float Size = 1f;
        [JsonProperty]
        public AnimationMetaData[] Animations;

        public Dictionary<string, AnimationMetaData> AnimationsByMetaCode = new Dictionary<string, AnimationMetaData>();


        public Shape LoadedShape;

        /// <summary>
        /// Returns the first texture in Textures dict
        /// </summary>
        public CompositeTexture FirstTexture { get { return (Textures == null || Textures.Count == 0) ? null : Textures.First().Value; } }


        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (Texture != null)
            {
                Textures["all"] = Texture;
            }
            Init();
        }
        
        public void Init()
        {
            if (Animations != null)
            {
                for (int i = 0; i < Animations.Length; i++)
                {
                    AnimationMetaData animMeta = Animations[i];
                    if (animMeta.Animation != null) AnimationsByMetaCode[animMeta.Code] = animMeta;
                }
            }
        }

        public void Bake(IAssetManager assetManager)
        {
            foreach (var val in Textures)
            {
                val.Value.Bake(assetManager);
            }
        }

        public void LoadShape(EntityType entityType, ICoreClientAPI capi)
        {
            ClientEntityConfig clientConf = entityType.Client;
            AssetLocation shapePath;
            Shape entityShape = null;

            if (clientConf.Shape?.Base == null || clientConf.Shape.Base.Path.Length == 0)
            {
                shapePath = new AssetLocation("shapes/block/basic/cube.json");
                if (clientConf.Shape?.VoxelizeTexture != true)
                {
                    capi.World.Logger.Warning("No entity shape supplied for entity {0}, using cube shape", entityType.Code);
                }
                clientConf.Shape.Base = new AssetLocation("block/basic/cube");
            }
            else
            {
                shapePath = clientConf.Shape.Base.Clone().WithPathPrefix("shapes/").WithPathAppendix(".json");
            }

            IAsset asset = capi.Assets.TryGet(shapePath);

            if (asset == null)
            {
                capi.World.Logger.Error("Entity shape {0} for entity {1} not found, was supposed to be at {2}. Entity will be invisible!", clientConf.Shape, entityType.Code, shapePath);
                return;
            }

            try
            {
                entityShape = asset.ToObject<Shape>();
            }
            catch (Exception e)
            {
                capi.World.Logger.Error("Exception thrown when trying to load entity shape {0} for entity {1}. Entity will be invisible! Exception: {2}", clientConf.Shape, entityType.Code, e);
                return;
            }

            clientConf.LoadedShape = entityShape;
            entityShape.ResolveReferences(capi.World.Logger, clientConf.Shape.Base.ToString());

            CacheInvTransforms(clientConf.LoadedShape.Elements);
            

        }

        private void CacheInvTransforms(ShapeElement[] elements)
        {
            if (elements == null) return;

            for (int i = 0; i < elements.Length; i++)
            {
                elements[i].CacheInverseTransformMatrix();
                CacheInvTransforms(elements[i].Children);
            }
        }
    }

    public class ServerEntityConfig
    {
        [JsonProperty(ItemConverterType = typeof(JsonAttributesConverter))]
        public JsonObject[] Behaviors;

        [JsonProperty, JsonConverter(typeof(JsonAttributesConverter))]
        public JsonObject Attributes;

        [JsonProperty]
        public SpawnConditions SpawnConditions;
    }




    public class JsonAttributesConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(JsonObject);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.ReadFrom(reader);

            JsonObject var = new JsonObject(token);
            return var;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            
        }
    }


    public class SpawnConditions
    {
        public RuntimeSpawnConditions Runtime;
        public WorldGenSpawnConditions Worldgen;
    }

    public class RuntimeSpawnConditions : BaseSpawnConditions
    {
        public double Chance = 1f;
        public int MaxQuantity = 20;
        public int MinDistanceToPlayer = 18;
    }


    public class WorldGenSpawnConditions : BaseSpawnConditions
    {
        public NatFloat TriesPerChunk = NatFloat.Zero;
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

        public float MinForestOrShrubs = 0;
    }





}
