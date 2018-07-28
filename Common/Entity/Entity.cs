using System.Collections.Generic;
using System.IO;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Config;
using System;
using System.Linq;

namespace Vintagestory.API.Common.Entities
{
    /// <summary>
    /// The basic class for all entities in the game
    /// </summary>
    public abstract class Entity : IEntity
    {
        public static WaterSplashParticles SplashParticleProps = new WaterSplashParticles();
        public static AirBubbleParticles AirBubbleParticleProps = new AirBubbleParticles();

        public List<EntityBehavior> Behaviors = new List<EntityBehavior>();

        public IWorldAccessor World;

        public long InChunkIndex3d;
        public long Entityid;
        /// <summary>
        /// Set by the client renderer when the entity was rendered last frame
        /// </summary>
        public bool IsRendered;
        public bool IsShadowRendered;
        /// <summary>
        /// Set by the game client
        /// </summary>
        public EntityRenderer Renderer;

        public bool BroadcastServerPos;
        public EntityDespawnReason DespawnReason;


        public int TrackingRange;
        public Cuboidf CollisionBox;

        private AssetLocation savedTypeCode;
        private EntityType entityType;

        public AssetLocation SavedTypeCode { get { return savedTypeCode; } }

        /// <summary>
        /// Color used when the entity is being attacked
        /// </summary>
        protected int HurtColor = ColorUtil.ColorFromArgb(255, 255, 100, 100);


        /// <summary>
        /// An uptime value running activities.
        /// </summary>
        public Dictionary<string, long> ActivityTimers = new Dictionary<string, long>();


        public Dictionary<string, AnimationMetaData> ActiveAnimationsByAnimCode = new Dictionary<string, AnimationMetaData>();
        public bool AnimationsDirty;
        public BlockFacing ClimbingOnFace;

        /// <summary>
        /// Client position
        /// </summary>
        public SyncedEntityPos Pos = new SyncedEntityPos();

        /// <summary>
        /// Server simulated position. If this value differs to greatly from client pos we have to override client pos
        /// </summary>
        public EntityPos ServerPos = new EntityPos();

        /// <summary>
        /// Server simulated animations. Only takes care of stopping animations once they're done
        /// Set and Called by the Entities ServerSystem
        /// </summary>
        public ServerEntityAnimator ServerAnimator;

        /// <summary>
        /// ServerPos on server, Pos on client
        /// </summary>
        public EntityPos LocalPos
        {
            get { return World.Side == EnumAppSide.Server ? ServerPos : Pos; }
        }

        /// <summary>
        /// Server simulated position copy. Needed by Entities system to send pos updatess only if ServerPos differs noticably from PreviousServerPos
        /// </summary>
        public EntityPos PreviousServerPos = new EntityPos();

        /// <summary>
        /// The position where the entity last had contact with the ground
        /// </summary>
        public Vec3d PositionBeforeFalling = new Vec3d();

        

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
        /// True if this entity is in touch with the ground
        /// </summary>
        public bool OnGround;

        /// <summary>
        /// True if the bottom of the collisionbox is inside a liquid
        /// </summary>
        public bool FeetInLiquid;

        /// <summary>
        /// True if the collisionbox is 2/3rds submerged in liquid
        /// </summary>
        public bool Swimming;

        /// <summary>
        /// True if the entity is in touch with something solid on the vertical axis
        /// </summary>
        public bool CollidedVertically;

        /// <summary>
        /// True if the entity is in touch with something solid on both horizontal axes
        /// </summary>
        public bool CollidedHorizontally;

        /// <summary>
        /// Players and whatever the player rides on will be stored seperatly
        /// </summary>
        public virtual bool StoreWithChunk { get { return true; } }

        /// <summary>
        /// Whether this entity should always stay in Active model, regardless on how far away other player are
        /// </summary>
        public virtual bool AlwaysActive { get { return false; } }

        /// <summary>
        /// CollidedVertically || CollidedHorizontally
        /// </summary>
        public bool Collided { get { return CollidedVertically || CollidedHorizontally; } }

        /// <summary>
        /// Determines on whether an entity floats on liquids or not. Water has a density of 1000.
        /// </summary>
        public virtual float MaterialDensity
        {
            get { return 9999; }
        }

