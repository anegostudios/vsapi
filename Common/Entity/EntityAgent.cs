using System;
using System.Collections.Generic;
using System.IO;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;


namespace Vintagestory.API.Common
{
    /// <summary>
    /// An autonomous, goal-directed entity which observes and acts upon an environment
    /// </summary>
    public class EntityAgent : Entity, IEntityAgent
    {

        /// <summary>
        /// True if all clients have to be informed about this entities death. Set to false once all clients have been notified
        /// </summary>
        public bool DeadNotify;

        /// <summary>
        /// Unique identifier for a herd
        /// </summary>
        public long HerdId;

        protected EntityControls controls;
        protected EntityControls servercontrols;
        public PathTraverserBase PathTraverser;

        public IMountable MountedOn { get; protected set; }
        public EnumEntityActivity CurrentControls;

        internal virtual bool LoadControlsFromServer
        {
            get { return true; }
        }

        public virtual ItemSlot LeftHandItemSlot
        {
            get { return null; }
        }

        public virtual ItemSlot RightHandItemSlot
        {
            get { return null; }
        }

        public virtual IInventory GearInventory
        {
            get { return null; }
        }




        public EntityAgent() : base()
        {
            controls = new EntityControls();
            servercontrols = new EntityControls();
        }


        public override void Initialize(ICoreAPI api, long InChunkIndex3d)
        {
            base.Initialize(api, InChunkIndex3d);

            PathTraverser = new StraightLinePathTraverser(this);

            if (Type.Client.Animations != null)
            {
                foreach (AnimationMetaData anim in Type.Client.Animations)
                {
                    anim.Init();
                }
            }

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

        public EntityControls Controls
        {
            get { return controls; }
        }

        public EntityControls ServerControls
        {
            get { return servercontrols; }
        }

        public bool IsEyesSubmerged()
        {
            int blockId = GetEyesBlockId();
            return World.Blocks[blockId].MatterState == EnumMatterState.Liquid;
        }

        public ushort GetEyesBlockId()
        {
            BlockPos pos = LocalPos.AsBlockPos.Add(0, (float)EyeHeight(), 0);
            return World.BlockAccessor.GetBlockId(pos.X, pos.Y, pos.Z);
        }

        public bool TryMount(IMountable onmount)
        {
            this.MountedOn = onmount;
            controls.StopAllMovement();

            if (MountedOn?.SuggestedAnimation != null)
            {
                string anim = MountedOn.SuggestedAnimation.ToLowerInvariant();
                StartAnimation(anim);
            }

            onmount.DidMount(this);

            return true;
        }

        public bool TryUnmount()
        {
            if (MountedOn?.SuggestedAnimation != null)
            {
                string anim = MountedOn.SuggestedAnimation.ToLowerInvariant();
                StopAnimation(anim);
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

            base.Die(reason, damageSourceForDeath);
        }

        public override void OnInteract(EntityAgent byEntity, IItemSlot slot, Vec3d hitPosition, int mode)
        {
            if (mode == 0 && byEntity.World.Side == EnumAppSide.Server)
            {
                float damage = slot.Itemstack == null ? 0.5f : slot.Itemstack.Collectible.GetAttackPower(slot.Itemstack);

                if (byEntity is EntityPlayer && !IsActivityRunning("invulnerable"))
                {
                    World.PlaySoundAt(new AssetLocation("sounds/player/slap"), ServerPos.X, ServerPos.Y, ServerPos.Z);
                    slot?.Itemstack?.Collectible.OnAttackingWith(byEntity.World, byEntity, slot);
                }

                ReceiveDamage(new DamageSource() { source = EnumDamageSource.Entity, sourceEntity = (Entity)byEntity, type = EnumDamageType.BluntAttack }, damage);

                
            }
        }

        public override bool ShouldReceiveDamage(DamageSource damageSource, float damage)
        {
            if (!Alive) return false;

            if (damageSource.type != EnumDamageType.Heal)
            {
                PlayEntitySound("hurt");
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

            if (ret && this is EntityPlayer && damageSource.GetSourcePosition() != null)
            {
                ((IServerPlayer)World.PlayerByUid(((EntityPlayer)this).PlayerUID)).SendPositionToClient();
            }

            return ret;
        }


        public virtual void ReceiveSaturation(float saturation)
        {
            if (!Alive) return;

            if (ShouldReceiveSaturation(saturation))
            {
                foreach (EntityBehavior behavior in Behaviors)
                {
                    behavior.OnEntityReceiveSaturation(saturation);
                }
            }
        }

        public virtual bool ShouldReceiveSaturation(float saturation)
        {
            return true;
        }


        public override void OnGameTick(float dt)
        {
            PathTraverser.OnGameTick(dt);

            if (World.Side == EnumAppSide.Client)
            {
                bool alive = Alive;

                CurrentControls =
                    (alive && (servercontrols.TriesToMove || ((servercontrols.Jump || servercontrols.Sneak) && servercontrols.IsClimbing)) ? EnumEntityActivity.Move : EnumEntityActivity.Idle) |
                    (alive && Swimming ? EnumEntityActivity.Swim : 0) |
                    (alive && servercontrols.FloorSitting ? EnumEntityActivity.FloorSitting : 0) |
                    (alive && servercontrols.Sneak && !servercontrols.IsClimbing && !servercontrols.FloorSitting ? EnumEntityActivity.SneakMode : 0) |
                    (alive && servercontrols.Sprint ? EnumEntityActivity.SprintMode : 0) |
                    (alive && servercontrols.IsFlying ? EnumEntityActivity.Fly : 0) |
                    (alive && servercontrols.IsClimbing ? EnumEntityActivity.Climb : 0) |
                    (alive && servercontrols.Jump && OnGround ? EnumEntityActivity.Jump : 0) |
                    (alive && !OnGround && !Swimming && !FeetInLiquid && !servercontrols.IsClimbing && !servercontrols.IsFlying && LocalPos.Motion.Y < -0.05 ? EnumEntityActivity.Fall : 0) |
                    (!alive ? EnumEntityActivity.Dead : 0)
                ;
                CurrentControls = CurrentControls == 0 ? EnumEntityActivity.Idle : CurrentControls;

                if (this is EntityPlayer) HandleHandAnimations();

                AnimationMetaData defaultAnim = null;
                bool anyAverageAnimActive = false;


                AnimationMetaData[] anims = Type.Client?.Animations;
                for (int i = 0; anims != null && i < anims.Length; i++)
                {
                    AnimationMetaData anim = anims[i];
                    bool wasActive = ActiveAnimationsByAnimCode.ContainsKey(anim.Animation);
                    bool nowActive = anim.Matches((int)CurrentControls);

                    anyAverageAnimActive |= nowActive && anim.BlendMode == EnumAnimationBlendMode.Average;

                    if (anim?.TriggeredBy?.DefaultAnim == true) defaultAnim = anim;

                    if (!wasActive && nowActive)
                    {
                        anim.WasStartedFromTrigger = true;
                        StartAnimation(anim);
             //           Console.WriteLine("start" + anim.Animation);
                    }

                    if (wasActive && !nowActive && anim.WasStartedFromTrigger)
                    {
                        anim.WasStartedFromTrigger = false;
                        StopAnimation(anim.Animation);
             //           Console.WriteLine("stop" + anim.Animation);
                    }
                }

                if (!anyAverageAnimActive && defaultAnim != null)
                {
                    defaultAnim.WasStartedFromTrigger = true;
          //          Console.WriteLine("start default " + defaultAnim.Animation);
                    StartAnimation(defaultAnim);
                }
            }

            if (Type.RotateModelOnClimb && World.Side == EnumAppSide.Server)
            {
                if (Controls.IsClimbing && ClimbingOnFace != null)
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


        string lastRunningHeldUseAnimation;
        string lastRunningHeldIdleAnimation;
        string lastRunningHeldHitAnimation;

        private void HandleHandAnimations()
        {
            ItemStack stack = RightHandItemSlot?.Itemstack;

            bool nowUseStack = servercontrols.RightMouseDown;
            bool wasUseStack = lastRunningHeldUseAnimation != null && ActiveAnimationsByAnimCode.ContainsKey(lastRunningHeldUseAnimation);

            bool nowHitStack = servercontrols.LeftMouseDown;
            bool wasHitStack = lastRunningHeldHitAnimation != null && ActiveAnimationsByAnimCode.ContainsKey(lastRunningHeldHitAnimation);

            bool nowIdleStack = !servercontrols.LeftMouseDown && !servercontrols.RightMouseDown;
            bool wasIdleStack = lastRunningHeldIdleAnimation != null && ActiveAnimationsByAnimCode.ContainsKey(lastRunningHeldIdleAnimation);

            string nowHeldUseAnim = stack?.Collectible.GetHeldTpUseAnimation(RightHandItemSlot, this);
            string nowHeldHitAnim = stack?.Collectible.GetHeldTpHitAnimation(RightHandItemSlot, this);
            string nowHeldIdleAnim = stack?.Collectible.GetHeldTpIdleAnimation(RightHandItemSlot, this);

            if (stack == null) nowHeldHitAnim = "breakhand";

            if (nowUseStack != wasUseStack || (lastRunningHeldUseAnimation != null && nowHeldUseAnim != lastRunningHeldUseAnimation))
            {
                StopHandAnims();
                if (nowUseStack)
                {
                    StartAnimation(lastRunningHeldUseAnimation = nowHeldUseAnim);
                }
            }

            if (nowHitStack != wasHitStack || (lastRunningHeldHitAnimation != null && nowHeldHitAnim != lastRunningHeldHitAnimation))
            {
                StopHandAnims();
                if (nowHitStack)
                {
                    StartAnimation(lastRunningHeldHitAnimation = nowHeldHitAnim);
                }
            }

            if (nowIdleStack != wasIdleStack || (lastRunningHeldIdleAnimation != null && nowHeldIdleAnim != lastRunningHeldIdleAnimation))
            {
                StopHandAnims();
                if (nowIdleStack)
                {
                    StartAnimation(lastRunningHeldIdleAnimation = nowHeldIdleAnim);
                }
            }
        }

        private void StopHandAnims()
        {
            StopAnimation(lastRunningHeldUseAnimation);
            lastRunningHeldUseAnimation = null;

            StopAnimation(lastRunningHeldHitAnimation);
            lastRunningHeldHitAnimation = null;

            StopAnimation(lastRunningHeldIdleAnimation);
            lastRunningHeldIdleAnimation = null;
        }


        public double GetWalkSpeedMultiplier(double groundDragFactor = 0.3)
        {
            Block belowBlock = World.BlockAccessor.GetBlock((int)LocalPos.X, (int)(LocalPos.Y - 0.05f), (int)LocalPos.Z);

            double multiplier = (servercontrols.Sneak ? GlobalConstants.SneakSpeedMultiplier : 1.0) * (servercontrols.Sprint ? GlobalConstants.SprintSpeedMultiplier : 1.0);

            if (FeetInLiquid) multiplier /= 2.5;

            multiplier *= 1 + (belowBlock.WalkSpeedMultiplier - 1) / (1 - groundDragFactor);

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
            base.FromBytes(reader, forClient);
            controls.FromBytes(reader, LoadControlsFromServer);

            if (!WatchedAttributes.HasAttribute("mountedOn"))
            {
                TryUnmount();
            }
        }

        


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

        public virtual void WalkInventory(OnInventorySlot handler)
        {
            
        }

        


    }
}
