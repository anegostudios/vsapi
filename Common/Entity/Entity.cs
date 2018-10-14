using System.Collections.Generic;
using System.IO;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Config;
using System;
using System.Linq;
using Vintagestory.API.Util;
using System.Text;

namespace Vintagestory.API.Common.Entities
{
    /// <summary>
    /// The basic class for all entities in the game
    /// </summary>
    public abstract class Entity : RegistryObject, IEntity
    {
        public static WaterSplashParticles SplashParticleProps = new WaterSplashParticles();
        public static AirBubbleParticles AirBubbleParticleProps = new AirBubbleParticles();

        /// <summary>
        /// World where the entity is spawned in
        /// </summary>
        public IWorldAccessor World;

        public ICoreAPI Api;

        public EntityProperties Properties { private set; get; }

        public EntitySidedProperties SidedProperties
        {
            get
            {
                if (Properties == null)
                    return null;
                if (World.Side.IsClient())
                    return Properties.Client;
                return Properties.Server;
            }
        }


        /// <summary>
        /// Server simulated animations. Only takes care of stopping animations once they're done
        /// Set and Called by the Entities ServerSystem
        /// </summary>
        public ServerEntityAnimator ServerAnimator;

        /// <summary>
        /// An uptime value running activities.
        /// </summary>
        public Dictionary<string, long> ActivityTimers = new Dictionary<string, long>();

        public Dictionary<string, AnimationMetaData> ActiveAnimationsByAnimCode = new Dictionary<string, AnimationMetaData>();


        public bool AnimationsDirty;
        
        public bool BroadcastServerPos;

        /// <summary>
        /// Client position
        /// </summary>
        public SyncedEntityPos Pos = new SyncedEntityPos();

        /// <summary>
        /// Server simulated position. If this value differs to greatly from client pos we have to override client pos
        /// </summary>
        public EntityPos ServerPos = new EntityPos();

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

        public long InChunkIndex3d;

        EntityPos IEntity.Pos
        {
            get { return Pos; }
        }

        EntityPos IEntity.ServerPos
        {
            get { return ServerPos; }
        }

        public Cuboidf CollisionBox;

        Cuboidf IEntity.CollisionBox
        {
            get { return CollisionBox; }
        }

        public bool Teleporting;

        public virtual double EyeHeight { get { return Properties.EyeHeight; } }


        /// <summary>
        /// A unique identifier for this entity
        /// </summary>
        public long EntityId;

        long IEntity.EntityId
        {
            get { return EntityId; }
        }

        AssetLocation IEntity.Code => Code;

        public int SimulationRange;

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

        ITreeAttribute IEntity.WatchedAttributes
        {
            get { return WatchedAttributes; }
        }

        ITreeAttribute IEntity.Attributes
        {
            get { return Attributes; }
        }


        /// <summary>
        /// The face the entity is climbing on. Null if the entity is not climbing
        /// </summary>
        public BlockFacing ClimbingOnFace;

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
        /// CollidedVertically || CollidedHorizontally
        /// </summary>
        public bool Collided { get { return CollidedVertically || CollidedHorizontally; } }

        /// <summary>
        /// If gravity should applied to this entity
        /// </summary>
        public virtual bool ApplyGravity
        {
            get { return Properties.Habitat == EnumHabitat.Land || (Properties.Habitat == EnumHabitat.Sea && !Swimming); }
        }


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

        

        // Stored in WatchedAttributes in from/tobytes 
        public EnumEntityState State;

        public EntityDespawnReason DespawnReason;

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
        /// Players and whatever the player rides on will be stored seperatly
        /// </summary>
        public virtual bool StoreWithChunk { get { return true; } }

        /// <summary>
        /// Whether this entity should always stay in Active model, regardless on how far away other player are
        /// </summary>
        public virtual bool AlwaysActive { get { return false; } }



        /// <summary>
        /// Set by the client renderer when the entity was rendered last frame
        /// </summary>
        public bool IsRendered;

        public bool IsShadowRendered;

        /// <summary>
        /// Color used when the entity is being attacked
        /// </summary>
        protected int HurtColor = ColorUtil.ToRgba(255, 255, 100, 100);

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
        /// Called when something tries to given an itemstack to this entity
        /// </summary>
        /// <param name="itemstack"></param>
        /// <returns></returns>
        public virtual bool TryGiveItemStack(ItemStack itemstack)
        {
            return false;
        }