        /// <summary>
        /// If set, the entity will emit dynamic light
        /// </summary>
        public virtual byte[] LightHsv
        {
            get { return null; }
        }

        /// <summary>
        /// If gravity should applied to this entity
        /// </summary>
        public virtual bool ApplyGravity
        {
            get { return Type?.Habitat == EnumHabitat.Land || (Type?.Habitat == EnumHabitat.Sea && !Swimming); }
        }

        /// <summary>
        /// A unique identifier for this entity
        /// </summary>
        public long EntityId
        {
            get { return Entityid; }
            set { Entityid = value; }
        }

        long IEntity.EntityId
        {
            get { return Entityid; }
        }

        EntityPos IEntity.Pos
        {
            get { return Pos; }
        }

        EntityPos IEntity.ServerPos
        {
            get { return ServerPos; }
        }

        ITreeAttribute IEntity.WatchedAttributes
        {
            get { return WatchedAttributes; }
        }

        ITreeAttribute IEntity.Attributes
        {
            get { return Attributes; }
        }

        Cuboidf IEntity.CollisionBox
        {
            get { return CollisionBox; }
        }

        // Stored in WatchedAttributes in from/tobytes 
        public EnumEntityState State;



        /// <summary>
        /// True if the entity is in state active or inactive
        /// </summary>
        public virtual bool Alive
        {
            get { return WatchedAttributes.GetInt("entityDead", 0) == 0; }
            set { WatchedAttributes.SetInt("entityDead", value ? 0 : 1); }
        }

        /// <summary>
        /// just a !Alive currently
        /// </summary>
        public virtual bool ShouldDespawn
        {
            get { return !Alive; }
        }


        EnumEntityState IEntity.State
        {
            get { return State; }
        }

        /// <summary>
        /// The type of the entity
        /// </summary>
        public virtual EntityType Type
        {
            get { return entityType; }
        }

        /// <summary>
        /// Used by various renderers to retrieve the entities texture it should be drawn with
        /// </summary>
        public virtual CompositeTexture Texture
        {
            get { return entityType.Client.FirstTexture; }
        }

        /// <summary>
        /// Name of there renderer system that draws this entity
        /// </summary>
        public virtual string RendererName
        {
            get { return entityType.Client.Renderer; }
        }


        /// <summary>
        /// Used by some renderers to apply an overal color tint on the entity
        /// </summary>
        public int RenderColor
        {
            get
            {
                int val = RemainingActivityTime("invulnerable");
                
                return val > 0 ? ColorUtil.ColorOverlay(HurtColor, ColorUtil.WhiteArgb, 1f - val / 500f) : ColorUtil.WhiteArgb;
            }
        }

        /// <summary>
        /// How high the camera should be placed if this entity were to be controlled by the player
        /// </summary>
        /// <returns></returns>
        public virtual double EyeHeight()
        {
               return entityType.EyeHeight;
        }

        /// <summary>
        /// Called when something tries to given an itemstack to this entity
        /// </summary>
        /// <param name="itemstack"></param>
        /// <returns></returns>
        public virtual bool TryGiveItemStack(ItemStack itemstack)
        {
            return false;
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
            get { return CollisionBox.Y1 + CollisionBox.Y2 / 2; }
        }


        IWorldAccessor IEntity.World
        {
            get { return World; }
        }


        /// <summary>
        /// Creates a new instance of an entity
        /// </summary>
        public Entity()
        {
            TrackingRange = GlobalConstants.DefaultTrackingRange;

            WatchedAttributes.SetAttribute("animations", new TreeAttribute());
        }

        /// <summary>
        /// Called when the entity got hurt. On the client side, dmgSource is null
        /// </summary>
        /// <param name="damage"></param>
        public virtual void OnHurt(DamageSource dmgSource, float damage)
        {

        }

