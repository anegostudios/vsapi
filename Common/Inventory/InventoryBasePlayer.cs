using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Abstract class used for all inventories that are "on" the player. Any inventory not inheriting from this class will not be stored to the savegame as part of the players inventory.
    /// </summary>
    public abstract class InventoryBasePlayer : InventoryBase
    {
        public override bool RemoveOnClose => false;

        protected string playerUID;

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

        public override void DropAll(Vec3d pos)
        {
            for (int i = 0; i < QuantitySlots; i++)
            {
                ItemSlot slot = GetSlot(i);

                if (slot.Itemstack != null)
                {
                    EnumHandling handling = EnumHandling.NotHandled;
                    slot.Itemstack.Collectible.OnHeldDropped(Api.World, Api.World.PlayerByUid(playerUID), slot, slot.Itemstack.StackSize, ref handling);
                    if (handling != EnumHandling.NotHandled) continue;

                    dirtySlots.Add(i);
                    Api.World.SpawnItemEntity(slot.Itemstack, pos);
                    slot.Itemstack = null;
                }
            }
        }
    }
}
