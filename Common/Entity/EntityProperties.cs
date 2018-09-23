using System;
using System.Collections.Generic;
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

        public AssetLocation Code;

        public string Class;

        /// <summary>
        /// Natural habitat of the entity. Decides whether to apply gravity or not
        /// </summary>
        public EnumHabitat Habitat = EnumHabitat.Land;

        public Vec2f HitBoxSize;

        /// <summary>
        /// How high the camera should be placed if this entity were to be controlled by the player
        /// </summary>
        internal double EyeHeight;

        public void SetEyeHeight(double height)
        {
            this.EyeHeight = height;
        }

        /// <summary>
        /// If true the entity can climb on walls
        /// </summary>
        public bool CanClimb;

        public bool CanClimbAnywhere;

        /// <summary>
        /// Whether the entity should take fall damage
        /// </summary>
        public bool FallDamage;

        public float ClimbTouchDistance;

        public bool RotateModelOnClimb;

        public float KnockbackResistance;

        /// <summary>
        /// Permanently stored entity attributes that are client and server side
        /// </summary>
        public JsonObject Attributes;

        public EntityClientProperties Client;

        public EntityServerProperties Server;

        public Dictionary<string, AssetLocation[]> Sounds;

        public float IdleSoundChance = 0.3f;

        public float IdleSoundRange = 24;

        public BlockDropItemStack[] Drops;

        public Cuboidf SpawnCollisionBox
        {
            get
            {
                return new Cuboidf()
                {
                    X1 = -HitBoxSize.X / 2,
                    Z1 = -HitBoxSize.X / 2,
                    X2 = HitBoxSize.X / 2,
                    Z2 = HitBoxSize.X / 2,
                    Y2 = HitBoxSize.Y
                };
            }
        }

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

            Dictionary<string, AssetLocation[]> csounds = new Dictionary<string, AssetLocation[]>();
            foreach (var val in Sounds)
            {
                AssetLocation[] locs = val.Value;
                csounds[val.Key] = new AssetLocation[locs.Length];
                for (int i = 0; i < locs.Length; i++)
                {
                    csounds[val.Key][i] = locs[i].Clone();
                }
            }

            return new EntityProperties()
            {
                Code = Code.Clone(),
                Class = Class,
                Habitat = Habitat,
                HitBoxSize = HitBoxSize,
                CanClimb = CanClimb,
                CanClimbAnywhere = CanClimbAnywhere,
                FallDamage = FallDamage,
                ClimbTouchDistance = ClimbTouchDistance,
                RotateModelOnClimb = RotateModelOnClimb,
                KnockbackResistance = KnockbackResistance,
                Attributes = Attributes?.Clone(),
                Sounds = new Dictionary<string, AssetLocation[]>(Sounds),
                IdleSoundChance = IdleSoundChance,
                IdleSoundRange = IdleSoundRange,
                Drops = DropsCopy,
                EyeHeight = EyeHeight,
                Client = Client?.Clone() as EntityClientProperties,
                Server = Server?.Clone() as EntityServerProperties
            };
        }

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

            Client?.Init(); // Init also on server
        }
    }

    public abstract class EntitySidedProperties
    {
        public List<EntityBehavior> Behaviors = new List<EntityBehavior>();
        internal JsonObject[] BehaviorsAsJsonObj;


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
                if (code == null) continue;

                EntityBehavior behavior = world.ClassRegistry.CreateEntityBehavior(entity, code);
                behavior.Initialize(properties, BehaviorsAsJsonObj[i]);
                Behaviors.Add(behavior);
            }
        }

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
        public Dictionary<string, CompositeTexture> Textures = new Dictionary<string, CompositeTexture>();

        /// <summary>
        /// Used by various renderers to retrieve the entities texture it should be drawn with
        /// </summary>
        public virtual CompositeTexture Texture
        {
            get { return (Textures == null || Textures.Count == 0) ? null : Textures.First().Value; }
        }

        public int GlowLevel = 0;

        public CompositeShape Shape;

        public float Size = 1f;

        public AnimationMetaData[] Animations;

        public Dictionary<string, AnimationMetaData> AnimationsByMetaCode = new Dictionary<string, AnimationMetaData>();


        public Shape LoadedShape;

        /// <summary>
        /// Returns the first texture in Textures dict
        /// </summary>
        public CompositeTexture FirstTexture { get { return (Textures == null || Textures.Count == 0) ? null : Textures.First().Value; } }

        public void LoadShape(EntityProperties entityType, ICoreClientAPI capi)
        {
            AssetLocation shapePath;
            Shape entityShape = null;

            // Not using a shape, it seems, so whatev ^_^
            if (Shape == null) return;

            if (Shape?.Base == null || Shape.Base.Path.Length == 0)
            {
                shapePath = new AssetLocation("shapes/block/basic/cube.json");
                if (Shape?.VoxelizeTexture != true)
                {
                    capi.World.Logger.Warning("No entity shape supplied for entity {0}, using cube shape", entityType.Code);
                }
                Shape.Base = new AssetLocation("block/basic/cube");
            }
            else
            {
                shapePath = Shape.Base.CopyWithPath("shapes/" + Shape.Base.Path + ".json");
            }

            IAsset asset = capi.Assets.TryGet(shapePath);

            if (asset == null)
            {
                capi.World.Logger.Error("Entity shape {0} for entity {1} not found, was supposed to be at {2}. Entity will be invisible!", Shape, entityType.Code, shapePath);
                return;
            }

            try
            {
                entityShape = asset.ToObject<Shape>();
            }
            catch (Exception e)
            {
                capi.World.Logger.Error("Exception thrown when trying to load entity shape {0} for entity {1}. Entity will be invisible! Exception: {2}", Shape, entityType.Code, e);
                return;
            }

            LoadedShape = entityShape;
            entityShape.ResolveReferences(capi.World.Logger, Shape.Base.ToString());

            CacheInvTransforms(LoadedShape.Elements);


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

        public void Init()
        {
            if (Animations != null)
            {
                for (int i = 0; i < Animations.Length; i++)
                {
                    AnimationMetaData animMeta = Animations[i];
                    if (animMeta.Animation != null) AnimationsByMetaCode[animMeta.Code] = animMeta;
                    animMeta.Init();
                }

            }
        }

        /*public void Bake(IAssetManager assetManager)
        {
            foreach (var val in Textures)
            {
                val.Value.Bake(assetManager);
            }
        }*/

        public override EntitySidedProperties Clone()
        {
            /*Dictionary<string, CompositeTexture> newTextures = new Dictionary<string, CompositeTexture>();
            foreach (var texture in Textures)
                newTextures[texture.Key] = texture.Value.Clone();*/

            AnimationMetaData[] newAnimations = null;
            if (Animations != null)
            {
                newAnimations = new AnimationMetaData[Animations.Length];
                for (int i = 0; i < newAnimations.Length; i++)
                    newAnimations[i] = Animations[i].Clone();
            }

            Dictionary<string, AnimationMetaData> newAnimationsByMetaData = new Dictionary<string, AnimationMetaData>();
            foreach (var animation in AnimationsByMetaCode)
                newAnimationsByMetaData[animation.Key] = animation.Value.Clone();
            
            return new EntityClientProperties(BehaviorsAsJsonObj.Clone() as JsonObject[])
            {
                Textures = Textures,
                RendererName = RendererName,
                GlowLevel = GlowLevel,
                Size = Size,
                Shape = Shape?.Clone(),
                Animations = newAnimations,
                AnimationsByMetaCode = newAnimationsByMetaData,
                LoadedShape = LoadedShape
            };
        }
    }

    public class EntityServerProperties : EntitySidedProperties
    {
        public EntityServerProperties(JsonObject[] behaviors) : base(behaviors) { }

        public ITreeAttribute Attributes;

        public SpawnConditions SpawnConditions;

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
