using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common.Entities
{
    public class EntityProperties
    {
        /// <summary>
        /// The entity code in the code.
        /// </summary>
        public AssetLocation Code;

        /// <summary>
        /// Variant values as resolved from blocktype/itemtype or entitytype
        /// </summary>
        public OrderedDictionary<string, string> Variant = new OrderedDictionary<string, string>();

        /// <summary>
        /// The classification of the entity.
        /// </summary>
        public string Class;

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

        /// <summary>
        /// Sets the eye height of the entity.
        /// </summary>
        /// <param name="height"></param>
        public void SetEyeHeight(double height)
        {
            this.EyeHeight = height;
        }

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
        /// The attributes of the entity.
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

            return new EntityProperties()
            {
                Code = Code.Clone(),
                Class = Class,
                Habitat = Habitat,
                CollisionBoxSize = CollisionBoxSize.Clone(),
                DeadCollisionBoxSize = DeadCollisionBoxSize.Clone(),
                SelectionBoxSize = SelectionBoxSize?.Clone(),
                DeadSelectionBoxSize = DeadSelectionBoxSize?.Clone(),
                CanClimb = CanClimb,
                Weight = Weight,
                CanClimbAnywhere = CanClimbAnywhere,
                FallDamage = FallDamage,
                ClimbTouchDistance = ClimbTouchDistance,
                RotateModelOnClimb = RotateModelOnClimb,
                KnockbackResistance = KnockbackResistance,
                Attributes = Attributes?.Clone(),
                Sounds = new Dictionary<string, AssetLocation>(Sounds),
                IdleSoundChance = IdleSoundChance,
                IdleSoundRange = IdleSoundRange,
                Drops = DropsCopy,
                EyeHeight = EyeHeight,
                Client = Client?.Clone() as EntityClientProperties,
                Server = Server?.Clone() as EntityServerProperties
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
                    if (val.Value.Path.EndsWith("*"))
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
        /// </summary>
        public List<EntityBehavior> Behaviors = new List<EntityBehavior>();


        
        public EntitySidedProperties(JsonObject[] behaviors)
        {
            this.BehaviorsAsJsonObj = behaviors;
        }

        internal void loadBehaviors(Entity entity, EntityProperties properties, IWorldAccessor world)
        {
            if (BehaviorsAsJsonObj == null) return;

            this.Behaviors.Clear();

            for (int i = 0; i < BehaviorsAsJsonObj.Length; i++)
            {
                string code = BehaviorsAsJsonObj[i]["code"].AsString();
                bool enabled = BehaviorsAsJsonObj[i]["enabled"].AsBool(true);

                if (!enabled)
                {
                    continue;
                }
                if (code == null) continue;

                if (world.ClassRegistry.GetEntityBehaviorClass(code) != null)
                {
                    EntityBehavior behavior = world.ClassRegistry.CreateEntityBehavior(entity, code);
                    Behaviors.Add(behavior);
                    behavior.Initialize(properties, BehaviorsAsJsonObj[i]);
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

        public EntityClientProperties(JsonObject[] behaviors) : base(behaviors) { }

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
        /// </summary>
        public IDictionary<string, CompositeTexture> Textures = new FakeDictionary<string, CompositeTexture>(0);

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
        /// Only loaded for World.EntityTypes instances of EntityProperties, because it makes no sense to have 1000 loaded entities needing to load 1000 shapes. During entity load/spawn this value is assigned however
        /// On the client it gets set by the EntityTextureAtlasManager
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


            var cprop = world.EntityTypes.FirstOrDefault(et => et.Code.Equals(entityTypeCode))?.Client;          
            LoadedShape = cprop?.LoadedShape;
            LoadedAlternateShapes = cprop?.LoadedAlternateShapes;
        }
        
        /// <summary>
        /// Does not clone textures, but does clone shapes
        /// </summary>
        /// <returns></returns>
        public override EntitySidedProperties Clone()
        {
            AnimationMetaData[] newAnimations = null;
            if (Animations != null)
            {
                newAnimations = new AnimationMetaData[Animations.Length];
                for (int i = 0; i < newAnimations.Length; i++)
                    newAnimations[i] = Animations[i].Clone();
            }

            Dictionary<string, AnimationMetaData> newAnimationsByMetaData = new Dictionary<string, AnimationMetaData>(StringComparer.OrdinalIgnoreCase);

            Dictionary<uint, AnimationMetaData> animsByCrc32 = new Dictionary<uint, AnimationMetaData>();

            foreach (var animation in AnimationsByMetaCode)
            {
                newAnimationsByMetaData[animation.Key] = animation.Value.Clone();
                animsByCrc32[newAnimationsByMetaData[animation.Key].CodeCrc32] = newAnimationsByMetaData[animation.Key];
            }

            Shape[] alternatesCloned = null;
            if (LoadedAlternateShapes != null)
            {
                alternatesCloned = new Shape[LoadedAlternateShapes.Length];
                for (int i = 0; i < alternatesCloned.Length; i++)
                {
                    alternatesCloned[i] = LoadedAlternateShapes[i].Clone();
                }
            }

            return new EntityClientProperties(BehaviorsAsJsonObj.Clone() as JsonObject[])
            {
                Textures = Textures,
                RendererName = RendererName,
                GlowLevel = GlowLevel,
                PitchStep = PitchStep,
                Size = Size,
                SizeGrowthFactor = SizeGrowthFactor,
                Shape = Shape?.Clone(),
                LoadedAlternateShapes = alternatesCloned,
                Animations = newAnimations,
                AnimationsByMetaCode = newAnimationsByMetaData,
                AnimationsByCrc32 = animsByCrc32
            };
        }
    }

    public class EntityServerProperties : EntitySidedProperties
    {
        public EntityServerProperties(JsonObject[] behaviors) : base(behaviors) { }

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
            return new EntityServerProperties(BehaviorsAsJsonObj.Clone() as JsonObject[])
            {
                Attributes = Attributes?.Clone(),
                SpawnConditions = SpawnConditions?.Clone()
            };
        }
    }
    
}
