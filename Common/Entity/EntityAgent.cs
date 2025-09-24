using System;
using System.IO;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    public enum EnumHand
    {
        Left,
        Right
    }

    public interface IWearableShapeSupplier
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="forEntity"></param>
        /// <param name="texturePrefixCode"></param>
        /// <returns>null for returning back to default behavior (read shape from attributes)</returns>
        Shape GetShape(ItemStack stack, Entity forEntity, string texturePrefixCode);
    }


    /// <summary>
    /// A goal-directed entity which observes and acts upon an environment
    /// </summary>
    public class EntityAgent : Entity
    {
        public override bool IsCreature { get { return true; } }

        /// <summary>
        /// No swivel when we are mounted
        /// </summary>
        public override bool CanSwivel => base.CanSwivel && MountedOn==null;
        public override bool CanStepPitch => base.CanStepPitch && MountedOn == null;

        /// <summary>
        /// The yaw of the agents body
        /// </summary>
        public virtual float BodyYaw { get; set; }

        /// <summary>
        /// The yaw of the agents body on the client, retrieved from the server (BehaviorInterpolatePosition lerps this value and sets BodyYaw)
        /// </summary>
        public virtual float BodyYawServer { get; set; }

        public float sidewaysSwivelAngle;

        /// <summary>
        /// True if all clients have to be informed about this entities death. Set to false once all clients have been notified
        /// </summary>
        public bool DeadNotify;

        /// <summary>
        /// Unique identifier for a herd
        /// </summary>
        public long HerdId
        {
            get => herdId;
            set
            {
                WatchedAttributes.SetLong("herdId", value);
                herdId = value;
            }
        }

        protected long herdId = 0;

        protected EntityControls controls;
        protected EntityControls servercontrols;
        protected bool alwaysRunIdle = false;
        public IMountableSeat MountedOn { get; protected set; }
        public EnumEntityActivity CurrentControls;


        internal virtual bool LoadControlsFromServer
        {
            get { return true; }
        }

        /// <summary>
        /// Item in the left hand slot of the entity agent.
        /// </summary>
        public virtual ItemSlot LeftHandItemSlot { get; set; }

        /// <summary>
        /// Item in the right hand slot of the entity agent.
        /// </summary>
        public virtual ItemSlot RightHandItemSlot { get; set; }

        public virtual ItemSlot ActiveHandItemSlot => RightHandItemSlot;


        /// <summary>
        /// Whether or not the entity should despawn.
        /// </summary>
        public override bool ShouldDespawn
        {
            get { return !Alive && AllowDespawn; }
        }

        /// <summary>
        /// Whether or not the entity is allowed to despawn (Default: true)
        /// </summary>
        public bool AllowDespawn = true;




        public EntityAgent() : base()
        {
            controls = new EntityControls();
            servercontrols = new EntityControls();
        }


        public override void Initialize(EntityProperties properties, ICoreAPI api, long InChunkIndex3d)
        {
            base.Initialize(properties, api, InChunkIndex3d);

            if (World.Side == EnumAppSide.Server)
            {
                servercontrols = controls;
            }

            WatchedAttributes.RegisterModifiedListener("mountedOn", updateMountedState);

            if (WatchedAttributes["mountedOn"] != null)
            {
                MountedOn = World.ClassRegistry.GetMountable(WatchedAttributes["mountedOn"] as TreeAttribute);
                if (MountedOn != null)
                {
                    if (TryMount(MountedOn) && Api.Side == EnumAppSide.Server && MountedOn.MountSupplier?.OnEntity is Entity entity)
                    {
                        Api.World.Logger.Audit("{0} loaded already mounted/seated on a {1} at {2}", GetName(), entity.Code.ToShortString(), entity.ServerPos.AsBlockPos);
                    }
                }
            }

            herdId = WatchedAttributes.GetLong("herdId", 0);
        }


        /// <summary>
        /// The controls for this entity.
        /// </summary>
        public EntityControls Controls
        {
            get { return controls; }
        }

        /// <summary>
        /// The server controls for this entity
        /// </summary>
        public EntityControls ServerControls
        {
            get { return servercontrols; }
        }

        /// <summary>
        /// Are the eyes of this entity submerged in liquid?
        /// </summary>
        /// <returns></returns>
        public bool IsEyesSubmerged()
        {
            BlockPos pos = SidedPos.AsBlockPos.Add(0, (float)(Swimming ? Properties.SwimmingEyeHeight : Properties.EyeHeight), 0);
            return World.BlockAccessor.GetBlock(pos).MatterState == EnumMatterState.Liquid;
        }


        /// <summary>
        /// Attempts to mount this entity on a target.
        /// </summary>
        /// <param name="onmount">The mount to mount</param>
        /// <returns>Whether it was mounted or not.</returns>
        public virtual bool TryMount(IMountableSeat onmount)
        {
            if (!onmount.CanMount(this)) return false;

            // load current controls when mounting onto the mountable
            onmount.Controls.FromInt(Controls.ToInt());

            if (MountedOn != null && MountedOn != onmount)
            {
                var seat = MountedOn.MountSupplier.GetSeatOfMountedEntity(this);
                if (seat != null) seat.DoTeleportOnUnmount = false;
                if (!TryUnmount()) return false;
                if (seat != null) seat.DoTeleportOnUnmount = true;
            }

            doMount(onmount);
            var mountableTree = new TreeAttribute();
            onmount.MountableToTreeAttributes(mountableTree);
            WatchedAttributes["mountedOn"] = mountableTree;
            if (World.Side == EnumAppSide.Server)
            {
                var sebh = MountedOn?.MountSupplier.OnEntity?.GetBehavior("seatable");
                if (sebh != null)
                {
                    sebh.ToBytes(true);
                    MountedOn?.MountSupplier.OnEntity.WatchedAttributes.MarkPathDirty("seatdata");
                }

                WatchedAttributes.MarkPathDirty("mountedOn");
            }
            return true;
        }

        protected virtual void updateMountedState()
        {
            if (WatchedAttributes.HasAttribute("mountedOn"))
            {
                var mountable = World.ClassRegistry.GetMountable(WatchedAttributes["mountedOn"] as TreeAttribute);
                var seat = MountedOn?.MountSupplier.GetSeatOfMountedEntity(this);
                if (MountedOn != null && mountable != null &&
                    (MountedOn.Entity?.EntityId != mountable.Entity?.EntityId || mountable.SeatId != seat?.SeatId))
                {
                    if (seat != null) seat.DoTeleportOnUnmount = false;
                    TryUnmount();
                    if (seat != null) seat.DoTeleportOnUnmount = true;
                }

                if (MountedOn == null || (mountable != null && mountable.SeatId != seat?.SeatId))
                {
                    doMount(mountable);
                }
            }
            else
            {
                TryUnmount();
            }

        }

        public AnimationMetaData curMountedAnim = null;
        protected virtual void doMount(IMountableSeat mountable)
        {
            this.MountedOn = mountable;
            controls.StopAllMovement();

            if (mountable == null)
            {
                WatchedAttributes.RemoveAttribute("mountedOn");
                return;
            }

            mountable.Entity?.AnimManager?.StopAllAnimations();

            if (MountedOn?.SuggestedAnimation != null)
            {
                curMountedAnim = MountedOn.SuggestedAnimation;
                AnimManager?.StopAllAnimations();
                AnimManager?.StartAnimation(curMountedAnim);
            }

            mountable.DidMount(this);
        }

        /// <summary>
        /// Attempts to un-mount the player.
        /// </summary>
        /// <returns>Whether or not unmounting was successful</returns>
        public bool TryUnmount()
        {
            if (MountedOn?.CanUnmount(this) == false) return false;

            if (curMountedAnim != null)
            {
                AnimManager?.StopAnimation(curMountedAnim.Animation);
                curMountedAnim = null;
            }

            // allow the tryTeleportToFreeLocation to tp the unmounted entity and not the mount
            var oldMountedOn = MountedOn;
            MountedOn = null;
            oldMountedOn?.DidUnmount(this);

            if (WatchedAttributes.HasAttribute("mountedOn"))
            {
                WatchedAttributes.RemoveAttribute("mountedOn");
            }

            if (World.Side == EnumAppSide.Server)
            {
                var sebh = oldMountedOn?.MountSupplier.OnEntity?.GetBehavior("seatable");
                if (sebh != null)
                {
                    sebh.ToBytes(true);
                    oldMountedOn?.MountSupplier.OnEntity.WatchedAttributes.MarkPathDirty("seatdata");
                }
            }

            if (Api.Side == EnumAppSide.Server && oldMountedOn != null && oldMountedOn.MountSupplier?.OnEntity is Entity entity) Api.World.Logger.Audit("{0} dismounts/disembarks from a {1} at {2}", GetName(), entity.Code.ToShortString(), entity.ServerPos.AsBlockPos);

            return true;
        }

        public override void Die(EnumDespawnReason reason = EnumDespawnReason.Death, DamageSource damageSourceForDeath = null)
        {
            if (Alive && reason == EnumDespawnReason.Death)
            {
                PlayEntitySound("death");
                if (damageSourceForDeath?.GetCauseEntity() is EntityPlayer player)
                {
                    Api.Logger.Audit("Player {0} killed {1} at {2}", player.GetName(), Code, Pos.AsBlockPos);
                }
            }
            if (reason != EnumDespawnReason.Death)
            {
                AllowDespawn = true;
            }

            controls.WalkVector.Set(0, 0, 0);
            controls.FlyVector.Set(0, 0, 0);
            ClimbingOnFace = null;

            base.Die(reason, damageSourceForDeath);
        }

        public override void OnEntityDespawn(EntityDespawnData despawn)
        {
            base.OnEntityDespawn(despawn);

            if (despawn != null && despawn.Reason == EnumDespawnReason.Removed)
            {
                if (this is EntityHumanoid || MountedOn != null) TryUnmount();
            }
        }



        public override void OnInteract(EntityAgent byEntity, ItemSlot slot, Vec3d hitPosition, EnumInteractMode mode)
        {
            EnumHandling handled = EnumHandling.PassThrough;

            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                behavior.OnInteract(byEntity, slot, hitPosition, mode, ref handled);
                if (handled == EnumHandling.PreventSubsequent) break;
            }

            if (handled == EnumHandling.PreventDefault || handled == EnumHandling.PreventSubsequent) return;

            if (mode == EnumInteractMode.Attack)
            {
                float damage = slot.Itemstack == null ? 0.5f : slot.Itemstack.Collectible.GetAttackPower(slot.Itemstack);
                int damagetier = slot.Itemstack == null ? 0 : slot.Itemstack.Collectible.ToolTier;

                var dmgMultiplier = byEntity.Stats.GetBlended("meleeWeaponsDamage");
                if (Properties.Attributes?["isMechanical"].AsBool() == true)
                {
                    dmgMultiplier *= byEntity.Stats.GetBlended("mechanicalsDamage");
                }

                damage *= dmgMultiplier;

                IPlayer byPlayer = null;

                if (byEntity is EntityPlayer && !IsActivityRunning("invulnerable"))
                {
                    byPlayer = (byEntity as EntityPlayer).Player;

                    World.PlaySoundAt(new AssetLocation("sounds/player/slap"), ServerPos.X, ServerPos.InternalY, ServerPos.Z, byPlayer);
                    slot?.Itemstack?.Collectible.OnAttackingWith(byEntity.World, byEntity, this, slot);
                }

                if (Api.Side == EnumAppSide.Client && damage > 1 && !IsActivityRunning("invulnerable") && Properties.Attributes?["spawnDamageParticles"].AsBool() == true)
                {
                    Vec3d pos = SidedPos.XYZ + hitPosition;
                    Vec3d minPos = pos.AddCopy(-0.15, -0.15, -0.15);
                    Vec3d maxPos = pos.AddCopy(0.15, 0.15, 0.15);

                    int textureSubId = this.Properties.Client.FirstTexture.Baked.TextureSubId;
                    Vec3f tmp = new Vec3f();

                    for (int i = 0; i < 10; i++)
                    {
                        int color = (Api as ICoreClientAPI).EntityTextureAtlas.GetRandomColor(textureSubId);

                        tmp.Set(
                            1f - 2*(float)World.Rand.NextDouble(),
                            2*(float)World.Rand.NextDouble(),
                            1f - 2*(float)World.Rand.NextDouble()
                        );

                        World.SpawnParticles(
                            1, color, minPos, maxPos,
                            tmp, tmp, 1.5f, 1f, 0.25f + (float)World.Rand.NextDouble() * 0.25f,
                            EnumParticleModel.Cube, byPlayer
                        );
                    }
                }

                DamageSource dmgSource = new DamageSource()
                {
                    Source = (byEntity as EntityPlayer).Player == null ? EnumDamageSource.Entity : EnumDamageSource.Player,
                    SourceEntity = byEntity,
                    Type = EnumDamageType.BluntAttack,
                    HitPosition = hitPosition,
                    DamageTier = damagetier
                };

                var im = GetInterface<IMountable>();
                if (im != null && im.Controller == byEntity) return;   // A rider cannot damage the entity he is currently riding on: prevents accidental hits due to hitboxes (we gamify this and assume the rider swings the falx to miss his own mount)

                if (ReceiveDamage(dmgSource, damage))
                {
                    byEntity.DidAttack(dmgSource, this);
                }

            }
        }

        protected bool ignoreTeleportCall;
        public override void TeleportToDouble(double x, double y, double z, Action onTeleported = null)
        {
            if (ignoreTeleportCall) return;
            ignoreTeleportCall = true;
            if (MountedOn != null)
            {
                if (MountedOn.Entity == null) TryUnmount();
                else
                {
                    MountedOn.Entity.TeleportToDouble(x, y, z, onTeleported);
                    ignoreTeleportCall = false;
                    return;
                }
            }

            // Add some villager logging for 1.21.2
            if (Code.Path.StartsWith("villager") && Api is Server.ICoreServerAPI sapi)
            {
                string name = "-";
                try
                {
                    name = WatchedAttributes.GetTreeAttribute("nametag")?.GetString("name") ?? "-";
                }
                catch (Exception) { }
                Vec3d target = new Vec3d(x, y, z);
                sapi.Logger.Log(EnumLogType.Worldgen, "In " + sapi.World.WorldName + ", villager " + name + " teleported to " + target.AsBlockPos);
            }

            base.TeleportToDouble(x, y, z, onTeleported);
            ignoreTeleportCall = false;
        }

        public virtual void DidAttack(DamageSource source, EntityAgent targetEntity)
        {
            EnumHandling handled = EnumHandling.PassThrough;

            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                behavior.DidAttack(source, targetEntity, ref handled);
                if (handled == EnumHandling.PreventSubsequent) break;
            }

            if (handled == EnumHandling.PreventDefault || handled == EnumHandling.PreventSubsequent) return;
        }

        public override bool ShouldReceiveDamage(DamageSource damageSource, float damage)
        {
            if (!Alive) return false;

            return true;
        }

        public override bool ReceiveDamage(DamageSource damageSource, float damage)
        {
            bool ret = base.ReceiveDamage(damageSource, damage);

            return ret;
        }

        /// <summary>
        /// Recieves the saturation from a food source.
        /// </summary>
        /// <param name="saturation">The amount of saturation recieved.</param>
        /// <param name="foodCat">The cat of food... err Category of food.</param>
        /// <param name="saturationLossDelay">The delay before the loss of saturation</param>
        /// <param name="nutritionGainMultiplier"></param>
        public virtual void ReceiveSaturation(float saturation, EnumFoodCategory foodCat = EnumFoodCategory.Unknown, float saturationLossDelay = 10, float nutritionGainMultiplier = 1f)
        {
            if (!Alive) return;

            if (ShouldReceiveSaturation(saturation, foodCat, saturationLossDelay))
            {
                foreach (EntityBehavior behavior in SidedProperties.Behaviors)
                {
                    behavior.OnEntityReceiveSaturation(saturation, foodCat, saturationLossDelay, nutritionGainMultiplier);
                }
            }
        }

        /// <summary>
        /// Whether or not the target should recieve saturation.
        /// </summary>
        /// <param name="saturation">The amount of saturation recieved.</param>
        /// <param name="foodCat">The cat of food... err Category of food.</param>
        /// <param name="saturationLossDelay">The delay before the loss of saturation</param>
        /// <param name="nutritionGainMultiplier"></param>
        public virtual bool ShouldReceiveSaturation(float saturation, EnumFoodCategory foodCat = EnumFoodCategory.Unknown, float saturationLossDelay = 10, float nutritionGainMultiplier = 1f)
        {
            return true;
        }

        public enum EntityServerPacketId
        {
            Teleport = 1,
            Revive = 196,
            Emote = 197,
            Death = 198,
            Hurt = 199,
            PlayPlayerAnim = 200,
            PlayMusic = 201,
            StopMusic = 202,
            Talk = 203
        }
        public enum EntityClientPacketId
        {
            SitfloorEdge = 296
        }



        public override void OnGameTick(float dt)
        {
            if (MountedOn != null)
            {
                var nowSuggestedAnim = MountedOn.SuggestedAnimation;
                if (curMountedAnim?.Code != nowSuggestedAnim?.Code)
                {
                    if (curMountedAnim != null) AnimManager?.StopAnimation(curMountedAnim.Code);
                    if (nowSuggestedAnim != null) AnimManager?.StartAnimation(nowSuggestedAnim);
                    curMountedAnim = nowSuggestedAnim;
                }
            }
            else if (curMountedAnim != null)
            {
                AnimManager?.StopAnimation(curMountedAnim.Code);
                curMountedAnim = null;
            }


            if (World.Side == EnumAppSide.Client)
            {
                bool alive = Alive;

                if (alive)
                {
                    CurrentControls =
                        (servercontrols.TriesToMove || ((servercontrols.Jump || servercontrols.Sneak) && servercontrols.IsClimbing) ? EnumEntityActivity.Move : EnumEntityActivity.Idle) |
                        (Swimming && !servercontrols.FloorSitting ? EnumEntityActivity.Swim : 0) |
                        (servercontrols.FloorSitting ? EnumEntityActivity.FloorSitting : 0) |
                        (servercontrols.Sneak && !servercontrols.IsClimbing && !servercontrols.FloorSitting && !Swimming ? EnumEntityActivity.SneakMode : 0) |
                        (servercontrols.TriesToMove && servercontrols.Sprint && !Swimming && !servercontrols.Sneak ? EnumEntityActivity.SprintMode : 0) |
                        (servercontrols.IsFlying ? servercontrols.Gliding ? EnumEntityActivity.Glide : EnumEntityActivity.Fly : 0) |
                        (servercontrols.IsClimbing ? EnumEntityActivity.Climb : 0) |
                        (servercontrols.Jump && OnGround ? EnumEntityActivity.Jump : 0) |
                        (!OnGround && !Swimming && !FeetInLiquid && !servercontrols.IsClimbing && !servercontrols.IsFlying && SidedPos.Motion.Y < -0.05 ? EnumEntityActivity.Fall : 0) |
                        (MountedOn != null ? EnumEntityActivity.Mounted : 0)
                    ;
                }
                else
                {
                    CurrentControls = EnumEntityActivity.Dead;
                }

                CurrentControls = CurrentControls == 0 ? EnumEntityActivity.Idle : CurrentControls;

                if (MountedOn != null && MountedOn.SkipIdleAnimation)
                {
                    CurrentControls &= ~EnumEntityActivity.Idle;
                }

            HandleHandAnimations(dt);

                AnimationMetaData defaultAnim = null;
                bool anyAverageAnimActive = false;
                bool skipDefaultAnim = false;

                // Tyron, Sep 27
                // Change of plans. Lets try to run the idle animation only when no other Animation runs with BlendMode Average
                // Tyron, Oct 3
                // Apparently we cannot do that for first person animations? It borks hard. EntityPlayer.OnGameTick() now sets alwaysRunIdle while in fp mode

                AnimationMetaData[] animations = Properties.Client.Animations;
                for (int i = 0; animations != null && i < animations.Length; i++)
                {
                    AnimationMetaData anim = animations[i];
                    bool wasActive = AnimManager.IsAnimationActive(anim.Animation);
                    bool isDefaultAnim = anim?.TriggeredBy?.DefaultAnim == true;
                    bool nowActive = anim.Matches((int)CurrentControls) || (isDefaultAnim && CurrentControls == EnumEntityActivity.Idle);

                    anyAverageAnimActive |= !isDefaultAnim && wasActive && anim.BlendMode == EnumAnimationBlendMode.Average; //  // (nowActive && anim.TriggeredBy?.DefaultAnim != true) || (wasActive && !anim.WasStartedFromTrigger) || (MountedOn != null && MountedOn.SkipIdleAnimation);
                    skipDefaultAnim |= (nowActive || (wasActive && !anim.WasStartedFromTrigger)) && anim.SupressDefaultAnimation;

                    if (isDefaultAnim)
                    {
                        defaultAnim = anim;
                    }

                    if (onAnimControls(anim, wasActive, nowActive)) continue;

                    if (!wasActive && nowActive)
                    {
                        anim.WasStartedFromTrigger = true;
                        AnimManager.StartAnimation(anim);
                    }

                    if (!isDefaultAnim /* Default anim is handled below */ && wasActive && !nowActive && anim.WasStartedFromTrigger)
                    {
                        anim.WasStartedFromTrigger = false;
                        AnimManager.StopAnimation(anim.Animation);
                    }
                }


                if (defaultAnim != null && Alive && !skipDefaultAnim)
                {
                    if (anyAverageAnimActive || MountedOn != null)
                    {
                         if (!alwaysRunIdle && AnimManager.IsAnimationActive(defaultAnim.Animation)) AnimManager.StopAnimation(defaultAnim.Animation);
                    }
                    else
                    {
                        defaultAnim.WasStartedFromTrigger = true;
                        if (!AnimManager.IsAnimationActive(defaultAnim.Animation)) AnimManager.StartAnimation(defaultAnim);
                    }
                }

                if ((!Alive || skipDefaultAnim) && defaultAnim != null)
                {
                    AnimManager.StopAnimation(defaultAnim.Code);
                }

                bool isSelf = (Api as ICoreClientAPI).World.Player.Entity.EntityId == EntityId;
                if (insideBlock?.GetBlockMaterial(Api.World.BlockAccessor, insidePos) == EnumBlockMaterial.Snow && isSelf)
                {
                    SpawnSnowStepParticles();
                }
            }
            else   // Server side
            {
                HandleHandAnimations(dt);

                if (Properties.RotateModelOnClimb)
            {
                if (!OnGround && Alive && Controls.IsClimbing && ClimbingOnFace != null && ClimbingOnCollBox.Y2 > 0.2 /* cheap hax so that locusts don't climb on very flat collision boxes */)
                {
                    ServerPos.Pitch = ClimbingOnFace.HorizontalAngleIndex * GameMath.PIHALF;
                }
                else
                {
                    ServerPos.Pitch = 0;
                }
            }
            }

            World.FrameProfiler.Mark("entityAgent-ticking");
            base.OnGameTick(dt);
        }


        protected virtual void SpawnSnowStepParticles()
        {
            ICoreClientAPI capi = Api as ICoreClientAPI;
            bool isSelf = capi.World.Player.Entity.EntityId == EntityId;
            EntityPos herepos = (isSelf ? Pos : ServerPos);
            double hormot = Pos.Motion.X * Pos.Motion.X + Pos.Motion.Z * Pos.Motion.Z;
            float val = (float)Math.Sqrt(hormot);
            if (Api.World.Rand.NextDouble() < 10 * val)
            {
                var rand = capi.World.Rand;
                Vec3f velo = new Vec3f(1f - 2 * (float)rand.NextDouble() + GameMath.Clamp((float)Pos.Motion.X * 15, -5, 5), 0.5f + 3.5f * (float)rand.NextDouble(), 1f - 2 * (float)rand.NextDouble() + GameMath.Clamp((float)Pos.Motion.Z * 15, -5, 5));
                float radius = Math.Min(SelectionBox.XSize, SelectionBox.ZSize) * 0.9f;

                World.SpawnCubeParticles(herepos.AsBlockPos, herepos.XYZ.Add(0, 0, 0), radius, 2 + (int)(rand.NextDouble() * val * 5), 0.5f + (float)rand.NextDouble() * 0.5f, null, velo);
            }
        }

        protected virtual void SpawnFloatingSediment(IAsyncParticleManager manager)
        {
            ICoreClientAPI capi = (Api as ICoreClientAPI);
            bool isSelf = capi.World.Player.Entity.EntityId == EntityId;
            EntityPos herepos = (isSelf ? Pos : ServerPos);

            double width = SelectionBox.XSize * 0.75f;

            SplashParticleProps.BasePos.Set(herepos.X - width / 2, herepos.InternalY + 0, herepos.Z - width / 2);
            SplashParticleProps.AddPos.Set(width, 0.5, width);

            float mot = (float)herepos.Motion.Length();
            SplashParticleProps.AddVelocity.Set((float)herepos.Motion.X * 20f, 0, (float)herepos.Motion.Z * 20f);
            float f = Properties.Attributes?["extraSplashParticlesMul"].AsFloat(1) ?? 1;
            SplashParticleProps.QuantityMul = 0.15f * mot * 5 * 2 * f;

            World.SpawnParticles(SplashParticleProps);

            SpawnWaterMovementParticles((float)Math.Max(Swimming ? 0.04f : 0, mot * 5));

            FloatingSedimentParticles FloatingSedimentParticles = new FloatingSedimentParticles();

            FloatingSedimentParticles.SedimentPos.Set((int)herepos.X, (int)herepos.InternalY - 1, (int)herepos.Z);//  = , herepos.XYZ.Add(0, 0.25, 0), 0.25f, 2

            var block = FloatingSedimentParticles.SedimentBlock = World.BlockAccessor.GetBlock(FloatingSedimentParticles.SedimentPos);
            if (insideBlock != null && (block.BlockMaterial == EnumBlockMaterial.Gravel || block.BlockMaterial == EnumBlockMaterial.Soil || block.BlockMaterial == EnumBlockMaterial.Sand))
            {
                FloatingSedimentParticles.BasePos.Set(SplashParticleProps.BasePos);
                FloatingSedimentParticles.AddPos.Set(SplashParticleProps.AddPos);
                FloatingSedimentParticles.quantity = mot * 150;
                FloatingSedimentParticles.waterColor = insideBlock.GetColor(capi, FloatingSedimentParticles.SedimentPos);
                manager.Spawn(FloatingSedimentParticles);
            }
        }


        protected virtual bool onAnimControls(AnimationMetaData anim, bool wasActive, bool nowActive)
        {
            return false;
        }

        protected virtual void HandleHandAnimations(float dt)
        {

        }

        /// <summary>
        /// updated by GetWalkSpeedMultiplier()
        /// </summary>
        protected Block insideBlock;
        /// <summary>
        /// updated by GetWalkSpeedMultiplier()
        /// </summary>
        protected BlockPos insidePos = new BlockPos();

        /// <summary>
        /// Gets the walk speed multiplier.
        /// </summary>
        /// <param name="groundDragFactor">The amount of drag provided by the current ground. (Default: 0.3)</param>
        public virtual double GetWalkSpeedMultiplier(double groundDragFactor = 0.3)
        {
            int y1 = (int)(SidedPos.InternalY - 0.05f);
            int y2 = (int)(SidedPos.InternalY + 0.01f);

            Block belowBlock = World.BlockAccessor.GetBlockRaw((int)SidedPos.X, y1, (int)SidedPos.Z);

            insidePos.Set((int)SidedPos.X, y2, (int)SidedPos.Z);
            insideBlock = World.BlockAccessor.GetBlock(insidePos);

            double multiplier = (servercontrols.Sneak ? GlobalConstants.SneakSpeedMultiplier : 1.0) * (servercontrols.Sprint ? GlobalConstants.SprintSpeedMultiplier : 1.0);

            if (FeetInLiquid)
            {
                multiplier /= 2.5;
            }

            multiplier *= belowBlock.WalkSpeedMultiplier * (y1 == y2 ? 1 : insideBlock.WalkSpeedMultiplier);

            return multiplier;
        }



        /// <summary>
        /// Serializes the slots contents to be stored in the SaveGame
        /// </summary>
        /// <returns></returns>
        public override void ToBytes(BinaryWriter writer, bool forClient)
        {
            if (MountedOn != null)
            {
                TreeAttribute mountableTree = new TreeAttribute();
                MountedOn?.MountableToTreeAttributes(mountableTree);
                WatchedAttributes["mountedOn"] = mountableTree;
            } else
            {
                if (WatchedAttributes.HasAttribute("mountedOn"))
                {
                    WatchedAttributes.RemoveAttribute("mountedOn");
                }
            }

            base.ToBytes(writer, forClient);
            controls.ToBytes(writer);
        }



        /// <summary>
        /// Loads the entity from a stored byte array from the SaveGame
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="forClient"></param>
        public override void FromBytes(BinaryReader reader, bool forClient)
        {
            try
            {
                base.FromBytes(reader, forClient);
                controls.FromBytes(reader, LoadControlsFromServer);

            } catch (EndOfStreamException e)
            {
                throw new Exception("EndOfStreamException thrown while reading entity, you might be able to recover your savegame through repair mode", e);

            }
        }

        /// <summary>
        /// Relevant only for entities with heads, implemented in EntityAgent.  Other sub-classes of Entity (if not EntityAgent) should similarly override this if the headYaw/headPitch are relevant to them
        /// </summary>
        protected override void SetHeadPositionToWatchedAttributes()
        {
            // Storing in binary form sucks when it comes to compatibility, so lets use WatchedAttributes for these 2 new fields
            // We don't use SetFloat to avoid triggering OnModified listeners on the server. This can be called outside the mainthread and produce deadlocks.
            WatchedAttributes["headYaw"] = new FloatAttribute(ServerPos.HeadYaw);
            WatchedAttributes["headPitch"] = new FloatAttribute(ServerPos.HeadPitch);
        }

        /// <summary>
        /// Relevant only for entities with heads, implemented in EntityAgent.  Other sub-classes of Entity (if not EntityAgent) should similarly override this if the headYaw/headPitch are relevant to them
        /// </summary>
        protected override void GetHeadPositionFromWatchedAttributes()
        {
            // Storing in binary form sucks when it comes to compatibility, so lets use WatchedAttributes
            ServerPos.HeadYaw = WatchedAttributes.GetFloat("headYaw");
            ServerPos.HeadPitch = WatchedAttributes.GetFloat("headPitch");
        }


        /// <summary>
        /// Attempts to stop the hand  action.
        /// </summary>
        /// <param name="isCancel">Whether or not the action is cancelled or stopped.</param>
        /// <param name="cancelReason">The reason for stopping the action.</param>
        /// <returns>Whether the stop was cancelled or not.</returns>
        public virtual bool TryStopHandAction(bool isCancel, EnumItemUseCancelReason cancelReason = EnumItemUseCancelReason.ReleasedMouse)
        {
            if (controls.HandUse == EnumHandInteract.None || RightHandItemSlot?.Itemstack == null) return true;

            float secondsPassed = (World.ElapsedMilliseconds - controls.UsingBeginMS) / 1000f;

            if (isCancel)
            {
                controls.HandUse = RightHandItemSlot.Itemstack.Collectible.OnHeldUseCancel(secondsPassed, RightHandItemSlot, this, null, null, cancelReason);
            }
            else
            {
                controls.HandUse = EnumHandInteract.None;
                RightHandItemSlot.Itemstack.Collectible.OnHeldUseStop(secondsPassed, RightHandItemSlot, this, null, null, controls.HandUse);
            }

            return controls.HandUse == EnumHandInteract.None;
        }

        /// <summary>
        /// This walks the inventory for the entity agent.
        /// </summary>
        /// <param name="handler">the event to fire while walking the inventory.</param>
        public virtual void WalkInventory(OnInventorySlot handler)
        {

        }


        public override void UpdateDebugAttributes()
        {
            base.UpdateDebugAttributes();

            DebugAttributes.SetString("Herd Id", "" + HerdId);
        }



        public override bool TryGiveItemStack(ItemStack itemstack)
        {
            if (itemstack == null || itemstack.StackSize == 0) return false;

            var bhs = SidedProperties?.Behaviors;
            EnumHandling handling = EnumHandling.PassThrough;
            if (bhs != null)
            {
                foreach (var bh in bhs)
                {
                    bh.TryGiveItemStack(itemstack, ref handling);
                    if (handling == EnumHandling.PreventSubsequent) break;
                }
            }

            return handling != EnumHandling.PassThrough;
        }

        /// <summary>
        /// If true, then this entity will not retaliate if attacked by the specified eOther
        /// </summary>
        /// <param name="eOther"></param>
        public bool ToleratesDamageFrom(Entity eOther)
        {
            bool tolerate = false;

            foreach (var bh in SidedProperties?.Behaviors)
            {
                EnumHandling handling = EnumHandling.PassThrough;
                bool thisTolerate = bh.ToleratesDamageFrom(eOther, ref handling);
                if (handling != EnumHandling.PassThrough) {
                    tolerate = thisTolerate;
                }
                if (handling == EnumHandling.PreventSubsequent) return thisTolerate;
            }

            return tolerate;
        }

        public override string GetInfoText()
        {
            StringBuilder infotext = new StringBuilder();

            infotext.Append(base.GetInfoText());

            var capi = Api as ICoreClientAPI;
            if (capi != null && capi.Settings.Bool["extendedDebugInfo"])
            {
                infotext.AppendLine("<font color=\"#bbbbbb\">Herd id: " + HerdId + "</font>");
                if (DebugAttributes.HasAttribute("AI Tasks")) infotext.AppendLine($"<font color=\"#bbbbbb\">AI tasks: {DebugAttributes.GetString("AI Tasks")}</font>");
            }

            return infotext.ToString();
        }
    }
}
