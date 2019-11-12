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


namespace Vintagestory.API.Common
{
    public enum EnumHand
    {
        Left,
        Right
    }


    /// <summary>
    /// An autonomous, goal-directed entity which observes and acts upon an environment
    /// </summary>
    public class EntityAgent : Entity
    {
        /// <summary>
        /// The yaw of the agents head
        /// </summary>
        public float HeadYaw;
        /// <summary>
        /// The pitch of the agents head
        /// </summary>
        public float HeadPitch;
        /// <summary>
        /// The yaw of the agents body
        /// </summary>
        public float BodyYaw;


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
        public virtual ItemSlot LeftHandItemSlot
        {
            get { return null; }
        }

        /// <summary>
        /// Item in the right hand slot of the entity agent.
        /// </summary>
        public virtual ItemSlot RightHandItemSlot
        {
            get { return null; }
        }

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

        //protected AnimationMetaData[] Animations;


        public override void Initialize(EntityProperties properties, ICoreAPI api, long InChunkIndex3d)
        {
            base.Initialize(properties, api, InChunkIndex3d);

            // why the eff are these cloned?! properties are already cloned for each individual entity
            /*if (properties.Client.Animations != null)
            {
                Animations = new AnimationMetaData[properties.Client.Animations.Length];
                for(int i = 0; i < Animations.Length; i++)
                {
                    Animations[i] = properties.Client.Animations[i].Clone();
                    Animations[i].Init();
                }
            }*/

            if (World.Side == EnumAppSide.Server)
            {
                servercontrols = controls;
            }

            if (WatchedAttributes["mountedOn"] != null)
            {
                MountedOn = World.ClassRegistry.CreateMountable(WatchedAttributes["mountedOn"] as TreeAttribute);
                if (MountedOn != null) MountedOn.DidMount(this);
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
            int blockId = GetEyesBlockId();
            return World.Blocks[blockId].MatterState == EnumMatterState.Liquid;
        }

        /// <summary>
        /// Gets the ID of the block the eyes are submerged in.
        /// </summary>
        /// <returns></returns>
        public int GetEyesBlockId()
        {
            BlockPos pos = LocalPos.AsBlockPos.Add(0, (float)Properties.EyeHeight, 0);
            return World.BlockAccessor.GetBlockId(pos.X, pos.Y, pos.Z);
        }

        /// <summary>
        /// Attempts to mount the player on a target.
        /// </summary>
        /// <param name="onmount">The mount to mount</param>
        /// <returns>Whether it was mounted or not.</returns>
        public bool TryMount(IMountable onmount)
        {
            this.MountedOn = onmount;
            controls.StopAllMovement();

            if (MountedOn?.SuggestedAnimation != null)
            {
                string anim = MountedOn.SuggestedAnimation.ToLowerInvariant();
                AnimManager?.StartAnimation(anim);
            }

            onmount.DidMount(this);

            return true;
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
            WatchedAttributes.RemoveAttribute("mountedOn");

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
                int damagetier = slot.Itemstack == null ? 0 : slot.Itemstack.Collectible.MiningTier;

                IPlayer byPlayer = null;

                if (byEntity is EntityPlayer && !IsActivityRunning("invulnerable"))
                {
                    byPlayer = (byEntity as EntityPlayer).Player;

                    World.PlaySoundAt(new AssetLocation("sounds/player/slap"), ServerPos.X, ServerPos.Y, ServerPos.Z, byPlayer);
                    slot?.Itemstack?.Collectible.OnAttackingWith(byEntity.World, byEntity, this, slot);
                }

                if (Api.Side == EnumAppSide.Client && damage > 1 && !IsActivityRunning("invulnerable") && Properties.Attributes?["spawnDamageParticles"].AsBool() == true)
                {
                    Vec3d pos = LocalPos.XYZ + hitPosition;
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
                    Source = EnumDamageSource.Entity,
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

            if (damageSource.Type != EnumDamageType.Heal)
            {
                PlayEntitySound("hurt", (damageSource.SourceEntity as EntityPlayer)?.Player);
            }

            /*if (damageSource.type == EnumDamageType.BluntAttack && damageSource.sourceEntity != null)
            {
                ServerPos.Motion.Add(
                    GameMath.Sin(damageSource.sourceEntity.ServerPos.Yaw + GameMath.PIHALF) / 10,
                    0.1,
                    GameMath.Cos(damageSource.sourceEntity.ServerPos.Yaw + GameMath.PIHALF) / 10
                );
                if (this is EntityPlayer)
                {
                    ((IServerPlayer)World.PlayerByUid(((EntityPlayer)this).PlayerUID)).SendPositionToClient();
                }
            }*/

            return true;
        }

        public override bool ReceiveDamage(DamageSource damageSource, float damage)
        {
            bool ret = base.ReceiveDamage(damageSource, damage);

            // What is this for? It causes players to glitch into walls when attacked and standing against a wall
            // (according to the logs apparently something related to explosion damage fling?)
            /*if (ret && this is EntityPlayer && damageSource.GetSourcePosition() != null && World.Side == EnumAppSide.Server)
            {
                ((IServerPlayer)World.PlayerByUid(((EntityPlayer)this).PlayerUID)).SendPositionToClient();
            }*/

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
                        (servercontrols.Sneak && !servercontrols.IsClimbing && !servercontrols.FloorSitting ? EnumEntityActivity.SneakMode : 0) |
                        (servercontrols.Sprint && !servercontrols.Sneak ? EnumEntityActivity.SprintMode : 0) |
                        (servercontrols.IsFlying ? EnumEntityActivity.Fly : 0) |
                        (servercontrols.IsClimbing ? EnumEntityActivity.Climb : 0) |
                        (servercontrols.Jump && OnGround ? EnumEntityActivity.Jump : 0) |
                        (!OnGround && !Swimming && !FeetInLiquid && !servercontrols.IsClimbing && !servercontrols.IsFlying && LocalPos.Motion.Y < -0.05 ? EnumEntityActivity.Fall : 0)
                    ;
                }
                else
                {
                    CurrentControls = EnumEntityActivity.Dead;
                ;
                }

                
                CurrentControls = CurrentControls == 0 ? EnumEntityActivity.Idle : CurrentControls;
            }

            if (this is EntityPlayer) HandleHandAnimations();

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
            }

