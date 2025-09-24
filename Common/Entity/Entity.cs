using System.Collections.Generic;
using System.IO;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Config;
using System;
using Vintagestory.API.Util;
using System.Text;

#nullable disable
using static Vintagestory.API.Common.EntityAgent;
using System.Linq;

namespace Vintagestory.API.Common.Entities
{
    public interface IPettable
    {
        bool CanPet(Entity byEntity);
    }

    /// <summary>
    /// Called after a physics tick has happened
    /// </summary>
    /// <param name="accum">Amount of seconds left in the accumulator after physics ticking</param>
    /// <param name="prevPos"></param>
    public delegate void PhysicsTickDelegate(float accum, Vec3d prevPos);

    /// <summary>
    /// The basic class for all entities in the game
    /// </summary>
    public abstract class Entity : RegistryObject
    {
        public static WaterSplashParticles SplashParticleProps = new WaterSplashParticles();
        public static AdvancedParticleProperties[] FireParticleProps = new AdvancedParticleProperties[3];
        public static FloatingSedimentParticles FloatingSedimentParticles = new FloatingSedimentParticles();

        public static AirBubbleParticles AirBubbleParticleProps = new AirBubbleParticles();
        public static SimpleParticleProperties bioLumiParticles;
        public static NormalizedSimplexNoise bioLumiNoise;

        public event Action OnInitialized;

        static Entity()
        {
            // Ember cubicles
            FireParticleProps[0] = new AdvancedParticleProperties()
            {
                HsvaColor = new NatFloat[] { NatFloat.createUniform(30, 20), NatFloat.createUniform(255, 50), NatFloat.createUniform(255, 50), NatFloat.createUniform(255, 0) },
                GravityEffect = NatFloat.createUniform(0,0),
                Velocity = new NatFloat[] { NatFloat.createUniform(0.2f, 0.05f), NatFloat.createUniform(0.5f, 0.1f), NatFloat.createUniform(0.2f, 0.05f) },
                Size = NatFloat.createUniform(0.25f, 0),
                Quantity = NatFloat.createUniform(0.25f, 0),
                VertexFlags = 128,
                SizeEvolve = EvolvingNatFloat.create(EnumTransformFunction.QUADRATIC, -0.5f),
                SelfPropelled = true

            };

            // Fire particles
            FireParticleProps[1] = new AdvancedParticleProperties()
            {
                HsvaColor = new NatFloat[] { NatFloat.createUniform(30, 20), NatFloat.createUniform(255, 50), NatFloat.createUniform(255, 50), NatFloat.createUniform(255, 0) },
                OpacityEvolve = EvolvingNatFloat.create(EnumTransformFunction.QUADRATIC, -16),
                GravityEffect = NatFloat.createUniform(0, 0),
                Velocity = new NatFloat[] { NatFloat.createUniform(0f, 0.02f), NatFloat.createUniform(0f, 0.02f), NatFloat.createUniform(0f, 0.02f) },
                Size = NatFloat.createUniform(0.3f, 0.05f),
                Quantity = NatFloat.createUniform(0.25f, 0),
                VertexFlags = 128,
                SizeEvolve = EvolvingNatFloat.create(EnumTransformFunction.LINEAR, 1f),
                LifeLength = NatFloat.createUniform(0.5f, 0),
                ParticleModel = EnumParticleModel.Quad
            };

            // Smoke particles
            FireParticleProps[2] = new AdvancedParticleProperties()
            {
                HsvaColor = new NatFloat[] { NatFloat.createUniform(0, 0), NatFloat.createUniform(0, 0), NatFloat.createUniform(40, 30), NatFloat.createUniform(220, 50) },
                OpacityEvolve = EvolvingNatFloat.create(EnumTransformFunction.QUADRATIC, -16),
                GravityEffect = NatFloat.createUniform(0, 0),
                Velocity = new NatFloat[] { NatFloat.createUniform(0f, 0.05f), NatFloat.createUniform(0.2f, 0.3f), NatFloat.createUniform(0f, 0.05f) },
                Size = NatFloat.createUniform(0.3f, 0.05f),
                Quantity = NatFloat.createUniform(0.25f, 0),
                SizeEvolve = EvolvingNatFloat.create(EnumTransformFunction.LINEAR, 1.5f),
                LifeLength = NatFloat.createUniform(1.5f, 0),
                ParticleModel = EnumParticleModel.Quad,
                SelfPropelled = true,
            };


            bioLumiParticles = new SimpleParticleProperties()
            {
                Color = ColorUtil.ToRgba(255, 0, 230, 142),
                MinSize = 0.02f,
                MaxSize = 0.07f,
                MinQuantity = 1,
                GravityEffect = 0f,
                LifeLength = 1f,
                ParticleModel = EnumParticleModel.Quad,
                ShouldDieInAir = true,
                VertexFlags = (byte)255,
                MinPos = new Vec3d(),
                AddPos = new Vec3d()
            };

            bioLumiParticles.ShouldDieInAir = true;
            bioLumiParticles.OpacityEvolve = EvolvingNatFloat.create(EnumTransformFunction.LINEAR, -150);
            bioLumiParticles.MinSize = 0.02f;
            bioLumiParticles.MaxSize = 0.07f;

            bioLumiNoise = new NormalizedSimplexNoise(new double[] { 1, 0.5 }, new double[] { 5, 10 }, 097901);
        }

        /// <summary>
        /// Color used when the entity is being attacked
        /// </summary>
        protected int HurtColor = ColorUtil.ToRgba(255, 255, 100, 100);



        #region Entity Fields

        /// <summary>
        /// World where the entity is spawned in. Available on the game client and server.
        /// </summary>
        public IWorldAccessor World;

        /// <summary>
        /// The api, if you need it. Available on the game client and server.
        /// </summary>
        public ICoreAPI Api;

        /// <summary>
        /// Used by AItasks for perfomance. When searching for nearby entities we distinguish between (A) Creatures and (B) Inanimate entitie. Inanimate entities are items on the ground, projectiles, armor stands, rafts, falling blocks etc
        /// <br/>Note 1: Dead creatures / corpses count as a Creature. EntityPlayer is a Creature of course.
        /// <br/>Note 2: Straw Dummy we count as a Creature, because weapons can target it and bees can attack it. In contrast, Armor Stand we count as Inanimate, because nothing should ever attack or target it.
        /// </summary>
        public virtual bool IsCreature { get { return false; } }

        /// <summary>
        /// List of current entity tags. Synced after initialization. Synchronization can be triggered by calling 'MarkTagDirty()' method.
        /// <br/>Dont set on client side, unless you setting it to the same value on server side at the same time.
        /// <br/>If set only on server side, 'MarkTagDirty()' should be called to sync value with clients. This will trigger full entity synchronization, which can be performance heavy.
        /// <br/>Tags are not saved into save file, and always loaded from entity type when entity is created. Indexes of tags are dynamically assigned on game start, and are not consistent between saves.
        /// </summary>
        public EntityTagArray Tags = EntityTagArray.Empty;

        internal bool tagsDirty = false;

        public virtual bool CanStepPitch => Properties.Habitat != EnumHabitat.Air;
        public virtual bool CanSwivel => Properties.Habitat != EnumHabitat.Air && (Properties.Habitat != EnumHabitat.Land || !Swimming);
        public virtual bool CanSwivelNow => OnGround;

        public virtual bool CanAttackInside => false;

        /// <summary>
        /// The vanilla physics systems will call this method if a physics behavior was assigned to it. The game client for example requires this to be called for the current player to properly render the player. Available on the game client and server.
        /// </summary>
        public PhysicsTickDelegate PhysicsUpdateWatcher;

        /// <summary>
        /// Server simulated animations. Only takes care of stopping animations once they're done
        /// Set and Called by the Entities ServerSystem
        /// </summary>
        public virtual IAnimationManager AnimManager { get; set; }

        /// <summary>
        /// An uptime value running activities. Available on the game client and server. Not synchronized.
        /// </summary>
        public Dictionary<string, long> ActivityTimers = new Dictionary<string, long>();


        /// <summary>
        /// Client position
        /// </summary>
        public EntityPos Pos = new EntityPos();

        /// <summary>
        /// Server simulated position. May not exactly match the client positon
        /// </summary>
        public EntityPos ServerPos = new EntityPos();

        /// <summary>
        /// Server simulated position copy. Needed by Entities server system to send pos updatess only if ServerPos differs noticably from PreviousServerPos
        /// </summary>
        public EntityPos PreviousServerPos = new EntityPos();

        /// <summary>
        /// The position where the entity last had contact with the ground. Set by the game client and server.
        /// </summary>
        public Vec3d PositionBeforeFalling = new Vec3d();

        public long InChunkIndex3d;

        /// <summary>
        /// The entities collision box. Offseted by the animation system when necessary. Set by the game client and server.
        /// </summary>
        public Cuboidf CollisionBox;

        /// <summary>
        /// The entities collision box. Not Offseted. Set by the game client and server.
        /// </summary>
        public Cuboidf OriginCollisionBox;


        /// <summary>
        /// The entities selection box. Offseted by the animation system when necessary. Set by the game client and server.
        /// </summary>
        public Cuboidf SelectionBox;
        /// <summary>
        /// The entities selection box. Not Offseted. Set by the game client and server.
        /// </summary>
        public Cuboidf OriginSelectionBox;

        /// <summary>
        /// Used by the teleporter block
        /// </summary>
        public bool Teleporting;


        /// <summary>
        /// A unique identifier for this entity. Set by the game client and server.
        /// </summary>
        public long EntityId;

        /// <summary>
        /// The range in blocks the entity has to be to a client to do physics and AI. When outside range, then <seealso cref="State"/> will be set to inactive
        /// </summary>
        public int SimulationRange;

        /// <summary>
        /// The face the entity is climbing on. Null if the entity is not climbing. Set by the game client and server.
        /// </summary>
        public BlockFacing ClimbingOnFace;
        public BlockFacing ClimbingIntoFace;

        /// <summary>
        /// Set by the game client and server.
        /// </summary>
        public Cuboidf ClimbingOnCollBox;

        /// <summary>
        /// True if this entity is in touch with the ground. Set by the game client and server.
        /// </summary>
        public bool OnGround;

        /// <summary>
        /// True if the bottom of the collisionbox is inside a liquid. Set by the game client and server.
        /// </summary>
        public bool FeetInLiquid;

        public bool IsOnFire
        {
            get
            {
                return WatchedAttributes.GetBool("onFire");
            }
            set
            {
                if (value != WatchedAttributes.GetBool("onFire"))
                {
                    WatchedAttributes.SetBool("onFire", value);
                }
            }
        }
        protected bool resetLightHsv;

        public bool InLava;
        public long InLavaBeginTotalMs;

        public long OnFireBeginTotalMs;

        /// <summary>
        /// True if the collisionbox is 2/3rds submerged in liquid. Set by the game client and server.
        /// </summary>
        public bool Swimming;