        public virtual void OnReceivedServerAnimations(int[] activeAnimations, int activeAnimationsCount, float[] activeAnimationSpeeds)
        {
            if (EntityId != (World as IClientWorldAccessor).Player.Entity.EntityId)
            {
                ActiveAnimationsByAnimCode.Clear();
            }

            for (int i = 0; i < activeAnimationsCount; i++)
            {
                int crc32 = activeAnimations[i];
                for (int j = 0; j < Properties.Client.LoadedShape.Animations.Length; j++)
                {
                    Animation anim = Properties.Client.LoadedShape.Animations[j];
                    int mask = ~(1 << 31); // Because I fail to get the sign bit transmitted correctly over the network T_T
                    if ((anim.CodeCrc32 & mask) == (crc32 & mask))
                    {
                        if (ActiveAnimationsByAnimCode.ContainsKey(anim.Code)) break;

                        string code = anim.Code == null ? anim.Name.ToLowerInvariant() : anim.Code;

                        AnimationMetaData animmeta = null;
                        Properties.Client.AnimationsByMetaCode.TryGetValue(code, out animmeta);

                        if (animmeta == null)
                        {
                            animmeta = new AnimationMetaData()
                            {
                                Code = code,
                                Animation = code,
                                CodeCrc32 = anim.CodeCrc32
                            };
                        }

                        animmeta.AnimationSpeed = activeAnimationSpeeds[i];

                        ActiveAnimationsByAnimCode[anim.Code] = animmeta;
                    }
                }
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
            SimulationRange = GlobalConstants.DefaultTrackingRange;

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
        /// Extra check if an ai task should be started
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public virtual bool ShouldExecuteTask(IAiTask task)
        {
            return true;
        }

        /// <summary>
        /// Called when this entity got created or loaded
        /// </summary>
        /// <param name="world"></param>
        /// <param name="InChunkIndex3d"></param>
        public virtual void Initialize(EntityProperties properties, ICoreAPI api, long InChunkIndex3d)
        {
            this.World = api.World;
            this.Api = api;
            this.Properties = properties;
            this.Class = properties.Class;
            this.InChunkIndex3d = InChunkIndex3d;

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

            if (Properties.HitBoxSize != null)
            {
                SetHitbox(Properties.HitBoxSize.X, Properties.HitBoxSize.Y);
            }

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
                    if (players[i].Entity == null) continue;

                    if (Pos.InRangeOf(players[i].Entity.Pos, SimulationRange * SimulationRange))
                    {
                        State = EnumEntityState.Active;
                        break;
                    }
                }
            }

            if (api.Side.IsServer())
            {
                if (properties.Client?.FirstTexture?.Alternates != null && !WatchedAttributes.HasAttribute("textureIndex"))
                {
                    WatchedAttributes.SetInt("textureIndex", World.Rand.Next(properties.Client.FirstTexture.Alternates.Length + 1));
                }

                loadServerAnimator();
            }

            this.Properties.Initialize(this, api);
        }

        /// <summary>
        /// Loads the server side lightweight animator (does not calcuate any transformation matrices). This one is used for correct syncing of animations.
        /// </summary>
        protected virtual void loadServerAnimator()
        {
            if (Properties.Client?.Shape?.Base != null)
            {
                Shape shape = World.AssetManager.Get(Properties.Client.Shape.Base.Clone().WithPathPrefix("shapes/").WithPathAppendix(".json")).ToObject<Shape>();
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

            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                stacks = behavior.GetDrops(world, pos, byPlayer, ref handled);
                if (handled == EnumHandling.PreventSubsequent) return stacks;
            }

            if (handled == EnumHandling.PreventDefault) return stacks;

            if (Properties.Drops == null) return null;
            List<ItemStack> todrop = new List<ItemStack>();

            for (int i = 0; i < Properties.Drops.Length; i++)
            {
                ItemStack stack = Properties.Drops[i].GetNextItemStack();
                if (stack == null) continue;

                todrop.Add(stack);
                if (Properties.Drops[i].LastDrop) break;
            }

            return todrop.ToArray();
        }