            if (Properties.RotateModelOnClimb && World.Side == EnumAppSide.Server)
            {
                if (Alive && Controls.IsClimbing && ClimbingOnFace != null && ClimbingOnCollBox.Y2 > 0.2 /* cheap hax so that locusts don't climb on very flat collision boxes */)
                {
                    ServerPos.Pitch = ClimbingOnFace.HorizontalAngleIndex * GameMath.PIHALF;
                }
                else
                {
                    ServerPos.Pitch = 0;
                }
            }
            
            
            base.OnGameTick(dt);
        }


        protected string lastRunningHeldUseAnimation;
        protected string lastRunningRightHeldIdleAnimation;
        protected string lastRunningLeftHeldIdleAnimation;
        protected string lastRunningHeldHitAnimation;

        private void HandleHandAnimations()
        {
            ItemStack rightstack = RightHandItemSlot?.Itemstack;

            bool nowUseStack = servercontrols.RightMouseDown && !servercontrols.LeftMouseDown;
            bool wasUseStack = lastRunningHeldUseAnimation != null && AnimManager.ActiveAnimationsByAnimCode.ContainsKey(lastRunningHeldUseAnimation);

            bool nowHitStack = servercontrols.LeftMouseDown;
            bool wasHitStack = lastRunningHeldHitAnimation != null && AnimManager.ActiveAnimationsByAnimCode.ContainsKey(lastRunningHeldHitAnimation);

            bool nowRightIdleStack = !servercontrols.LeftMouseDown && !servercontrols.RightMouseDown;
            bool wasRightIdleStack = lastRunningRightHeldIdleAnimation != null && AnimManager.ActiveAnimationsByAnimCode.ContainsKey(lastRunningRightHeldIdleAnimation);

            bool nowLeftIdleStack = true;
            bool wasLeftIdleStack = lastRunningLeftHeldIdleAnimation != null && AnimManager.ActiveAnimationsByAnimCode.ContainsKey(lastRunningLeftHeldIdleAnimation);


            string nowHeldRightUseAnim = rightstack?.Collectible.GetHeldTpUseAnimation(RightHandItemSlot, this);
            string nowHeldRightHitAnim = rightstack?.Collectible.GetHeldTpHitAnimation(RightHandItemSlot, this);
            string nowHeldRightIdleAnim = rightstack?.Collectible.GetHeldTpIdleAnimation(RightHandItemSlot, this, EnumHand.Right);
            string nowHeldLeftIdleAnim = LeftHandItemSlot?.Itemstack?.Collectible.GetHeldTpIdleAnimation(LeftHandItemSlot, this, EnumHand.Left);

            if (rightstack == null) nowHeldRightHitAnim = "breakhand";

            if (nowUseStack != wasUseStack || (lastRunningHeldUseAnimation != null && nowHeldRightUseAnim != lastRunningHeldUseAnimation))
            {
                StopHandAnims();
                if (nowUseStack)
                {
                    AnimManager.StartAnimation(lastRunningHeldUseAnimation = nowHeldRightUseAnim);
                }
            }

            if (nowHitStack != wasHitStack || (lastRunningHeldHitAnimation != null && nowHeldRightHitAnim != lastRunningHeldHitAnimation))
            {
                StopHandAnims();
                if (nowHitStack)
                {
                    AnimManager.StartAnimation(lastRunningHeldHitAnimation = nowHeldRightHitAnim);
                }
            }

            if (nowRightIdleStack != wasRightIdleStack || (lastRunningRightHeldIdleAnimation != null && nowHeldRightIdleAnim != lastRunningRightHeldIdleAnimation))
            {
                StopHandAnims();
                if (nowRightIdleStack)
                {
                    AnimManager.StartAnimation(lastRunningRightHeldIdleAnimation = nowHeldRightIdleAnim);
                }
            }

            if (nowLeftIdleStack != wasLeftIdleStack || (lastRunningLeftHeldIdleAnimation != null && nowHeldLeftIdleAnim != lastRunningLeftHeldIdleAnimation))
            {
                AnimManager.StopAnimation(lastRunningLeftHeldIdleAnimation);
                lastRunningLeftHeldIdleAnimation = null;

                if (nowLeftIdleStack)
                {
                    AnimManager.StartAnimation(lastRunningLeftHeldIdleAnimation = nowHeldLeftIdleAnim);
                }
            }
        }

