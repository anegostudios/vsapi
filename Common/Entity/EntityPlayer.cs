using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
    public delegate bool CanSpawnNearbyDelegate(EntityProperties type, Vec3d spawnPosition, RuntimeSpawnConditions sc);

    public class EntityPlayer : EntityHumanoid
    {
        /// <summary>
        /// The block position previously selected by the player
        /// </summary>
        public BlockPos PreviousBlockSelection;

        /// <summary>
        /// The block or blocks currently selected by the player
        /// </summary>
        public BlockSelection BlockSelection;

        /// <summary>
        /// The entity or entities selected by the player
        /// </summary>
        public EntitySelection EntitySelection;

        /// <summary>
        /// The reason the player died (if the player did die). Set only by the game server.
        /// </summary>
        public DamageSource DeathReason;

        /// <summary>
        /// The camera position of the player's view. Set only by the game client.
        /// </summary>
        public Vec3d CameraPos = new Vec3d();

        /// <summary>
        /// The yaw the player currently wants to walk towards to. Value set by the PlayerPhysics system. Set by the game client and server.
        /// </summary>
        public float WalkYaw;
        /// <summary>
        /// The pitch the player currently wants to move to. Only relevant while swimming. Value set by the PlayerPhysics system. Set by the game client and server.
        /// </summary>
        public float WalkPitch;

        /// <summary>
        /// Set this to hook into the foot step sound creator thingy. Currently used by the armor system to create armor step sounds. Called by the game client and server.
        /// </summary>
        public Action OnFootStep;

        /// <summary>
        /// Called when the player falls onto the ground. Called by the game client and server.
        /// </summary>
        public Action<double> OnImpact;

        /// <summary>
        /// Called whenever the game wants to spawn new creatures around the player. Called only by the game server.
        /// </summary>
        public CanSpawnNearbyDelegate OnCanSpawnNearby;

        public EntityTalkUtil talkUtil;

        public double LastReviveTotalHours
        {
            get
            {
                if (!WatchedAttributes.attributes.TryGetValue("lastReviveTotalHours", out IAttribute hrsAttr))
                {
                    return -9999;
                }

                return (hrsAttr as DoubleAttribute).value;
            }
            set
            {
                WatchedAttributes.SetDouble("lastReviveTotalHours", value);
            }
        }

        public override bool StoreWithChunk
        {
            get { return false; }

        }

        /// <summary>
        /// The player's internal Universal ID. Available on the client and the server.
        /// </summary>
        public string PlayerUID
        {
            get { return WatchedAttributes.GetString("playerUID"); }
        }

        /// <summary>
        /// The players right hand contents. Available on the client and the server.
        /// </summary>
        public override ItemSlot RightHandItemSlot
        {
            get
            {
                IPlayer player = World.PlayerByUid(PlayerUID);
                return player?.InventoryManager.ActiveHotbarSlot;
            }
        }

        /// <summary>
        /// The playres left hand contents. Available on the client and the server.
        /// </summary>
        public override ItemSlot LeftHandItemSlot
        {
            get
            {
                IPlayer player = World.PlayerByUid(PlayerUID);
                return player?.InventoryManager?.GetHotbarInventory()?[10];
            }
        }

        /// <summary>
        /// The players wearables. Available on the client and the server.
        /// </summary>
        public override IInventory GearInventory
        {
            get
            {
                IPlayer player = World.PlayerByUid(PlayerUID);
                return player?.InventoryManager.GetOwnInventory(GlobalConstants.characterInvClassName);
            }
        }


        bool newSpawnGlow;


        public override byte[] LightHsv
        {
            get {
                byte[] rightHsv = RightHandItemSlot?.Itemstack?.Block?.GetLightHsv(World.BlockAccessor, null, RightHandItemSlot.Itemstack);
                byte[] leftHsv = LeftHandItemSlot?.Itemstack?.Block?.GetLightHsv(World.BlockAccessor, null, LeftHandItemSlot.Itemstack);

                if ((rightHsv == null || rightHsv[2] == 0) && (leftHsv == null || leftHsv[2] == 0))
                {
                    double hoursAlive = Api.World.Calendar.TotalHours - LastReviveTotalHours;

                    if (hoursAlive < 2f)
                    {
                        newSpawnGlow = true;
                        Properties.Client.GlowLevel = (int)GameMath.Clamp((100 * (2f - hoursAlive)), 0, 255);
                    }

                    if (hoursAlive < 1.5f)
                    {
                        newSpawnGlow = true;
                        return new byte[]
                        {
                            33, 7, (byte)Math.Min(10, (11 * (1.5f - hoursAlive)))
                        };
                    }
                } else
                {
                    if (newSpawnGlow) // Don't repeatedly set glowlevel to 0, but only once after our respawn glow expired.
                    {
                        Properties.Client.GlowLevel = 0; 
                        newSpawnGlow = false;
                    }
                }

                if (rightHsv == null) return leftHsv;
                if (leftHsv == null) return rightHsv;

                float totalval = rightHsv[2] + leftHsv[2];
                float t = leftHsv[2] / totalval;

                return new byte[]
                {
                    (byte)(leftHsv[0] * t + rightHsv[0] * (1-t)),
                    (byte)(leftHsv[1] * t + rightHsv[1] * (1-t)),
                    Math.Max(leftHsv[2], rightHsv[2])
                };
            }
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
                return !(World is IClientWorldAccessor) || ((IClientWorldAccessor)World).Player.Entity.EntityId != EntityId;
            }
        }

        public override bool IsInteractable
        {
            get
            {
                IWorldPlayerData worldData = World?.PlayerByUid(PlayerUID)?.WorldData;
                return worldData?.CurrentGameMode != EnumGameMode.Spectator && worldData?.NoClip != true;
            }
        }

        public override double LadderFixDelta { get { return Properties.SpawnCollisionBox.Y2 - CollisionBox.YSize; } }

        /// <summary>
        /// The base player attached to this EntityPlayer.
        /// </summary>
        public IPlayer Player
        {
            get
            {
                return World?.PlayerByUid(PlayerUID);
            }
        }


        public EntityPlayer() : base()
        {
            Stats
                .Register("healingeffectivness")
                //.Register("maxhealthMul")
                .Register("maxhealthExtraPoints")
                .Register("walkspeed")
                .Register("hungerrate")
                .Register("rangedWeaponsAcc")
                .Register("rangedWeaponsSpeed")

                .Register("rangedWeaponsDamage")
                .Register("meleeWeaponsDamage")
                .Register("mechanicalsDamage")
                .Register("animalLootDropRate")
                .Register("forageDropRate")
                .Register("wildCropDropRate")
                
                .Register("vesselContentsDropRate")
                .Register("oreDropRate")
                .Register("rustyGearDropRate")
                .Register("miningSpeedMul")
                .Register("animalSeekingRange")
                .Register("armorDurabilityLoss")
                .Register("bowDrawingStrength")
                .Register("wholeVesselLootChance", EnumStatBlendType.FlatSum)
                .Register("temporalGearTLRepairCost", EnumStatBlendType.FlatSum)
                .Register("animalHarvestingTime")
            ;

        }

        public override void Initialize(EntityProperties properties, ICoreAPI api, long chunkindex3d)
        {
            controls.StopAllMovement();

            if (api.Side == EnumAppSide.Client)
            {
                talkUtil = new EntityTalkUtil(api as ICoreClientAPI, this);
                talkUtil.soundName = new AssetLocation("sounds/voice/altoflute");
                talkUtil.idleTalkChance = 0f;
            }

            base.Initialize(properties, api, chunkindex3d);

            if (api.Side == EnumAppSide.Server && !WatchedAttributes.attributes.ContainsKey("lastReviveTotalHours"))
            {
                double hrs = Api.World.Calendar.GetDayLightStrength(ServerPos.X, ServerPos.Z) < 0.5 ? Api.World.Calendar.TotalHours : -9999;
                WatchedAttributes.SetDouble("lastReviveTotalHours", hrs);
            }
        }


        double walkCounter;
        double prevStepHeight;
        int direction = 0;

        public bool PrevFrameCanStandUp;
        public ClimateCondition selfClimateCond;
        float climateCondAccum;

        public override double GetWalkSpeedMultiplier(double groundDragFactor = 0.3)
        {
            double mul = base.GetWalkSpeedMultiplier(groundDragFactor);

            if (!servercontrols.Sneak && !PrevFrameCanStandUp)
            {
                mul *= GlobalConstants.SneakSpeedMultiplier;
            }

            return mul;
        }

        // This method is called on the client for the own entity player, because some things need to happen every frame
        public void OnSelfBeforeRender(float dt)
        {
            updateEyeHeight(dt);
        }


        public override void OnTesselation(ref Shape entityShape, string shapePathForLogging)
        {
            base.OnTesselation(ref entityShape, shapePathForLogging);

            IInventory backPackInv = Player?.InventoryManager.GetOwnInventory(GlobalConstants.backpackInvClassName);

            Dictionary<string, ItemSlot> uniqueGear = new Dictionary<string, ItemSlot>();
            for (int i = 0; backPackInv != null && i < 4; i++)
            {
                ItemSlot slot = backPackInv[i];
                if (slot.Empty) continue;
                uniqueGear["" + slot.Itemstack.Class + slot.Itemstack.Collectible.Id] = slot;
            }

            foreach (var val in uniqueGear)
            {
                entityShape = addGearToShape(val.Value, entityShape, shapePathForLogging);
            }
        }

        private void updateEyeHeight(float dt)
        {
            IPlayer player = World.PlayerByUid(PlayerUID);
            PrevFrameCanStandUp = true;

            if (player != null && player?.WorldData?.CurrentGameMode != EnumGameMode.Spectator)
            {
                PrevFrameCanStandUp = !servercontrols.Sneak && canStandUp();
                bool moving = (servercontrols.TriesToMove && SidedPos.Motion.LengthSq() > 0.00001) && !servercontrols.NoClip && !servercontrols.FlyMode && OnGround;
                double newEyeheight = Properties.EyeHeight;

                if (player.ImmersiveFpMode)
                {
                    updateLocalEyePosImmersiveFpMode();

                    newEyeheight = LocalEyePos.Y;
                }
                else
                {

                    double newModelHeight = Properties.HitBoxSize.Y;

                    if (servercontrols.FloorSitting)
                    {
                        newEyeheight *= 0.5f;
                        newModelHeight *= 0.55f;
                    }
                    else if ((servercontrols.Sneak || !PrevFrameCanStandUp) && !servercontrols.IsClimbing && !servercontrols.IsFlying)
                    {
                        newEyeheight *= 0.8f;
                        newModelHeight *= 0.8f;
                    }
                    else if (!Alive)
                    {
                        newEyeheight *= 0.25f;
                        newModelHeight *= 0.25f;
                    }


                    double diff = (newEyeheight - LocalEyePos.Y) * 5 * dt;
                    LocalEyePos.Y = diff > 0 ? Math.Min(LocalEyePos.Y + diff, newEyeheight) : Math.Max(LocalEyePos.Y + diff, newEyeheight);

                    diff = (newModelHeight - OriginCollisionBox.Y2) * 5 * dt;
                    OriginCollisionBox.Y2 = CollisionBox.Y2 = (float)(diff > 0 ? Math.Min(CollisionBox.Y2 + diff, newModelHeight) : Math.Max(CollisionBox.Y2 + diff, newModelHeight));

                    LocalEyePos.X = 0;
                    LocalEyePos.Z = 0;

                    if (MountedOn?.SuggestedAnimation == "sleep")
                    {
                        LocalEyePos.Y = 0.3;
                    }
                }



                double frequency = dt * servercontrols.MovespeedMultiplier * GetWalkSpeedMultiplier(0.3) * (servercontrols.Sprint ? 0.9 : 1.2) * (servercontrols.Sneak ? 1.2f : 1);

                walkCounter = moving ? walkCounter + frequency : 0;
                walkCounter = walkCounter % GameMath.TWOPI;

                double sneakDiv = (servercontrols.Sneak ? 3 : 1.8);

                double amplitude = (FeetInLiquid ? 0.8 : 1 + (servercontrols.Sprint ? 0.07 : 0)) / (3 * sneakDiv);
                double offset = -0.2 / sneakDiv;

                double stepHeight = -Math.Max(0, Math.Abs(GameMath.Sin(5.5f * walkCounter) * amplitude) + offset);

                if (World.Side == EnumAppSide.Client)
                {
                    ICoreClientAPI capi = World.Api as ICoreClientAPI;
                    if (capi.Settings.Bool["viewBobbing"] && capi.Render.CameraType == EnumCameraMode.FirstPerson)
                    {
                        LocalEyePos.Y += stepHeight / 3f * dt * 60f;
                    }
                }


                if (moving)
                {
                    if (stepHeight > prevStepHeight)
                    {
                        if (direction == -1)
                        {
                            float volume = controls.Sneak ? 0.5f : 1f;

                            EntityPos pos = SidedPos;
                            int blockIdUnder = BlockUnderPlayer(pos);
                            int blockIdInside = BlockInsidePlayer(pos);

                            AssetLocation soundwalk = World.Blocks[blockIdUnder].GetSounds(Api.World.BlockAccessor, new BlockPos((int)pos.X, (int)(pos.Y - 0.1f), (int)pos.Z))?.Walk;
                            AssetLocation soundinside = World.Blocks[blockIdInside].GetSounds(Api.World.BlockAccessor, new BlockPos((int)pos.X, (int)(pos.Y + 0.1f), (int)pos.Z))?.Inside;

                            if (!Swimming && soundwalk != null)
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

                                OnFootStep?.Invoke();
                            }

                        }
                        direction = 1;
                    }
                    else
                    {
                        direction = -1;

                    }

                }

                prevStepHeight = stepHeight;
            }
        }

        public override void OnGameTick(float dt)
        {
            if (World.Side == EnumAppSide.Client)
            {
                talkUtil.OnGameTick(dt);
            }

            if (Api.Side == EnumAppSide.Server || (Api as ICoreClientAPI).World.Player.PlayerUID != PlayerUID)
            {
                updateEyeHeight(dt);
            }

            climateCondAccum += dt;
            if (World.Side == EnumAppSide.Client && climateCondAccum > 0.5f)
            {
                climateCondAccum = 0;
                selfClimateCond = Api.World.BlockAccessor.GetClimateAt(Pos.XYZ.AsBlockPos);
            }

            if (!servercontrols.Sneak && !PrevFrameCanStandUp)
            {
                // So the sneak animation plays still
                servercontrols.Sneak = true;
                base.OnGameTick(dt);
                servercontrols.Sneak = false;
            } else
            {
                base.OnGameTick(dt);
            }
        }


        Cuboidf tmpCollBox = new Cuboidf();
        bool holdPosition = false;
        
        float[] prevAnimModelMatrix;

        void updateLocalEyePosImmersiveFpMode()
        {
            AttachmentPointAndPose apap = AnimManager.Animator.GetAttachmentPointPose("Eyes");
            AttachmentPoint ap = apap.AttachPoint;

            float[] ModelMat = Mat4f.Create();
            Matrixf tmpModelMat = new Matrixf();

            float bodyYaw = BodyYaw;
            float rotX = Properties.Client.Shape != null ? Properties.Client.Shape.rotateX : 0;
            float rotY = Properties.Client.Shape != null ? Properties.Client.Shape.rotateY : 0;
            float rotZ = Properties.Client.Shape != null ? Properties.Client.Shape.rotateZ : 0;
            float bodyPitch = WalkPitch;

            float lookOffset = (SidedPos.Pitch - GameMath.PI) / 9f;

            bool wasHoldPos = holdPosition;
            holdPosition = false;
            
            for (int i = 0; i < AnimManager.Animator.RunningAnimations.Length; i++)
            {
                RunningAnimation anim = AnimManager.Animator.RunningAnimations[i];
                if (anim.Running && anim.EasingFactor > anim.meta.HoldEyePosAfterEasein)
                {
                    if (!wasHoldPos)
                    {
                        prevAnimModelMatrix = (float[])apap.AnimModelMatrix.Clone();
                    }
                    holdPosition = true;
                    break;
                }
            }


            tmpModelMat
                .Set(ModelMat)
                .RotateX(SidedPos.Roll + rotX * GameMath.DEG2RAD)
                .RotateY(bodyYaw + (180 + rotY) * GameMath.DEG2RAD)
                .RotateZ(bodyPitch + rotZ * GameMath.DEG2RAD)
                .Mul(holdPosition ? prevAnimModelMatrix : apap.AnimModelMatrix)
                .Scale(Properties.Client.Size, Properties.Client.Size, Properties.Client.Size)
                .Translate(-0.5f, 0, -0.5f)
                .Translate(ap.PosX / 16f - lookOffset, ap.PosY / 16f - lookOffset / 1.3f, ap.PosZ / 16f)
            ;

            float[] pos = new float[4] { 0, 0, 0, 1 };
            float[] endVec = Mat4f.MulWithVec4(tmpModelMat.Values, pos);

            LocalEyePos.Set(endVec[0], endVec[1], endVec[2]);
        }


        private bool canStandUp()
        {
            tmpCollBox.Set(CollisionBox);
            tmpCollBox.Y2 = Properties.HitBoxSize.Y;
            tmpCollBox.Y1 += 1f; // Don't care about the bottom block
            bool collide = World.CollisionTester.IsColliding(World.BlockAccessor, tmpCollBox, Pos.XYZ, false); ;
            return !collide;
        }

        public virtual bool CanSpawnNearby(EntityProperties type, Vec3d spawnPosition, RuntimeSpawnConditions sc)
        {
            if (OnCanSpawnNearby != null)
            {
                return OnCanSpawnNearby(type, spawnPosition, sc);
            }

            return true;
        }

        public override void OnFallToGround(double motionY)
        {
            IPlayer player = World.PlayerByUid(PlayerUID);

            if (player?.WorldData?.CurrentGameMode != EnumGameMode.Spectator)
            {
                EntityPos pos = SidedPos;
                int blockIdUnder = BlockUnderPlayer(pos);
                AssetLocation soundwalk = World.Blocks[blockIdUnder].GetSounds(Api.World.BlockAccessor, new BlockPos((int)pos.X, (int)(pos.Y - 0.1f), (int)pos.Z))?.Walk;
                if (soundwalk != null && !Swimming)
                {
                    World.PlaySoundAt(soundwalk, this, player, true, 12, 1.5f);
                }

                OnImpact?.Invoke(motionY);
            }

            base.OnFallToGround(motionY);
        }



        internal int BlockUnderPlayer(EntityPos pos)
        {
            return World.BlockAccessor.GetBlockId(
                (int)pos.X,
                (int)(pos.Y - 0.1f),
                (int)pos.Z);
        }

        internal int BlockInsidePlayer(EntityPos pos)
        {
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
            TryUnmount();
            
            // Execute this one frame later so that in case right after this method some other code still returns an item (e.g. BlockMeal), it is also ditched
            Api.Event.EnqueueMainThreadTask(() =>
            {
                if (Properties.Server?.Attributes?.GetBool("keepContents", false) != true)
                {
                    World.PlayerByUid(PlayerUID).InventoryManager.OnDeath();
                }

                if (Properties.Server?.Attributes?.GetBool("dropArmorOnDeath", false) == true)
                {
                    foreach (var slot in GearInventory)
                    {
                        if (slot.Empty) continue;
                        if (slot.Itemstack.ItemAttributes?["protectionModifiers"].Exists == true)
                        {
                            Api.World.SpawnItemEntity(slot.Itemstack, ServerPos.XYZ);
                            slot.Itemstack = null;
                            slot.MarkDirty();
                        }
                    }
                }
            }, "dropinventoryondeath");

        }

        public override void Revive()
        {
            base.Revive();

            LastReviveTotalHours = Api.World.Calendar.TotalHours;

            (Api as ICoreServerAPI).Network.SendEntityPacket(Api.World.PlayerByUid(PlayerUID) as IServerPlayer, this.EntityId, 196);
        }

        public override void PlayEntitySound(string type, IPlayer dualCallByPlayer = null, bool randomizePitch = true, float range = 24)
        {
            ICoreClientAPI capi = Api as ICoreClientAPI;

            if (type == "hurt")
            {
                if (World.Side == EnumAppSide.Server)
                {
                    (World.Api as ICoreServerAPI).Network.BroadcastEntityPacket(this.EntityId, 1001);
                } else
                {
                    if (!capi.Settings.Bool["newSeraphVoices"]) { base.PlayEntitySound(type, dualCallByPlayer, randomizePitch, range); return; }

                    talkUtil.Talk(EnumTalkType.Hurt2);
                }
                
                return;
            }
            if (type == "death")
            {
                if (World.Side == EnumAppSide.Server)
                {
                    (World.Api as ICoreServerAPI).Network.BroadcastEntityPacket(this.EntityId, 1002);
                } else
                {
                    if (!capi.Settings.Bool["newSeraphVoices"]) { base.PlayEntitySound(type, dualCallByPlayer, randomizePitch, range); return; }

                    talkUtil.Talk(EnumTalkType.Death);
                }

                
                return;
            }

            base.PlayEntitySound(type, dualCallByPlayer, randomizePitch, range);
        }


        public override void OnReceivedServerPacket(int packetid, byte[] data)
        {
            if (packetid == 196)
            {
                AnimManager?.StopAnimation("die");
                return;
            }

            ICoreClientAPI capi = Api as ICoreClientAPI;

            if (packetid == 1203)
            {
                string animation = SerializerUtil.Deserialize<string>(data);
                StartAnimation(animation);

                if (capi.Settings.Bool["newSeraphVoices"])
                {

                    switch (animation)
                    {
                        case "wave":
                            talkUtil?.Talk(EnumTalkType.Meet);
                            break;

                        case "nod":
                            talkUtil?.Talk(EnumTalkType.Purchase);
                            break;

                        case "rage":
                            talkUtil?.Talk(EnumTalkType.Complain);
                            break;

                        case "shrug":
                            talkUtil?.Talk(EnumTalkType.Goodbye);
                            break;

                        case "facepalm":
                            talkUtil?.Talk(EnumTalkType.IdleShort);
                            break;

                        case "laugh":
                            talkUtil?.Talk(EnumTalkType.Laugh);
                            break;
                    }
                }
            }

            if (capi.Settings.Bool["newSeraphVoices"])
            {
                if (packetid == 1001)
                {
                    if (!Alive) return;
                    talkUtil.Talk(EnumTalkType.Hurt2);
                }
                if (packetid == 1002)
                {
                    talkUtil.Talk(EnumTalkType.Death);
                }
            } else
            {
                if (packetid == 1002) base.PlayEntitySound("death", null, true, 24);
                if (packetid == 1001) base.PlayEntitySound("hurt", null, true, 24);
            }

            base.OnReceivedServerPacket(packetid, data);
        }

        public override bool ShouldReceiveDamage(DamageSource damageSource, float damage)
        {
            EnumGameMode mode = World?.PlayerByUid(PlayerUID)?.WorldData?.CurrentGameMode ?? EnumGameMode.Survival;
            if ((mode == EnumGameMode.Creative || mode == EnumGameMode.Spectator) && damageSource?.Type != EnumDamageType.Heal) return false;

            return base.ShouldReceiveDamage(damageSource, damage);
        }

        public override void Ignite()
        {
            EnumGameMode mode = World?.PlayerByUid(PlayerUID)?.WorldData?.CurrentGameMode ?? EnumGameMode.Survival;
            if (mode == EnumGameMode.Creative || mode == EnumGameMode.Spectator) return;

            base.Ignite();
        }


        public override void OnHurt(DamageSource damageSource, float damage)
        {
            if (damage > 0 && World != null && World.Side == EnumAppSide.Client && (World as IClientWorldAccessor).Player.Entity.EntityId == this.EntityId)
            {
                (World as IClientWorldAccessor).AddCameraShake(0.3f);
            }

            if (damage != 0 && World?.Side == EnumAppSide.Server)
            {
                bool heal = damageSource.Type == EnumDamageType.Heal;
                string msg;

                if (damageSource.Type == EnumDamageType.BluntAttack || damageSource.Type == EnumDamageType.PiercingAttack || damageSource.Type == EnumDamageType.SlashingAttack)
                {
                    msg = Lang.Get(heal ? "Gained {0:0.##} hp through {1}" : "Lost {0:0.##} hp through {1} (source: {2})", damage, damageSource.Type.ToString().ToLowerInvariant(), damageSource.Source);
                } else
                {
                    msg = Lang.Get(heal ? "Gained {0:0.##} hp through {1}" : "Lost {0:0.##} hp through {1}", damage, damageSource.Type.ToString().ToLowerInvariant());
                }

                if (damageSource.Source == EnumDamageSource.Player)
                {
                    EntityPlayer eplr = damageSource.SourceEntity as EntityPlayer;
                    msg = Lang.Get(heal ? "Gained {0:0.##} hp by player {1}" : "Lost {0:0.##} hp by player {1}", damage, damageSource.Source.ToString().ToLowerInvariant(), World.PlayerByUid(eplr.PlayerUID).PlayerName);
                }

                if (damageSource.Source == EnumDamageSource.Entity)
                {
                    string creatureName = Lang.Get("prefixandcreature-" + damageSource.SourceEntity.Code.Path.Replace("-", ""));
                    msg = Lang.Get(heal ? "Gained {0:0.##} hp by {1}" : "Lost {0:0.##} hp by {1} (source: {2})", damage, creatureName, damageSource.Source);
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
            }
            else
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

            foreach (InventoryBase inv in player.InventoryManager.Inventories.Values)
            {
                if (!inv.HasOpened(player)) continue;

                int q = inv.Count;
                for (int i = 0; i < q; i++)
                {
                    if (!handler(inv[i])) return;
                }
            }
        }

        /// <summary>
        /// Sets the current player.
        /// </summary>
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


       
        

        public override void TeleportToDouble(double x, double y, double z, API.Common.Action onTeleported = null)
        {
            Teleporting = true;
            ICoreServerAPI sapi = this.World.Api as ICoreServerAPI;
            if (sapi != null)
            {
                sapi.WorldManager.LoadChunkColumnPriority((int)ServerPos.X / World.BlockAccessor.ChunkSize, (int)ServerPos.Z / World.BlockAccessor.ChunkSize, new ChunkLoadOptions()
                {
                    OnLoaded = () =>
                    {
                        Pos.SetPos(x, y, z);
                        ServerPos.SetPos(x, y, z);
                        PreviousServerPos.SetPos(-99, -99, -99);
                        PositionBeforeFalling.Set(x, y, z);
                        Pos.Motion.Set(0, 0, 0);
                        if (this is EntityPlayer)
                        {
                            sapi.Network.BroadcastEntityPacket(EntityId, 1, SerializerUtil.Serialize(ServerPos.XYZ));
                            IServerPlayer player = this.Player as IServerPlayer;
                            int chunksize = World.BlockAccessor.ChunkSize;
                            player.CurrentChunkSentRadius = 0;
                            
                            sapi.Event.RegisterCallback((bla) => {
                                if (player.ConnectionState == EnumClientState.Offline) return;

                                if (!sapi.WorldManager.HasChunk((int)x / chunksize, (int)y / chunksize, (int)z / chunksize, player))
                                {
                                    sapi.WorldManager.SendChunk((int)x / chunksize, (int)y / chunksize, (int)z / chunksize, player, false);
                                }

                                // Oherwise we get an endlessly looping exception spam and break the server
                                player.CurrentChunkSentRadius = 0;

                            }, 50);

                        }

                        WatchedAttributes.SetInt("positionVersionNumber", WatchedAttributes.GetInt("positionVersionNumber", 0) + 1);


                        onTeleported?.Invoke();

                        Teleporting = false;
                    },
                });

            }
        }


        public override string GetName()
        {
            string name = GetBehavior<EntityBehaviorNameTag>()?.DisplayName;
            if (name == null) return base.GetName();
            return name;
        }

        public override string GetInfoText()
        {
            StringBuilder sb = new StringBuilder();

            if (!Alive)
            {
                sb.AppendLine(Lang.Get(Code.Domain + ":item-dead-creature-" + Code.Path));
            }
            else
            {
                sb.AppendLine(Lang.Get(Code.Domain + ":item-creature-" + Code.Path));
            }

            string charClass = WatchedAttributes.GetString("characterClass");

            if (Lang.HasTranslation("characterclass-" + charClass))
            {
                sb.AppendLine(Lang.Get("characterclass-" + charClass));
            }

            return sb.ToString();
        }

        public override void FromBytes(BinaryReader reader, bool forClient)
        {
            base.FromBytes(reader, forClient);

            lastRunningHeldUseAnimation = WatchedAttributes.GetString("lrHeldUseAnim");
            lastRunningHeldHitAnimation = WatchedAttributes.GetString("lrHeldHitAnim");
            lastRunningRightHeldIdleAnimation = WatchedAttributes.GetString("lrRightHeldIdleAnim");
        }

        public override void ToBytes(BinaryWriter writer, bool forClient)
        {
            if (lastRunningHeldUseAnimation != null)
            {
                WatchedAttributes.SetString("lrHeldUseAnim", lastRunningHeldUseAnimation);
            }
            if (lastRunningHeldHitAnimation != null)
            {
                WatchedAttributes.SetString("lrHeldHitAnim", lastRunningHeldHitAnimation);
            }
            if (lastRunningRightHeldIdleAnimation != null)
            {
                WatchedAttributes.SetString("lrRightHeldIdleAnim", lastRunningRightHeldIdleAnimation);
            }


            base.ToBytes(writer, forClient);
        }
    }
}
