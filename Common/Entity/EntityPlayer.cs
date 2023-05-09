using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace Vintagestory.API.Common
{
    public delegate bool CanSpawnNearbyDelegate(EntityProperties type, Vec3d spawnPosition, RuntimeSpawnConditions sc);

    public class PlayerAnimationManager : AnimationManager
    {
        public override bool StartAnimation(AnimationMetaData animdata)
        {
            if (api.Side == EnumAppSide.Client && capi.World.Player.ImmersiveFpMode && !animdata.Code.EndsWith("-ifp"))
            {
                if (entity.Properties.Client.AnimationsByMetaCode.TryGetValue(animdata.Code + "-ifp", out var animdatafp))
                {
                    if (ActiveAnimationsByAnimCode.TryGetValue(animdatafp.Animation, out var activeAnimdata) && activeAnimdata == animdatafp) return false;
                    return base.StartAnimation(animdatafp);
                }
            }

            return base.StartAnimation(animdata);
        }

        public override void StopAnimation(string code)
        {
            base.StopAnimation(code);
            base.StopAnimation(code + "-ifp");
        }

        public override bool IsAnimationActive(params string[] anims)
        {
            if (api.Side == EnumAppSide.Client && capi.World.Player.ImmersiveFpMode)
            {
                foreach (var val in anims)
                {
                    if (ActiveAnimationsByAnimCode.ContainsKey(val + "-ifp")) return true;
                }
            }

            return base.IsAnimationActive(anims);
        }

        public override void OnReceivedServerAnimations(int[] activeAnimations, int activeAnimationsCount, float[] activeAnimationSpeeds)
        {
            base.OnReceivedServerAnimations(activeAnimations, activeAnimationsCount, activeAnimationSpeeds);
        }
    }

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
        public event Action OnFootStep;

        /// <summary>
        /// Called when the player falls onto the ground. Called by the game client and server.
        /// </summary>
        public event Action<double> OnImpact;

        /// <summary>
        /// Called whenever the game wants to spawn new creatures around the player. Called only by the game server.
        /// </summary>
        public CanSpawnNearbyDelegate OnCanSpawnNearby;

        public EntityTalkUtil talkUtil;
        public Vec2f BodyYawLimits;

        /// <summary>
        /// Used to assist if this EntityPlayer needs to be repartitioned
        /// </summary>
        public List<Entity> entityListForPartitioning;
        /// <summary>
        /// This is not walkspeed per se, it is the walkspeed modifier as a result of armor and other gear.  It corresponds to Stats.GetBlended("walkspeed") and gets updated every tick
        /// </summary>
        public float walkSpeed = 1f;

        string[] randomIdleAnimations;

        public override float BodyYaw {
            get
            {
                return base.BodyYaw;
            }
            set {
                if (BodyYawLimits != null)
                {
                    base.BodyYaw = GameMath.Clamp(value, BodyYawLimits.X, BodyYawLimits.Y);
                } else
                {
                    base.BodyYaw = value;
                }
                
            }
        }

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

        public void UpdatePartitioning()
        {
            var partitionUtil = Api.ModLoader.GetModSystem("Vintagestory.GameContent.EntityPartitioning") as IEntityPartitioning;
            partitionUtil?.RePartitionPlayer(this);
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
                if (Player?.WorldData.CurrentGameMode == EnumGameMode.Spectator) return null;

                byte[] rightHsv = RightHandItemSlot?.Itemstack?.Collectible?.GetLightHsv(World.BlockAccessor, null, RightHandItemSlot.Itemstack);
                byte[] leftHsv = LeftHandItemSlot?.Itemstack?.Collectible?.GetLightHsv(World.BlockAccessor, null, LeftHandItemSlot.Itemstack);

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

        public override double LadderFixDelta { get { return Properties.SpawnCollisionBox.Y2 - SelectionBox.YSize; } }

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
            AnimManager = new PlayerAnimationManager();

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
                .Register("armorWalkSpeedAffectedness")
                .Register("bowDrawingStrength")
                .Register("wholeVesselLootChance", EnumStatBlendType.FlatSum)
                .Register("temporalGearTLRepairCost", EnumStatBlendType.FlatSum)
                .Register("animalHarvestingTime")
            ;

        }

        public override void Initialize(EntityProperties properties, ICoreAPI api, long chunkindex3d)
        {
            controls.StopAllMovement();

            talkUtil = new EntityTalkUtil(api, this);

            if (api.Side == EnumAppSide.Client)
            {
                talkUtil.soundName = new AssetLocation("sounds/voice/altoflute");
                talkUtil.idleTalkChance = 0f;
                talkUtil.AddSoundLengthChordDelay = true;
                talkUtil.volumneModifier = 0.8f;
            }

            base.Initialize(properties, api, chunkindex3d);

            if (api.Side == EnumAppSide.Server && !WatchedAttributes.attributes.ContainsKey("lastReviveTotalHours"))
            {
                double hrs = Api.World.Calendar.GetDayLightStrength(ServerPos.X, ServerPos.Z) < 0.5 ? Api.World.Calendar.TotalHours : -9999;
                WatchedAttributes.SetDouble("lastReviveTotalHours", hrs);
            }

            if (Api.Side == EnumAppSide.Client)
            {
                AnimManager.HeadController = new PlayerHeadController(AnimManager, this, Properties.Client.LoadedShapeForEntity);
            }

            randomIdleAnimations = properties.Attributes["randomIdleAnimations"].AsArray<string>(null);
        }


        double walkCounter;
        double prevStepHeight;
        int direction = 0;

        public bool PrevFrameCanStandUp;
        public ClimateCondition selfClimateCond;
        float climateCondAccum;
        float secondsIdleAccum;

        public override double GetWalkSpeedMultiplier(double groundDragFactor = 0.3)
        {
            double mul = base.GetWalkSpeedMultiplier(groundDragFactor);

            if (Player.WorldData.CurrentGameMode == EnumGameMode.Creative)
            {
                // For Creative mode players, revert the normal walkspeed modifier from the block the entity is currently standing on/in
                int y1 = (int)(SidedPos.Y - 0.05f);
                int y2 = (int)(SidedPos.Y + 0.01f);
                Block belowBlock = World.BlockAccessor.GetBlock((int)SidedPos.X, y1, (int)SidedPos.Z);
                mul /= belowBlock.WalkSpeedMultiplier * (y1 == y2 ? 1 : insideBlock.WalkSpeedMultiplier);
            }

            // Apply walk speed modifiers from armor, etc
            mul *= GameMath.Clamp(walkSpeed, 0, 999);


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
                var controls = MountedOn != null ? MountedOn.Controls : servercontrols;

                PrevFrameCanStandUp = !controls.Sneak && canStandUp();
                bool moving = (controls.TriesToMove && SidedPos.Motion.LengthSq() > 0.00001) && !controls.NoClip && !controls.DetachedMode && OnGround;
                double newEyeheight = Properties.EyeHeight;

                if (player.ImmersiveFpMode)
                {
                    secondsDead = Alive ? 0 : secondsDead + dt;

                    updateLocalEyePosImmersiveFpMode();

                    newEyeheight = LocalEyePos.Y;
                }
                else
                {
                    double newModelHeight = Properties.CollisionBoxSize.Y;

                    if (controls.FloorSitting)
                    {
                        newEyeheight *= 0.5f;
                        newModelHeight *= 0.55f;
                    }
                    else if ((controls.Sneak || !PrevFrameCanStandUp) && !controls.IsClimbing && !controls.IsFlying)
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

                    diff = (newModelHeight - OriginSelectionBox.Y2) * 5 * dt;
                    OriginSelectionBox.Y2 = SelectionBox.Y2 = (float)(diff > 0 ? Math.Min(SelectionBox.Y2 + diff, newModelHeight) : Math.Max(SelectionBox.Y2 + diff, newModelHeight));

                    diff = (newModelHeight - OriginCollisionBox.Y2) * 5 * dt;
                    OriginCollisionBox.Y2 = CollisionBox.Y2 = (float)(diff > 0 ? Math.Min(CollisionBox.Y2 + diff, newModelHeight) : Math.Max(CollisionBox.Y2 + diff, newModelHeight));

                    LocalEyePos.X = 0;
                    LocalEyePos.Z = 0;

                    if (MountedOn?.LocalEyePos != null)
                    {
                        LocalEyePos.Set(MountedOn.LocalEyePos);
                    }
                }



                double frequency = dt * controls.MovespeedMultiplier * GetWalkSpeedMultiplier(0.3) * (controls.Sprint ? 0.9 : 1.2) * (controls.Sneak ? 1.2f : 1);

                walkCounter = moving ? walkCounter + frequency : 0;
                walkCounter = walkCounter % GameMath.TWOPI;

                double sneakDiv = (controls.Sneak ? 3 : 1.8);

                double amplitude = (FeetInLiquid ? 0.8 : 1 + (controls.Sprint ? 0.07 : 0)) / (3 * sneakDiv);
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
                            var blockUnder = BlockUnderPlayer(pos);
                            var blockInside = BlockInsidePlayer(pos);

                            AssetLocation soundwalk = blockUnder.GetSounds(Api.World.BlockAccessor, new BlockPos((int)pos.X, (int)(pos.Y - 0.1f), (int)pos.Z))?.Walk;
                            AssetLocation soundinside = blockInside.GetSounds(Api.World.BlockAccessor, new BlockPos((int)pos.X, (int)(pos.Y + 0.1f), (int)pos.Z))?.Inside;

                            bool isSelf = player.PlayerUID == (Api as ICoreClientAPI)?.World.Player?.PlayerUID;

                            if (!Swimming && soundwalk != null)
                            {
                                if (blockInside.Id != blockUnder.Id && soundinside != null)
                                {
                                    if (isSelf)
                                    {
                                        World.PlaySoundAt(soundwalk, 0, 0, 0, null, true, 12, volume * 0.5f);
                                        World.PlaySoundAt(soundinside, 0, 0, 0, null, true, 12, volume);
                                    } else
                                    {
                                        World.PlaySoundAt(soundwalk, this, player, true, 12, volume * 0.5f);
                                        World.PlaySoundAt(soundinside, this, player, true, 12, volume);
                                    }
                                    
                                }
                                else
                                {
                                    if (isSelf)
                                    {
                                        World.PlaySoundAt(soundwalk, 0, 0, 0, null, true, 12, volume); 
                                    } else
                                    {
                                        World.PlaySoundAt(soundwalk, this, player, true, 12, volume);
                                    }
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
            walkSpeed = Stats.GetBlended("walkspeed");   // update every tick in case this is not current with the stats value for some reason - this will then be accessed multiple times by the locomotors later in the tick

            if (World.Side == EnumAppSide.Client)
            {
                talkUtil.OnGameTick(dt);
            }

            bool isSelf = (Api as ICoreClientAPI)?.World.Player.PlayerUID == PlayerUID;

            if (Api.Side == EnumAppSide.Server || !isSelf)
            {
                updateEyeHeight(dt);
            }

            climateCondAccum += dt;
            if (isSelf && World.Side == EnumAppSide.Client && climateCondAccum > 0.5f)
            {
                climateCondAccum = 0;
                selfClimateCond = Api.World.BlockAccessor.GetClimateAt(Pos.XYZ.AsBlockPos, EnumGetClimateMode.WorldGenValues);
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

            if (!servercontrols.TriesToMove && !controls.IsFlying && !controls.Gliding && RightHandItemSlot?.Empty == true)
            {
                secondsIdleAccum += dt;
                if (secondsIdleAccum > 20 && World.Rand.NextDouble() < 0.004)
                {
                    StartAnimation(randomIdleAnimations[World.Rand.Next(randomIdleAnimations.Length)]);
                    secondsIdleAccum = 0;
                }
            }
            else secondsIdleAccum = 0;
        }

        public override void OnAsyncParticleTick(float dt, IAsyncParticleManager manager)
        {
            base.OnAsyncParticleTick(dt, manager);

            bool isSelf = (Api as ICoreClientAPI).World.Player.Entity.EntityId == EntityId;
            EntityPos herepos = isSelf ? Pos : ServerPos;
            bool moving = herepos.Motion.LengthSq() > 0.00001 && !servercontrols.NoClip;
            if ((FeetInLiquid || Swimming) && moving && Properties.Habitat != EnumHabitat.Underwater)
            {
                SpawnFloatingSediment(manager);
            }
        }


        Cuboidf tmpCollBox = new Cuboidf();
        bool holdPosition = false;
        
        float[] prevAnimModelMatrix;
        public float sidewaysSwivelAngle;

        float secondsDead;

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
            if (!Alive) lookOffset /= secondsDead * 10;

            bool wasHoldPos = holdPosition;
            holdPosition = false;
            
            for (int i = 0; i < AnimManager.Animator.RunningAnimations.Length; i++)
            {
                RunningAnimation anim = AnimManager.Animator.RunningAnimations[i];
                if (anim.Running && anim.EasingFactor >= anim.meta.HoldEyePosAfterEasein)
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
                .Scale(Properties.Client.Size, Properties.Client.Size, Properties.Client.Size)
                .Translate(-0.5f, 0, -0.5f)
                .RotateX(sidewaysSwivelAngle)
                .Translate(ap.PosX / 16f - lookOffset * 1.3f, ap.PosY / 16f, ap.PosZ / 16f)
                .Mul(holdPosition ? prevAnimModelMatrix : apap.AnimModelMatrix)
                .Translate(0.07f, Alive ? 0.0f : 0.2f * Math.Min(1, secondsDead), 0f)
            ;

            float[] pos = new float[4] { 0, 0, 0, 1 };
            float[] endVec = Mat4f.MulWithVec4(tmpModelMat.Values, pos);

            LocalEyePos.Set(endVec[0], endVec[1], endVec[2]);
        }



        protected string lastRunningHeldUseAnimation;
        protected string lastRunningRightHeldIdleAnimation;
        protected string lastRunningLeftHeldIdleAnimation;
        protected string lastRunningHeldHitAnimation;
        float strongWindAccum = 0;

        protected override void HandleHandAnimations(float dt)
        {
            protectedEyesFromWind(dt);

            // Prevent this method from getting called for other players on the client side because it has incomplete information (servercontrols&interact are not synced to client)
            // It's also not necessary to call this method because the server will sync the animations to the client
            if (Api is ICoreClientAPI capi && capi.World.Player.PlayerUID != PlayerUID) return;

            ItemStack rightstack = RightHandItemSlot?.Itemstack;

            EnumHandInteract interact = servercontrols.HandUse;

            bool nowUseStack = (interact == EnumHandInteract.BlockInteract || interact == EnumHandInteract.HeldItemInteract) || (servercontrols.RightMouseDown && !servercontrols.LeftMouseDown);
            bool wasUseStack = lastRunningHeldUseAnimation != null && AnimManager.IsAnimationActive(lastRunningHeldUseAnimation);

            bool nowHitStack = interact == EnumHandInteract.HeldItemAttack || (servercontrols.LeftMouseDown);
            bool wasHitStack = lastRunningHeldHitAnimation != null && AnimManager.IsAnimationActive(lastRunningHeldHitAnimation);


            string nowHeldRightUseAnim = rightstack?.Collectible.GetHeldTpUseAnimation(RightHandItemSlot, this);
            string nowHeldRightHitAnim = rightstack?.Collectible.GetHeldTpHitAnimation(RightHandItemSlot, this);
            string nowHeldRightIdleAnim = rightstack?.Collectible.GetHeldTpIdleAnimation(RightHandItemSlot, this, EnumHand.Right);
            string nowHeldLeftIdleAnim = LeftHandItemSlot?.Itemstack?.Collectible.GetHeldTpIdleAnimation(LeftHandItemSlot, this, EnumHand.Left);

            bool nowRightIdleStack = nowHeldRightIdleAnim != null && !nowUseStack && !nowHitStack;
            bool wasRightIdleStack = lastRunningRightHeldIdleAnimation != null && AnimManager.IsAnimationActive(lastRunningRightHeldIdleAnimation);

            bool nowLeftIdleStack = nowHeldLeftIdleAnim != null;
            bool wasLeftIdleStack = lastRunningLeftHeldIdleAnimation != null && AnimManager.IsAnimationActive(lastRunningLeftHeldIdleAnimation);

            if (rightstack == null)
            {
                nowHeldRightHitAnim = "breakhand";
                nowHeldRightUseAnim = "interactstatic";

                if (EntitySelection != null && EntitySelection.Entity.Pos.DistanceTo(Pos) <= 1.15)
                {
                    if (EntitySelection.Entity.SelectionBox.Y2 > 0.8) nowHeldRightUseAnim = "petlarge";
                    if (EntitySelection.Entity.SelectionBox.Y2 <= 0.8 && controls.Sneak) nowHeldRightUseAnim = "petsmall";
                }
            }

            if (nowUseStack != wasUseStack || (lastRunningHeldUseAnimation != null && nowHeldRightUseAnim != lastRunningHeldUseAnimation))
            {
                AnimManager.StopAnimation(lastRunningHeldUseAnimation);
                lastRunningHeldUseAnimation = null;

                if (nowUseStack)
                {
                    AnimManager.StopAnimation(lastRunningRightHeldIdleAnimation);
                    AnimManager.StartAnimation(lastRunningHeldUseAnimation = nowHeldRightUseAnim);
                }
            }

            if (nowHitStack != wasHitStack || (lastRunningHeldHitAnimation != null && nowHeldRightHitAnim != lastRunningHeldHitAnimation))
            {
                AnimManager.StopAnimation(lastRunningHeldHitAnimation);
                lastRunningHeldHitAnimation = null;


                if (nowHitStack)
                {
                    AnimManager.StopAnimation(lastRunningLeftHeldIdleAnimation);
                    AnimManager.StopAnimation(lastRunningRightHeldIdleAnimation);
                    AnimManager.StartAnimation(lastRunningHeldHitAnimation = nowHeldRightHitAnim);
                }
            }

            if (nowRightIdleStack != wasRightIdleStack || (lastRunningRightHeldIdleAnimation != null && nowHeldRightIdleAnim != lastRunningRightHeldIdleAnimation))
            {
                AnimManager.StopAnimation(lastRunningRightHeldIdleAnimation);
                lastRunningRightHeldIdleAnimation = null;

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

        protected void protectedEyesFromWind(float dt)
        {
            if (Api?.Side == EnumAppSide.Client && AnimManager != null)
            {
                strongWindAccum = (GlobalConstants.CurrentWindSpeedClient.Length() > 0.85 && !Swimming) ? strongWindAccum + dt : 0;

                float windAngle = (float)Math.Atan2(GlobalConstants.CurrentWindSpeedClient.X, GlobalConstants.CurrentWindSpeedClient.Z);
                float yawDiff = GameMath.AngleRadDistance(windAngle, Pos.Yaw - GameMath.PIHALF);
                bool lookingIntoWind = Math.Abs(yawDiff) < 45 * GameMath.DEG2RAD;
                bool isOutside = GlobalConstants.CurrentDistanceToRainfallClient < 6;

                if (isOutside && lookingIntoWind && RightHandItemSlot?.Empty == true && strongWindAccum > 2)
                {
                    AnimManager.StartAnimation("protecteyes");
                }
                else
                {
                    AnimManager.StopAnimation("protecteyes");
                }
            }
        }

        private bool canStandUp()
        {
            tmpCollBox.Set(SelectionBox);

            bool collideSneaking = World.CollisionTester.IsColliding(World.BlockAccessor, tmpCollBox, Pos.XYZ, false);

            tmpCollBox.Y2 = Properties.CollisionBoxSize.Y;
            tmpCollBox.Y1 += 1f; // Don't care about the bottom block
            bool collideStanding = World.CollisionTester.IsColliding(World.BlockAccessor, tmpCollBox, Pos.XYZ, false);
            
            return !collideStanding || collideSneaking;
        }

        protected override bool onAnimControls(AnimationMetaData anim, bool wasActive, bool nowActive)
        {
            if (anim.TriggeredBy?.MatchExact == true && anim.Animation == "sitflooridle")
            {
                bool canDoEdgeSit = canPlayEdgeSitAnim();
                bool edgeSitActive = this.AnimManager.IsAnimationActive("sitidle");
                wasActive |= edgeSitActive;

                if (nowActive)
                {
                    bool floorSitActive = this.AnimManager.IsAnimationActive(anim.Code);
                    
                    if (canDoEdgeSit)
                    {
                        if (floorSitActive)
                        {
                            AnimManager.StopAnimation(anim.Animation);
                        }
                        if (!edgeSitActive)
                        {
                            AnimManager.StartAnimation("sitflooredge");

                            BodyYaw = (float)Math.Round(BodyYaw * GameMath.RAD2DEG / 90) * 90f * GameMath.DEG2RAD;
                            BodyYawLimits = new Vec2f(BodyYaw - 0.2f, BodyYaw + 0.2f);
                        }
                        return true;
                    }
                    if (edgeSitActive && !canDoEdgeSit && !floorSitActive)
                    {
                        AnimManager.StopAnimation("sitidle");
                        BodyYawLimits = null;
                    }
                }
                else
                {
                    if (wasActive)
                    {
                        AnimManager.StopAnimation("sitidle");
                        BodyYawLimits = null;
                    }
                }

                return canDoEdgeSit;
            }

            return false;
        }

        protected bool canPlayEdgeSitAnim()
        {
            var frontPos = Pos.XYZ.AsBlockPos;
            frontPos.Y = (int)Math.Ceiling(Pos.Y);

            float byaw = BodyYawLimits == null ? Pos.Yaw : (BodyYawLimits.X + BodyYawLimits.Y) / 2f;
            float cosYaw = GameMath.Cos(byaw + GameMath.PI / 2);
            float sinYaw = GameMath.Sin(byaw + GameMath.PI / 2);
            var backPos = new Vec3d(Pos.X + sinYaw * -0.3f, Pos.Y - 1, Pos.Z + cosYaw * -0.3f).AsBlockPos;
            
            var frontBelowPos = frontPos.AddCopy(0, -1, 0);

            Block frontBlock = Api.World.BlockAccessor.GetBlock(frontPos);
            Block backBlock = Api.World.BlockAccessor.GetBlock(backPos);
            Block frontBelowBlock = Api.World.BlockAccessor.GetBlock(frontBelowPos);

            var face = BlockFacing.FromNormal(new Vec3i(backPos.X - frontPos.X, 0, backPos.Z - frontPos.Z));

            var frontFree = face != null && !frontBlock.CanAttachBlockAt(Api.World.BlockAccessor, frontBlock, frontPos, face);
            var frontBelowFree = frontBelowBlock.GetCollisionBoxes(Api.World.BlockAccessor, frontBelowPos)?.FirstOrDefault(c => c.Y2 > 0.5) == null;

            return /*CollidedVertically && - always false for other players o.o */
                frontFree && frontBelowFree && backBlock.Replaceable < 6000;
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

            if (player?.WorldData?.CurrentGameMode != EnumGameMode.Spectator && motionY < -0.1)
            {
                EntityPos pos = SidedPos;
                var blockUnder = BlockUnderPlayer(pos);
                AssetLocation soundwalk = blockUnder.GetSounds(Api.World.BlockAccessor, new BlockPos((int)pos.X, (int)(pos.Y - 0.1f), (int)pos.Z))?.Walk;
                if (soundwalk != null && !Swimming)
                {
                    World.PlaySoundAt(soundwalk, this, player, true, 12, 1.5f);
                }

                OnImpact?.Invoke(motionY);
            }

            base.OnFallToGround(motionY);
        }



        internal Block BlockUnderPlayer(EntityPos pos)
        {
            return World.BlockAccessor.GetBlock(
                (int)pos.X,
                (int)(pos.Y - 0.1f),
                (int)pos.Z);
        }

        internal Block BlockInsidePlayer(EntityPos pos)
        {
            return World.BlockAccessor.GetBlock(
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
            WatchedAttributes.SetFloat("intoxication", 0);

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

        public override bool TryMount(IMountable onmount)
        {
            bool ok = base.TryMount(onmount);
            if (ok && Alive && Player != null)
            {
                Player.WorldData.FreeMove = false;
            }
            return ok;
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
                if (World.Side == EnumAppSide.Client && !capi.Settings.Bool["newSeraphVoices"]) { base.PlayEntitySound(type, dualCallByPlayer, randomizePitch, range); return; }
                talkUtil?.Talk(EnumTalkType.Hurt2);
                return;
            }
            if (type == "death")
            {
                if (World.Side == EnumAppSide.Client && !capi.Settings.Bool["newSeraphVoices"]) { base.PlayEntitySound(type, dualCallByPlayer, randomizePitch, range); return; }
                talkUtil?.Talk(EnumTalkType.Death);
                return;
            }

            base.PlayEntitySound(type, dualCallByPlayer, randomizePitch, range);
        }

        protected static Dictionary<string, EnumTalkType> talkTypeByAnimation = new Dictionary<string, EnumTalkType>()
        {
            { "wave", EnumTalkType.Meet },
            { "nod", EnumTalkType.Purchase },
            { "rage", EnumTalkType.Complain },
            { "shrug", EnumTalkType.Shrug },
            { "facepalm", EnumTalkType.IdleShort },
            { "laugh", EnumTalkType.Laugh },
        };

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

                if (capi.Settings.Bool["newSeraphVoices"] && talkTypeByAnimation.TryGetValue(animation, out var talktype))
                {
                    talkUtil?.Talk(talktype);
                }
            }

            if (packetid == 1002)
            {
                TryStopHandAction(true, EnumItemUseCancelReason.Death);
            }

            if (capi.Settings.Bool["newSeraphVoices"])
            {
                if (packetid == EntityTalkUtil.TalkPacketId)
                {
                    var tt = SerializerUtil.Deserialize<EnumTalkType>(data);

                    if (tt != EnumTalkType.Death && !Alive) return;

                    talkUtil.Talk(tt);
                }

                /*if (packetid == 1001)
                {
                    if (!Alive) return;
                    talkUtil.Talk(EnumTalkType.Hurt2);
                }
                if (packetid == 1002)
                {
                    talkUtil.Talk(EnumTalkType.Death);
                }*/
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
                    msg = Lang.Get(heal ? "damagelog-heal-attack" : "damagelog-damage-attack", damage, damageSource.Type.ToString().ToLowerInvariant(), damageSource.Source);
                } else
                {
                    msg = Lang.Get(heal ? "damagelog-heal" : "damagelog-damage", damage, damageSource.Type.ToString().ToLowerInvariant());
                }

                if (damageSource.Source == EnumDamageSource.Player)
                {
                    EntityPlayer eplr = damageSource.SourceEntity as EntityPlayer;
                    msg = Lang.Get(heal ? "damagelog-heal-byplayer" : "damagelog-damage-byplayer", damage, World.PlayerByUid(eplr.PlayerUID).PlayerName);
                }

                if (damageSource.Source == EnumDamageSource.Entity)
                {
                    string creatureName = Lang.Get("prefixandcreature-" + damageSource.SourceEntity.Code.Path.Replace("-", ""));
                    msg = Lang.Get(heal ? "damagelog-heal-byentity" : "damagelog-damage-byentity", damage, creatureName);
                }

                (World.PlayerByUid(PlayerUID) as IServerPlayer).SendMessage(GlobalConstants.DamageLogChatGroup, msg, EnumChatType.Notification);
            }
        }



        public override bool TryStopHandAction(bool forceStop, EnumItemUseCancelReason cancelReason = EnumItemUseCancelReason.ReleasedMouse)
        {
            if (controls.HandUse == EnumHandInteract.None || RightHandItemSlot?.Itemstack == null) return true;

            IPlayer player = World.PlayerByUid(PlayerUID);
            float secondsPassed = (World.ElapsedMilliseconds - controls.UsingBeginMS) / 1000f;

            controls.HandUse = RightHandItemSlot.Itemstack.Collectible.OnHeldUseCancel(secondsPassed, RightHandItemSlot, this, player.CurrentBlockSelection, player.CurrentEntitySelection, cancelReason);

            if (forceStop)
            {
                controls.HandUse = EnumHandInteract.None;
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
                if (inv.ClassName == "creative") continue;
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




        public override void TeleportToDouble(double x, double y, double z, Action onTeleported = null)
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

            walkSpeed = Stats.GetBlended("walkspeed");
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
