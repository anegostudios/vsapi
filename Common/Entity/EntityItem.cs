using System;
using System.IO;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class EntityItem : Entity
    {
        public ItemSlot Slot = new DummySlot();
        
        public long itemSpawnedMilliseconds;

        /// <summary>
        /// The itemstack attached to this Item Entity.
        /// </summary>
        public ItemStack Itemstack
        {
            get { return WatchedAttributes.GetItemstack("itemstack"); }
            set { WatchedAttributes.SetItemstack("itemstack", value); Slot.Itemstack = value; }
        }

        /// <summary>
        /// The UID of the player that dropped this itemstack.
        /// </summary>
        public string ByPlayerUid
        {
            get { return WatchedAttributes.GetString("byPlayerUid"); }
            set { WatchedAttributes.SetString("byPlayerUid", value); }
        }

        /// <summary>
        /// Returns the material density of the item.
        /// </summary>
        public override float MaterialDensity
        {
            get { return (Slot.Itemstack?.Collectible != null) ? Slot.Itemstack.Collectible.MaterialDensity : 2000; }
        }

        /// <summary>
        /// Whether or not the EntityItem is interactable.
        /// </summary>
        public override bool IsInteractable
        {
            get { return false; }
        }

        /// <summary>
        /// Get the HSV colors for the lighting.
        /// </summary>
        public override byte[] LightHsv
        {
            get
            {
                return Slot.Itemstack?.Block?.GetLightHsv(World.BlockAccessor, null, Slot.Itemstack);
            }
        }



        public EntityItem() : base()
        {
            
        }

        public override void Initialize(EntityProperties properties, ICoreAPI api, long chunkindex3d)
        {
            base.Initialize(properties, api, chunkindex3d);

            if (Itemstack == null || !Itemstack.ResolveBlockOrItem(World))
            {
                Die();
                this.Itemstack = null;
                return;
            }

            // If attribute was modified and resent to client, make sure we still have the resolved thing in memory
            WatchedAttributes.RegisterModifiedListener("itemstack", () => {
                if (Itemstack != null && Itemstack.Collectible == null) Itemstack.ResolveBlockOrItem(World);
                Slot.Itemstack = Itemstack;
            });


            itemSpawnedMilliseconds = World.ElapsedMilliseconds;
            Swimming = FeetInLiquid = World.BlockAccessor.GetBlock(Pos.AsBlockPos).IsLiquid();
        }


        public override void OnGameTick(float dt)
        {
            base.OnGameTick(dt);

            if (Itemstack != null)
            {
                Itemstack.Collectible.OnGroundIdle(this);
            }
            else Die();
        }







        /// <summary>
        /// Builds and spawns an EntityItem from a provided ItemStack.
        /// </summary>
        /// <param name="itemstack">The contents of the EntityItem</param>
        /// <param name="position">The position of the EntityItem</param>
        /// <param name="velocity">The velocity of the EntityItem</param>
        /// <param name="world">The world the EntityItems preside in.</param>
        /// <returns>A freshly baked EntityItem to introduce to the world.</returns>
        public static EntityItem FromItemstack(ItemStack itemstack, Vec3d position, Vec3d velocity, IWorldAccessor world)
        {
            EntityItem item = new EntityItem();
            item.Code = GlobalConstants.EntityItemTypeCode;
            item.SimulationRange = 3 * world.BlockAccessor.ChunkSize;
            item.Itemstack = itemstack;

            item.ServerPos.SetPos(position);

            if (velocity == null)
            {
                velocity = new Vec3d((float)world.Rand.NextDouble() * 0.1f - 0.05f, (float)world.Rand.NextDouble() * 0.1f - 0.05f, (float)world.Rand.NextDouble() * 0.1f - 0.05f);
            }

            item.ServerPos.Motion = velocity;


            item.Pos.SetFrom(item.ServerPos);

            return item;
        }


        public override bool CanCollect(Entity byEntity)
        {
            return Alive && World.ElapsedMilliseconds - itemSpawnedMilliseconds > 1000;
        }

        public override ItemStack OnCollected(Entity byEntity)
        {
            return Slot.Itemstack;
        }

        public override void OnCollideWithLiquid()
        {
            base.OnCollideWithLiquid();
            
        }

        public override bool ShouldReceiveDamage(DamageSource damageSource, float damage)
        {
            return false;
        }



        public override void FromBytes(BinaryReader reader, bool forClient)
        {
            base.FromBytes(reader, forClient);

            if (Itemstack != null)
            {
                Slot.Itemstack = Itemstack;
            }

            if (World != null)
            {
                if (!Slot.Itemstack.ResolveBlockOrItem(World))
                {
                    this.Itemstack = null;
                    Die();
                }
            }
        }

        public override double SwimmingOffsetY
        {
            get { return base.SwimmingOffsetY; }
        }
    }
}