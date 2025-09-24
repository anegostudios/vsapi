using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common.Entities
{
    public class EntityProperties
    {
        /// <summary>
        /// Assigned on registering the entity type
        /// </summary>
        public int Id;

        public string Color;

        /// <summary>
        /// The entity code in the code.
        /// </summary>
        public AssetLocation Code;

        /// <summary>
        /// Variant values as resolved from blocktype/itemtype or entitytype
        /// </summary>
        public Datastructures.OrderedDictionary<string, string> Variant = new ();

        /// <summary>
        /// The classification of the entity.
        /// </summary>
        public string Class;

        /// <summary>
        /// List of entity tags ids
        /// </summary>
        public EntityTagArray Tags = EntityTagArray.Empty;

        /// <summary>
        /// Natural habitat of the entity. Decides whether to apply gravity or not
        /// </summary>
        public EnumHabitat Habitat = EnumHabitat.Land;

        /// <summary>
        /// The size of the entity's hitbox (default: 0.2f/0.2f)
        /// </summary>
        public Vec2f CollisionBoxSize = new Vec2f(0.2f, 0.2f);

        /// <summary>
        /// The size of the hitbox while the entity is dead.
        /// </summary>
        public Vec2f DeadCollisionBoxSize = new Vec2f(0.3f, 0.3f);

        /// <summary>
        /// The size of the entity's hitbox (default: null, i.e. same as collision box)
        /// </summary>
        public Vec2f SelectionBoxSize = null;

        /// <summary>
        /// The size of the hitbox while the entity is dead.  (default: null, i.e. same as dead collision box)
        /// </summary>
        public Vec2f DeadSelectionBoxSize = null;

        /// <summary>
        /// How high the camera should be placed if this entity were to be controlled by the player
        /// </summary>
        public double EyeHeight;

        public double SwimmingEyeHeight;


        /// <summary>
        /// The mass of this type of entity in kilograms, on average - defaults to 25kg (medium-low) if not set by the asset
        /// </summary>
        public float Weight = 25f;

        /// <summary>
        /// If true the entity can climb on walls
        /// </summary>
        public bool CanClimb;

        /// <summary>
        /// If true the entity can climb anywhere.
        /// </summary>
        public bool CanClimbAnywhere;

        /// <summary>
        /// Whether the entity should take fall damage
        /// </summary>
        public bool FallDamage = true;

        /// <summary>
        /// If less than one, mitigates fall damage (e.g. could be used for mountainous creatures); if more than one, increases fall damage (e.g fragile creatures?)
        /// </summary>
        public float FallDamageMultiplier = 1.0f;

        public float ClimbTouchDistance;

        /// <summary>
        /// Should the model in question rotate if climbing?
        /// </summary>
        public bool RotateModelOnClimb;

        /// <summary>
        /// The resistance to being pushed back by an impact.
        /// </summary>
        public float KnockbackResistance;

        /// <summary>
        /// The attributes of the entity.  These are the Attributes read from the entity type's JSON file.
        /// <br/>If your code modifies these Attributes (not recommended!), the changes will apply to all entities of the same type.
        /// </summary>
        public JsonObject Attributes;

        /// <summary>
        /// The client properties of the entity.
        /// </summary>
        public EntityClientProperties Client;

        /// <summary>
        /// The server properties of the entity.
        /// </summary>
        public EntityServerProperties Server;

        /// <summary>
        /// The sounds that this entity can make.
        /// </summary>
        public Dictionary<string, AssetLocation> Sounds;

        /// <summary>
        /// The sounds this entity can make after being resolved.
        /// </summary>
        public Dictionary<string, AssetLocation[]> ResolvedSounds = new Dictionary<string, AssetLocation[]>();

        /// <summary>
        /// The chance that an idle sound will play for the entity.
        /// </summary>
        public float IdleSoundChance = 0.3f;

        /// <summary>
        /// The sound range for the idle sound in blocks.
        /// </summary>
        public float IdleSoundRange = 24;

        /// <summary>
        /// The drops for the entity when they are killed.
        /// </summary>
        public BlockDropItemStack[] Drops;
        public byte[] DropsPacket;

        /// <summary>
        /// The collision box they have.
        /// </summary>
        public Cuboidf SpawnCollisionBox
        {
            get
            {
                return new Cuboidf()
                {
                    X1 = -CollisionBoxSize.X / 2,
                    Z1 = -CollisionBoxSize.X / 2,
                    X2 = CollisionBoxSize.X / 2,
                    Z2 = CollisionBoxSize.X / 2,
                    Y2 = CollisionBoxSize.Y
                };
            }
        }

        /// <summary>
        /// Creates a copy of this object.
        /// </summary>
        /// <returns></returns>
        public EntityProperties Clone()
        {
            BlockDropItemStack[] DropsCopy;
            if (Drops == null)
                DropsCopy = null;
            else
            {
                DropsCopy = new BlockDropItemStack[Drops.Length];
                for (int i = 0; i < DropsCopy.Length; i++)
                    DropsCopy[i] = Drops[i].Clone();
            }

            Dictionary<string, AssetLocation> csounds = new Dictionary<string, AssetLocation>();
            foreach (var val in Sounds)
            {
                csounds[val.Key] = val.Value.Clone();
            }


            Dictionary<string, AssetLocation[]> cresolvedsounds = new Dictionary<string, AssetLocation[]>();
            foreach (var val in ResolvedSounds)
            {
                AssetLocation[] locs = val.Value;
                cresolvedsounds[val.Key] = new AssetLocation[locs.Length];
                for (int i = 0; i < locs.Length; i++)
                {
                    cresolvedsounds[val.Key][i] = locs[i].Clone();
                }
            }

            // Make Attributes read-only from now on, so that there is no need to DeepClone it for every new entity spawned.  If a mod needs to change the EntityProperties Attributes, it can re-assign this to a clone
            // Note, a JsonObject is in any case virtually read-only.   JsonObject[key] is read-only.   Exceptions are writing directly to the Token, and FillPlaceHolder (but that is only used in recipe loading)
            if (!(Attributes is JsonObject_ReadOnly) && Attributes != null) Attributes = new JsonObject_ReadOnly(Attributes);

            return new EntityProperties()
            {
                Code = Code.Clone(),
                Tags = Tags,
                Class = Class,
                Color = Color,
                Habitat = Habitat,
                CollisionBoxSize = CollisionBoxSize.Clone(),
                DeadCollisionBoxSize = DeadCollisionBoxSize.Clone(),
                SelectionBoxSize = SelectionBoxSize?.Clone(),
                DeadSelectionBoxSize = DeadSelectionBoxSize?.Clone(),
                CanClimb = CanClimb,
                Weight = Weight,
                CanClimbAnywhere = CanClimbAnywhere,
                FallDamage = FallDamage,
                FallDamageMultiplier = FallDamageMultiplier,
                ClimbTouchDistance = ClimbTouchDistance,
                RotateModelOnClimb = RotateModelOnClimb,
                KnockbackResistance = KnockbackResistance,
                Attributes = Attributes,
                Sounds = new Dictionary<string, AssetLocation>(Sounds),
                IdleSoundChance = IdleSoundChance,
                IdleSoundRange = IdleSoundRange,
                Drops = DropsCopy,
                EyeHeight = EyeHeight,
                SwimmingEyeHeight = SwimmingEyeHeight,
                Client = Client?.Clone() as EntityClientProperties,
                Server = Server?.Clone() as EntityServerProperties,
                Variant = new (Variant)
            };
        }

        /// <summary>
        /// Initalizes the properties for the entity.
        /// </summary>
        /// <param name="entity">the entity to tie this to.</param>
        /// <param name="api">The Core API</param>
        public void Initialize(Entity entity, ICoreAPI api)
        {
            if (api.Side.IsClient())
            {
                if (Client == null)
                {
                    return;
                }
                Client.loadBehaviors(entity, this, api.World);
            }

            else if (Server != null)
            {
                Server.loadBehaviors(entity, this, api.World);
            }

            Client?.Init(Code, api.World);

            InitSounds(api.Assets);
        }

        /// <summary>
        /// Initializes the sounds for this entity type.
        /// </summary>
        /// <param name="assetManager"></param>
        public void InitSounds(IAssetManager assetManager)
        {
            if (Sounds != null)
            {
                foreach (var val in Sounds)
                {
                    if (val.Value.Path.EndsWith('*'))
                    {
                        List<IAsset> assets = assetManager.GetManyInCategory("sounds", val.Value.Path.Substring(0, val.Value.Path.Length - 1), val.Value.Domain);
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

        internal void PopulateDrops(IWorldAccessor worldForResolve)
        {
            using (MemoryStream ms = new MemoryStream(DropsPacket))
            {
                BinaryReader reader = new BinaryReader(ms);
                int len = reader.ReadInt32();
                BlockDropItemStack[] drops = new BlockDropItemStack[len];
                for (int i = 0; i < drops.Length; i++)
                {
                    drops[i] = new BlockDropItemStack();
                    drops[i].FromBytes(reader, worldForResolve.ClassRegistry);
                    drops[i].Resolve(worldForResolve, "decode entity drops for ", Code);
                }

                Drops = drops;
            }
            DropsPacket = null;
        }
    }

    public abstract class EntitySidedProperties
    {
        /// <summary>
        /// The attributes of the entity type.
        /// </summary>
        public ITreeAttribute Attributes;

        /// <summary>
        /// Entity type behaviors
        /// </summary>
        public JsonObject[] BehaviorsAsJsonObj;

        /// <summary>
        /// When this property is attached to an entity - the behaviors attached of entity.
        /// <br/>To modify this list, please call Entity.AddBehavior() or Entity.RemoveBehavior()
        /// </summary>
        public List<EntityBehavior> Behaviors = new List<EntityBehavior>();



        public EntitySidedProperties(JsonObject[] behaviors, Dictionary<string, JsonObject> commonConfigs)
        {
            BehaviorsAsJsonObj = new JsonObject[behaviors.Length];
            int count = 0;
            for (int i = 0; i < behaviors.Length; i++)
            {
                JsonObject jobj = behaviors[i];
                bool enabled = jobj["enabled"].AsBool(true);
                if (!enabled) continue;
                string code = jobj["code"].AsString();
                if (code == null) continue;

                JsonObject mergedobj = jobj;
                if (commonConfigs != null && commonConfigs.ContainsKey(code))
                {
                    var clonedObj = commonConfigs[code].Token.DeepClone() as JObject;
                    clonedObj.Merge(jobj.Token as JObject);
                    mergedobj = new JsonObject(clonedObj);
                }

                BehaviorsAsJsonObj[count++] = new JsonObject_ReadOnly(mergedobj);
            }

            if (count < behaviors.Length) Array.Resize(ref BehaviorsAsJsonObj, count);
        }

        public void loadBehaviors(Entity entity, EntityProperties properties, IWorldAccessor world)
        {
            if (BehaviorsAsJsonObj == null) return;

            this.Behaviors.Clear();

            for (int i = 0; i < BehaviorsAsJsonObj.Length; i++)
            {
                JsonObject jobj = BehaviorsAsJsonObj[i];
                string code = jobj["code"].AsString();

                if (world.ClassRegistry.GetEntityBehaviorClass(code) != null)
                {
                    EntityBehavior behavior = world.ClassRegistry.CreateEntityBehavior(entity, code);
                    Behaviors.Add(behavior);
                    behavior.FromBytes(false);
                    behavior.Initialize(properties, jobj);
                } else
                {
                    world.Logger.Notification("Entity behavior {0} for entity {1} not found, will not load it.", code, properties.Code);
                }
            }
        }

        /// <summary>
        /// Use this to make a deep copy of these properties.
        /// </summary>
        /// <returns></returns>
        public abstract EntitySidedProperties Clone();
    }


    public class EntityClientProperties : EntitySidedProperties
    {

        public EntityClientProperties(JsonObject[] behaviors, Dictionary<string, JsonObject> commonConfigs) : base(behaviors, commonConfigs) { }

        /// <summary>
        /// Set by the game client
        /// </summary>
        public EntityRenderer Renderer;

        /// <summary>
        /// Name of there renderer system that draws this entity
        /// </summary>
        public string RendererName;

        /// <summary>
        /// Directory of all available textures. First one will be default one
        /// <br/>Note: from game version 1.20.4, this is <b>null on server-side</b> (except during asset loading start-up stage)
        /// </summary>
        public IDictionary<string, CompositeTexture> Textures = new FastSmallDictionary<string, CompositeTexture>(0);
        /// <summary>
        /// Set by a server at the end of asset loading, immediately prior to setting Textures to null; relevant to spawning entities with variant textures
        /// </summary>
        public int TexturesAlternatesCount = 0;

        /// <summary>
        /// Used by various renderers to retrieve the entities texture it should be drawn with
        /// </summary>
        public virtual CompositeTexture Texture
        {
            get { return (Textures == null || Textures.Count == 0) ? null : Textures.First().Value; }
        }

        /// <summary>
        /// The glow level for the entity.
        /// </summary>
        public int GlowLevel = 0;

        /// <summary>
        /// Makes entities pitch forward and backwards when stepping
        /// </summary>
        public bool PitchStep = true;

        /// <summary>
        /// The shape of the entity
        /// </summary>
        public CompositeShape Shape;

        /// <summary>
        /// Only loaded for World.EntityTypes instances of EntityProperties, because it makes no sense to have 1000 loaded entities needing to load 1000 shapes. During entity load/spawn this value is assigned however.
        /// On the client it gets set by the EntityTextureAtlasManager.
        /// On the server by the EntitySimulation system
        /// </summary>
        public Shape LoadedShape;

        public Shape[] LoadedAlternateShapes;

        /// <summary>
        /// The shape for this particular entity who owns this properties object
        /// </summary>
        public Shape LoadedShapeForEntity;
        public CompositeShape ShapeForEntity;

        /// <summary>
        /// The size of the entity (default: 1f)
        /// </summary>
        public float Size = 1f;

        /// <summary>
        /// The rate at which the entity's size grows with age - used for chicks and other small baby animals
        /// </summary>
        public float SizeGrowthFactor = 0f;

        /// <summary>
        /// The animations of the entity.
        /// </summary>
        public AnimationMetaData[] Animations;

        public Dictionary<string, AnimationMetaData> AnimationsByMetaCode = new Dictionary<string, AnimationMetaData>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<uint, AnimationMetaData> AnimationsByCrc32 = new Dictionary<uint, AnimationMetaData>();

        /// <summary>
        /// Returns the first texture in Textures dict
        /// </summary>
        public CompositeTexture FirstTexture { get { return (Textures == null || Textures.Count == 0) ? null : Textures.First().Value; } }


        public void DetermineLoadedShape(long forEntityId)
        {
            if (LoadedAlternateShapes != null && LoadedAlternateShapes.Length > 0)
            {
                int index = GameMath.MurmurHash3Mod(0, 0, (int)forEntityId, 1 + LoadedAlternateShapes.Length);
                if (index == 0)
                {
                    LoadedShapeForEntity = LoadedShape;
                    ShapeForEntity = Shape;
                }
                else
                {
                    LoadedShapeForEntity = LoadedAlternateShapes[index - 1];
                    ShapeForEntity = Shape.Alternates[index - 1];
                }
                return;
            }

            LoadedShapeForEntity = LoadedShape;
            ShapeForEntity = Shape;
        }


        /// <summary>
        /// Initializes the client properties.
        /// </summary>
        /// <param name="entityTypeCode"></param>
        /// <param name="world"></param>
        public void Init(AssetLocation entityTypeCode, IWorldAccessor world)
        {
            if (Animations != null)
            {
                for (int i = 0; i < Animations.Length; i++)
                {
                    AnimationMetaData animMeta = Animations[i];
                    animMeta.Init();

                    if (animMeta.Animation != null)
                    {
                        AnimationsByMetaCode[animMeta.Code] = animMeta;
                    }

                    if (animMeta.Animation != null)
                    {
                        AnimationsByCrc32[animMeta.CodeCrc32] = animMeta;
                    }
                }
            }

            if (world != null)
            {
                var cprop = world.EntityTypes.FirstOrDefault(et => et.Code.Equals(entityTypeCode))?.Client;
                LoadedShape = cprop?.LoadedShape;
                LoadedAlternateShapes = cprop?.LoadedAlternateShapes;
            }
        }

        /// <summary>
        /// Does not clone textures, but does clone Shape
        /// <br/>Note: This method does not clone the LoadedShape (nor the LoadedAlternateShapes, if present). The LoadedShape (and any alternate) is not normally modified after entityType loading. An Entity created at runtime (each Entity carries a clone of its EntityProperties) should not normally require its own unique copy of the LoadedShape or the LoadedAlternateShapes. Exceptionally, an entity needing to clone and modify LoadedShape or one of the LoadedAlternateShapes (e.g. EntityDressedHumanoid, or a boat furling and unfurling sails) should do so in its own custom OnTesselation() method, likely accessing the shape via EntityClientProperties.LoadedShapeForEntity.
        /// </summary>
        /// <returns></returns>
        public override EntitySidedProperties Clone()
        {
            Dictionary<string, AnimationMetaData> newAnimationsByMetaData = new Dictionary<string, AnimationMetaData>(StringComparer.OrdinalIgnoreCase);

            Dictionary<uint, AnimationMetaData> animsByCrc32 = new Dictionary<uint, AnimationMetaData>();

            AnimationMetaData[] newAnimations = null;
            if (Animations != null)
            {
                var oldAnimations = Animations;
                newAnimations = new AnimationMetaData[oldAnimations.Length];
                for (int i = 0; i < newAnimations.Length; i++)
                {
                    var clonedAnimation = oldAnimations[i].Clone();
                    newAnimations[i] = clonedAnimation;
                    if (AnimationsByMetaCode.ContainsKey(clonedAnimation.Code))    // Only include in newAnimationsByMetaData if it was in the old one (guess it always should be, but then you never know ...)
                    {
                        newAnimationsByMetaData[clonedAnimation.Code] = clonedAnimation;
                        animsByCrc32[clonedAnimation.CodeCrc32] = clonedAnimation;
                    }
                }
            }

            return new EntityClientProperties(BehaviorsAsJsonObj, null)
            {
                Textures = Textures == null ? null : new FastSmallDictionary<string, CompositeTexture>(Textures),
                TexturesAlternatesCount = TexturesAlternatesCount,
                RendererName = RendererName,
                GlowLevel = GlowLevel,
                PitchStep = PitchStep,
                Size = Size,
                SizeGrowthFactor = SizeGrowthFactor,
                Shape = Shape?.Clone(),
                LoadedAlternateShapes = this.LoadedAlternateShapes,
                Animations = newAnimations,
                AnimationsByMetaCode = newAnimationsByMetaData,
                AnimationsByCrc32 = animsByCrc32
            };
        }

        public virtual void FreeRAMServer()
        {
            // Save memory on server. The Textures are no longer needed, now that they have been packetised for sending to clients
            // (but we still need to retain the Animations data permanently on the server, and we also require to retain the Shape e.g. for adjustCollisionBoxToAnimations calls)
            var alternates = FirstTexture?.Alternates;
            TexturesAlternatesCount = alternates == null ? 0 : alternates.Length;
            Textures = null;
            if (Animations != null)
            {
                var AnimationsMetaData = this.Animations;
                for (int i = 0; i < AnimationsMetaData.Length; i++)
                {
                    AnimationsMetaData[i].DeDuplicate();
                }
            }
        }
    }

    public class EntityServerProperties : EntitySidedProperties
    {
        public EntityServerProperties(JsonObject[] behaviors, Dictionary<string, JsonObject> commonConfigs) : base(behaviors, commonConfigs) { }

        /// <summary>
        /// The conditions for spawning the entity.
        /// </summary>
        public SpawnConditions SpawnConditions;

        /// <summary>
        /// Makes a copy of this EntiyServerProperties type
        /// </summary>
        /// <returns></returns>
        public override EntitySidedProperties Clone()
        {
            return new EntityServerProperties(BehaviorsAsJsonObj, null)
            {
                Attributes = Attributes,
                SpawnConditions = SpawnConditions
            };
        }

        /// <summary>
        /// Returns a list of all entity behaviors that return true for ShouldEarlyLoadCollectibleMappings
        /// <br/>Note, these behaviors are temporarily created in this list only, not attached to the entity, and should be discarded by the caller
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="world"></param>
        /// <returns></returns>
        public List<EntityBehaviorAndConfig> BehaviorsWithEarlyLoadCollectibleMappings(Entity entity, IWorldAccessor world)
        {
            if (BehaviorsAsJsonObj == null) return null;
            var behaviors = new List<EntityBehaviorAndConfig>();

            for (int i = 0; i < BehaviorsAsJsonObj.Length; i++)
            {
                JsonObject jobj = BehaviorsAsJsonObj[i];
                string code = jobj["code"].AsString();

                if (world.ClassRegistry.GetEntityBehaviorClass(code) != null)
                {
                    EntityBehavior behavior = world.ClassRegistry.CreateEntityBehavior(entity, code);
                    if (behavior.ShouldEarlyLoadCollectibleMappings())
                    {
                        behavior.FromBytes(false);
                        behaviors.Add(new EntityBehaviorAndConfig(behavior,jobj));
                    }
                }
            }
            return behaviors;
        }
    }

    public class EntityBehaviorAndConfig
    {
        public EntityBehaviorAndConfig(EntityBehavior behavior, JsonObject behaviorConfig)
        {
            Behavior = behavior;
            BehaviorConfig = behaviorConfig;
        }

        public EntityBehavior Behavior;
        public JsonObject BehaviorConfig;
    }
}