        /// <summary>
        /// Called when this entity got created or loaded
        /// </summary>
        /// <param name="world"></param>
        /// <param name="InChunkIndex3d"></param>
        public virtual void Initialize(ICoreAPI api, long InChunkIndex3d)
        {
            this.World = api.World;
            this.InChunkIndex3d = InChunkIndex3d;

            if (entityType == null)
            {
                entityType = World.GetEntityType(savedTypeCode);
            }

            WatchedAttributes.SetFloat("onHurt", 0);
            int onHurtCounter = WatchedAttributes.GetInt("onHurtCounter");
            WatchedAttributes.RegisterModifiedListener("onHurt", () => {
                if (WatchedAttributes.GetFloat("onHurt", 0) == 0) return;
                int newOnHurtCounter = WatchedAttributes.GetInt("onHurtCounter");
                if (newOnHurtCounter == onHurtCounter) return;

                onHurtCounter = newOnHurtCounter;
                SetActivityRunning("invulnerable", 500);

                // Gets already called on the server directly
                if (World.Side == EnumAppSide.Client)
                {
                    OnHurt(null, WatchedAttributes.GetFloat("onHurt", 0));
                }
            });


            if (entityType != null)
            {
                if (entityType.HitBoxSize != null)
                {
                    SetHitbox(entityType.HitBoxSize.X, entityType.HitBoxSize.Y);
                }

                if (World.Side == EnumAppSide.Server)
                {
                    loadBehaviors(entityType.Server?.Behaviors);

                    if (entityType.Client?.FirstTexture?.Alternates != null && !WatchedAttributes.HasAttribute("textureIndex"))
                    {
                        WatchedAttributes.SetInt("textureIndex", World.Rand.Next(entityType.Client.FirstTexture.Alternates.Length + 1));
                    }

                    loadServerAnimator();

                    DebugAttributes.SetString("Origin", Attributes.GetString("origin", "??"));
                }
                else
                {
                    loadBehaviors(entityType.Client?.Behaviors);
                }
            }
            

            if (AlwaysActive)
            {
                State = EnumEntityState.Active;
            } else
            {
                State = EnumEntityState.Inactive;

                IPlayer[] players = World.AllOnlinePlayers;
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].Entity == null) continue;