        /// <summary>
        /// Teleports the entity to given position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public virtual void TeleportToDouble(double x, double y, double z)
        {
            Teleporting = true;
            ICoreServerAPI sapi = this.World.Api as ICoreServerAPI;
            if (sapi != null)
            {
                sapi.World.LoadChunkColumn((int)ServerPos.X / World.BlockAccessor.ChunkSize, (int)ServerPos.Z / World.BlockAccessor.ChunkSize, () =>
                {
                    Pos.SetPos(x, y, z);
                    ServerPos.SetPos(x, y, z);
                    PreviousServerPos.SetPos(-99, -99, -99);
                    PositionBeforeFalling.Set(x, y, z);
                    Pos.Motion.Set(0, 0, 0);
                    Teleporting = false;
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
            if ((!Alive || IsActivityRunning("invulnerable")) && damageSource.Type != EnumDamageType.Heal) return false;

            if (ShouldReceiveDamage(damageSource, damage)) {
                if (damageSource.Type != EnumDamageType.Heal)
                {
                    WatchedAttributes.SetInt("onHurtCounter", WatchedAttributes.GetInt("onHurtCounter") + 1);
                    WatchedAttributes.SetFloat("onHurt", damage); // Causes the client to be notified
                    StartAnimation("hurt");
                }

                foreach (EntityBehavior behavior in SidedProperties.Behaviors)
                {
                    behavior.OnEntityReceiveDamage(damageSource, damage);
                }

                if (damageSource.GetSourcePosition() != null)
                {
                    Vec3d dir = (ServerPos.XYZ - damageSource.GetSourcePosition()).Normalize();
                    dir.Y = 0.1f;
                    float factor = GameMath.Clamp((1 - Properties.KnockbackResistance) / 10f, 0, 2);
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
            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                behavior.OnFallToGround(PositionBeforeFalling, motionY);
            }
        }

        /// <summary>
        /// Called when the entity got in touch with a liquid
        /// </summary>
        public virtual void OnCollideWithLiquid()
        {
            if (World.Side == EnumAppSide.Server) return;

            EntityPos pos = LocalPos;
            float yDistance = (float)Math.Abs(PositionBeforeFalling.Y - pos.Y);

            double width = CollisionBox.X2 - CollisionBox.X1;
            double height = CollisionBox.Y2 - CollisionBox.Y1;

            double splashStrength = 2 * GameMath.Sqrt(width * height) + pos.Motion.Length() * 10;

            if (splashStrength < 0.4f || yDistance < 0.25f) return;

            Block block = World.BlockAccessor.GetBlock((int)pos.X, (int)(pos.Y - CollisionBox.Y1), (int)pos.Z);

            string[] soundsBySize = new string[]{ "sounds/environment/smallsplash", "sounds/environment/mediumsplash", "sounds/environment/largesplash" };
            string sound = soundsBySize[(int)GameMath.Clamp(splashStrength / 1.6, 0, 2)];

            splashStrength = Math.Min(10, splashStrength);

            World.PlaySoundAt(new AssetLocation(sound), (float)pos.X, (float)pos.Y, (float)pos.Z, null);
            World.SpawnCubeParticles(pos.AsBlockPos, pos.XYZ, CollisionBox.X2 - CollisionBox.X1, (int)(4 * splashStrength));

            if (splashStrength >= 2)
            {
                ////return new Vec3d(BasePos.X + rand.NextDouble() * 0.25 - 0.125, BasePos.Y + 0.1 + rand.NextDouble() * 0.2, BasePos.Z + rand.NextDouble() * 0.25 - 0.125);

                SplashParticleProps.BasePos.Set(pos.X - width / 2, pos.Y, pos.Z - width / 2);
                SplashParticleProps.AddPos.Set(width, 0.3, width);

                SplashParticleProps.AddVelocity.Set((float)pos.Motion.X * 30f, 0, (float)pos.Motion.Z * 30f);
                SplashParticleProps.QuantityMul = (float)splashStrength - 1;
                
                World.SpawnParticles(SplashParticleProps);

                /*AirBubbleParticleProps.BasePos.Set(pos.X, pos.Y - 0.75f, pos.Z);
                AirBubbleParticleProps.AddVelocity.Set((float)pos.Motion.X * 30f, 0, (float)pos.Motion.Z * 30f);
                World.SpawnParticles(AirBubbleParticleProps);*/
            }
            
        }

        /// <summary>
        /// Called when the entity has left a liquid
        /// </summary>
        public virtual void OnExitedLiquid()
        {

        }

        /// <summary>
        /// Called every 1/75 second
        /// </summary>
        /// <param name="dt"></param>
        public virtual void OnGameTick(float dt)
        {
            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                behavior.OnGameTick(dt);
            }

            if (World is IClientWorldAccessor && World.Rand.NextDouble() < Properties.IdleSoundChance / 100.0)
            {
                PlayEntitySound("idle", null, true, Properties.IdleSoundRange);
            }
        }

        /// <summary>
        /// Called when the entity spawns
        /// </summary>
        public virtual void OnEntitySpawn()
        {
            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
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
            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
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

            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                behavior.OnReceivedServerPos(ref handled);
                if (handled == EnumHandling.PreventSubsequent) break;
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
            SidedProperties.Behaviors.Add(behavior);
        }


        /// <summary>
        /// Removes given behavior to the entities list of active behaviors. Does nothing if the behavior has already been removed
        /// </summary>
        /// <param name="behavior"></param>
        public virtual void RemoveBehavior(EntityBehavior behavior)
        {
            SidedProperties.Behaviors.Remove(behavior);
        }

        /// <summary>
        /// Returns true if the entity has given active behavior
        /// </summary>
        /// <param name="behaviorName"></param>
        /// <returns></returns>
        public virtual bool HasBehavior(string behaviorName)
        {
            for (int i = 0; i < SidedProperties.Behaviors.Count; i++)
            {
                if (SidedProperties.Behaviors[i].PropertyName().Equals(behaviorName)) return true;
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
            return SidedProperties.Behaviors.FirstOrDefault(bh => bh.PropertyName().Equals(name));
        }

        /// <summary>
        /// Returns the first behavior instance for given entity of given type. Returns null if it doesn't exist.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual T GetBehavior<T>() where T : EntityBehavior
        {
            return (T)SidedProperties.Behaviors.FirstOrDefault(bh => bh is T);
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

            if (Properties.Client.AnimationsByMetaCode.TryGetValue(configCode, out animdata))
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
            
            writer.Write(EntityId);
            WatchedAttributes.SetInt("entityState", (int)State);
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

            EntityId = reader.ReadInt64();
            WatchedAttributes.FromBytes(reader);

            if (GameVersion.IsLowerVersionThan(version, "1.7.0") && this is EntityPlayer)
            {
                ITreeAttribute healthTree = WatchedAttributes.GetTreeAttribute("health");
                if (healthTree != null)
                {
                    healthTree.SetFloat("basemaxhealth", 15);
                }
            }

            if (!fromServer)
            {
                State = (EnumEntityState)WatchedAttributes.GetInt("entityState", 0);
            }
            

            ServerPos.FromBytes(reader);
            Pos.SetFrom(ServerPos);
            PositionBeforeFalling.X = reader.ReadDouble();
            PositionBeforeFalling.Y = reader.ReadDouble();
            PositionBeforeFalling.Z = reader.ReadDouble();
            Code = new AssetLocation(reader.ReadString());

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
        /// Called when on the client side something called capi.Network.SendEntityPacket()
        /// </summary>
        /// <param name="player"></param>
        /// <param name="packetid"></param>
        /// <param name="data"></param>
        public virtual void OnReceivedClientPacket(IServerPlayer player, int packetid, byte[] data)
        {
        }

        /// <summary>
        /// Called when on the server side something called sapi.Network.SendEntityPacket()
        /// </summary>
        /// <param name="player"></param>
        /// <param name="packetid"></param>
        /// <param name="data"></param>
        public virtual void OnReceivedServerPacket(int packetid, byte[] data)
        {
            // Teleport packet
            if (packetid == 1)
            {
                Vec3d newPos = SerializerUtil.Deserialize<Vec3d>(data);
                this.Pos.SetPos(newPos);
                this.ServerPos.SetPos(newPos);
                this.World.BlockAccessor.MarkBlockDirty(newPos.AsBlockPos);
            }
        }



        /// <summary>
        /// Makes the entity despawn. Entities only drop something on EnumDespawnReason.Death
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

                StartAnimation("die");
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
            if (Properties.ResolvedSounds != null && Properties.ResolvedSounds.TryGetValue(type, out locations) && locations.Length > 0)
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

            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                behavior.OnStateChanged(beforeState, ref handled);
                if (handled == EnumHandling.PreventSubsequent) return;
            }
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
        /// True if given emotion state is currently set
        /// </summary>
        /// <param name="statecode"></param>
        /// <returns></returns>
        public virtual bool HasEmotionState(string statecode)
        {
            ITreeAttribute attr = Attributes.GetTreeAttribute("emotionstates");
            return attr != null && attr.HasAttribute(statecode);
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

            Pos.X += startPos.X;
            Pos.Y += startPos.Y;
            Pos.Z += startPos.Z;

            PositionBeforeFalling.X += startPos.X;
            PositionBeforeFalling.Y += startPos.Y;
            PositionBeforeFalling.Z += startPos.Z;
        }


        public virtual string GetName()
        {
            return Lang.Get(Code.Domain + ":item-creature-" + Code.Path);
        }

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


            return infotext.ToString();
        }

    }
}