        public virtual void StopHandAnims()
        {
            AnimManager.StopAnimation(lastRunningHeldUseAnimation);
            lastRunningHeldUseAnimation = null;

            AnimManager.StopAnimation(lastRunningHeldHitAnimation);
            lastRunningHeldHitAnimation = null;

            AnimManager.StopAnimation(lastRunningRightHeldIdleAnimation);
            lastRunningRightHeldIdleAnimation = null;
        }

        /// <summary>
        /// Gets the walk speed multiplier.
        /// </summary>
        /// <param name="groundDragFactor">The amount of drag provided by the current ground. (Default: 0.3)</param>
        public virtual double GetWalkSpeedMultiplier(double groundDragFactor = 0.3)
        {
            int y1 = (int)(LocalPos.Y - 0.05f);
            int y2 = (int)(LocalPos.Y + 0.01f);

            Block belowBlock = World.BlockAccessor.GetBlock((int)LocalPos.X, y1, (int)LocalPos.Z);
            Block insideblock = World.BlockAccessor.GetBlock((int)LocalPos.X, y2, (int)LocalPos.Z);

            double multiplier = (servercontrols.Sneak ? GlobalConstants.SneakSpeedMultiplier : 1.0) * (servercontrols.Sprint ? GlobalConstants.SprintSpeedMultiplier : 1.0);
            
            if (FeetInLiquid) multiplier /= 2.5;

            IPlayer player = (this as EntityPlayer)?.Player;
            if (player == null || player.WorldData.CurrentGameMode != EnumGameMode.Creative)
            {
                multiplier *= belowBlock.WalkSpeedMultiplier * (y1 == y2 ? 1 : insideblock.WalkSpeedMultiplier);
            }

            multiplier *= Stats.GetBlended("walkspeed");

            return multiplier;
        }

        /// <summary>
        /// Sets a walk speed modifier that affects the entity's movement speed. Overrides existing value with the same key.
        /// Is multiplied with other modifiers like so: <code>baseMovementSpeed * mod1 * mod2 * ...</code>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="persistent">Whether the modifier should be saved and loaded.</param>
        [Obsolete("Use entity.Stats.Set(\"walkspeed\", key, value, persistent) instead")]
        public virtual void SetWalkSpeedModifier(string key, float value, bool persistent)
        {
            Stats.Set("walkspeed", key, value, persistent);
        }

        /// <summary>
        /// Removes a previously set walk speed modifier. Does nothing if it doesn't exist.
        /// </summary>
        /// <param name="key"></param>
        [Obsolete("Use entity.Stats.Remove(\"walkspeed\", key) instead")]
        public virtual void RemoveWalkSpeedModifier(string key)
        {
            Stats.Remove("walkspeed", key);
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
            base.FromBytes(reader, forClient);
            controls.FromBytes(reader, LoadControlsFromServer);
            
            if (!WatchedAttributes.HasAttribute("mountedOn"))
            {
                TryUnmount();
            }
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

    }
}