                    if (Pos.InRangeOf(players[i].Entity.Pos, TrackingRange * TrackingRange))
                    {
                        State = EnumEntityState.Active;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Loads the server side lightweight animator (does not calcuate any transformation matrices). This one is used for correct syncing of animations.
        /// </summary>
        protected virtual void loadServerAnimator()
        {
            if (entityType?.Client?.Shape?.Base != null)
            {
                Shape shape = World.AssetManager.Get(entityType.Client.Shape.Base.Clone().WithPathPrefix("shapes/").WithPathAppendix(".json")).ToObject<Shape>();
                if (shape.Animations == null) return;

                for (int i = 0; i < shape.Animations.Length; i++)
                {
                    if (shape.Animations[i].Code == null)
                    {
                        shape.Animations[i].Code = shape.Animations[i].Name.ToLowerInvariant().Replace(" ", "");
                    }
                }

                ServerAnimator = new ServerEntityAnimator(this, shape.Animations);
            }
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
            EnumHandling handled = EnumHandling.NotHandled;
            ItemStack[] stacks = null;

            foreach (EntityBehavior behavior in Behaviors)
            {
                stacks = behavior.GetDrops(world, pos, byPlayer, ref handled);
                if (handled == EnumHandling.Last) return stacks;
            }

            if (handled == EnumHandling.PreventDefault) return stacks;

            if (entityType.Drops == null) return null;
            List<ItemStack> todrop = new List<ItemStack>();

            for (int i = 0; i < entityType.Drops.Length; i++)
            {
                ItemStack stack = entityType.Drops[i].GetNextItemStack();
                if (stack == null) continue;

                todrop.Add(stack);
                if (entityType.Drops[i].LastDrop) break;
            }

            return todrop.ToArray();
        }

        /// <summary>
        /// Sets the entity to given type
        /// </summary>
        /// <param name="entityType"></param>
        public virtual void SetType(EntityType entityType)
        {
            this.entityType = entityType;
        }


        void loadBehaviors(JsonObject[] behaviors)
        {
            if (behaviors == null) return;

            this.Behaviors.Clear();

            for (int i = 0; i < behaviors.Length; i++)
            {
                string code = behaviors[i]["code"].AsString();
                if (code == null) continue;

                EntityBehavior behavior = World.ClassRegistry.CreateEntityBehavior(this, code);
                behavior.Initialize(entityType, behaviors[i]);
                AddBehavior(behavior);
            }
        }

        /// <summary>
        /// Teleports the entity to given position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public virtual void TeleportToDouble(double x, double y, double z)
        {
            Pos.SetPos(x, y, z);
            ServerPos.SetPos(x, y, z);
            PreviousServerPos.SetPos(-99, -99, -99);
            PositionBeforeFalling.Set(x, y, z);
            Pos.Motion.Set(0, 0, 0);
            BroadcastServerPos = true;
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
        public virtual void TeleportTo(EntityPos position)
        {
            Pos.Yaw = position.Yaw;
            Pos.Pitch = position.Pitch;
            Pos.Roll = position.Roll;
            ServerPos.Yaw = position.Yaw;
            ServerPos.Pitch = position.Pitch;
            ServerPos.Roll = position.Roll;

            TeleportToDouble(position.X, position.Y, position.Z);
        }


        /// <summary>
        /// Called when the entity should be receiving damage from given source
        /// </summary>
        /// <param name="damageSource"></param>
        /// <param name="damage"></param>
        public virtual bool ReceiveDamage(DamageSource damageSource, float damage)
        {
            if ((!Alive || IsActivityRunning("invulnerable")) && damageSource.type != EnumDamageType.Heal) return false;

            if (ShouldReceiveDamage(damageSource, damage)) {
                if (damageSource.type != EnumDamageType.Heal)
                {
                    WatchedAttributes.SetInt("onHurtCounter", WatchedAttributes.GetInt("onHurtCounter") + 1);
                    WatchedAttributes.SetFloat("onHurt", damage); // Causes the client to be notified
                    StartAnimation("hurt");
                }

                foreach (EntityBehavior behavior in Behaviors)
                {
                    behavior.OnEntityReceiveDamage(damageSource, damage);
                }

                if (damageSource.GetSourcePosition() != null)
                {
                    Vec3d dir = (ServerPos.XYZ - damageSource.GetSourcePosition()).Normalize();
                    dir.Y = 0.1f;
                    float factor = GameMath.Clamp((1 - Type.KnockbackResistance) / 10f, 0, 2);
                    ServerPos.Motion.Add(dir.X * factor, dir.Y * factor, dir.Z * factor);
                }

                return true;
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


        /// <summary>
        /// Called when the entity collided vertically
        /// </summary>
        /// <param name="motionY"></param>
        public virtual void OnFallToGround(double motionY)
        {
            foreach (EntityBehavior behavior in Behaviors)
            {
                behavior.OnFallToGround(PositionBeforeFalling, motionY);
            }
        }

        /// <summary>
        /// Called when the entity got in touch with a liquid
        /// </summary>
        public virtual void OnCollideWithLiquid()
        {
            EntityPos pos = LocalPos;
            float yDistance = (float)Math.Abs(PositionBeforeFalling.Y - pos.Y);

            float splashStrength = (2 * GameMath.Sqrt((CollisionBox.X2 - CollisionBox.X1) * (CollisionBox.Y2 - CollisionBox.Y1)) + GameMath.Sqrt(yDistance) / 2);

            if (splashStrength < 0.4f || yDistance < 0.25f) return;

            Block block = World.BlockAccessor.GetBlock((int)pos.X, (int)(pos.Y - CollisionBox.Y1), (int)pos.Z);

            string[] soundsBySize = new string[]{ "sounds/environment/smallsplash", "sounds/environment/mediumsplash", "sounds/environment/largesplash" };
            string sound = soundsBySize[(int)GameMath.Clamp(splashStrength / 1.2f, 0, 2)];

            
            World.PlaySoundAt(new AssetLocation(sound), (float)pos.X, (float)pos.Y, (float)pos.Z, null);
            World.SpawnBlockVoxelParticles(pos.XYZ, block, CollisionBox.X2 - CollisionBox.X1, (int)(4 * splashStrength));

            if (splashStrength >= 2)
            {
                SplashParticleProps.basePos.Set(pos.X, pos.Y, pos.Z);
                World.SpawnParticles(SplashParticleProps);

                SplashParticleProps.basePos.Set(pos.X, pos.Y - 0.5f, pos.Z);
                World.SpawnParticles(AirBubbleParticleProps);
            }
            
        }

        /// <summary>
        /// Called when the entity has left a liquid
        /// </summary>
        public virtual void OnExitedLiquid()
        {

        }

        /// <summary>
        /// Sets the name of the entity. E.g. the players name for EntityPlayers
        /// </summary>
        /// <param name="playername"></param>
        public virtual void SetName(string playername)
        {
            foreach (EntityBehavior behavior in Behaviors)
            {
                behavior.OnSetEntityName(playername);
            }
        }

        /// <summary>
        /// Called every 1/75 second
        /// </summary>
        /// <param name="dt"></param>
        public virtual void OnGameTick(float dt)
        {
            foreach (EntityBehavior behavior in Behaviors)
            {
                behavior.OnGameTick(dt);
            }

            if (World is IClientWorldAccessor && World.Rand.NextDouble() < Type.IdleSoundChance / 100.0)
            {
                PlayEntitySound("idle", null, true, Type.IdleSoundRange);
            }
        }

        /// <summary>
        /// Called when the entity spawns
        /// </summary>
        public virtual void OnEntitySpawn()
        {
            foreach (EntityBehavior behavior in Behaviors)
            {
                behavior.OnEntitySpawn();
            }
        }

        /// <summary>
        /// Called when an entity has interacted with this entity
        /// </summary>
        /// <param name="byEntity"></param>
        /// <param name="itemslot">If being interacted with a block/item, this should be the slot the item is being held in</param>
        /// <param name="hitPosition">Relative position on the entites hitbox where the entity interacted at</param>
        /// <param name="mode">0 = attack, 1 = interact</param>
        public virtual void OnInteract(EntityAgent byEntity, IItemSlot itemslot, Vec3d hitPosition, int mode)
        {
            
        }




        /// <summary>
        /// Called when the entity despawns
        /// </summary>
        /// <param name="despawn"></param>
        public virtual void OnEntityDespawn(EntityDespawnReason despawn)
        {
            foreach (EntityBehavior behavior in Behaviors)
            {
                behavior.OnEntityDespawn(despawn);
            }

        }

        /// <summary>
        /// Called by client when a new server pos arrived
        /// </summary>
        public void OnReceivedServerPos()
        {
            EnumHandling handled = EnumHandling.NotHandled;

            foreach (EntityBehavior behavior in Behaviors)
            {
                behavior.OnReceivedServerPos(ref handled);
                if (handled == EnumHandling.Last) break;
            }

            if (handled == EnumHandling.NotHandled)
            {
                Pos.SetFrom(ServerPos);
            }
        }



        /// <summary>
        /// Helper method to set the CollisionBox
        /// </summary>
        /// <param name="length"></param>
        /// <param name="height"></param>
        public virtual void SetHitbox(float length, float height)
        {
            CollisionBox = new Cuboidf()
            {
                X1 = -length/2,
                Z1 = -length/2,
                X2 = length/2,
                Z2 = length/2,
                Y2 = height
            };
        }

        /// <summary>
        /// Adds given behavior to the entities list of active behaviors
        /// </summary>
        /// <param name="behavior"></param>
        public virtual void AddBehavior(EntityBehavior behavior)
        {
            Behaviors.Add(behavior);
        }


        /// <summary>
        /// Removes given behavior to the entities list of active behaviors. Does nothing if the behavior has already been removed
        /// </summary>
        /// <param name="behavior"></param>
        public virtual void RemoveBehavior(EntityBehavior behavior)
        {
            Behaviors.Remove(behavior);
        }

        /// <summary>
        /// Returns true if the entity has given active behavior
        /// </summary>
        /// <param name="behaviorName"></param>
        /// <returns></returns>
        public virtual bool HasBehavior(string behaviorName)
        {
            for (int i = 0; i < Behaviors.Count; i++)
            {
                if (Behaviors[i].PropertyName().Equals(behaviorName)) return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the behavior instance for given entity. Returns null if it doesn't exist.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual EntityBehavior GetBehavior(string name)
        {
            return Behaviors.FirstOrDefault(bh => bh.PropertyName().Equals(name));
        }


        

        /// <summary>
        /// Returns true if given activity is running
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool IsActivityRunning(string key)
        {
            long val;
            ActivityTimers.TryGetValue(key, out val);
            return val > World.ElapsedMilliseconds;
        }

        /// <summary>
        /// Returns the remaining time on an activity in milliesconds
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual int RemainingActivityTime(string key)
        {
            long val;
            ActivityTimers.TryGetValue(key, out val);
            return (int)(val - World.ElapsedMilliseconds);
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
        /// Client: Starts given animation
        /// Server: Sends all active anims to all connected clients then purges the ActiveAnimationsByAnimCode list
        /// </summary>
        /// <param name="animdata"></param>
        public virtual void StartAnimation(AnimationMetaData animdata)
        {
            AnimationMetaData activeAnimdata = null;

            // Already active, won't do anything
            if (ActiveAnimationsByAnimCode.TryGetValue(animdata.Animation, out activeAnimdata) && activeAnimdata == animdata) return;

            if (animdata.Code == null)
            {
                throw new Exception("anim meta data code cannot be null!");
            }

            AnimationsDirty = true;
            if (animdata.Code == null) animdata.Code = animdata.Animation;
            ActiveAnimationsByAnimCode[animdata.Animation] = animdata;
            SetDebugAnimsInfo();
        }


        /// <summary>
        /// Start a new animation defined in the entity config file. If it's not defined, it won't play.
        /// Use StartAnimation(AnimationMetaData animdata) to circumvent the entity config anim data.
        /// </summary>
        /// <param name="configCode">Anim config code, not the animation code!</param>
        public virtual void StartAnimation(string configCode)
        {
            if (configCode == null) return;

            AnimationMetaData animdata = null;

            if (Type.Client.AnimationsByMetaCode.TryGetValue(configCode, out animdata))
            {
                StartAnimation(animdata);
                
                return;
            } else
            {
                //Console.WriteLine("anim not found " + configCode);
            }
        }

        /// <summary>
        /// Stops given animation
        /// </summary>
        /// <param name="code"></param>
        public virtual void StopAnimation(string code)
        {
            if (code == null) return;

            if (World.Side == EnumAppSide.Server)
            {
                AnimationsDirty = true;
            }
            
            if (!ActiveAnimationsByAnimCode.Remove(code) && ActiveAnimationsByAnimCode.Count > 0)
            {
                foreach (var val in ActiveAnimationsByAnimCode)
                {
                    if (val.Value.Code == code)
                    {
                        ActiveAnimationsByAnimCode.Remove(val.Key);
                        break;
                    }
                }
            }

            SetDebugAnimsInfo();
        }

        /// <summary>
        /// Updates the DebugAttributes tree
        /// </summary>
        public void SetDebugAnimsInfo()
        {
            if (World.Side != EnumAppSide.Client) return;

            DebugAttributes.SetString("Entity Id", ""+EntityId);
            
            string anims = "";
            int i = 0;
            foreach (string anim in ActiveAnimationsByAnimCode.Keys)
            {
                if (i++ > 0) anims += ",";
                anims += anim;
            }

            DebugAttributes.SetString("Active Animations", anims.Length > 0 ? anims : "-");
        }



        /// <summary>
        /// Serializes the slots contents to be stored in the SaveGame
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="forClient">True when being used to send an entity to the client</param>
        public virtual void ToBytes(BinaryWriter writer, bool forClient)
        {
            if (!forClient)
            {
                writer.Write(GameVersion.ShortGameVersion);
            }
            
            writer.Write(Entityid);
            WatchedAttributes.SetInt("entityState", (int)State);
            WatchedAttributes.ToBytes(writer);
            ServerPos.ToBytes(writer);
            writer.Write(PositionBeforeFalling.X);
            writer.Write(PositionBeforeFalling.Y);
            writer.Write(PositionBeforeFalling.Z);

            if (entityType.Code == null)
            {
                World.Logger.Error("Entity.ToBytes(): entityType.Code is null?! Entity will probably be incorrectly saved to disk");
            }

            writer.Write(entityType.Code?.ToShortString());

            if (!forClient)
            {
                Attributes.ToBytes(writer);
            }

            writer.Write(ActiveAnimationsByAnimCode.Count);
            foreach (var val in ActiveAnimationsByAnimCode)
            {
                if (val.Value == null) continue;

                writer.Write(val.Key);
                if (val.Value.Code == null) val.Value.Code = val.Key; // ah wtf.
                val.Value.ToBytes(writer);
            }
        }



        /// <summary>
        /// Loads the entity from a stored byte array from the SaveGame
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="fromServer"></param>
        public virtual void FromBytes(BinaryReader reader, bool fromServer)
        {
            string version = "";
            if (!fromServer)
            {
                version = reader.ReadString();
            }

            Entityid = reader.ReadInt64();
            WatchedAttributes.FromBytes(reader);
            State = (EnumEntityState)WatchedAttributes.GetInt("entityState", 0);

            ServerPos.FromBytes(reader);
            Pos.SetFrom(ServerPos);
            PositionBeforeFalling.X = reader.ReadDouble();
            PositionBeforeFalling.Y = reader.ReadDouble();
            PositionBeforeFalling.Z = reader.ReadDouble();
            savedTypeCode = new AssetLocation(reader.ReadString());

            if (!fromServer)
            {
                Attributes.FromBytes(reader);
            }

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string code = reader.ReadString();

                if (GameVersion.IsAtLeastVersion(version, "1.4.9"))
                {
                    ActiveAnimationsByAnimCode[code] = AnimationMetaData.FromBytes(reader);

                    if (ActiveAnimationsByAnimCode[code] == null)
                    {
                        throw new Exception("anim meta data code cannot be null!");
                    }
                }
                else if (GameVersion.IsAtLeastVersion(version, "1.3.8"))
                {
                    ActiveAnimationsByAnimCode[code] = new AnimationMetaData() { Animation = code, AnimationSpeed = reader.ReadSingle() };
                }
                else
                {
                    ActiveAnimationsByAnimCode[code] = new AnimationMetaData() { Animation = code, AnimationSpeed = 1f };
                }
            }
        }

        /// <summary>
        /// Makes the entity despawn
        /// </summary>
        public virtual void Die(EnumDespawnReason reason = EnumDespawnReason.Death, DamageSource damageSourceForDeath = null)
        {
            Alive = false;

            if (reason == EnumDespawnReason.Death)
            {
                ItemStack[] drops = GetDrops(World, Pos.AsBlockPos, null);
                if (drops != null)
                {
                    for (int i = 0; i < drops.Length; i++)
                    {
                        World.SpawnItemEntity(drops[i], LocalPos.XYZ.AddCopy(0, 0.25, 0));
                    }
                }
            }

            DespawnReason = new EntityDespawnReason() {
                reason = reason,
                damageSourceForDeath = damageSourceForDeath
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
            AssetLocation[] locations = null;
            if (entityType.ResolvedSounds != null && entityType.ResolvedSounds.TryGetValue(type, out locations) && locations.Length > 0)
            {
                World.PlaySoundAt(
                    locations[World.Rand.Next(locations.Length)], 
                    (float)LocalPos.X, (float)LocalPos.Y, (float)LocalPos.Z, 
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
            EnumHandling handled = EnumHandling.NotHandled;

            foreach (EntityBehavior behavior in Behaviors)
            {
                behavior.OnStateChanged(beforeState, ref handled);
                if (handled == EnumHandling.Last) return;
            }
        }

        /// <summary>
        /// This method pings the Notify() method of all behaviors and ai tasks. Can be used to spread information to other creatures.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public virtual void Notify(string key, object data)
        {
            foreach (EntityBehavior behavior in Behaviors)
            {
                behavior.Notify(key, data);
            }
        }

        /// <summary>
        /// True if given emotion state is currently set
        /// </summary>
        /// <param name="statecode"></param>
        /// <returns></returns>
        public bool HasEmotionState(string statecode)
        {
            ITreeAttribute attr = Attributes.GetTreeAttribute("emotionstates");
            return attr != null && attr.HasAttribute(statecode);
        }

    }
}
