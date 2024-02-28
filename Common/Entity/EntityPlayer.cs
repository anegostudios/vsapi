using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
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
        public Vec2f HeadYawLimits;

        /// <summary>
        /// Used to assist if this EntityPlayer needs to be repartitioned
        /// </summary>
        public List<Entity> entityListForPartitioning;
        /// <summary>
        /// This is not walkspeed per se, it is the walkspeed modifier as a result of armor and other gear.  It corresponds to Stats.GetBlended("walkspeed") and gets updated every tick
        /// </summary>
        public float walkSpeed = 1f;

        long lastInsideSoundTimeFinishTorso;
        long lastInsideSoundTimeFinishLegs;

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

        IAnimationManager animManager;
        IAnimationManager selfFpAnimManager;
        bool IsSelf => PlayerUID == (Api as ICoreClientAPI)?.Settings.String["playeruid"];
        public bool selfNowShadowPass;

        public override IAnimationManager AnimManager
        {
            get
            {
                var capi = Api as ICoreClientAPI;
                if (IsSelf && capi.Render.CameraType == EnumCameraMode.FirstPerson && !selfNowShadowPass)
                {
                    return selfFpAnimManager;
                }

                return animManager;
            }
            set { }
        }

        public IAnimationManager OtherAnimManager
        {
            get
            {
                var capi = Api as ICoreClientAPI;
                if (IsSelf && capi.Render.CameraType == EnumCameraMode.FirstPerson && !selfNowShadowPass)
                {
                    return animManager;
                }

                return selfFpAnimManager;
            }
        }

        public IAnimationManager TpAnimManager => animManager;

        public EntityPlayer() : base()
        {
            // For the first person mode (and just for the current client player, not other players), we have animation sets with hands that are detached from the seraph body,
            // which must only run while in first person mode and not while doing the shadow pass. 
            // To achieve this, we run two paralell animation managers and flip back and forth between the two as needed
            animManager = new PlayerAnimationManager();
            (animManager as PlayerAnimationManager).UseFpAnmations = false;
            selfFpAnimManager = new PlayerAnimationManager();

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

            if (IsSelf)
            {
                OtherAnimManager.Init(api, this);
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

            if (Player?.WorldData.CurrentGameMode == EnumGameMode.Creative)
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

            AnimationCache.ClearCache(Api, this);

            base.OnTesselation(ref entityShape, shapePathForLogging);

            AnimManager.HeadController = new PlayerHeadController(AnimManager, this, entityShape);

            if (IsSelf)
            {
                AnimationCache.InitManager(World.Api, OtherAnimManager, this, entityShape, OtherAnimManager.Animator?.RunningAnimations, "head");
                OtherAnimManager.HeadController = new PlayerHeadController(OtherAnimManager, this, entityShape);
            }
        }

        /// <summary>
        /// If not null multiplies amplitude of view bobbing caused by walking/sprinting
        /// </summary>
        public float? ViewBobbingAmplitudeMultiplier { get; set; }

        private void updateEyeHeight(float dt)
        {
            IPlayer player = World.PlayerByUid(PlayerUID);
            PrevFrameCanStandUp = true;

            if (player != null && player?.WorldData?.CurrentGameMode != EnumGameMode.Spectator)
            {
                var controls = MountedOn != null ? MountedOn.Controls : servercontrols;

                PrevFrameCanStandUp = !controls.Sneak && canStandUp();
                bool moving = (controls.TriesToMove && SidedPos.Motion.LengthSq() > 0.00001) && !controls.NoClip && !controls.DetachedMode;
                bool walking = moving && OnGround;
                double newEyeheight = Properties.EyeHeight;
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

                // Immersive fp mode has its own way of setting the eye pos
                // but we still need to run above non-ifp code for the hitbox
                if (player.ImmersiveFpMode || !Alive)
                {
                    secondsDead = Alive ? 0 : secondsDead + dt;
                    updateLocalEyePosImmersiveFpMode();
                }


                double frequency = dt * controls.MovespeedMultiplier * GetWalkSpeedMultiplier(0.3) * (controls.Sprint ? 0.9 : 1.2) * (controls.Sneak ? 1.2f : 1);

                walkCounter = walking ? walkCounter + frequency : 0;
                walkCounter = walkCounter % GameMath.TWOPI;

                double sneakDiv = (controls.Sneak ? 5 : 1.8);

                double amplitude = (FeetInLiquid ? 0.8 : 1 + (controls.Sprint ? 0.07 : 0)) / (3 * sneakDiv) * ViewBobbingAmplitudeMultiplier ?? 1.0f;
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
                    bool playingInsideSound = PlayInsideSound(player);

                    if (walking)
                    {
                        if (stepHeight > prevStepHeight)
                        {
                            if (direction == -1)
                            {
                                PlayStepSound(player, playingInsideSound);
                            }
                            direction = 1;
                        }
                        else
                        {
                            direction = -1;
                        }
                    }
                }

                prevStepHeight = stepHeight;
            }
        }

        const int overlapPercentage = 10;    // Hard-coding crime!

        public virtual Block GetInsideTorsoBlockSoundSource(BlockPos tmpPos)
        {
            // We look for a block around the player's torso height, typically leaves are at height 1 above the ground
            return GetNearestBlockSoundSource(tmpPos, +1.1, BlockLayersAccess.Solid, false);
        }

        public virtual Block GetInsideLegsBlockSoundSource(BlockPos tmpPos)
        {
            // We look for a block around the player's mid-leg height, deliberately chosen to be above the height of small ground clutter like loose stones (0.125 height)
            return GetNearestBlockSoundSource(tmpPos, +0.2, BlockLayersAccess.Solid, false);
        }

        public virtual bool PlayInsideSound(IPlayer player)
        {
            if (Swimming) return false;   // We don't play solid block's inside sounds while swimming

            BlockPos tmpPos = new BlockPos((int)Pos.X, (int)Pos.Y, (int)Pos.Z, Pos.Dimension);
            AssetLocation soundinsideTorso = GetInsideTorsoBlockSoundSource(tmpPos)?.GetSounds(Api.World.BlockAccessor, tmpPos).Inside;
            AssetLocation soundinsideLegs = GetInsideLegsBlockSoundSource(tmpPos)?.GetSounds(Api.World.BlockAccessor, tmpPos).Inside;

            bool makingSound = false;
            if (soundinsideTorso != null)
            {
                if (Api.Side == EnumAppSide.Client)
                {
                    long timeNow = Environment.TickCount;
                    if (timeNow > lastInsideSoundTimeFinishTorso)
                    {
                        float volume = controls.Sneak ? 0.25f : 1f;
                        int duration = PlaySound(player, soundinsideTorso, 12, volume, 1.4);
                        lastInsideSoundTimeFinishTorso = timeNow + duration * (100 - overlapPercentage) / 100;
                    }
                }
                makingSound = true;
            }

            if (soundinsideLegs != null && soundinsideLegs != soundinsideTorso)
            {
                if (Api.Side == EnumAppSide.Client)
                {
                    long timeNow = Environment.TickCount;
                    if (timeNow > lastInsideSoundTimeFinishLegs)
                    {
                        float volume = controls.Sneak ? 0.35f : 1f;
                        int duration = PlaySound(player, soundinsideLegs, 12, volume, 0.6);
                        lastInsideSoundTimeFinishLegs = timeNow + duration * (100 - overlapPercentage) / 100;
                    }
                }
                makingSound = true;
            }

            return makingSound;
        }

        public virtual void PlayStepSound(IPlayer player, bool playingInsideSound)
        {
            float volume = controls.Sneak ? 0.5f : 1f;

            EntityPos pos = SidedPos;
            BlockPos tmpPos = new BlockPos((int)pos.X, (int)pos.Y, (int)pos.Z, pos.Dimension);

            var soundWalkLoc = 
                GetNearestBlockSoundSource(tmpPos, -0.03, BlockLayersAccess.MostSolid, true)?.GetSounds(Api.World.BlockAccessor, tmpPos)?.Walk ??
                GetNearestBlockSoundSource(tmpPos, -0.7, BlockLayersAccess.MostSolid, true)?.GetSounds(Api.World.BlockAccessor, tmpPos)?.Walk     // When stepping stairs, seraphs is a bit floaty. And for fences we need to peek further down to find the block
            ;

            tmpPos.Set((int)pos.X, (int)(pos.Y + 0.1f), (int)pos.Z);
            var liquidblockInside = World.BlockAccessor.GetBlock(tmpPos, BlockLayersAccess.Fluid);
            AssetLocation soundinsideliquid = liquidblockInside.GetSounds(Api.World.BlockAccessor, tmpPos)?.Inside;

            if (soundinsideliquid != null)
            {
                PlaySound(player, soundinsideliquid, 12, volume, 1);
            }

            if (!Swimming && soundWalkLoc != null)
            {
                PlaySound(player, soundWalkLoc, 12, playingInsideSound ? volume * 0.5f : volume, 0);   // Need to half volume if inside sounds as well
                OnFootStep?.Invoke();
            }
        }

        private int PlaySound(IPlayer player, AssetLocation sound, int range, float volume, double yOffset)
        {
            bool isSelf = player.PlayerUID == (Api as ICoreClientAPI)?.World.Player?.PlayerUID;
            var srvplayer = player as IServerPlayer; // Don't send the sound to the current player, he plays the sound himself
            double x = 0, y = 0, z = 0;
            if (!isSelf) { x = Pos.X; y = Pos.Y + yOffset; z = Pos.Z; }

            if (Api.Side == EnumAppSide.Client) return ((IClientWorldAccessor)World).PlaySoundAtAndGetDuration(sound, x, y, z, srvplayer, true, range, volume);

            World.PlaySoundAt(sound, x, y, z, srvplayer, true, range, volume);
            return 0;
        }

        public override void OnGameTick(float dt)
        {
            // Update every tick in case this is not current with the stats value for some reason - this will then be accessed multiple times by the locomotors later in the tick
            walkSpeed = Stats.GetBlended("walkspeed");

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
                selfClimateCond = Api.World.BlockAccessor.GetClimateAt(Pos.XYZ.AsBlockPos, EnumGetClimateMode.NowValues);
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
            if (AnimManager.Animator == null) return;

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



        float strongWindAccum = 0;
        bool haveHandUseOrHit;

        protected override void HandleHandAnimations(float dt)
        {
            protectEyesFromWind(dt);

            // Prevent this method from getting called for other players on the client side because it has incomplete information (servercontrols&interact are not synced to client)
            // It's also not necessary to call this method because the server will sync the animations to the client
            if (Api is ICoreClientAPI capi && capi.World.Player.PlayerUID != PlayerUID) return;

            ItemStack rightstack = RightHandItemSlot?.Itemstack;

            EnumHandInteract interact = servercontrols.HandUse;

            PlayerAnimationManager plrAnimMngr = this.AnimManager as PlayerAnimationManager;

            bool nowUseStack = interact == EnumHandInteract.BlockInteract || interact == EnumHandInteract.HeldItemInteract || (servercontrols.RightMouseDown && !servercontrols.LeftMouseDown);
            bool wasUseStack = plrAnimMngr.IsHeldUseActive();

            bool nowHitStack = interact == EnumHandInteract.HeldItemAttack || servercontrols.LeftMouseDown;
            bool wasHitStack = plrAnimMngr.IsHeldHitActive();


            string nowHeldRightUseAnim = rightstack?.Collectible.GetHeldTpUseAnimation(RightHandItemSlot, this);
            string nowHeldRightHitAnim = rightstack?.Collectible.GetHeldTpHitAnimation(RightHandItemSlot, this);
            string nowHeldRightIdleAnim = rightstack?.Collectible.GetHeldTpIdleAnimation(RightHandItemSlot, this, EnumHand.Right);
            string nowHeldLeftIdleAnim = LeftHandItemSlot?.Itemstack?.Collectible.GetHeldTpIdleAnimation(LeftHandItemSlot, this, EnumHand.Left);
            string nowHeldRightReadyAnim = rightstack?.Collectible.GetHeldReadyAnimation(RightHandItemSlot, this, EnumHand.Right);


            bool shouldRightReadyStack = 
                haveHandUseOrHit 
                && !servercontrols.LeftMouseDown && !servercontrols.RightMouseDown 
            //    && Controls.HandUse == EnumHandInteract.None - Cannot wait until fully complete, need to start at 95% completion of the action
                && !plrAnimMngr.IsAnimationActiveOrRunning(plrAnimMngr.lastRunningHeldHitAnimation) && !plrAnimMngr.IsAnimationActiveOrRunning(plrAnimMngr.lastRunningHeldUseAnimation)
            ;
            bool isRightReadyStack = plrAnimMngr.IsRightHeldReadyActive();

            bool shouldRightIdleStack =
                nowHeldRightIdleAnim != null &&
                !nowUseStack && !nowHitStack &&
                !shouldRightReadyStack &&
                /*!isRightReadyStack &&  - why was this here?? It causes endless loops! */
                !plrAnimMngr.IsAnimationActiveOrRunning(plrAnimMngr.lastRunningHeldHitAnimation) &&
                !plrAnimMngr.IsAnimationActiveOrRunning(plrAnimMngr.lastRunningHeldUseAnimation)
            ;

            bool isRightIdleStack = plrAnimMngr.IsRightHeldActive();

            bool shouldLeftIdleStack = nowHeldLeftIdleAnim != null;
            bool isLeftIdleStack = plrAnimMngr.IsLeftHeldActive();

            if (rightstack == null)
            {
                nowHeldRightHitAnim = "breakhand";
                nowHeldRightUseAnim = "interactstatic";

                if (EntitySelection != null && EntitySelection.Entity.Pos.DistanceTo(Pos) <= 1.15)
                {
                    if (EntitySelection.Entity.SelectionBox.Y2 > 0.8) nowHeldRightUseAnim = "petlarge";
                    if (EntitySelection.Entity.SelectionBox.Y2 <= 0.8 && controls.Sneak) nowHeldRightUseAnim = "petsmall";
                    if (EntitySelection.Entity is EntityPlayer eplr && !eplr.controls.FloorSitting) nowHeldRightUseAnim = "petseraph";
                }
            }


            if (shouldRightReadyStack && !isRightReadyStack)
            {
                plrAnimMngr.StopRightHeldIdleAnim();
                plrAnimMngr.StartHeldReadyAnim(nowHeldRightReadyAnim);
                haveHandUseOrHit = false;
            }

            if (nowUseStack != wasUseStack || plrAnimMngr.HeldUseAnimChanged(nowHeldRightUseAnim))
            {
                plrAnimMngr.StopHeldUseAnim();

                if (nowUseStack)
                {
                    plrAnimMngr.StartHeldUseAnim(nowHeldRightUseAnim);
                    haveHandUseOrHit = true;

                }
            }

            if (nowHitStack != wasHitStack || plrAnimMngr.HeldHitAnimChanged(nowHeldRightHitAnim))
            {
                bool authorative = plrAnimMngr.IsHeldHitAuthorative();
                plrAnimMngr.StopHeldAttackAnim();

                if (!authorative && nowHitStack)
                {
                    plrAnimMngr.StartHeldHitAnim(nowHeldRightHitAnim);
                    haveHandUseOrHit = true;
                }
            }

            if (shouldRightIdleStack != isRightIdleStack || plrAnimMngr.RightHeldIdleChanged(nowHeldRightIdleAnim))
            {
                plrAnimMngr.StopRightHeldIdleAnim();

                if (shouldRightIdleStack)
                {
                    plrAnimMngr.StartRightHeldIdleAnim(nowHeldRightIdleAnim);
                }
            }

            if (shouldLeftIdleStack != isLeftIdleStack || plrAnimMngr.LeftHeldIdleChanged(nowHeldLeftIdleAnim))
            {
                plrAnimMngr.StopLeftHeldIdleAnim();

                if (shouldLeftIdleStack)
                {
                    plrAnimMngr.StartLeftHeldIdleAnim(nowHeldLeftIdleAnim);
                }
            }
        }

        protected void protectEyesFromWind(float dt)
        {
            if (Api?.Side == EnumAppSide.Client && AnimManager != null)
            {
                var climate = this.selfClimateCond;
                float discomfortLevelSandstorm = climate == null ? 0 : (GlobalConstants.CurrentWindSpeedClient.Length() * (1 - climate.WorldgenRainfall) * (1 - climate.Rainfall));
                float discomfortColdSnowStorm = climate == null ? 0 : (GlobalConstants.CurrentWindSpeedClient.Length() * climate.Rainfall * Math.Max(0, (1 - climate.Temperature) / 5f));
                float discomfortLevel = Math.Max(discomfortLevelSandstorm, discomfortColdSnowStorm);

                strongWindAccum = (discomfortLevel > 0.75 && !Swimming) ? strongWindAccum + dt : 0;

                float windAngle = (float)Math.Atan2(GlobalConstants.CurrentWindSpeedClient.X, GlobalConstants.CurrentWindSpeedClient.Z);
                float yawDiff = GameMath.AngleRadDistance(windAngle, Pos.Yaw - GameMath.PIHALF);
                bool lookingIntoWind = Math.Abs(yawDiff) < 45 * GameMath.DEG2RAD;
                bool isOutside = GlobalConstants.CurrentDistanceToRainfallClient < 6;

                if (isOutside && lookingIntoWind && RightHandItemSlot?.Empty == true && strongWindAccum > 2 && Player.WorldData.CurrentGameMode != EnumGameMode.Creative && !hasEyeProtectiveGear())
                {   
                    AnimManager.StartAnimation("protecteyes");
                }
                else
                {
                    if (AnimManager.IsAnimationActive("protecteyes")) AnimManager.StopAnimation("protecteyes");
                }
            }
        }

        private bool hasEyeProtectiveGear()
        {
            return GearInventory != null && GearInventory.FirstOrDefault((slot) => !slot.Empty && slot.Itemstack.Collectible.Attributes?.IsTrue("eyeprotective") == true) != null;
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

                var capi = Api as ICoreClientAPI;

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
                            capi.Network.SendEntityPacket(EntityId, (int)EntityClientPacketId.SitfloorEdge, SerializerUtil.Serialize(1));
                            BodyYaw = (float)Math.Round(BodyYaw * GameMath.RAD2DEG / 90) * 90f * GameMath.DEG2RAD;
                            BodyYawLimits = new Vec2f(BodyYaw - 0.2f, BodyYaw + 0.2f);
                            HeadYawLimits = new Vec2f(BodyYaw - GameMath.PIHALF*0.95f, BodyYaw + GameMath.PIHALF * 0.95f);
                        }
                        return true;
                    }
                    if (edgeSitActive && !canDoEdgeSit && !floorSitActive)
                    {
                        AnimManager.StopAnimation("sitidle");
                        capi.Network.SendEntityPacket(EntityId, (int)EntityClientPacketId.SitfloorEdge, SerializerUtil.Serialize(0));
                        BodyYawLimits = null;
                        HeadYawLimits = null;
                    }
                }
                else
                {
                    if (wasActive)
                    {
                        AnimManager.StopAnimation("sitidle");
                        capi.Network.SendEntityPacket(EntityId, (int)EntityClientPacketId.SitfloorEdge, SerializerUtil.Serialize(0));
                        BodyYawLimits = null;
                        HeadYawLimits = null;
                    }
                }

                return canDoEdgeSit;
            }

            return false;
        }

        protected bool canPlayEdgeSitAnim()
        {
            var bl = Api.World.BlockAccessor;
            var pos = Pos.XYZ;

            float byaw = BodyYawLimits == null ? Pos.Yaw : (BodyYawLimits.X + BodyYawLimits.Y) / 2f;
            float cosYaw = GameMath.Cos(byaw + GameMath.PI / 2);
            float sinYaw = GameMath.Sin(byaw + GameMath.PI / 2);
            var frontBelowPos = new Vec3d(Pos.X + sinYaw * 0.3f, Pos.Y - 1, Pos.Z + cosYaw * 0.3f).AsBlockPos;
            
            Block frontBelowBlock = bl.GetBlock(frontBelowPos);
            var frontBellowCollBoxes = frontBelowBlock.GetCollisionBoxes(bl, frontBelowPos);
            if (frontBellowCollBoxes == null) return true;

            double sitHeight = pos.Y - (frontBelowPos.Y + frontBellowCollBoxes.Max(box => box.Y2));

            return sitHeight >= 0.45;

            // WTF is this completely utter nonesense
            /*var frontPos = Pos.XYZ.AsBlockPos;
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

            return frontFree && frontBelowFree && backBlock.Replaceable < 6000;*/
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
                BlockPos tmpPos = new BlockPos((int)pos.X, (int)(pos.Y - 0.1f), (int)pos.Z, pos.Dimension);
                var blockUnder = GetNearestBlockSoundSource(tmpPos, -0.1, BlockLayersAccess.MostSolid, true);

                var soundWalkLoc =
                    GetNearestBlockSoundSource(tmpPos, -0.1, BlockLayersAccess.MostSolid, true)?.GetSounds(Api.World.BlockAccessor, tmpPos)?.Walk ??
                    GetNearestBlockSoundSource(tmpPos, -0.7, BlockLayersAccess.MostSolid, true)?.GetSounds(Api.World.BlockAccessor, tmpPos)?.Walk     // Stairs and Fences are a special snowflake
                ;

                if (soundWalkLoc != null && !Swimming)
                {
                    World.PlaySoundAt(soundWalkLoc, this, player, true, 12, 1.5f);
                }

                OnImpact?.Invoke(motionY);
            }

            base.OnFallToGround(motionY);
        }



        /// <summary>
        /// Returns null if there is no nearby sound source
        /// </summary>
        /// <param name="tmpPos">Might get intentionally modified if the nearest sound source the player is intersecting with is in an adjacent block</param>
        /// <param name="yOffset"></param>
        /// <param name="blockLayer"></param>
        /// <param name="usecollisionboxes"></param>
        /// <returns></returns>
        public Block GetNearestBlockSoundSource(BlockPos tmpPos, double yOffset, int blockLayer, bool usecollisionboxes)
        {
            EntityPos pos = SidedPos;
            Cuboidd entityBox = new Cuboidd();
            Cuboidf colBox = CollisionBox;
            entityBox.SetAndTranslate(colBox, pos.X, pos.Y + yOffset, pos.Z);
            entityBox.GrowBy(-0.001, 0, -0.001);   // Prevent detection when hardly inside, just touching  (movement system or rounding errors can push an entity fractionally inside a block)


            int yo = (int)(pos.Y + yOffset);
            tmpPos.Set(pos.XInt, yo, pos.ZInt);
            Block block = getSoundSourceBlockAt(entityBox, tmpPos, blockLayer, usecollisionboxes);
            if (block != null) return block;


            double xdistToBlockCenter = GameMath.Mod(pos.X, 1.0) - 0.5;
            double zdistToBlockCenter = GameMath.Mod(pos.Z, 1.0) - 0.5;

            int adjacentX = pos.XInt + Math.Sign(xdistToBlockCenter);
            int adjacentZ = pos.ZInt + Math.Sign(zdistToBlockCenter);

            // A bit of extra code to prioritise the block in x-direction if that one is closer than the block in z-direction and vice versa
            int nearerNeibX, furtherNeibX, nearerNeibZ, furtherNeibZ;
            if (Math.Abs(xdistToBlockCenter) > Math.Abs(zdistToBlockCenter))
            {
                nearerNeibX = adjacentX;
                nearerNeibZ = pos.ZInt;

                furtherNeibX = pos.XInt;
                furtherNeibZ = adjacentZ;
            } else
            {
                nearerNeibX = pos.XInt;
                nearerNeibZ = adjacentZ;

                furtherNeibX = adjacentX;
                furtherNeibZ = pos.ZInt;
            }

            return
                getSoundSourceBlockAt(entityBox, tmpPos.Set(nearerNeibX, yo, nearerNeibZ), blockLayer, usecollisionboxes) ??
                getSoundSourceBlockAt(entityBox, tmpPos.Set(furtherNeibX, yo, furtherNeibZ), blockLayer, usecollisionboxes) ??
                getSoundSourceBlockAt(entityBox, tmpPos.Set(adjacentX, yo, adjacentZ), blockLayer, usecollisionboxes)
            ;
        }


        protected Block getSoundSourceBlockAt(Cuboidd entityBox, BlockPos tmpPos, int blockLayer, bool usecollisionboxes)
        {
            Block block = World.BlockAccessor.GetBlock(tmpPos, blockLayer);
            if (!usecollisionboxes && block.GetSounds(Api.World.BlockAccessor, tmpPos)?.Inside == null) return null;

            Cuboidf[] blockBoxes = usecollisionboxes ? block.GetCollisionBoxes(World.BlockAccessor, tmpPos) : block.GetSelectionBoxes(World.BlockAccessor, tmpPos);
            if (blockBoxes == null) return null;

            for (int i = 0; i < blockBoxes.Length; i++)
            {
                Cuboidf blockBox = blockBoxes[i];
                if (blockBox != null && entityBox.Intersects(blockBox, tmpPos.X, tmpPos.Y, tmpPos.Z))
                {
                    return block;
                }
            }

            return null;
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

            (Api as ICoreServerAPI).Network.SendEntityPacket(Api.World.PlayerByUid(PlayerUID) as IServerPlayer, this.EntityId, (int)EntityServerPacketId.Revive);
        }


        public override void PlayEntitySound(string type, IPlayer dualCallByPlayer = null, bool randomizePitch = true, float range = 24)
        {
            if (type == "hurt")
            {
                talkUtil?.Talk(EnumTalkType.Hurt2);
                return;
            }
            if (type == "death")
            {
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
            if (packetid == (int)EntityServerPacketId.Revive)
            {
                AnimManager?.StopAnimation("die");
                return;
            }

            if (packetid == (int)EntityServerPacketId.Emote)
            {
                string animation = SerializerUtil.Deserialize<string>(data);
                StartAnimation(animation);

                if (talkTypeByAnimation.TryGetValue(animation, out var talktype))
                {
                    talkUtil?.Talk(talktype);
                }
            }

            if (packetid == (int)EntityServerPacketId.Death)
            {
                TryStopHandAction(true, EnumItemUseCancelReason.Death);
            }
            
            if (packetid == EntityTalkUtil.TalkPacketId)
            {
                var tt = SerializerUtil.Deserialize<EnumTalkType>(data);

                if (tt != EnumTalkType.Death && !Alive) return;

                talkUtil.Talk(tt);
            }

            if (packetid == (int)EntityServerPacketId.PlayPlayerAnim)
            {
                AnimManager.StartAnimation(SerializerUtil.Deserialize<string>(data));
            }

            base.OnReceivedServerPacket(packetid, data);
    	}
        public override void OnReceivedClientPacket(IServerPlayer player, int packetid, byte[] data)
        {
            base.OnReceivedClientPacket(player, packetid, data);

            if (packetid == (int)EntityClientPacketId.SitfloorEdge)
            {
                bool on = SerializerUtil.Deserialize<int>(data) > 0;
                if (on)
                {
                    AnimManager.StopAnimation("sitidle");
                    AnimManager.StartAnimation("sitflooredge");
                } else
                {
                    AnimManager.StopAnimation("sitflooredge");
                }
            }
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
                    EntityPlayer eplr = damageSource.GetCauseEntity() as EntityPlayer;
                    msg = Lang.Get(heal ? "damagelog-heal-byplayer" : "damagelog-damage-byplayer", damage, World.PlayerByUid(eplr.PlayerUID).PlayerName);
                }

                if (damageSource.Source == EnumDamageSource.Entity)
                {
                    string creatureName = Lang.Get("prefixandcreature-" + damageSource.GetCauseEntity().Code.Path.Replace("-", ""));
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

            foreach (InventoryBase inv in player.InventoryManager.InventoriesOrdered)
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
                sapi.WorldManager.LoadChunkColumnPriority((int)ServerPos.X / GlobalConstants.ChunkSize, (int)ServerPos.Z / GlobalConstants.ChunkSize, new ChunkLoadOptions()
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
                            int chunksize = GlobalConstants.ChunkSize;
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

            walkSpeed = Stats.GetBlended("walkspeed");
        }

        public override void UpdateDebugAttributes()
        {
            base.UpdateDebugAttributes();

            string anims = "";
            int i = 0;
            foreach (string anim in OtherAnimManager.ActiveAnimationsByAnimCode.Keys)
            {
                if (i++ > 0) anims += ",";
                anims += anim;
            }

            i = 0;
            StringBuilder runninganims = new StringBuilder();
            if (OtherAnimManager.Animator != null)
            {
                foreach (var anim in OtherAnimManager.Animator.RunningAnimations)
                {
                    if (!anim.Active) continue;

                    if (i++ > 0) runninganims.Append(",");
                    runninganims.Append(anim.Animation.Code);
                }

                DebugAttributes.SetString("Other Active Animations", anims.Length > 0 ? anims : "-");
                DebugAttributes.SetString("Other Running Animations", runninganims.Length > 0 ? runninganims.ToString() : "-");
            }
        }

    }
}