        /// <summary>
        /// True if the entity is in touch with something solid on the vertical axis. Set by the game client and server.
        /// </summary>
        public bool CollidedVertically;

        /// <summary>
        /// True if the entity is in touch with something solid on both horizontal axes. Set by the game client and server.
        /// </summary>
        public bool CollidedHorizontally;

        /// <summary>
        /// The current entity state. NOT stored in WatchedAttributes in from/tobytes when sending to client as always set to Active on client-side Initialize().  Server-side if saved it would likely initially be Despawned when an entity is first loaded from the save due to entities being despawned during the UnloadChunks process, so let's make it always Despawned for consistent behavior (it will be set to Active/Inactive during Initialize() anyhow)
        /// </summary>
        public EnumEntityState State = EnumEntityState.Despawned;

        public EntityDespawnData DespawnReason;

        /// <summary>
        /// Permanently stored entity attributes that are sent to client everytime they have been changed
        /// </summary>
        public SyncedTreeAttribute WatchedAttributes = new SyncedTreeAttribute();

        /// <summary>
        /// If entity debug mode is on, this info will be transitted to client and displayed above the entities head
        /// </summary>
        public SyncedTreeAttribute DebugAttributes = new SyncedTreeAttribute();

        /// <summary>
        /// Permanently stored entity attributes that are only client or only server side
        /// </summary>
        public SyncedTreeAttribute Attributes = new SyncedTreeAttribute();


        /// <summary>
        /// Set by the client renderer when the entity was rendered last frame
        /// </summary>
        public bool IsRendered;

        /// <summary>
        /// Set by the client renderer when the entity shadow was rendered last frame
        /// </summary>
        public bool IsShadowRendered;

        public EntityStats Stats;
        protected float fireDamageAccum;

        /// <summary>
        /// Set during initialization by calling Entity.GetTouchDistance().  A mod wishing to change the value returned by .GetTouchDistance() dynamically should probably call .updateColSelBoxes() or update this field directly
        /// </summary>
        public double touchDistance;   // Here for performance
        public double touchDistanceSq;   // Here for performance
        [Obsolete("Unused but retained for mod API backwards compatibility")]
        public bool hasRepulseBehavior = false;
        [Obsolete("Unused but retained for mod API backwards compatibility")]
        public bool customRepulseBehavior = false;
        /// <summary>
        /// The Entity's BehaviorRepulseAgents, if it has one - here for critical performance reasons. This may also be accessed from Physics ticking
        /// </summary>
        public EntityBehavior BHRepulseAgents;   // Here for performance. This helps with the N-squared problem where every entity with BehaviorRepulseAgents needs to interact with the BehaviorRepulseAgents of every other nearby entity, it would be a costly waste of CPU time to iterate all the Behaviors for every other entity each tick searching for this Behavior
        /// <summary>
        /// Invoked once per server tick, immediately following the physics ticking, if this Entity has any of the Physics behaviors. Example use, anything which requires an update of the entity's current position, such as BehaviorRepulseAgents
        /// </summary>
        public Action AfterPhysicsTick;
        /// <summary>
        /// Used by PhysicsManager. Added here to increase performance
        /// 0 = not tracked, 1 = lowResTracked, 2 = fullyTracked
        /// </summary>
        public byte IsTracked;
        /// <summary>
        /// Used by PhysicsManager. Added here to increase performance
        /// Set to true when position packet prepared during physics ticking, set to false following AfterPhysicsTick()
        /// </summary>
        public bool PositionTicked;
        /// <summary>
        /// Used by the PhysicsManager to tell connected clients that the next entity position packet should not have its position change get interpolated. Gets set to false after the packet was sent
        /// </summary>
        public bool IsTeleport;

        /// <summary>
        /// If true, will call EntityBehavior.IntersectsRay. Default off to increase performance.
        /// </summary>
        public bool trickleDownRayIntersects;
        /// <summary>
        /// If true, will fully simulate animations on the server so one has access to the positions of all attachment points.
        /// If false, only root level attachment points will be available server side
        /// </summary>
        public bool requirePosesOnServer;

        /// <summary>
        /// Used for efficiency in multi-player servers, to avoid regenerating the packet again for each connected client
        /// </summary>
        public object packet;

        /// <summary>
        /// Used for efficiency in multi-player servers, to speed up iterating over relevant behaviors only. These are the behaviors returning Threadsafe == false (which is the default)
        /// </summary>
        public EntityBehavior[] ServerBehaviorsMainThread;

        /// <summary>
        /// Used for efficiency in multi-player servers, to speed up iterating over relevant behaviors only. These are the behaviors returning Threadsafe == true;
        /// These behaviors may have their regular ticking (including ShouldExecute() etc) called EITHER from the main thread OR from another physics thread, depending on PhysicsManager logic
        /// </summary>
        public EntityBehavior[] ServerBehaviorsThreadsafe;

        /// <summary>
        /// Used only when deserialising an entity, otherwise null
        /// </summary>
        private Dictionary<string, string> codeRemaps;
        #endregion

        #region Properties

        public EntityProperties Properties { protected set; get; }

        public EntitySidedProperties SidedProperties
        {
            get
            {
                if (Properties == null) return null;
                if (World.Side.IsClient()) return Properties.Client;

                return Properties.Server;
            }
        }

        /// <summary>
        /// Should return true when this entity should be interactable by a player or other entities
        /// </summary>
        public virtual bool IsInteractable
        {
            get { return true; }
        }


        /// <summary>
        /// Used for passive physics simulation, together with the MaterialDensity to check how deep in the water the entity should float
        /// </summary>
        public virtual double SwimmingOffsetY
        {
            get { return SelectionBox.Y1 + SelectionBox.Y2 * 0.66; }
        }


        /// <summary>
        /// CollidedVertically || CollidedHorizontally
        /// </summary>
        public bool Collided { get { return CollidedVertically || CollidedHorizontally; } }

        /// <summary>
        /// ServerPos on server, Pos on client
        /// </summary>
        public EntityPos SidedPos
        {
            get { return World.Side == EnumAppSide.Server ? ServerPos : Pos; }
        }

        /// <summary>
        /// The height of the eyes for the given entity.
        /// </summary>
        public virtual Vec3d LocalEyePos { get; set; } = new Vec3d();


        /// <summary>
        /// If gravity should applied to this entity
        /// </summary>
        public virtual bool ApplyGravity
        {
            get { return Properties.Habitat == EnumHabitat.Land || (Properties.Habitat == EnumHabitat.Sea || Properties.Habitat == EnumHabitat.Underwater) && !Swimming; }
        }


        /// <summary>
        /// Determines on whether an entity floats on liquids or not and how strongly items get pushed by water. Water has a density of 1000.
        /// A density below 1000 means the entity floats on top of water if has a physics simulation behavior attached to it.
        /// </summary>
        public virtual float MaterialDensity
        {
            get { return 3000; }
        }

        /// <summary>
        /// If set, the entity will emit dynamic light
        /// </summary>
        public virtual byte[] LightHsv { get; set; } = null;


        /// <summary>
        /// If the entity should despawn next server tick. By default returns !Alive for non-creatures and creatures that don't have a Decay behavior
        /// </summary>
        public virtual bool ShouldDespawn
        {
            get { return !Alive; }
        }

        /// <summary>
        /// Players and whatever the player rides on will be stored seperatly
        /// </summary>
        public virtual bool StoreWithChunk { get { return true; } }

        public virtual bool AllowOutsideLoadedRange => false;

        /// <summary>
        /// Whether this entity should always stay in Active model, regardless on how far away other player are
        /// </summary>
        public virtual bool AlwaysActive { get; set; } = false;

        /// <summary>
        /// True if the entity is in state active or inactive, or generally not dead (for non-living entities, 'dead' means ready to despawn)
        /// </summary>
        public virtual bool Alive
        {
            get { return alive; /* Updated client-side from the WatchedAttributes every game tick. Updates to alive status on the server may therefore lag by up to one tick, client-side. */ }
            set {
                WatchedAttributes.SetInt("entityDead", value ? 0 : 1); alive = value;
            }
        }
        protected bool alive=true;
        // Distance to the nearest player, updated every 1.5s
        public float NearestPlayerDistance;

        public virtual bool AdjustCollisionBoxToAnimation => !alive || AnimManager.AdjustCollisionBoxToAnimation;

        public float IdleSoundChanceModifier
        {
            get { return WatchedAttributes.GetFloat("idleSoundChanceModifier", 1); }
            set { WatchedAttributes.SetFloat("idleSoundChanceModifier", value); }
        }

        /// <summary>
        /// Used by some renderers to apply an overal color tint on the entity
        /// </summary>
        public int RenderColor { get; set; } = ColorUtil.WhiteArgb;

        /// <summary>
        /// A small offset used to prevent players from clipping through the blocks above ladders: relevant if the entity's collision box is sometimes adjusted by the game code
        /// </summary>
        public virtual double LadderFixDelta { get { return 0D; } }

        /// <summary>
        /// The chance that this entity, walking or jumping on or falling onto a block, will trigger a block-update - so potentially unstable dirt or sand falling, avalanche etc.
        /// </summary>
        public virtual float ImpactBlockUpdateChance { get; set; }   // (Here for performance reasons! radfast 6.2.25  This is read every tick for every entity on the ground, maybe even more than once if the entity is covering 2 or 4 blocks. See Block.OnEntityCollide().)

        #endregion


        /// <summary>
        /// Creates a new instance of an entity
        /// </summary>
        public Entity()
        {
            SimulationRange = GlobalConstants.DefaultSimulationRange;
            AnimManager = new AnimationManager();
            Stats = new EntityStats(this);
            WatchedAttributes.SetAttribute("animations", new TreeAttribute());
            WatchedAttributes.SetAttribute("extraInfoText", new TreeAttribute());
        }

        /// <summary>
        /// Creates a minimally populated entity with configurable tracking range, no Stats, no AnimManager and no animations attribute. Currently used by EntityItem.
        /// </summary>
        /// <param name="trackingRange"></param>
        protected Entity(int trackingRange)
        {
            SimulationRange = trackingRange;
            WatchedAttributes.SetAttribute("extraInfoText", new TreeAttribute());
        }

        /// <summary>
        /// Called when the entity got hurt. On the client side, dmgSource is null
        /// </summary>
        /// <param name="dmgSource"></param>
        /// <param name="damage"></param>
        public virtual void OnHurt(DamageSource dmgSource, float damage)
        {

        }


