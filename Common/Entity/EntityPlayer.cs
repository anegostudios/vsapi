using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Vintagestory.API.Common
{
    public class EntityPlayer : EntityHumanoid, IEntityPlayer
    {
        public API.Common.Action<float> PhysicsUpdateWatcher;

        public BlockSelection blockSelection;
        public EntitySelection entitySelection;

        public Vec3d CameraPos = new Vec3d();
        public float WalkYaw;
        public float WalkPitch;

        public DamageSource DeathReason;

        double eyeHeightCurrent;
        ICoreAPI api;

        Vec3d IEntityPlayer.CameraPos
        {
            get { return CameraPos; }
        }

        public override bool StoreWithChunk
        {
            get { return false; }

        }

        public string PlayerUID
        {
            get { return WatchedAttributes.GetString("playerUID"); }
        }

        public override ItemSlot RightHandItemSlot
        {
            get
            {
                IPlayer player = World.PlayerByUid(PlayerUID);
                return player?.InventoryManager.ActiveHotbarSlot;
            }
        }


        public override IInventory GearInventory
        {
            get
            {
                IPlayer player = World.PlayerByUid(PlayerUID);
                return player?.InventoryManager.GetOwnInventory(GlobalConstants.characterInvClassName);
            }
        }


        public override byte[] LightHsv
        {
            get { return RightHandItemSlot?.Itemstack?.Block?.LightHsv; }
        }

        public override bool AlwaysActive
        {
            get { return true; }
        }

        public override bool ShouldDespawn
        {
            get { return false; }
        }

        internal override bool LoadControlsFromServer
        {
            get
            {
                return !(World is IClientWorldAccessor) || ((IClientWorldAccessor)World).Player.Entity.EntityId != Entityid;
            }
        }

        public override bool IsInteractable
        {
            get
            {
                return World?.PlayerByUid(PlayerUID)?.WorldData.CurrentGameMode != EnumGameMode.Spectator;
            }
        }


        public EntityPlayer() : base()
        {

        }

        public override void Initialize(ICoreAPI api, long chunkindex3d)
        {
            this.api = api;
            base.Initialize(api, chunkindex3d);
        }


        public override double EyeHeight()
        {
            if (MountedOn?.SuggestedAnimation == "sleep")
            {
                return 0.3;
            }

            return eyeHeightCurrent;
        }

        double walkCounter;
        double prevStepHeight;
        int direction = 0;
        

        public override void OnGameTick(float dt)
        {
            double newEyeheight = Type.EyeHeight;
            double newModelHeight = Type.HitBoxSize.Y;

            if (servercontrols.FloorSitting)
            {
                newEyeheight *= 0.45f;
                newModelHeight *= 0.5f;
            }
            else if (servercontrols.Sneak && !servercontrols.IsClimbing && !servercontrols.IsFlying)
            {
                newEyeheight *= 0.85f;
                newModelHeight *= 0.85f;
            }

            bool moving = (servercontrols.TriesToMove && LocalPos.Motion.LengthSq() > 0.00001) && !servercontrols.NoClip && !servercontrols.FlyMode && OnGround;

            double frequency = dt * servercontrols.MovespeedMultiplier * GetWalkSpeedMultiplier(0.3);

            walkCounter = moving ? walkCounter + frequency : 0;
            walkCounter = walkCounter % GameMath.TWOPI;

            double sneakMul = (servercontrols.Sneak ? 1.7 : 1);

            double amplitude = (FeetInLiquid ? 0.8 : 1) /  (3 * sneakMul);
            double offset = -0.2 / sneakMul;


            double stepHeight = -Math.Max(0, Math.Abs(GameMath.Sin(6 * walkCounter) * amplitude) + offset);
            if (api.Side == EnumAppSide.Client && (api as ICoreClientAPI).Settings.Bool["viewBobbing"]) newEyeheight += stepHeight;

            

            if (moving)
            {
                if (stepHeight > prevStepHeight)
                {
                    if (direction == -1)
                    {
                        IPlayer player = World.PlayerByUid(PlayerUID);
                        float volume = controls.Sneak ? 0.5f : 1f;

                        int blockIdUnder = BlockUnderPlayer();
                        int blockIdInside = BlockInsidePlayer();

                        AssetLocation soundwalk = World.Blocks[blockIdUnder].Sounds?.Walk;
                        AssetLocation soundinside = World.Blocks[blockIdInside].Sounds?.Inside;

                        if (soundwalk != null)
                        {
                            if (blockIdInside != blockIdUnder && soundinside != null)
                            {
                                World.PlaySoundAt(soundwalk, this, player, true, 12, volume * 0.5f);
                                World.PlaySoundAt(soundinside, this, player, true, 12, volume);
                            }
                            else
                            {
                                World.PlaySoundAt(soundwalk, this, player, true, 12, volume);
                            }
                        }
                    }
                    direction = 1;
                } else
                {
                    direction = -1;
                }
                
            }

            prevStepHeight = stepHeight;

            double diff = (newEyeheight - eyeHeightCurrent) * 5 * dt;
            eyeHeightCurrent = diff > 0 ? Math.Min(eyeHeightCurrent + diff, newEyeheight) : Math.Max(eyeHeightCurrent + diff, newEyeheight);

            diff = (newModelHeight - CollisionBox.Y2) * 5 * dt;
            CollisionBox.Y2 = (float)(diff > 0 ? Math.Min(CollisionBox.Y2 + diff, newModelHeight) : Math.Max(CollisionBox.Y2 + diff, newModelHeight));

            base.OnGameTick(dt);


        }


        public override void OnFallToGround(double motionY)
        {
            IPlayer player = World.PlayerByUid(PlayerUID);
            int blockIdUnder = BlockUnderPlayer();
            AssetLocation soundwalk = World.Blocks[blockIdUnder].Sounds?.Walk;
            if (soundwalk != null)
            {
                World.PlaySoundAt(soundwalk, this, player, true, 12, 1.5f);
            }

            base.OnFallToGround(motionY);
        }



        internal int BlockUnderPlayer()
        {
            EntityPos pos = LocalPos;
            return World.BlockAccessor.GetBlockId(
                (int)pos.X,
                (int)(pos.Y - 0.1f),
                (int)pos.Z);
        }

        internal int BlockInsidePlayer()
        {
            EntityPos pos = LocalPos;

            return World.BlockAccessor.GetBlockId(
                (int)pos.X,
                (int)(pos.Y + 0.1f),
                (int)pos.Z);
        }




        public override bool TryGiveItemStack(ItemStack itemstack)
        {
            IPlayer player = World.PlayerByUid(PlayerUID);
            if (player != null) return player.InventoryManager.TryGiveItemstack(itemstack, true);
            return false;
        }


        public override void Die(EnumDespawnReason reason = EnumDespawnReason.Death, DamageSource damageSourceForDeath = null)
        {
            base.Die(reason, damageSourceForDeath);

            DeathReason = damageSourceForDeath;
            DespawnReason = null;
            DeadNotify = true;
            TryStopHandAction(true, EnumItemUseCancelReason.Death);
            

            if (Type.Server?.Attributes?["keepContents"]?.AsBool(false) != true)
            {
                World.PlayerByUid(PlayerUID).InventoryManager.OnDeath();
            }
        }

        public virtual void Revive()
        {
            Alive = true;
            ReceiveDamage(new DamageSource() { source = EnumDamageSource.Respawn, type = EnumDamageType.Heal }, 9999);
        }

        public override bool ShouldReceiveDamage(DamageSource damageSource, float damage)
        {
            EnumGameMode mode = World.PlayerByUid(PlayerUID).WorldData.CurrentGameMode;
            if ((mode == EnumGameMode.Creative || mode == EnumGameMode.Spectator) && damageSource.type != EnumDamageType.Heal) return false;

            return base.ShouldReceiveDamage(damageSource, damage);
        }


        public override void OnHurt(DamageSource damageSource, float damage)
        {
            if (damage > 0 && World != null && World.Side == EnumAppSide.Client && (World as IClientWorldAccessor).Player.Entity.EntityId == this.EntityId)
            {
                (World as IClientWorldAccessor).ShakeCamera(0.3f);
            }

            if (damage != 0 && World?.Side == EnumAppSide.Server)
            {
                string strDamage = "Lost " + (damage).ToString();
                if (damageSource.type == EnumDamageType.Heal) strDamage = "Gained " + damage;

                string msg = Lang.Get("{0} hp through {1}", strDamage, damageSource.type.ToString().ToLowerInvariant());// damageSource.source.ToString().ToLowerInvariant());
                
                if (damageSource.source == EnumDamageSource.Player)
                {
                    EntityPlayer eplr = damageSource.sourceEntity as EntityPlayer;
                    msg = Lang.Get("{0} hp by player {1}", strDamage, damageSource.source.ToString().ToLowerInvariant(), World.PlayerByUid(eplr.PlayerUID).PlayerName);
                }

                if (damageSource.source == EnumDamageSource.Entity)
                {
                    string creatureName = Lang.Get("prefixandcreature-" + damageSource.sourceEntity.Type.Code.Path.Replace("-",""));
                    msg = Lang.Get("{0} hp by {1}", strDamage, creatureName);
                }
                
                (World.PlayerByUid(PlayerUID) as IServerPlayer).SendMessage(GlobalConstants.DamageLogChatGroup, msg, EnumChatType.Notification);
            }
        }



        public override bool TryStopHandAction(bool forceStop, EnumItemUseCancelReason cancelReason = EnumItemUseCancelReason.ReleasedMouse)
        {
            if (controls.HandUse == EnumHandInteract.None || RightHandItemSlot?.Itemstack == null) return true;

            IPlayer player = World.PlayerByUid(PlayerUID);
            float secondsPassed = (World.ElapsedMilliseconds - controls.UsingBeginMS) / 1000f;

            if (forceStop)
            {
                controls.HandUse = EnumHandInteract.None; 
            } else
            {
                controls.HandUse = RightHandItemSlot.Itemstack.Collectible.OnHeldUseCancel(secondsPassed, RightHandItemSlot, this, player.CurrentBlockSelection, player.CurrentEntitySelection, cancelReason);
            }


            if (controls.HandUse == EnumHandInteract.None)
            {
                RightHandItemSlot.Itemstack.Collectible.OnHeldUseStop(secondsPassed, RightHandItemSlot, this, player.CurrentBlockSelection, player.CurrentEntitySelection, controls.HandUse);
            }

            return controls.HandUse == EnumHandInteract.None;
        }


        public override void WalkInventory(OnInventorySlot handler)
        {
            IPlayer player = World.PlayerByUid(PlayerUID);

            foreach(InventoryBase inv in player.InventoryManager.Inventories.Values)
            {
                if (!inv.HasOpened(player)) continue;

                int q = inv.QuantitySlots;
                for (int i = 0; i < q; i++)
                {
                    if (!handler(inv.GetSlot(i))) return;
                }
            }
        }

        public void SetCurrentlyControlledPlayer()
        {
            this.servercontrols = controls;
        }

        public override void OnCollideWithLiquid()
        {
            if (World?.PlayerByUid(PlayerUID)?.WorldData != null && World.PlayerByUid(PlayerUID).WorldData.CurrentGameMode != EnumGameMode.Spectator)
            {
                base.OnCollideWithLiquid();
            }
        }
    }
}
