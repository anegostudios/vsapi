using System.IO;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

#nullable disable

namespace Vintagestory.API.Common
{
    public class EntityChunky : Entity
    {
        protected IMiniDimension blocks;

        /// <summary>
        /// Used to map chunks from load/save game and server-client packets to this specific entity.
        /// The position of saved chunks will include a reference to this index
        /// </summary>
        protected int subDimensionIndex;

        /// <summary>
        /// Whether or not the EntityChunky is interactable.
        /// </summary>
        public override bool IsInteractable
        {
            get { return false; }
        }

        public override bool ApplyGravity
        {
            get { return false; }
        }

        public EntityChunky() : base(GlobalConstants.DefaultSimulationRange)   // we call a parameterised constructor instead of the parameterless base constructor
        {
            Stats = new EntityStats(this);
            WatchedAttributes.SetAttribute("dim", new IntAttribute());
        }


        public static EntityChunky CreateAndLinkWithDimension(ICoreServerAPI sapi, IMiniDimension dimension)
        {
            EntityChunky entity = (EntityChunky)sapi.World.ClassRegistry.CreateEntity("EntityChunky");
            entity.Code = new AssetLocation("chunky");
            entity.AssociateWithDimension(dimension);
            return entity;
        }


        public void AssociateWithDimension(IMiniDimension blocks)
        {
            this.blocks = blocks;
            this.subDimensionIndex = blocks.subDimensionId;
            (WatchedAttributes.GetAttribute("dim") as IntAttribute).value = this.subDimensionIndex;
            this.ServerPos.SetFrom(blocks.CurrentPos);
            this.Pos = blocks.CurrentPos;
        }

        public override void Initialize(EntityProperties properties, ICoreAPI api, long chunkindex3d)
        {
            this.World = api.World;
            this.Api = api;
            this.Properties = properties;
            this.Class = properties.Class;
            this.InChunkIndex3d = chunkindex3d;

            // ----- The following code is a reduced version of Entity.Initialize() -----

            alive = WatchedAttributes.GetInt("entityDead", 0) == 0;

            WatchedAttributes.RegisterModifiedListener("onFire", updateOnFire);

            if (Properties.CollisionBoxSize != null || properties.SelectionBoxSize != null)
            {
                updateColSelBoxes();
            }

            DoInitialActiveCheck(api);

            this.Properties.Initialize(this, api);

            LocalEyePos.Y = Properties.EyeHeight;

            TriggerOnInitialized();

            Swimming = FeetInLiquid = World.BlockAccessor.GetBlock(Pos.AsBlockPos, BlockLayersAccess.Fluid).IsLiquid();
        }


        public override void OnGameTick(float dt)
        {
            if (blocks == null) { this.Die(EnumDespawnReason.Removed); return; }
            if (blocks.subDimensionId == 0) { Pos.Yaw = 0; Pos.Pitch = 0; Pos.Roll = 0; return; }

            if (World.Side == EnumAppSide.Client)
            {
                base.OnGameTick(dt);
            }
            else
            {
                // simplified server tick
                foreach (EntityBehavior behavior in SidedProperties.Behaviors)
                {
                    behavior.OnGameTick(dt);
                }

            }

            if (!this.Alive) return;
        }


        public override void OnReceivedServerPos(bool isTeleport)
        {
            ServerPos.SetFrom(Pos);
            //EnumHandling handled = EnumHandling.PassThrough;

            ////foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            ////{
            ////    behavior.OnReceivedServerPos(isTeleport, ref handled);
            ////    if (handled == EnumHandling.PreventSubsequent) break;
            ////}

            //if (handled == EnumHandling.PassThrough)
            //{
            //    Pos.SetFrom(ServerPos);
            //}

            // This .SetFrom introduces desync jitter in the pitch, yaw and roll for unknown reasons ... server angles generally much lower absolute values
        }


        public override void OnEntityDespawn(EntityDespawnData despawn)
        {
            if (SidedProperties == null) return;
            foreach (EntityBehavior behavior in SidedProperties.Behaviors)
            {
                behavior.OnEntityDespawn(despawn);
            }

            WatchedAttributes.OnModified.Clear();
        }

        public override void OnReceivedServerAnimations(int[] activeAnimations, int activeAnimationsCount, float[] activeAnimationSpeeds)
        {
            // no animations
        }

        public override void UpdateDebugAttributes()
        {
            // no animations
        }

        public override void StartAnimation(string code)
        {
        }

        public override void StopAnimation(string code)
        {
        }

        public override void Die(EnumDespawnReason reason = EnumDespawnReason.Death, DamageSource damageSourceForDeath = null)
        {
            if (!Alive) return;

            Alive = false;
            if (this.blocks != null)
            {
                if (Api.Side == EnumAppSide.Server)
                {
                    this.blocks.ClearChunks();
                    this.blocks.UnloadUnusedServerChunks();
                }
                else
                {
                    ((IClientWorldAccessor)this.World).SetBlocksPreviewDimension(-1);
                }
            }


            DespawnReason = new EntityDespawnData()
            {
                Reason = reason,
                DamageSourceForDeath = damageSourceForDeath
            };
        }


        public override void OnCollideWithLiquid()
        {
            base.OnCollideWithLiquid();
            
        }

        public override bool ShouldReceiveDamage(DamageSource damageSource, float damage)
        {
            return false;
        }

        public override bool ReceiveDamage(DamageSource damageSource, float damage)
        {
            return base.ReceiveDamage(damageSource, damage);
        }

        public override void FromBytes(BinaryReader reader, bool forClient)
        {
            base.FromBytes(reader, forClient);

            this.subDimensionIndex = WatchedAttributes.GetInt("dim", 1);
            if (Api is ICoreClientAPI capi)
            {
                IMiniDimension dimension = capi.World.GetOrCreateDimension(subDimensionIndex, new Vec3d(this.Pos));
                this.blocks = dimension;
                this.blocks.CurrentPos = this.Pos;  // Forces syncing; allows BehaviorInterpolatePosition to have effect
            }
        }

        public override double SwimmingOffsetY
        {
            get { return base.SwimmingOffsetY; }
        }
    }
}