        /// <summary>
        /// Called when this entity got created or loaded
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="api"></param>
        /// <param name="InChunkIndex3d"></param>
        public virtual void Initialize(EntityProperties properties, ICoreAPI api, long InChunkIndex3d)
        {
            World = api.World;
            Api = api;
            Properties = properties;
            Class = properties.Class;
            this.InChunkIndex3d = InChunkIndex3d;
            Tags = properties.Tags;

            Stats.Initialize(api);
            alive = WatchedAttributes.GetInt("entityDead", 0) == 0;
            WatchedAttributes.SetFloat("onHurt", 0);
            int onHurtCounter = WatchedAttributes.GetInt("onHurtCounter");
            WatchedAttributes.RegisterModifiedListener("onHurt", () => {
                float damage = WatchedAttributes.GetFloat("onHurt", 0);
                if (damage == 0) return;
                int newOnHurtCounter = WatchedAttributes.GetInt("onHurtCounter");
                if (newOnHurtCounter == onHurtCounter) return;

                onHurtCounter = newOnHurtCounter;

                if (Attributes.GetInt("dmgkb") == 0)
                {
                    Attributes.SetInt("dmgkb", 1);
                }

                if (damage > 0.05)
                {
                    SetActivityRunning("invulnerable", 500);
                    // Gets already called on the server directly
                    if (World.Side == EnumAppSide.Client)
                    {
                        OnHurt(null, WatchedAttributes.GetFloat("onHurt", 0));
                    }
                }
            });

            WatchedAttributes.RegisterModifiedListener("onFire", updateOnFire);
            WatchedAttributes.RegisterModifiedListener("entityDead", updateColSelBoxes);

            if (World.Side == EnumAppSide.Client && Properties.Client.SizeGrowthFactor != 0)
            {
                WatchedAttributes.RegisterModifiedListener("grow", () =>
                {
                    if (World != null && Properties != null)   // The following code can crash if the entity is not yet initialised e.g. during updating from packets (see SyncedTreeAttribute.FromBytes() which calls all listeners)
                    {
                        float factor = Properties.Client.SizeGrowthFactor;
                        if (factor != 0)
                        {
                            var origc = World.GetEntityType(this.Code).Client;
                            Properties.Client.Size = origc.Size + WatchedAttributes.GetTreeAttribute("grow").GetFloat("age") * factor;
                        }
                    }
                });
            }

            /*if (WatchedAttributes.HasAttribute("generation"))
            {
                int generation = WatchedAttributes.GetAsInt("generation");
                if (generation >= 10)
                {
                    EntityTagArray domesticatedTag = Api.TagRegistry.EntityTagsToTagArray("domesticated");
                    Tags |= domesticatedTag;
                }
            }*/

            if (Properties.CollisionBoxSize != null || properties.SelectionBoxSize != null)
            {
                updateColSelBoxes();
            }

            DoInitialActiveCheck(api);

            if (api.Side == EnumAppSide.Server)
            {
                if (properties.Client != null && properties.Client.TexturesAlternatesCount > 0 && !WatchedAttributes.HasAttribute("textureIndex"))
                {
                    WatchedAttributes.SetInt("textureIndex", World.Rand.Next(properties.Client.TexturesAlternatesCount + 1));
                }
            }

            this.Properties.Initialize(this, api);

            Properties.Client.DetermineLoadedShape(EntityId);

            if (api.Side == EnumAppSide.Server)
            {
                AnimManager.LoadAnimator(api, this, properties.Client.LoadedShapeForEntity, null, requirePosesOnServer, "head");
                AnimManager.OnServerTick(0);
            } else
            {
                AnimManager.Init(api, this); // Fully initialized in entity.OnTesselation()
            }

            LocalEyePos.Y = Properties.EyeHeight;

            // entity height:
            //   Player: 1.85
            //   Chicken: 0.6  - reduce chance to 0.05f (!)
            //   Hare: 0.5     - reduces chance to 0
            //   Fox: 0.75     - same chance as player (!)
            float chanceReduction = Math.Max(0, 0.75f - CollisionBox.Height);
            ImpactBlockUpdateChance = Attributes.GetFloat("impactBlockUpdateChance", 0.2f - chanceReduction);

            TriggerOnInitialized();
        }

        public virtual void AfterInitialized(bool onFirstSpawn)
        {
            touchDistance = GetTouchDistance();

            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                behavior.AfterInitialized(onFirstSpawn);
            }

            if (Api is ICoreServerAPI) CacheServerBehaviors();
        }

        protected void TriggerOnInitialized()
        {
            OnInitialized?.Invoke();
        }

        protected void DoInitialActiveCheck(ICoreAPI api)
        {
            if (AlwaysActive || api.Side == EnumAppSide.Client)
            {
                State = EnumEntityState.Active;
            }
            else
            {
                State = EnumEntityState.Inactive;

                IPlayer[] players = World.AllOnlinePlayers;
                for (int i = 0; i < players.Length; i++)
                {
                    EntityPlayer entityPlayer = players[i].Entity;
                    if (entityPlayer == null) continue;

                    if (Pos.InRangeOf(entityPlayer.Pos, SimulationRange * SimulationRange))
                    {
                        State = EnumEntityState.Active;
                        break;
                    }
                }
            }
        }

        public virtual bool InRangeOf(Vec3d position, float horRangeSq, float vertRange)
        {
            return SidedPos.InRangeOf(position, horRangeSq, vertRange);
        }

        protected void updateColSelBoxes()
        {
            bool alive = WatchedAttributes.GetInt("entityDead", 0) == 0;

            if (alive || Properties.DeadCollisionBoxSize == null)
            {
                SetCollisionBox(Properties.CollisionBoxSize.X, Properties.CollisionBoxSize.Y);

                var selboxs = Properties.SelectionBoxSize ?? Properties.CollisionBoxSize;
                SetSelectionBox(selboxs.X, selboxs.Y);
            }
            else
            {
                SetCollisionBox(Properties.DeadCollisionBoxSize.X, Properties.DeadCollisionBoxSize.Y);

                var selboxs = Properties.DeadSelectionBoxSize ?? Properties.DeadCollisionBoxSize;
                SetSelectionBox(selboxs.X, selboxs.Y);
            }

            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                behavior.UpdateColSelBoxes();
            }

