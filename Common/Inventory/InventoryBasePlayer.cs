using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Abstract class used for all inventories that are "on" the player. Any inventory not inheriting from this class will not be stored to the savegame as part of the players inventory.
    /// </summary>
    public abstract class InventoryBasePlayer : InventoryBase
    {
        public override bool RemoveOnClose => false;

        /// <summary>
        /// The player ID for the inventory.
        /// </summary>
        protected string playerUID;

        /// <summary>
        /// The owning player of this inventory
        /// </summary>
        public IPlayer Player => Api.World.PlayerByUid(playerUID);


        public InventoryBasePlayer(string className, string playerUID, ICoreAPI api) : base(className, playerUID, api)
        {
            this.playerUID = playerUID;
        }

        public InventoryBasePlayer(string inventoryID, ICoreAPI api) : base(inventoryID, api)
        {
            this.playerUID = instanceID;
        }

        public override bool CanPlayerAccess(IPlayer player, EntityPos position)
        {
            return player.PlayerUID == this.playerUID;
        }

        public override bool HasOpened(IPlayer player)
        {
            return player.PlayerUID == playerUID || base.HasOpened(player);
        }

        public override void DropAll(Vec3d pos, int maxStackSize = 0)
        {
            for (int i = 0; i < Count; i++)
            {
                ItemSlot slot = this[i];

                if (slot.Itemstack != null)
                {
                    EnumHandling handling = EnumHandling.PassThrough;
                    slot.Itemstack.Collectible.OnHeldDropped(Api.World, Api.World.PlayerByUid(playerUID), slot, slot.Itemstack.StackSize, ref handling);
                    if (handling != EnumHandling.PassThrough) continue;

                    dirtySlots.Add(i);

                    if (maxStackSize > 0)
                    {
                        while (slot.StackSize > 0)
                        {
                            ItemStack split = slot.TakeOut(GameMath.Clamp(slot.StackSize, 1, maxStackSize));
                            spawnItemEntity(split, pos);
                        }
                    }
                    else
                    {

                        spawnItemEntity(slot.Itemstack, pos);
                    }
                    


                    slot.Itemstack = null;
                }
            }
        }

        private void spawnItemEntity(ItemStack itemstack, Vec3d pos)
        {
            Entity eItem = Api.World.SpawnItemEntity(itemstack, pos);

            var bh = eItem.GetBehavior("timeddespawn");   // Set the despawn timer to 10 minutes for a player's despawned items, even if the despawn timer is different for other items
            if (bh is ITimedDespawn bhDespawn) bhDespawn.SetTimer(GlobalConstants.TimeToDespawnPlayerInventoryDrops);
        }
    }
}
