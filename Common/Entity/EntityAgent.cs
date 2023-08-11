using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

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
        /// <returns>null for returning back to default behavior (read shape from attributes)</returns>
        Shape GetShape(ItemStack stack, EntityAgent forEntity);
    }


    /// <summary>
    /// A goal-directed entity which observes and acts upon an environment
    /// </summary>
    public class EntityAgent : Entity
    {
        /// <summary>
        /// The yaw of the agents body
        /// </summary>
        public virtual float BodyYaw { get; set; }

        /// <summary>
        /// The yaw of the agents body on the client, retrieved from the server (BehaviorInterpolatePosition lerps this value and sets BodyYaw)
        /// </summary>
        public virtual float BodyYawServer { get; set; }

        /// <summary>
        /// True if all clients have to be informed about this entities death. Set to false once all clients have been notified
        /// </summary>
        public bool DeadNotify;

        /// <summary>
        /// Unique identifier for a herd
        /// </summary>
        public long HerdId
        {
            get { return WatchedAttributes.GetLong("herdId"); }
            set { WatchedAttributes.SetLong("herdId", value); }
        }

        protected EntityControls controls;
        protected EntityControls servercontrols;

        


        public IMountable MountedOn { get; protected set; }
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
        /// The inventory of the entity agent.
        /// </summary>
        public virtual IInventory GearInventory
        {
            get { return null; }
        }

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
            // Temporary code for VS 1.15 dev team to remove previously created "land" salmon which don't have the correct entity
            if (properties.Habitat == EnumHabitat.Underwater && !(this.GetType().Name == "EntityFish")) this.Alive = false;

            base.Initialize(properties, api, InChunkIndex3d);

            if (World.Side == EnumAppSide.Server)
            {
                servercontrols = controls;
            }

            WatchedAttributes.RegisterModifiedListener("mountedOn", updateMountedState);

            if (WatchedAttributes["mountedOn"] != null)
            {
                MountedOn = World.ClassRegistry.CreateMountable(WatchedAttributes["mountedOn"] as TreeAttribute);
                if (MountedOn != null)
                {
                    TryMount(MountedOn);
                }
            }

            
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
        /// Attempts to mount the player on a target.
        /// </summary>
        /// <param name="onmount">The mount to mount</param>
        /// <returns>Whether it was mounted or not.</returns>
        public virtual bool TryMount(IMountable onmount)
        {
            // load current controls when mounting onto the mountable
            onmount.Controls.FromInt(Controls.ToInt());
            
            if (MountedOn != onmount)
            {
                if (!TryUnmount()) return false;
            }

            TreeAttribute mountableTree = new TreeAttribute();
            onmount?.MountableToTreeAttributes(mountableTree);
            WatchedAttributes["mountedOn"] = mountableTree;

            var mountable = World.ClassRegistry.CreateMountable(WatchedAttributes["mountedOn"] as TreeAttribute);
            if (mountable != null) WatchedAttributes.MarkPathDirty("mountedOn");
            else doMount(onmount); // ClassRegistry.CreateMountable() might not have the onmount as a loaded entity yet
            return true;
        }

        protected virtual void updateMountedState()
        {
            if (WatchedAttributes.HasAttribute("mountedOn"))
            {
                var mountable = World.ClassRegistry.CreateMountable(WatchedAttributes["mountedOn"] as TreeAttribute);
                doMount(mountable);
            }
            else
            {
                TryUnmount();
            }

        }

        protected virtual void doMount(IMountable mountable)
        {
            this.MountedOn = mountable;
            controls.StopAllMovement();

            if (mountable == null)
            {
                WatchedAttributes.RemoveAttribute("mountedOn");
                return;
            }

            if (MountedOn?.SuggestedAnimation != null)
            {
                string anim = MountedOn.SuggestedAnimation.ToLowerInvariant();
                AnimManager?.StartAnimation(anim);
            }

            mountable.DidMount(this);
        }

        /// <summary>
        /// Attempts to un-mount the player.
        /// </summary>
        /// <returns>Whether or not unmounting was successful</returns>
        public bool TryUnmount()
        {
            if (MountedOn?.SuggestedAnimation != null)
            {
                string anim = MountedOn.SuggestedAnimation.ToLowerInvariant();
                AnimManager?.StopAnimation(anim);
            }

            MountedOn?.DidUnmount(this);
            this.MountedOn = null;

            if (WatchedAttributes.HasAttribute("mountedOn"))
            {
                WatchedAttributes.RemoveAttribute("mountedOn");
            }

            return true;
        }

        public override void Die(EnumDespawnReason reason = EnumDespawnReason.Death, DamageSource damageSourceForDeath = null)
        {
            if (Alive && reason == EnumDespawnReason.Death)
            {
                PlayEntitySound("death");
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
        
        /// <summary>
        /// Called when the path finder does not find a path to given target
        /// </summary>
        /// <param name="target"></param>
        public void OnNoPath(Vec3d target)
        {
            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                behavior.OnNoPath(target);
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

                damage *= byEntity.Stats.GetBlended("meleeWeaponsDamage");

                if (Attributes.GetBool("isMechanical", false))
                {
                    damage *= byEntity.Stats.GetBlended("mechanicalsDamage");
                }

                IPlayer byPlayer = null;

                if (byEntity is EntityPlayer && !IsActivityRunning("invulnerable"))
                {
                    byPlayer = (byEntity as EntityPlayer).Player;

                    World.PlaySoundAt(new AssetLocation("sounds/player/slap"), ServerPos.X, ServerPos.Y, ServerPos.Z, byPlayer);
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

                if (ReceiveDamage(dmgSource, damage))
                {
                    byEntity.DidAttack(dmgSource, this);
                }
 
            }
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
        public virtual bool ShouldReceiveSaturation(float saturation, EnumFoodCategory foodCat = EnumFoodCategory.Unknown, float saturationLossDelay = 10, float nutritionGainMultiplier = 1f)
        {
            return true;
        }


        public override void OnGameTick(float dt)
        {
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
                        (servercontrols.Sprint && !Swimming && !servercontrols.Sneak ? EnumEntityActivity.SprintMode : 0) |
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
            }

            HandleHandAnimations(dt);

            if (World.Side == EnumAppSide.Client)
            {
                AnimationMetaData defaultAnim = null;
                bool anyAverageAnimActive = false;
                bool skipDefaultAnim = false;

                AnimationMetaData[] animations = Properties.Client.Animations;
                for (int i = 0; animations != null && i < animations.Length; i++)
                {
                    AnimationMetaData anim = animations[i];
                    bool wasActive = AnimManager.ActiveAnimationsByAnimCode.ContainsKey(anim.Animation);
                    bool nowActive = anim.Matches((int)CurrentControls);
                    bool isDefaultAnim = anim?.TriggeredBy?.DefaultAnim == true;

                    anyAverageAnimActive |= nowActive || (wasActive && !anim.WasStartedFromTrigger);
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

                    if (wasActive && !nowActive && (anim.WasStartedFromTrigger || isDefaultAnim))
                    {
                        anim.WasStartedFromTrigger = false;
                        AnimManager.StopAnimation(anim.Animation);
                    }
                }

                if (!anyAverageAnimActive && defaultAnim != null && Alive && !skipDefaultAnim)
                {
                    defaultAnim.WasStartedFromTrigger = true;
                    AnimManager.StartAnimation(defaultAnim);
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

            if (Properties.RotateModelOnClimb && World.Side == EnumAppSide.Server)
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

            SplashParticleProps.BasePos.Set(herepos.X - width / 2, herepos.Y + 0, herepos.Z - width / 2);
            SplashParticleProps.AddPos.Set(width, 0.5, width);

            float mot = (float)herepos.Motion.Length();
            SplashParticleProps.AddVelocity.Set((float)herepos.Motion.X * 20f, 0, (float)herepos.Motion.Z * 20f);
            float f = Properties.Attributes?["extraSplashParticlesMul"].AsFloat(1) ?? 1;
            SplashParticleProps.QuantityMul = 0.15f * mot * 5 * 2 * f;

            World.SpawnParticles(SplashParticleProps);

            SpawnWaterMovementParticles((float)Math.Max(Swimming ? 0.04f : 0, mot * 5));

            FloatingSedimentParticles FloatingSedimentParticles = new FloatingSedimentParticles();

            FloatingSedimentParticles.SedimentPos.Set((int)herepos.X, (int)herepos.Y - 1, (int)herepos.Z);//  = , herepos.XYZ.Add(0, 0.25, 0), 0.25f, 2

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

        public virtual void StopHandAnims()
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
            int y1 = (int)(SidedPos.Y - 0.05f);
            int y2 = (int)(SidedPos.Y + 0.01f);

            Block belowBlock = World.BlockAccessor.GetBlock((int)SidedPos.X, y1, (int)SidedPos.Z);

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
                Exception ex = new Exception("EndOfStreamException thrown while reading entity, you might be able to recover your savegame through repair mode", e);
                throw ex;
            }

            if (!WatchedAttributes.HasAttribute("mountedOn") && MountedOn != null)
            {
                TryUnmount();
            }
        }

        /// <summary>
        /// Relevant only for entities with heads, implemented in EntityAgent.  Other sub-classes of Entity (if not EntityAgent) should similarly override this if the headYaw/headPitch are relevant to them
        /// </summary>
        protected override void SetHeadPositionToWatchedAttributes()
        {
            // Storing in binary form sucks when it comes to compatibility, so lets use WatchedAttributes for these 2 new fields
            lock (WatchedAttributes.attributesLock)
            {
                // We don't use SetFloat to avoid triggering OnModified listeners on the server. This can be called outside the mainthread and produce deadlocks.
                WatchedAttributes["headYaw"] = new FloatAttribute(ServerPos.HeadYaw);
                WatchedAttributes["headPitch"] = new FloatAttribute(ServerPos.HeadPitch);
            }
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


        public override void OnTesselation(ref Shape entityShape, string shapePathForLogging)
        {
            addGearToShape(ref entityShape, shapePathForLogging);
        }

        public bool hideClothing;


        protected Shape addGearToShape(ref Shape entityShape, string shapePathForLogging)
        {
            IInventory inv = GearInventory;

            if (inv == null || (!(this is EntityPlayer) && inv.Empty)) return entityShape;

            // Make a copy so we don't mess up the original
            Shape newShape = entityShape.Clone();
            newShape.ResolveAndLoadJoints("head");
            entityShape = newShape;

            foreach (var slot in inv)
            {
                if (hideClothing && !slot.Empty)
                {
                    continue;
                }

                entityShape = addGearToShape(slot, entityShape, shapePathForLogging);
            }

            return entityShape;
        }


        public Dictionary<string, CompositeTexture> extraTexturesByTextureName
        {
            get
            {
                return ObjectCacheUtil.GetOrCreate(Api, "entityShapeExtraTexturesByName", () => new Dictionary<string, CompositeTexture>());
            }
        }

        public Dictionary<AssetLocation, BakedCompositeTexture> extraTextureByLocation
        {
            get
            {
                return ObjectCacheUtil.GetOrCreate(Api, "entityShapeExtraTexturesByLoc", () => new Dictionary<AssetLocation, BakedCompositeTexture>());
            }
        }

        protected Shape addGearToShape(ItemSlot slot, Shape entityShape, string shapePathForLogging)
        {
            if (slot.Empty) return entityShape;
            ItemStack stack = slot.Itemstack;
            JsonObject attrObj = stack.Collectible.Attributes;

            float damageEffect = 0;
            if (stack.ItemAttributes?["visibleDamageEffect"].AsBool() == true)
            {
                damageEffect = Math.Max(0, 1 - (float)stack.Collectible.GetRemainingDurability(stack) / stack.Collectible.GetMaxDurability(stack) * 1.1f);
            }

            string[] disableElements = attrObj?["disableElements"]?.AsArray<string>(null);
            if (disableElements != null)
            {
                foreach (var val in disableElements)
                {
                    entityShape.RemoveElementByName(val);
                }
            }

            if (attrObj?["wearableAttachment"].Exists != true) return entityShape;

            Shape armorShape=null;
            AssetLocation shapePath=null;
            CompositeShape compArmorShape = null;
            if (stack.Collectible is IWearableShapeSupplier iwss)
            {
                armorShape = iwss.GetShape(stack, this);
            }

            if (armorShape == null) {
                compArmorShape = !attrObj["attachShape"].Exists ? (stack.Class == EnumItemClass.Item ? stack.Item.Shape : stack.Block.Shape) : attrObj["attachShape"].AsObject<CompositeShape>(null, stack.Collectible.Code.Domain);
                shapePath = shapePath = compArmorShape.Base.CopyWithPath("shapes/" + compArmorShape.Base.Path + ".json");
                armorShape = Shape.TryGet(Api, shapePath);
                if (armorShape == null)
                {
                    Api.World.Logger.Warning("Entity armor shape {0} defined in {1} {2} not found or errored, was supposed to be at {3}. Armor piece will be invisible.", compArmorShape.Base, stack.Class, stack.Collectible.Code, shapePath);
                    return null;
                }
            }


            bool added = false;
            foreach (var val in armorShape.Elements)
            {
                ShapeElement elem;

                if (val.StepParentName != null)
                {
                    elem = entityShape.GetElementByName(val.StepParentName, StringComparison.InvariantCultureIgnoreCase);
                    if (elem == null)
                    {
                        Api.World.Logger.Warning("Entity gear shape {0} defined in {1} {2} requires step parent element with name {3}, but no such element was found in shape {4}. Will not be visible.", compArmorShape.Base, slot.Itemstack.Class, slot.Itemstack.Collectible.Code, val.StepParentName, shapePathForLogging);
                        continue;
                    }
                }
                else
                {
                    Api.World.Logger.Warning("Entity gear shape element {0} in shape {1} defined in {2} {3} did not define a step parent element. Will not be visible.", val.Name, compArmorShape.Base, slot.Itemstack.Class, slot.Itemstack.Collectible.Code);
                    continue;
                }

                if (elem.Children == null)
                {
                    elem.Children = new ShapeElement[] { val };
                }
                else
                {
                    elem.Children = elem.Children.Append(val);
                }

                val.SetJointIdRecursive(elem.JointId);
                val.WalkRecursive((el) =>
                {
                	el.DamageEffect = damageEffect;
                	
                    foreach (var face in el.FacesResolved)
                    {
                        if (face != null) face.Texture = stack.Collectible.Code + "-" + face.Texture;
                    }
                });

                added = true;
            }

            if (added && armorShape.Textures != null)
            {
                Dictionary<string, AssetLocation> newdict = new Dictionary<string, AssetLocation>();
                foreach (var val in armorShape.Textures)
                {
                    newdict[stack.Collectible.Code + "-" + val.Key] = val.Value;
                }

                // Item overrides
                var collDict = stack.Class == EnumItemClass.Block ? stack.Block.Textures : stack.Item.Textures;
                foreach (var val in collDict)
                {
                    newdict[stack.Collectible.Code + "-" + val.Key] = val.Value.Base;
                }

                armorShape.Textures = newdict;

                foreach (var val in armorShape.Textures)
                {
                    CompositeTexture ctex = new CompositeTexture() { Base = val.Value };

                    entityShape.TextureSizes[val.Key] = new int[] { armorShape.TextureWidth, armorShape.TextureHeight };

                    AssetLocation armorTexLoc = val.Value;

                    // Weird backreference to the shaperenderer. Should be refactored.
                    var texturesByLoc = extraTextureByLocation;
                    var texturesByName = extraTexturesByTextureName;

                    BakedCompositeTexture bakedCtex;

                    ICoreClientAPI capi = Api as ICoreClientAPI;

                    if (!texturesByLoc.TryGetValue(armorTexLoc, out bakedCtex))
                    {
                        int textureSubId = 0;
                        TextureAtlasPosition texpos;

                        capi.EntityTextureAtlas.GetOrInsertTexture(armorTexLoc, out textureSubId, out texpos, () =>
                        {
                            IAsset texAsset = Api.Assets.TryGet(armorTexLoc.Clone().WithPathPrefixOnce("textures/").WithPathAppendixOnce(".png"));
                            if (texAsset != null)
                            {
                                return texAsset.ToBitmap(capi);
                            }

                            capi.World.Logger.Warning("Entity armor shape {0} defined texture {1}, no such texture found.", shapePath, armorTexLoc);
                            return null;
                        });

                        ctex.Baked = new BakedCompositeTexture() { BakedName = armorTexLoc, TextureSubId = textureSubId };

                        texturesByName[val.Key] = ctex;
                        texturesByLoc[armorTexLoc] = ctex.Baked;
                    }
                    else
                    {
                        ctex.Baked = bakedCtex;
                        texturesByName[val.Key] = ctex;
                    }
                }

                foreach (var val in armorShape.TextureSizes)
                {
                    entityShape.TextureSizes[val.Key] = val.Value;
                }
            }

            return entityShape;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="code">Any unique Identifier</param>
        /// <param name="cshape"></param>
        /// <param name="entityShape"></param>
        /// <param name="shapePathForLogging"></param>
        /// <param name="disableElements"></param>
        /// <returns></returns>
        protected Shape addGearToShape(string code, CompositeShape cshape, Shape entityShape, string shapePathForLogging, string[] disableElements = null, Dictionary<string, AssetLocation> textureOverrides = null)
        {
            AssetLocation shapePath = cshape.Base.CopyWithPath("shapes/" + cshape.Base.Path + ".json");

            if (disableElements != null)
            {
                foreach (var val in disableElements)
                {
                    entityShape.RemoveElementByName(val);
                }
            }

            Shape armorShape = Shape.TryGet(Api, shapePath);
            if (armorShape == null)
            {
                Api.World.Logger.Warning("Compositshape {0} (code: {2}) defined but not found or errored, was supposed to be at {1}. Part will be invisible.", cshape.Base, shapePath, code);
                return null;
            }

            bool added = applyStepParents(null, armorShape.Elements, entityShape, code, cshape, shapePathForLogging);


            if (added && armorShape.Textures != null)
            {
                Dictionary<string, AssetLocation> newdict = new Dictionary<string, AssetLocation>();
                foreach (var val in armorShape.Textures)
                {
                    newdict[code + "-" + val.Key] = val.Value;
                }

                // Texture overrides
                if (textureOverrides != null)
                {
                    foreach (var val in textureOverrides)
                    {
                        newdict[code + "-" + val.Key] = val.Value;
                    }
                }

                armorShape.Textures = newdict;

                foreach (var val in armorShape.Textures)
                {
                    CompositeTexture ctex = new CompositeTexture() { Base = val.Value };

                    entityShape.TextureSizes[val.Key] = new int[] { armorShape.TextureWidth, armorShape.TextureHeight };

                    AssetLocation armorTexLoc = val.Value;

                    // Weird backreference to the shaperenderer. Should be refactored.
                    var texturesByLoc = extraTextureByLocation;
                    var texturesByName = extraTexturesByTextureName;

                    BakedCompositeTexture bakedCtex;

                    ICoreClientAPI capi = Api as ICoreClientAPI;

                    if (!texturesByLoc.TryGetValue(armorTexLoc, out bakedCtex))
                    {
                        int textureSubId = 0;
                        TextureAtlasPosition texpos;

                        IAsset texAsset = Api.Assets.TryGet(val.Value.Clone().WithPathPrefixOnce("textures/").WithPathAppendixOnce(".png"));
                        if (texAsset != null)
                        {
                            BitmapRef bmp = texAsset.ToBitmap(capi);
                            capi.EntityTextureAtlas.InsertTexture(bmp, out textureSubId, out texpos);
                        }
                        else
                        {
                            capi.World.Logger.Warning("Entity armor shape {0} defined texture {1}, not no such texture found.", shapePath, val.Value);
                        }

                        ctex.Baked = new BakedCompositeTexture() { BakedName = val.Value, TextureSubId = textureSubId };

                        texturesByName[val.Key] = ctex;
                        texturesByLoc[armorTexLoc] = ctex.Baked;
                    }
                    else
                    {
                        ctex.Baked = bakedCtex;
                        texturesByName[val.Key] = ctex;
                    }
                }

                foreach (var val in armorShape.TextureSizes)
                {
                    entityShape.TextureSizes[val.Key] = val.Value;
                }
            }

            return entityShape;
        }

        private bool applyStepParents(ShapeElement parentElem, ShapeElement[] elements, Shape toShape, string code, CompositeShape cshape, string shapePathForLogging)
        {
            bool added = false;

            foreach (var cElem in elements)
            {
                ShapeElement refelem;

                if (cElem.Children != null)
                {
                    added |= applyStepParents(cElem, cElem.Children, toShape, code, cshape, shapePathForLogging);
                }

                if (cElem.StepParentName != null)
                {
                    refelem = toShape.GetElementByName(cElem.StepParentName, StringComparison.InvariantCultureIgnoreCase);
                    if (refelem == null)
                    {
                        Api.World.Logger.Warning("Shape {0} requires step parent element with name {1}, but no such element was found in shape {2}. Will not be visible.", cshape.Base, cElem.StepParentName, shapePathForLogging);
                        continue;
                    }
                }
                else
                {
                    if (parentElem == null)
                    {
                        Api.World.Logger.Warning("Entity armor shape element {0} in shape {1} did not define a step parent element. Will not be visible.", cElem.Name, cshape.Base);
                    }
                    continue;
                }

                if (parentElem != null)
                {
                    parentElem.Children = parentElem.Children.Remove(cElem);
                }

                if (refelem.Children == null)
                {
                    refelem.Children = new ShapeElement[] { cElem };
                }
                else
                {
                    refelem.Children = refelem.Children.Append(cElem);
                }

                cElem.SetJointIdRecursive(refelem.JointId);

                cElem.WalkRecursive((el) =>
                {
                    foreach (var face in el.FacesResolved)
                    {
                        if (face != null) face.Texture = code + "-" + face.Texture;
                    }
                });

                added = true;
            }

            return added;
        }

        public override bool TryGiveItemStack(ItemStack itemstack)
        {
            if (itemstack == null || itemstack.StackSize == 0) return false;

            ItemSlot dummySlot = new DummySlot(null);
            dummySlot.Itemstack = itemstack.Clone();

            ItemStackMoveOperation op = new ItemStackMoveOperation(World, EnumMouseButton.Left, 0, EnumMergePriority.AutoMerge, itemstack.StackSize);

            if (GearInventory != null)
            {
                WeightedSlot wslot = GearInventory.GetBestSuitedSlot(dummySlot, new List<ItemSlot>());
                if (wslot.weight > 0)
                {
                    dummySlot.TryPutInto(wslot.slot, ref op);
                    itemstack.StackSize -= op.MovedQuantity;
                    WatchedAttributes.MarkAllDirty();
                    return op.MovedQuantity > 0;
                }
            }

            if (LeftHandItemSlot?.Inventory != null)
            {
                WeightedSlot wslot = LeftHandItemSlot.Inventory.GetBestSuitedSlot(dummySlot, new List<ItemSlot>());
                if (wslot.weight > 0)
                {
                    dummySlot.TryPutInto(wslot.slot, ref op);
                    itemstack.StackSize -= op.MovedQuantity;
                    WatchedAttributes.MarkAllDirty();
                    return op.MovedQuantity > 0;
                }
            }

            return false;
        }


        public override void OnLoadCollectibleMappings(IWorldAccessor worldForResolve, Dictionary<int, AssetLocation> oldBlockIdMapping, Dictionary<int, AssetLocation> oldItemIdMapping, int schematicSeed)
        {
            base.OnLoadCollectibleMappings(worldForResolve, oldBlockIdMapping, oldItemIdMapping, schematicSeed);

            if (GearInventory != null)
            {
                foreach (var slot in GearInventory)
                {
                    if (slot.Empty) continue;
                    if (slot.Itemstack.FixMapping(oldBlockIdMapping, oldItemIdMapping, worldForResolve) == false)
                    {
                        slot.Itemstack = null;
                    }
                }
            }
        }


        public override void OnStoreCollectibleMappings(Dictionary<int, AssetLocation> blockIdMapping, Dictionary<int, AssetLocation> itemIdMapping)
        {
            base.OnStoreCollectibleMappings(blockIdMapping, itemIdMapping);

            if (GearInventory != null)
            {
                foreach (var slot in GearInventory)
                {
                    if (slot.Empty) continue;
                    var stack = slot.Itemstack;

                    if (stack.Class == EnumItemClass.Item)
                    {
                        itemIdMapping[stack.Id] = stack.Item.Code;
                    }
                    else
                    {
                        blockIdMapping[stack.Id] = stack.Block.Code;
                    }
                }
                
            }
        }

    }
}
