using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Abstract class used for all inventories that are "on" the player. Any inventory not inheriting from this class will not be stored to the savegame as part of the players inventory.
    /// </summary>
    public abstract class InventoryBasePlayer : InventoryBase, IOwnedInventory
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

        public Entity Owner => Player.Entity;

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
            var attr = Player?.Entity?.Properties.Attributes;
            int despawnSeconds = attr == null ? GlobalConstants.TimeToDespawnPlayerInventoryDrops : attr["droppedItemsOnDeathTimer"].AsInt(GlobalConstants.TimeToDespawnPlayerInventoryDrops);

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
                            spawnItemEntity(split, pos, despawnSeconds);
                        }
                    }
                    else
                    {

                        spawnItemEntity(slot.Itemstack, pos, despawnSeconds);
                    }
                    


                    slot.Itemstack = null;
                }
            }
        }

        protected void spawnItemEntity(ItemStack itemstack, Vec3d pos, int despawnSeconds)
        {
            Entity eItem = Api.World.SpawnItemEntity(itemstack, pos);
            eItem.Attributes.SetInt("minsecondsToDespawn", despawnSeconds);     // Set the despawn timer to the configured value for a player's despawned items, even if the despawn timer is different for other items
            var bh = eItem.GetBehavior("timeddespawn");   // Also set the despawn timer for the already-initialised behavior for the entity just spawned; the attribute will do the same job if the world or chunk is re-loaded
            if (bh is ITimedDespawn bhDespawn)
            {
                bhDespawn.DespawnSeconds = despawnSeconds;
            }
        }
    }
}