            double touchdist = GetTouchDistance();
            touchDistance = touchdist;
            touchDistanceSq = touchdist * touchdist;
        }

        protected void updateOnFire()
        {
            bool onfire = IsOnFire;
            if (onfire)
            {
                OnFireBeginTotalMs = World.ElapsedMilliseconds;
            }

            if (onfire && LightHsv == null)
            {
                LightHsv = new byte[] { 5, 7, 10 };
                resetLightHsv = true;
            }
            if (!onfire && resetLightHsv)
            {
                LightHsv = null;
            }
        }


        /// <summary>
        /// Called when something tries to given an itemstack to this entity
        /// </summary>
        /// <param name="itemstack"></param>
        /// <returns></returns>
        public virtual bool TryGiveItemStack(ItemStack itemstack)
        {
            EnumHandling handled = EnumHandling.PassThrough;

            bool ok = false;
            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                ok |= behavior.TryGiveItemStack(itemstack, ref handled);
                if (handled == EnumHandling.PreventSubsequent) return ok;
            }

            return ok;
        }




        /// <summary>
        /// Is called before the entity is killed, should return what items this entity should drop. Return null or empty array for no drops.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        /// <param name="byPlayer"></param>
        /// <returns></returns>
        public virtual ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer)
        {
            EnumHandling handled = EnumHandling.PassThrough;
            ItemStack[] stacks = null;

            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                stacks = behavior.GetDrops(world, pos, byPlayer, ref handled);
                if (handled == EnumHandling.PreventSubsequent) return stacks;
            }

            if (handled == EnumHandling.PreventDefault) return stacks;

            if (Properties.Drops == null) return null;
            List<ItemStack> todrop = new List<ItemStack>();

            float dropMul = 1;

            if (Properties.Attributes?["isMechanical"].AsBool() != true && byPlayer?.Entity != null)
            {
                dropMul = 1 + byPlayer.Entity.Stats.GetBlended("animalLootDropRate");
            }

            for (int i = 0; i < Properties.Drops.Length; i++)
            {
                BlockDropItemStack bdStack = Properties.Drops[i];

                float extraMul = 1f;
                if (bdStack.DropModbyStat != null && byPlayer?.Entity != null)
                {
                    // If the stat does not exist, then GetBlended returns 1 \o/
                    extraMul = byPlayer.Entity.Stats.GetBlended(bdStack.DropModbyStat);
                }

                ItemStack stack = bdStack.GetNextItemStack(dropMul * extraMul);
                if (stack == null) continue;

                if (stack.Collectible is IResolvableCollectible irc)
                {
                    var slot = new DummySlot(stack);
                    irc.Resolve(slot, world);
                    stack = slot.Itemstack;
                }

                todrop.Add(stack);
                if (bdStack.LastDrop) break;
            }

            return todrop.ToArray();
        }

        /// <summary>
        /// Teleports the entity to given position. Actual teleport is delayed until target chunk is loaded.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="onTeleported"></param>
        public virtual void TeleportToDouble(double x, double y, double z, Action onTeleported = null)
        {
            Teleporting = true;

            ICoreServerAPI sapi = this.World.Api as ICoreServerAPI;
            if (sapi != null)
            {
                sapi.WorldManager.LoadChunkColumnPriority((int)x / GlobalConstants.ChunkSize, (int)z / GlobalConstants.ChunkSize, new ChunkLoadOptions() {  OnLoaded = () =>
                    {
                        IsTeleport = true;
                        Pos.SetPos(x, y, z);
                        ServerPos.SetPos(x, y, z);
                        PositionBeforeFalling.Set(x, y, z);
                        Pos.Motion.Set(0, 0, 0);
                        onTeleported?.Invoke();
                        Teleporting = false;
                    }
                });

            }
        }

        /// <summary>
        /// Teleports the entity to given position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public virtual void TeleportTo(int x, int y, int z)
        {
            TeleportToDouble(x, y, z);
        }

        /// <summary>
        /// Teleports the entity to given position
        /// </summary>
        /// <param name="position"></param>
        public virtual void TeleportTo(Vec3d position)
        {
            TeleportToDouble(position.X, position.Y, position.Z);
        }

        /// <summary>
        /// Teleports the entity to given position
        /// </summary>
        /// <param name="position"></param>
        public virtual void TeleportTo(BlockPos position)
        {
            TeleportToDouble(position.X, position.Y, position.Z);
        }

        /// <summary>
        /// Teleports the entity to given position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="onTeleported"></param>
        public virtual void TeleportTo(EntityPos position, Action onTeleported = null)
        {
            Pos.Yaw = position.Yaw;
            Pos.Pitch = position.Pitch;
            Pos.Roll = position.Roll;
            ServerPos.Yaw = position.Yaw;
            ServerPos.Pitch = position.Pitch;
            ServerPos.Roll = position.Roll;

            TeleportToDouble(position.X, position.Y, position.Z, onTeleported);
        }



        /// <summary>
        /// Called when the entity should be receiving damage from given source
        /// </summary>
        /// <param name="damageSource"></param>
        /// <param name="damage"></param>
        /// <returns>True if the entity actually received damage</returns>
        public virtual bool ReceiveDamage(DamageSource damageSource, float damage)
        {
            if ((!Alive || IsActivityRunning("invulnerable") && !damageSource.IgnoreInvFrames) && damageSource.Type != EnumDamageType.Heal) return false;

            if (ShouldReceiveDamage(damageSource, damage)) {
                foreach (EntityBehavior behavior in SidedProperties.Behaviors)
                {
                    behavior.OnEntityReceiveDamage(damageSource, ref damage);
                }

                if (damageSource.Type != EnumDamageType.Heal && damage > 0)
                {
                    WatchedAttributes.SetInt("onHurtCounter", WatchedAttributes.GetInt("onHurtCounter") + 1);
                    WatchedAttributes.SetFloat("onHurt", damage); // Causes the client to be notified
                    if (damage > 0.05f)
                    {
                        AnimManager.StartAnimation("hurt");
                    }
                }

                if (damageSource.GetSourcePosition() != null)
                {
                    bool verticalAttack = false;
                    if (damageSource.GetAttackAngle(Pos.XYZ, out var attackYaw, out var attackPitch))
                    {
                        verticalAttack = Math.Abs(attackPitch) > 80 * GameMath.DEG2RAD || Math.Abs(attackPitch) < 10 * GameMath.DEG2RAD;
                    }

                    Vec3d dir = (SidedPos.XYZ - damageSource.GetSourcePosition()).Normalize();

                    if (verticalAttack)
                    {
                        dir.Y = 0.05f;
                        dir.Normalize();
                    } else
                    {
                        dir.Y = 0.7f;
                    }

                    dir.Y /= damageSource.YDirKnockbackDiv;
                    float factor = damageSource.KnockbackStrength * GameMath.Clamp((1 - Properties.KnockbackResistance) / 10f, 0, 1);

                    WatchedAttributes.SetFloat("onHurtDir", (float)Math.Atan2(dir.X, dir.Z));
                    WatchedAttributes.SetDouble("kbdirX", dir.X * factor);
                    WatchedAttributes.SetDouble("kbdirY", dir.Y * factor);
                    WatchedAttributes.SetDouble("kbdirZ", dir.Z * factor);
                } else
                {
                    WatchedAttributes.SetDouble("kbdirX", 0);
                    WatchedAttributes.SetDouble("kbdirY", 0);
                    WatchedAttributes.SetDouble("kbdirZ", 0);
                    WatchedAttributes.SetFloat("onHurtDir", -999);
                }

                return damage > 0;
            }

            return false;
        }


        /// <summary>
        /// Should return true if the entity can get damaged by given damageSource. Is called by ReceiveDamage.
        /// </summary>
        /// <param name="damageSource"></param>
        /// <param name="damage"></param>
        /// <returns></returns>
        public virtual bool ShouldReceiveDamage(DamageSource damageSource, float damage)
        {
            return true;
        }


        int prevInvulnerableTime;
        /// <summary>
        /// Called every 1/75 second
        /// </summary>
        /// <param name="dt"></param>
        public virtual void OnGameTick(float dt)
        {
            var World = this.World;
            if (World.EntityDebugMode) {
                UpdateDebugAttributes();
                DebugAttributes.MarkAllDirty();
            }

            if (World.Side == EnumAppSide.Client)
            {
                int val = RemainingActivityTime("invulnerable");
                if (prevInvulnerableTime != val)
                {
                    RenderColor = ColorUtil.ColorOverlay(HurtColor, ColorUtil.WhiteArgb, 1f - val / 500f);
                    prevInvulnerableTime = val;
                }

                alive = WatchedAttributes.GetInt("entityDead", 0) == 0;

                if (World.FrameProfiler.Enabled)
                {
                    World.FrameProfiler.Enter("behaviors");
                    foreach (EntityBehavior behavior in Properties.Client.Behaviors)
                    {
                        behavior.OnGameTick(dt);
                        World.FrameProfiler.Mark(behavior.ProfilerName);
                    }

                    World.FrameProfiler.Leave();
                }
                else
                {
                    foreach (EntityBehavior behavior in Properties.Client.Behaviors)
                    {
                        behavior.OnGameTick(dt);
                    }
                }

                if (World.Rand.NextDouble() < IdleSoundChanceModifier * Properties.IdleSoundChance / 100.0 && Alive)
                {
                    PlayEntitySound("idle", null, true, Properties.IdleSoundRange);
                }
            }
            else   // Serverside
            {
                if (requirePosesOnServer && !shapeFresh)
                {
                    CompositeShape compositeShape = Properties.Client.Shape;
                    Shape entityShape = Properties.Client.LoadedShapeForEntity;

                    if (entityShape != null)
                    {
                        OnTesselation(ref entityShape, compositeShape.Base.ToString());
                        OnTesselated();
                    }
                }

                // We include this check in case a mod changed the Behaviors without using .AddBehavior() or .RemoveBehavior()
                if (Properties.Server.Behaviors.Count != ServerBehaviorsMainThread.Length + ServerBehaviorsThreadsafe.Length) CacheServerBehaviors();

                var serverBehaviors = ServerBehaviorsMainThread;     // Thread-safe behaviors will be ticked from PhysicsManager
                if (World.FrameProfiler.Enabled)
                {
                    var profiler = World.FrameProfiler;
                    profiler.Enter("behaviors");
                    for (int i = 0; i < serverBehaviors.Length; i++)
                    {
                        EntityBehavior behavior = serverBehaviors[i];
                        behavior.OnGameTick(dt);
                        profiler.Mark(behavior.ProfilerName);
                    }
                    profiler.Leave();
                }
                else
                {
                    for (int i = 0; i < serverBehaviors.Length; i++)
                    {
                        serverBehaviors[i].OnGameTick(dt);
                    }
                }

                if (InLava) Ignite();

            }


            if (IsOnFire)
            {
                Block fluidBlock = World.BlockAccessor.GetBlock(Pos.AsBlockPos, BlockLayersAccess.Fluid);
                if (((fluidBlock.IsLiquid() && fluidBlock.LiquidCode != "lava") || World.ElapsedMilliseconds - OnFireBeginTotalMs > 12000) && !InLava)
                {
                    IsOnFire = false;
                }
                else
                {

                    if (World.Side == EnumAppSide.Client)
                    {
                        int index = Math.Min(FireParticleProps.Length - 1, Api.World.Rand.Next(FireParticleProps.Length + 1));
                        AdvancedParticleProperties particles = FireParticleProps[index];
                        particles.basePos.Set(Pos.X, Pos.InternalY + SelectionBox.YSize / 2, Pos.Z);

                        particles.PosOffset[0].var = SelectionBox.XSize / 2;
                        particles.PosOffset[1].var = SelectionBox.YSize / 2;
                        particles.PosOffset[2].var = SelectionBox.ZSize / 2;
                        particles.Velocity[0].avg = (float)Pos.Motion.X * 10;
                        particles.Velocity[1].avg = (float)Pos.Motion.Y * 5;
                        particles.Velocity[2].avg = (float)Pos.Motion.Z * 10;
                        particles.Quantity.avg = GameMath.Sqrt(particles.PosOffset[0].var + particles.PosOffset[1].var + particles.PosOffset[2].var) * (index == 0 ? 0.5f : (index == 1 ? 3 : 1.25f));
                        Api.World.SpawnParticles(particles);
                    }
                    else
                    {
                        ApplyFireDamage(dt);
                    }

                    if (!alive && InLava && !(this is EntityPlayer))
                    {
                        DieInLava();
                    }
                }
            }

            if (World.Side == EnumAppSide.Server && State == EnumEntityState.Active)
            {
                try
                {
                    AnimManager.OnServerTick(dt);
                }
                catch (Exception)
                {
                    World.Logger.Error("Error ticking animations for entity " + Code.ToShortString() + " at " + SidedPos.AsBlockPos);
                    throw;
                }

            World.FrameProfiler.Mark("entity-animation-ticking");
        }
        }

        protected void ApplyFireDamage(float dt)
        {
            fireDamageAccum += dt;
            if (fireDamageAccum > 1f)
            {
                ReceiveDamage(new DamageSource() { Source = EnumDamageSource.Internal, Type = EnumDamageType.Fire }, 0.5f);
                fireDamageAccum = 0;
            }
        }

        protected void DieInLava()
        {
            float q = GameMath.Clamp(SelectionBox.XSize * SelectionBox.YSize * SelectionBox.ZSize * 150, 10, 150);
            Api.World.SpawnParticles(
                q,
                ColorUtil.ColorFromRgba(20, 20, 20, 255),
                new Vec3d(ServerPos.X + SelectionBox.X1, ServerPos.InternalY + SelectionBox.Y1, ServerPos.Z + SelectionBox.Z1),
                new Vec3d(ServerPos.X + SelectionBox.X2, ServerPos.InternalY + SelectionBox.Y2, ServerPos.Z + SelectionBox.Z2),
                new Vec3f(-1f, -1f, -1f),
                new Vec3f(2f, 2f, 2f),
                2, 1, 1, EnumParticleModel.Cube
            );

            Die(EnumDespawnReason.Combusted);
        }

        public virtual void OnAsyncParticleTick(float dt, IAsyncParticleManager manager)
        {

        }


        public virtual void Ignite()
        {
            IsOnFire = true;
        }


        #region Events

        public virtual ITexPositionSource GetTextureSource()
        {
            if (Api.Side != EnumAppSide.Client) return null;

            ITexPositionSource texSource = null;

            var bhs = Properties.Client?.Behaviors;
            EnumHandling handling = EnumHandling.PassThrough;
            if (bhs != null)
            {
                foreach (var bh in bhs)
                {
                    texSource = bh.GetTextureSource(ref handling);
                    if (handling == EnumHandling.PreventSubsequent) return texSource;
                }
            }

            if (handling == EnumHandling.PreventDefault) return texSource;

            int altTexNumber = WatchedAttributes.GetInt("textureIndex", 0);
            return (Api as ICoreClientAPI).Tesselator.GetTextureSource(this, null, altTexNumber);
        }


        public bool ShapeFresh => shapeFresh;
        public virtual double FrustumSphereRadius => Math.Max(3, Math.Max(SelectionBox?.XSize ?? 1, SelectionBox?.YSize ?? 1));

        protected bool shapeFresh;

        public virtual void MarkShapeModified()
        {
            shapeFresh = false;
        }


        /// <summary>
        /// Called by EntityShapeRenderer.cs before tesselating the entity shape
        /// </summary>
        /// <param name="entityShape"></param>
        /// <param name="shapePathForLogging"></param>
        public virtual void OnTesselation(ref Shape entityShape, string shapePathForLogging)
        {
            shapeFresh = true;

            try
            {
                bool shapeIsCloned = false;
                OnTesselation(ref entityShape, shapePathForLogging, ref shapeIsCloned);

                if (shapeIsCloned)
                {
                    // Clear cached values. ClientAnimator regenerates these
                    if (entityShape.Animations != null)
                    {
                        foreach (var anim in entityShape.Animations)
                        {
                            anim.PrevNextKeyFrameByFrame = null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Api.Logger.Warning(new Exception("Cannot tesselate shape " + shapePathForLogging + " for entity " + Code.ToShortString() + ", may be invisible", e));
            }
        }

        protected virtual void OnTesselation(ref Shape entityShape, string shapePathForLogging, ref bool shapeIsCloned)
        {
            shapeFresh = true;
            var cshape = Properties.Client.Shape;
            if (cshape?.Overlays != null && cshape.Overlays.Length > 0)
            {
                shapeIsCloned = true;
                entityShape = entityShape.Clone();

                var textures = Properties.Client.Textures;

                foreach (var overlay in cshape.Overlays)
                {
                    var shape = Api.Assets.TryGet(overlay.Base.WithPathPrefixOnce("shapes/").WithPathAppendixOnce(".json"))?.ToObject<Shape>();
                    if (shape == null)
                    {
                        Api.Logger.Error("Entity {0} defines a shape overlay {1}, but no such file found. Will ignore.", this.Code, overlay.Base);
                        continue;
                    }

                    string texturePrefixCode = null;

                    if (Properties.Attributes?["wearableTexturePrefixCode"].Exists == true)
                    {
                        texturePrefixCode = Properties.Attributes["wearableTexturePrefixCode"].AsString();
                    }

                    entityShape.StepParentShape(shape, overlay.Base.ToShortString(), shapePathForLogging, Api.Logger, (texcode, tloc) =>
                    {
                        if (Api is ICoreClientAPI capi)
                        {
                            // Entity textures take precedence over shape textures
                            if (texturePrefixCode == null && textures.ContainsKey(texcode)) return;

                            var cmpt = textures[texturePrefixCode + "-" + texcode] = new CompositeTexture(tloc);
                            cmpt.Bake(Api.Assets);
                            capi.EntityTextureAtlas.GetOrInsertTexture(cmpt.Baked.TextureFilenames[0], out int textureSubid, out _);
                            cmpt.Baked.TextureSubId = textureSubid;
                        }
                    });
                }
            }

            string[] willDisableElements = null;
            if (Properties.Attributes?["disableElements"].Exists == true)
            {
                willDisableElements = Properties.Attributes["disableElements"].AsArray<string>();
            }

            var bhs = World.Side == EnumAppSide.Server ? Properties.Server?.Behaviors : Properties.Client?.Behaviors;
            EnumHandling handling = EnumHandling.PassThrough;
            if (bhs != null)
            {
                foreach (var bh in bhs)
                {
                    bh.OnTesselation(ref entityShape, shapePathForLogging, ref shapeIsCloned, ref willDisableElements);
                    if (handling == EnumHandling.PreventSubsequent) break;
                }
            }

            if (willDisableElements != null && willDisableElements.Length > 0)
            {
                if (!shapeIsCloned)
                {
                    // Make a full copy so we don't mess up the original
                    Shape newShape = entityShape.Clone();
                    entityShape = newShape;
                    shapeIsCloned = true;
                }

                entityShape.RemoveElements(willDisableElements);
            }

            if (shapeIsCloned)
            {
                AnimManager.LoadAnimator(World.Api, this, entityShape, AnimManager.Animator?.Animations, requirePosesOnServer, "head");
            }
            else
            {
                AnimManager.LoadAnimatorCached(World.Api, this, entityShape, AnimManager.Animator?.Animations, requirePosesOnServer, "head");
            }
        }

        public virtual void OnTesselated()
        {
            var bhs = World.Side == EnumAppSide.Server ? Properties.Server?.Behaviors : Properties.Client?.Behaviors;
            if (bhs != null)
            {
                foreach (var bh in bhs)
                {
                    bh.OnTesselated();
                }
            }
        }


        /// <summary>
        /// Called when the entity collided vertically
        /// </summary>
        /// <param name="motionY"></param>
        public virtual void OnFallToGround(double motionY)
        {
            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                behavior.OnFallToGround(PositionBeforeFalling, motionY);
            }
        }

        /// <summary>
        /// Called when the entity collided with something solid and <see cref="Collided"/> was false before
        /// </summary>
        public virtual void OnCollided()
        {

        }

        /// <summary>
        /// Called when the entity got in touch with a liquid
        /// </summary>
        public virtual void OnCollideWithLiquid()
        {
            if (World.Side == EnumAppSide.Server) return;

            EntityPos pos = SidedPos;
            float yDistance = (float)Math.Abs(PositionBeforeFalling.Y - pos.Y);

            double width = SelectionBox.XSize;
            double height = SelectionBox.YSize;

            double splashStrength = 2 * GameMath.Sqrt(width * height) + pos.Motion.Length() * 10;

            if (splashStrength < 0.4f || yDistance < 0.25f) return;

            string[] soundsBySize = new string[] { "sounds/environment/smallsplash", "sounds/environment/mediumsplash", "sounds/environment/largesplash" };
            string sound = soundsBySize[(int)GameMath.Clamp(splashStrength / 1.6, 0, 2)];

            splashStrength = Math.Min(10, splashStrength);

            float qmod = GameMath.Sqrt(width * height);

            World.PlaySoundAt(new AssetLocation(sound), (float)pos.X, (float)pos.InternalY, (float)pos.Z, null);
            BlockPos blockpos = pos.AsBlockPos;
            Vec3d aboveBlockPos = new Vec3d(Pos.X, blockpos.InternalY + 1.02, Pos.Z);
            World.SpawnCubeParticles(blockpos, aboveBlockPos, SelectionBox.XSize, (int)(qmod * 8 * splashStrength), 0.75f);
            World.SpawnCubeParticles(blockpos, aboveBlockPos, SelectionBox.XSize, (int)(qmod * 8 * splashStrength), 0.25f);

            if (splashStrength >= 2)
            {
                SplashParticleProps.BasePos.Set(pos.X - width / 2, pos.Y - 0.75, pos.Z - width / 2);
                SplashParticleProps.AddPos.Set(width, 0.75, width);

                SplashParticleProps.AddVelocity.Set((float)GameMath.Clamp(pos.Motion.X * 30f, -2, 2), 1, (float)GameMath.Clamp(pos.Motion.Z * 30f, -2, 2));
                SplashParticleProps.QuantityMul = (float)(splashStrength - 1) * qmod;

                World.SpawnParticles(SplashParticleProps);
            }

            SpawnWaterMovementParticles((float)Math.Min(0.25f, splashStrength / 10f), 0, -0.5f, 0);
        }

        protected virtual void SpawnWaterMovementParticles(float quantityMul, double offx=0, double offy = 0, double offz = 0)
        {
            if (World.Side == EnumAppSide.Server) return;

            ICoreClientAPI capi = (Api as ICoreClientAPI);
            ClimateCondition climate = capi.World.Player.Entity.selfClimateCond;
            if (climate == null) return;

            float dist = Math.Max(0, (28 - climate.Temperature)/6f) + Math.Max(0, (0.8f - climate.Rainfall) * 3f);

            double noise = bioLumiNoise.Noise(SidedPos.X / 300.0, SidedPos.Z / 300.0);
            double qmul = noise * 2 - 1 - dist;


            if (qmul < 0) return;

            // Hard coded player swim hitbox thing
            if (this is EntityPlayer && Swimming)
            {
                bioLumiParticles.MinPos.Set(SidedPos.X + 2f * SelectionBox.X1, SidedPos.Y + offy + 0.5f + 1.25f * SelectionBox.Y1, SidedPos.Z + 2f * SelectionBox.Z1);
                bioLumiParticles.AddPos.Set(3f * SelectionBox.XSize, 0.5f * SelectionBox.YSize, 3f * SelectionBox.ZSize);
            }
            else
            {
                bioLumiParticles.MinPos.Set(SidedPos.X + 1.25f * SelectionBox.X1, SidedPos.Y + offy + 1.25f * SelectionBox.Y1, SidedPos.Z + 1.25f * SelectionBox.Z1);
                bioLumiParticles.AddPos.Set(1.5f * SelectionBox.XSize, 1.5f * SelectionBox.YSize, 1.5f * SelectionBox.ZSize);
            }

            bioLumiParticles.MinQuantity = Math.Min(200, 100 * quantityMul * (float)qmul);

            bioLumiParticles.MinVelocity.Set(-0.2f + 2 * (float)Pos.Motion.X, -0.2f + 2 * (float)Pos.Motion.Y, -0.2f + 2*(float)Pos.Motion.Z);
            bioLumiParticles.AddVelocity.Set(0.4f + 2 * (float)Pos.Motion.X, 0.4f + 2 * (float)Pos.Motion.Y, 0.4f + 2 * (float)Pos.Motion.Z);
            World.SpawnParticles(bioLumiParticles);
        }

        /// <summary>
        /// Called when after the got loaded from the savegame (not called during spawn)
        /// </summary>
        public virtual void OnEntityLoaded()
        {
            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                behavior.OnEntityLoaded();
            }

            Properties.Client.Renderer?.OnEntityLoaded();
            MarkShapeModified();
        }

        /// <summary>
        /// Called when the entity spawns (not called when loaded from the savegame).
        /// </summary>
        public virtual void OnEntitySpawn()
        {
            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                behavior.OnEntitySpawn();
            }

            Properties.Client.Renderer?.OnEntityLoaded();
            MarkShapeModified();
        }

        /// <summary>
        /// Called when the entity despawns
        /// </summary>
        /// <param name="despawn"></param>
        public virtual void OnEntityDespawn(EntityDespawnData despawn)
        {
            if (SidedProperties == null) return;
            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                behavior.OnEntityDespawn(despawn);
            }

            AnimManager.Dispose();

            WatchedAttributes.OnModified.Clear();
        }


        /// <summary>
        /// Called when the entity has left a liquid
        /// </summary>
        public virtual void OnExitedLiquid()
        {

        }

        /// <summary>
        /// Called when an entity has interacted with this entity
        /// </summary>
        /// <param name="byEntity"></param>
        /// <param name="itemslot">If being interacted with a block/item, this should be the slot the item is being held in</param>
        /// <param name="hitPosition">Relative position on the entites hitbox where the entity interacted at</param>
        /// <param name="mode">0 = attack, 1 = interact</param>
        public virtual void OnInteract(EntityAgent byEntity, ItemSlot itemslot, Vec3d hitPosition, EnumInteractMode mode)
        {
            EnumHandling handled = EnumHandling.PassThrough;

            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                behavior.OnInteract(byEntity, itemslot, hitPosition, mode, ref handled);
                if (handled == EnumHandling.PreventSubsequent) break;
            }
        }

        /// <summary>
        /// Called when a player looks at the entity with interaction help enabled
        /// </summary>
        /// <param name="world"></param>
        /// <param name="es"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual WorldInteraction[] GetInteractionHelp(IClientWorldAccessor world, EntitySelection es, IClientPlayer player)
        {
            EnumHandling handled = EnumHandling.PassThrough;

            List<WorldInteraction> interactions = new List<WorldInteraction>();

            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                WorldInteraction[] wis = behavior.GetInteractionHelp(world, es, player, ref handled);
                if (wis != null) interactions.AddRange(wis);

                if (handled == EnumHandling.PreventSubsequent) break;
            }

            return interactions.ToArray();
        }


        /// <summary>
        /// Called by client when a new server pos arrived
        /// </summary>
        public virtual void OnReceivedServerPos(bool isTeleport)
        {
            EnumHandling handled = EnumHandling.PassThrough;

            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                behavior.OnReceivedServerPos(isTeleport, ref handled);
                if (handled == EnumHandling.PreventSubsequent)
                {
                    break;
                }
            }

            // Position not set automatically if the entity has interpolation. I'd rather not check this every time.
            if (handled == EnumHandling.PassThrough && GetBehavior("entityinterpolation") == null)
            {
                Pos.SetFrom(ServerPos);
            }
        }

        /// <summary>
        /// Called when on the client side something called capi.Network.SendEntityPacket()
        /// </summary>
        /// <param name="player"></param>
        /// <param name="packetid"></param>
        /// <param name="data"></param>
        public virtual void OnReceivedClientPacket(IServerPlayer player, int packetid, byte[] data)
        {
            EnumHandling handled = EnumHandling.PassThrough;

            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                behavior.OnReceivedClientPacket(player, packetid, data, ref handled);
                if (handled == EnumHandling.PreventSubsequent) break;
            }
        }

        /// <summary>
        /// Called when on the server side something called sapi.Network.SendEntityPacket()
        /// Packetid = 1 is used for teleporting
        /// Packetid = 2 is used for BehaviorHarvestable
        /// </summary>
        /// <param name="packetid"></param>
        /// <param name="data"></param>
        public virtual void OnReceivedServerPacket(int packetid, byte[] data)
        {
            // Teleport packet
            if (packetid == (int)EntityServerPacketId.Teleport)
            {
                Vec3d newPos = SerializerUtil.Deserialize<Vec3d>(data);
                if (Api is ICoreClientAPI ic && ic.World.Player.Entity.EntityId == EntityId)
                {
                    Pos.SetPosWithDimension(newPos);
                    ((EntityPlayer)this).UpdatePartitioning();
                }
                ServerPos.SetPosWithDimension(newPos);
                World.BlockAccessor.MarkBlockDirty(newPos.AsBlockPos);
                return;
            }

            EnumHandling handled = EnumHandling.PassThrough;
            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                behavior.OnReceivedServerPacket(packetid, data, ref handled);
                if (handled == EnumHandling.PreventSubsequent) break;
            }
        }

        public virtual void OnReceivedServerAnimations(int[] activeAnimations, int activeAnimationsCount, float[] activeAnimationSpeeds)
        {
            AnimManager.OnReceivedServerAnimations(activeAnimations, activeAnimationsCount, activeAnimationSpeeds);
        }

        /// <summary>
        /// Called by BehaviorCollectEntities of nearby entities. Should return the itemstack that should be collected. If the item stack was fully picked up, BehaviorCollectEntities will kill this entity
        /// </summary>
        /// <param name="byEntity"></param>
        /// <returns></returns>
        public virtual ItemStack OnCollected(Entity byEntity)
        {
            return null;
        }

        /// <summary>
        /// Called on the server when the entity was changed from active to inactive state or vice versa
        /// </summary>
        /// <param name="beforeState"></param>
        public virtual void OnStateChanged(EnumEntityState beforeState)
        {
            EnumHandling handled = EnumHandling.PassThrough;

            foreach (EntityBehavior behavior in Properties.Server.Behaviors)
            {
                behavior.OnStateChanged(beforeState, ref handled);
                if (handled == EnumHandling.PreventSubsequent) return;
            }
        }

        #endregion


        /// <summary>
        /// Helper method to set the CollisionBox
        /// </summary>
        /// <param name="length"></param>
        /// <param name="height"></param>
        public virtual void SetCollisionBox(float length, float height)
        {
            CollisionBox = new Cuboidf()
            {
                X1 = -length/2,
                Z1 = -length/2,
                X2 = length/2,
                Z2 = length/2,
                Y2 = height
            };
            OriginCollisionBox = CollisionBox.Clone();
        }

        public virtual void SetSelectionBox(float length, float height)
        {
            SelectionBox = new Cuboidf()
            {
                X1 = -length / 2,
                Z1 = -length / 2,
                X2 = length / 2,
                Z2 = length / 2,
                Y2 = height
            };
            OriginSelectionBox = SelectionBox.Clone();
        }

        /// <summary>
        /// Adds given behavior to the entities list of active behaviors
        /// </summary>
        /// <param name="behavior"></param>
        public virtual void AddBehavior(EntityBehavior behavior)
        {
            SidedProperties.Behaviors.Add(behavior);
            if (Api is ICoreServerAPI) CacheServerBehaviors();
        }


        /// <summary>
        /// Removes given behavior to the entities list of active behaviors. Does nothing if the behavior has already been removed
        /// </summary>
        /// <param name="behavior"></param>
        public virtual void RemoveBehavior(EntityBehavior behavior)
        {
            SidedProperties.Behaviors.Remove(behavior);
            if (Api is ICoreServerAPI) CacheServerBehaviors();
        }

        /// <summary>
        /// Create the cached arrays of thread-safe and mainthread behaviors, for ticking performance
        /// </summary>
        public void CacheServerBehaviors()
        {
            int threadsafeCount = 0;
            var behaviors = SidedProperties.Behaviors;
            for (int i = 0; i < behaviors.Count; i++)
            {
                if (behaviors[i].ThreadSafe) threadsafeCount++;
            }

            ServerBehaviorsMainThread = new EntityBehavior[behaviors.Count - threadsafeCount];
            ServerBehaviorsThreadsafe = new EntityBehavior[threadsafeCount];
            int mt = 0;
            int ts = 0;
            for (int i = 0; i < behaviors.Count; i++)
            {
                var bh = behaviors[i];
                if (bh.ThreadSafe) ServerBehaviorsThreadsafe[ts++] = bh;
                else ServerBehaviorsMainThread[mt++] = bh;
            }
        }

        /// <summary>
        /// Returns true if the entity has given active behavior
        /// </summary>
        /// <param name="behaviorName"></param>
        /// <returns></returns>
        public virtual bool HasBehavior(string behaviorName)
        {
            var behaviors = SidedProperties.Behaviors;
            for (int i = 0; i < behaviors.Count; i++)
            {
                if (behaviors[i].PropertyName().Equals(behaviorName)) return true;
            }

            return false;
        }

        public virtual bool HasBehavior<T>() where T : EntityBehavior
        {
            var behaviors = SidedProperties.Behaviors;
            for (int i = 0; i < behaviors.Count; i++)
            {
                if (behaviors[i] is T) return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the behavior instance for given entity. Returns null if it doesn't exist.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual EntityBehavior? GetBehavior(string name)
        {
            var behaviors = SidedProperties.Behaviors;
            for (int i = 0; i < behaviors.Count; i++)
            {
                if (behaviors[i].PropertyName().Equals(name)) return behaviors[i];
            }
            return null;
        }

        /// <summary>
        /// Returns the first behavior instance for given entity of given type. Returns null if it doesn't exist.
        /// </summary>
        /// <returns></returns>
        public virtual T? GetBehavior<T>() where T : EntityBehavior
        {
            var behaviors = SidedProperties.Behaviors;
            for (int i = 0; i < behaviors.Count; i++)
            {
                if (behaviors[i] is T result) return result;
            }
            return null;
        }

        /// <summary>
        /// Returns itself and any behaviors that implement the interface T as a List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual List<T> GetInterfaces<T>() where T : class
        {
            List<T> interfaces = new List<T>();
            if (this is T self) interfaces.Add(self);

            var behaviors = SidedProperties.Behaviors;
            for (int i = 0; i < behaviors.Count; i++)
            {
                if (behaviors[i] is T found) interfaces.Add(found);
            }

            return interfaces;
        }

        /// <summary>
        /// Returns itself or the first behavior that implements the interface T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T GetInterface<T>() where T : class
        {
            if (this is T) return this as T;
            var behaviors = SidedProperties.Behaviors;
            for (int i = 0; i < behaviors.Count; i++)
            {
                if (behaviors[i] is T found) return found;
            }
            return null;
        }


        /// <summary>
        /// Returns true if given activity is running
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool IsActivityRunning(string key)
        {
            ActivityTimers.TryGetValue(key, out long val);
            return val > World.ElapsedMilliseconds;
        }

        /// <summary>
        /// Returns the remaining time on an activity in milliesconds
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual int RemainingActivityTime(string key)
        {
            ActivityTimers.TryGetValue(key, out long val);
            return (int)Math.Max(0, (val - World.ElapsedMilliseconds));
        }

        /// <summary>
        /// Starts an activity for a given duration
        /// </summary>
        /// <param name="key"></param>
        /// <param name="milliseconds"></param>
        public virtual void SetActivityRunning(string key, int milliseconds)
        {
            ActivityTimers[key] = World.ElapsedMilliseconds + milliseconds;
        }


        /// <summary>
        /// Updates the DebugAttributes tree
        /// </summary>
        public virtual void UpdateDebugAttributes()
        {
            if (World.Side != EnumAppSide.Client) return;

            DebugAttributes.SetString("Entity Id", "" + EntityId);
            DebugAttributes.SetString("Yaw, Pitch", string.Format("{0:0.##}, {1:0.##}", Pos.Yaw * GameMath.RAD2DEG, Pos.Pitch * GameMath.RAD2DEG));

            if (AnimManager != null) UpdateAnimationDebugAttributes();  // for EntityItem, AnimManager will be null
        }

        protected virtual void UpdateAnimationDebugAttributes()
        {
            string anims = "";
            int i = 0;
            foreach (string anim in AnimManager.ActiveAnimationsByAnimCode.Keys)
            {
                if (i++ > 0) anims += ",";
                anims += anim;
            }

            i = 0;
            StringBuilder runninganims = new StringBuilder();
            if (AnimManager.Animator != null)
            {
                foreach (var anim in AnimManager.Animator.Animations)
                {
                    if (!anim.Running) continue;

                    if (i++ > 0) runninganims.Append(',');
                    runninganims.Append(anim.Animation.Code);
                }

                DebugAttributes.SetString("Active Animations", anims.Length > 0 ? anims : "-");
                DebugAttributes.SetString("Running Animations", runninganims.Length > 0 ? runninganims.ToString() : "-");
            }
        }


        /// <summary>
        /// In order to maintain legacy mod API compatibility of FromBytes(BinaryReader reader, bool isSync), we create an overload which server-side calling code will actually call, and store the remaps parameter in a field
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="isSync"></param>
        /// <param name="serversideRemaps"></param>
        public virtual void FromBytes(BinaryReader reader, bool isSync, Dictionary<string, string> serversideRemaps)
        {
            this.codeRemaps = serversideRemaps;
            this.FromBytes(reader, isSync);
            this.codeRemaps = null;
        }

        /// <summary>
        /// Loads the entity from a stored byte array from the SaveGame
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="isSync">True if this is a sync operation, not a chunk read operation</param>
        public virtual void FromBytes(BinaryReader reader, bool isSync)
        {
            string version = "";
            if (!isSync)
            {
                version = reader.ReadString().DeDuplicate();
            }

            EntityId = reader.ReadInt64();
            WatchedAttributes.FromBytes(reader);

            if (!WatchedAttributes.HasAttribute("extraInfoText"))
            {
                WatchedAttributes["extraInfoText"] = new TreeAttribute();
            }

            if (this is EntityPlayer && GameVersion.IsLowerVersionThan(version, "1.7.0"))
            {
                ITreeAttribute healthTree = WatchedAttributes.GetTreeAttribute("health");
                if (healthTree != null)
                {
                    healthTree.SetFloat("basemaxhealth", 15);
                }
            }


            ServerPos.FromBytes(reader);

            GetHeadPositionFromWatchedAttributes();

            Pos.SetFrom(ServerPos);
            PositionBeforeFalling.X = reader.ReadDouble();
            PositionBeforeFalling.Y = reader.ReadDouble();
            PositionBeforeFalling.Z = reader.ReadDouble();

            string codeString = reader.ReadString().DeDuplicate();
            if (codeRemaps != null && codeRemaps.TryGetValue(codeString, out string remappedString)) codeString = remappedString;
            Code = new AssetLocation(codeString);

            if (!isSync)
            {
                Attributes.FromBytes(reader);
            }

            // In 1.8 animation data format was changed to use a TreeAttribute.
            if (isSync || GameVersion.IsAtLeastVersion(version, "1.8.0-pre.1"))
            {
                TreeAttribute tree = new TreeAttribute();
                tree.FromBytes(reader);
                AnimManager?.FromAttributes(tree, version);

                if (Properties?.Server?.Behaviors != null)
                {
                    foreach (var bh in Properties.Server.Behaviors)
                    {
                        bh.FromBytes(isSync);
                    }
                }
            }
            else
            {
                // Should not be too bad to just ditch pre 1.8 animations
                // as the entity ai systems start new ones eventually anyway
            }


            // Upgrade to 1500 sat
            if (this is EntityPlayer && GameVersion.IsLowerVersionThan(version, "1.10-dev.2"))
            {
                ITreeAttribute hungerTree = WatchedAttributes.GetTreeAttribute("hunger");
                if (hungerTree != null)
                {
                    hungerTree.SetFloat("maxsaturation", 1500);
                }
            }

            Stats.FromTreeAttributes(WatchedAttributes);

            if (isSync)
            {
                Tags = EntityTagArray.FromBytes(reader);

                var seatControls = SidedProperties != null ? GetInterface<IMountable>()?.ControllingControls : null;
                int controlsSerialized = reader.ReadInt32();
                seatControls?.FromInt(controlsSerialized);
            }

            // Any new data loading added here should not be loaded if below version 1.8 or
            // you might get corrupt data from old binary animation data
        }



        /// <summary>
        /// Serializes the slots contents to be stored in the SaveGame
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="forClient">True when being used to send an entity to the client</param>
        public virtual void ToBytes(BinaryWriter writer, bool forClient)
        {
            if (Properties?.Server?.Behaviors != null)
            {
                foreach (var bh in Properties.Server.Behaviors)
                {
                    bh.ToBytes(forClient);
                }
            }

            if (!forClient)
            {
                writer.Write(GameVersion.ShortGameVersion);
            }

            writer.Write(EntityId);

            SetHeadPositionToWatchedAttributes();

            Stats.ToTreeAttributes(WatchedAttributes, forClient);

            WatchedAttributes.ToBytes(writer);
            ServerPos.ToBytes(writer);
            writer.Write(PositionBeforeFalling.X);
            writer.Write(PositionBeforeFalling.Y);
            writer.Write(PositionBeforeFalling.Z);

            if (Code == null)
            {
                World.Logger.Error("Entity.ToBytes(): entityType.Code is null?! Entity will probably be incorrectly saved to disk");
            }

            writer.Write(Code?.ToShortString());

            if (!forClient)
            {
                Attributes.ToBytes(writer);
            }

            if (AnimManager is AnimationManager animManager)
            {
                // Tyron 19.oct 2019. Don't write animations to the savegame. I think it causes that some animations start but never stop
                // if we want to save the creatures current state to disk, we would also need to save the current AI state!
                // Tyron 26 oct. Do write animations, but only the die one.
                // Tyron 8 nov. Do write all animations if its for the client
                //if (forClient)
                {
                        animManager.ToAttributeBytes(writer, forClient);
                }
                TreeAttribute.TerminateWrite(writer);
            }
            else if (AnimManager == null || AnimManager is NoAnimationManager)
            {
                TreeAttribute.TerminateWrite(writer);    // Effectively, write an empty TreeAttribute
            }
            else    // For mod backwards compatibility, if it makes its own IAnimationManager
            {
                TreeAttribute tree = new TreeAttribute();
                AnimManager.ToAttributes(tree, forClient);
                tree.ToBytes(writer);
            }

            if (forClient)
            {
                Tags.ToBytes(writer);

                var seatControls = SidedProperties != null ? GetInterface<IMountable>()?.ControllingControls : null;
                writer.Write(seatControls == null ? 0 : seatControls.ToInt());
            }
        }

        /// <summary>
        /// Relevant only for entities with heads, implemented in EntityAgent.  Other sub-classes of Entity (if not EntityAgent) should similarly override this if the headYaw/headPitch are relevant to them
        /// </summary>
        protected virtual void SetHeadPositionToWatchedAttributes()
        {
        }

        /// <summary>
        /// Relevant only for entities with heads, implemented in EntityAgent.  Other sub-classes of Entity (if not EntityAgent) should similarly override this if the headYaw/headPitch are relevant to them
        /// </summary>
        protected virtual void GetHeadPositionFromWatchedAttributes()
        {
        }





        /// <summary>
        /// Revives the entity and heals for 9999.
        /// </summary>
        public virtual void Revive()
        {
            Alive = true;
            ReceiveDamage(new DamageSource() { Source = EnumDamageSource.Revive, Type = EnumDamageType.Heal }, 9999);
            AnimManager?.StopAnimation("die");
            IsOnFire = false;

            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                behavior.OnEntityRevive();
            }
        }


        /// <summary>
        /// Makes the entity despawn. Entities only drop something on EnumDespawnReason.Death
        /// </summary>
        public virtual void Die(EnumDespawnReason reason = EnumDespawnReason.Death, DamageSource damageSourceForDeath = null)
        {
            if (!Alive) return;

            Alive = false;

            if (reason == EnumDespawnReason.Death)
            {
                Api.Event.TriggerEntityDeath(this, damageSourceForDeath);

                ItemStack[] drops = GetDrops(World, Pos.AsBlockPos, null);

                if (drops != null)
                {
                    for (int i = 0; i < drops.Length; i++)
                    {
                        World.SpawnItemEntity(drops[i], SidedPos.XYZ.Add(0, 0.25, 0));
                    }
                }

                AnimManager.ActiveAnimationsByAnimCode.Clear();
                AnimManager.StartAnimation("die");

                WatchedAttributes.SetDouble("deathTotalHours", Api.World.Calendar.TotalHours);

                if (reason == EnumDespawnReason.Death && damageSourceForDeath != null && World.Side == EnumAppSide.Server) {
                    WatchedAttributes.SetInt("deathReason", (int)damageSourceForDeath.Source);
                    WatchedAttributes.SetInt("deathDamageType", (int)damageSourceForDeath.Type);
                    Entity byEntity = damageSourceForDeath.GetCauseEntity();
                    if (byEntity != null)
                    {
                        WatchedAttributes.SetString("deathByEntity", byEntity.Code.ToString());
                    }
                    if (byEntity is EntityPlayer eplayer)
                    {
                        WatchedAttributes.SetString("deathByPlayer", eplayer.Player?.PlayerName);
                    }
                }


                foreach (EntityBehavior behavior in SidedProperties.Behaviors)
                {
                    behavior.OnEntityDeath(damageSourceForDeath);
                }
            }

            DespawnReason = new EntityDespawnData() {
                Reason = reason,
                DamageSourceForDeath = damageSourceForDeath
            };
        }


        /// <summary>
        /// Assumes that it is only called on the server
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dualCallByPlayer"></param>
        /// <param name="randomizePitch"></param>
        /// <param name="range"></param>
        public virtual void PlayEntitySound(string type, IPlayer dualCallByPlayer = null, bool randomizePitch = true, float range = 24)
        {
            if (Properties.ResolvedSounds != null && Properties.ResolvedSounds.TryGetValue(type, out AssetLocation[] locations) && locations.Length > 0)
            {
                World.PlaySoundAt(
                    locations[World.Rand.Next(locations.Length)],
                    (float)SidedPos.X, (float)SidedPos.InternalY, (float)SidedPos.Z,
                    dualCallByPlayer,
                    randomizePitch,
                    range
                );
            }
        }


        /// <summary>
        /// Should return true if this item can be picked up as an itemstack
        /// </summary>
        /// <param name="byEntity"></param>
        /// <returns></returns>
        public virtual bool CanCollect(Entity byEntity)
        {
            return false;
        }




        /// <summary>
        /// This method pings the Notify() method of all behaviors and ai tasks. Can be used to spread information to other creatures.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public virtual void Notify(string key, object data)
        {
            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                behavior.Notify(key, data);
            }
        }


        /// <summary>
        /// This method is called by the BlockSchematic class a moment before a schematic containing this entity is getting exported.
        /// Since a schematic can be placed anywhere in the world, this method has to make sure the entities position is set to a value
        /// relative of the schematic origin point defined by startPos
        /// Right after calling this method, the world edit system will call .ToBytes() to serialize the entity
        /// </summary>
        /// <param name="startPos"></param>
        public virtual void WillExport(BlockPos startPos)
        {
            ServerPos.X -= startPos.X;
            ServerPos.Y -= startPos.Y;
            ServerPos.Z -= startPos.Z;

            Pos.X -= startPos.X;
            Pos.Y -= startPos.Y;
            Pos.Z -= startPos.Z;

            PositionBeforeFalling.X -= startPos.X;
            PositionBeforeFalling.Y -= startPos.Y;
            PositionBeforeFalling.Z -= startPos.Z;
        }

        /// <summary>
        /// This method is called by the BlockSchematic class a moment after a schematic containing this entity has been exported.
        /// Since a schematic can be placed anywhere in the world, this method has to make sure the entities position is set to the correct
        /// position in relation to the target position of the schematic to be imported.
        /// </summary>
        /// <param name="startPos"></param>
        public virtual void DidImportOrExport(BlockPos startPos)
        {
            ServerPos.X += startPos.X;
            ServerPos.Y += startPos.Y;
            ServerPos.Z += startPos.Z;
            ServerPos.Dimension = startPos.dimension;

            Pos.X += startPos.X;
            Pos.Y += startPos.Y;
            Pos.Z += startPos.Z;
            Pos.Dimension = startPos.dimension;

            PositionBeforeFalling.X += startPos.X;
            PositionBeforeFalling.Y += startPos.Y;
            PositionBeforeFalling.Z += startPos.Z;
        }



        /// <summary>
        /// Called by the worldedit schematic exporter so that it can also export the mappings of items/blocks stored inside blockentities
        /// </summary>
        /// <param name="blockIdMapping"></param>
        /// <param name="itemIdMapping"></param>
        public virtual void OnStoreCollectibleMappings(Dictionary<int, AssetLocation> blockIdMapping, Dictionary<int, AssetLocation> itemIdMapping)
        {
            foreach (var val in Properties.Server.Behaviors)
            {
                val.OnStoreCollectibleMappings(blockIdMapping, itemIdMapping);
            }
        }

        /// <summary>
        /// Called by the blockschematic loader so that you may fix any blockid/itemid mappings against the mapping of the savegame, if you store any collectibles in this blockentity.
        /// Note: Some vanilla blocks resolve randomized contents in this method.
        /// Hint: Use itemstack.FixMapping() to do the job for you.
        /// </summary>
        /// <param name="worldForNewMappings"></param>
        /// <param name="oldBlockIdMapping"></param>
        /// <param name="oldItemIdMapping"></param>
        /// <param name="schematicSeed">If you need some sort of randomness consistency accross an imported schematic, you can use this value</param>
        /// <param name="resolveImports">Turn it off to spawn structures as they are. For example, in this mode, instead of traders, their meta spawners will spawn</param>
        public virtual void OnLoadCollectibleMappings(IWorldAccessor worldForNewMappings, Dictionary<int, AssetLocation> oldBlockIdMapping, Dictionary<int, AssetLocation> oldItemIdMapping, int schematicSeed, bool resolveImports)
        {
            foreach (var val in Properties.Server.Behaviors)
            {
                val.OnLoadCollectibleMappings(worldForNewMappings, oldBlockIdMapping, oldItemIdMapping, resolveImports);
            }
        }

        /// <summary>
        /// Used only during worldgen when an entity is placed by a schematic. Overall purpose is like OnLoadCollectibleMappings() except that it is called prior to initialising the Entity
        /// <br/>Should typically do nothing and return false
        /// <br/>May optionally load the collectible mappings here (doing any necessary initialisation itself, e.g. initialising the Inventory) in which case return true. For example, this is useful for Armorstands
        /// </summary>
        /// <param name="worldForNewMappings"></param>
        /// <param name="oldBlockIdMapping"></param>
        /// <param name="oldItemIdMapping"></param>
        /// <param name="schematicSeed"></param>
        /// <param name="resolveImports"></param>
        /// <returns>If returns true, the caller should skip calling OnLoadCollectibleMappings()</returns>
        public virtual bool TryEarlyLoadCollectibleMappings(IWorldAccessor worldForNewMappings, Dictionary<int, AssetLocation> oldBlockIdMapping, Dictionary<int, AssetLocation> oldItemIdMapping, int schematicSeed, bool resolveImports)
        {
            bool result = false;
            var props = worldForNewMappings.GetEntityType(Code);
            World = worldForNewMappings;
            var behaviors = props?.Server.BehaviorsWithEarlyLoadCollectibleMappings(this, worldForNewMappings);
            if (behaviors != null)
            {
                foreach(var bh in behaviors)
                {
                    try
                    {
                        result |= bh.Behavior.TryEarlyLoadCollectibleMappings(worldForNewMappings, oldBlockIdMapping, oldItemIdMapping, resolveImports, props, bh.BehaviorConfig);
                    }
                    catch (Exception e)
                    {
                        worldForNewMappings.Logger.Error($"Error during TryEarlyLoadCollectibleMappings for {bh.Behavior.GetType()} on {Code} at {Pos.AsBlockPos}");
                        worldForNewMappings.Logger.Error(e);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the name for this entity
        /// </summary>
        /// <returns></returns>
        public virtual string GetName()
        {
            string name = null;
            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                EnumHandling handling = EnumHandling.PassThrough;
                string bhname = behavior.GetName(ref handling);
                if (handling == EnumHandling.PreventSubsequent) return name;
                if (handling == EnumHandling.PreventDefault) name = bhname;
            }

            if (name != null) return name;

            if (!Alive)
            {
                return Lang.GetMatching(Code.Domain + ":item-dead-creature-" + Code.Path);
            }

            return Lang.GetMatching(Code.Domain + ":item-creature-" + Code.Path);
        }

        /// <summary>
        /// Gets the translated prefix and creature name, e.g. "a goat"
        /// </summary>
        /// <returns></returns>
        public virtual string GetPrefixAndCreatureName(string languageCode = null)
        {
            languageCode ??= Lang.CurrentLocale;
            string translationKey = Code.Domain + AssetLocation.LocationSeparator + "prefixandcreature-" + Code.Path;
            string oldFormatKey = Code.Domain + AssetLocation.LocationSeparator + "prefixandcreature-" + Code.Path.Replace("-", "");
            return Lang.AvailableLanguages[languageCode].GetMatchingIfExists(translationKey)
                ?? Lang.AvailableLanguages[languageCode].GetMatchingIfExists(oldFormatKey)
                ?? Lang.GetL(languageCode, "generic-wildanimal");
        }

        /// <summary>
        /// gets the info text for the entity.
        /// </summary>
        /// <returns></returns>
        public virtual string GetInfoText()
        {
            StringBuilder infotext = new StringBuilder();
            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                behavior.GetInfoText(infotext);
            }

            int generation = WatchedAttributes.GetInt("generation", 0);

            if (generation > 0)
            {
                infotext.AppendLine(Lang.Get("Generation: {0}", generation));
            }

            if (!Alive)
            {
                if (WatchedAttributes.HasAttribute("deathByPlayer")) {
                    infotext.AppendLine(Lang.Get("Killed by Player: {0}", WatchedAttributes.GetString("deathByPlayer")));
                }
            }

            if (WatchedAttributes.HasAttribute("extraInfoText"))
            {
                ITreeAttribute tree = WatchedAttributes.GetTreeAttribute("extraInfoText");
                foreach (var val in tree.SortedCopy())
                {
                    infotext.AppendLine(val.Value.ToString());
                }
            }

            var capi = Api as ICoreClientAPI;
            if (capi != null && capi.Settings.Bool["extendedDebugInfo"])
            {
                infotext.AppendLine("<font color=\"#bbbbbb\">Id:" + EntityId + "</font>");
                infotext.AppendLine("<font color=\"#bbbbbb\">Code: " + Code + "</font>");

                IEnumerable<string> tags = Tags.ToArray().Select(Api.TagRegistry.EntityTagIdToTag).Order();
                if (tags.Any())
                {
                    infotext.AppendLine($"<font color=\"#bbbbbb\">Tags: {tags.Aggregate((first, second) => $"{first}, {second}")}</font>");
                }
            }

            return infotext.ToString();
        }

        /// <summary>
        /// Starts the animation for the entity.
        /// </summary>
        /// <param name="code"></param>
        public virtual void StartAnimation(string code)
        {
            AnimManager.StartAnimation(code);
        }

        /// <summary>
        /// stops the animation for the entity.
        /// </summary>
        /// <param name="code"></param>
        public virtual void StopAnimation(string code)
        {
            AnimManager.StopAnimation(code);
        }

        /// <summary>
        /// To test for player->entity selection.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="interesectionTester">Is already preloaded with the ray</param>
        /// <param name="intersectionDistance"></param>
        /// <param name="selectionBoxIndex"></param>
        /// <returns></returns>
        public virtual bool IntersectsRay(Ray ray, AABBIntersectionTest interesectionTester, out double intersectionDistance, ref int selectionBoxIndex)
        {
            if (trickleDownRayIntersects)
            {
                EnumHandling handled = EnumHandling.PassThrough;
                bool preventDefault = false;
                bool intersects = false;
                intersectionDistance = 0;

                foreach (EntityBehavior behavior in SidedProperties.Behaviors)
                {
                    intersects |= behavior.IntersectsRay(ray, interesectionTester, out intersectionDistance, ref selectionBoxIndex, ref handled);
                    preventDefault |= handled == EnumHandling.PreventDefault;
                    if (handled == EnumHandling.PreventSubsequent) return intersects;
                }

                if (preventDefault) return intersects;
            }

            if (interesectionTester.RayIntersectsWithCuboid(SelectionBox, SidedPos.X, SidedPos.InternalY, SidedPos.Z))
            {
                intersectionDistance = Pos.SquareDistanceTo(ray.origin);
                return true;
            }

            if (CanAttackInside && SelectionBox.Contains(ray.origin.X - SidedPos.X, ray.origin.Y - SidedPos.Y, ray.origin.Z - SidedPos.Z))
            {
                intersectionDistance = 0;
                return true;
            }

            intersectionDistance = 0;
            return false;
        }

        /// <summary>
        /// The distance at which entities are counted as "touching" each other, for example used by EntityPartitioning and RepulseAgents
        /// <br/>Note: from 1.20.4 this is gathered and cached in a field when each entity is Initialized, if for any reason a mod or behavior needs to change the result later than Initialization then you should also update the Entity field .touchDistance
        /// </summary>
        /// <returns></returns>
        public virtual double GetTouchDistance()
        {
            float dist = SelectionBox?.XSize / 2 ?? 0.25f;
            foreach (var val in SidedProperties.Behaviors)
            {
                EnumHandling handling = EnumHandling.PassThrough;
                float d = val.GetTouchDistance(ref handling);
                if (handling != EnumHandling.PassThrough) dist = d;
                if (handling == EnumHandling.PreventSubsequent) break;
            }

            return dist;
        }

        /// <summary>
        /// Triggers full entity synchronization, including entity tags. Should not be called too frequently.
        /// </summary>
        public void MarkTagsDirty() => tagsDirty = true;

        /// <summary>
        /// Returns true if all named tags are present
        /// </summary>
        /// <param name="tagsToMatch"></param>
        /// <returns></returns>
        public virtual bool HasTags(params string[] tagsToMatch)
        {
            EntityTagArray needle = Api.TagRegistry.EntityTagsToTagArray(tagsToMatch);
            return Tags.ContainsAll(needle);
        }


        public bool IsFirstTick()
        {
            return PreviousServerPos.X == 0 && PreviousServerPos.Y == 0 && PreviousServerPos.Z == 0;
        }
    }
}
