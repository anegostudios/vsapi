using Vintagestory.API.Common.Entities;

#nullable disable

namespace Vintagestory.API.Common
{
    public interface IInventoryNetworkUtil
    {
        /// <summary>
        /// The core API
        /// </summary>
        ICoreAPI Api { get; set; }

        /// <summary>
        /// Gets the active slot packet.
        /// </summary>
        /// <param name="slotId">The slot ID</param>
        /// <param name="op">The operation of the slot.</param>
        object GetActivateSlotPacket(int slotId, ItemStackMoveOperation op);

        /// <summary>
        /// Flips the items between the source slot and target slot.
        /// </summary>
        /// <param name="sourceInv">The inventory.</param>
        /// <param name="sourceSlotId">The source slot ID</param>
        /// <param name="targetSlotId">The target slot ID</param>
        object GetFlipSlotsPacket(IInventory sourceInv, int sourceSlotId, int targetSlotId);

        /// <summary>
        /// Handles the client packet.
        /// </summary>
        /// <param name="byPlayer">The player the packet came from</param>
        /// <param name="packetId">the ID of the packet.</param>
        /// <param name="data">the contents of the packet.</param>
        void HandleClientPacket(IPlayer byPlayer, int packetId, byte[] data);

        /// <summary>
        /// Opens a target inventory, passing it to the player.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        object DidOpen(IPlayer player);

        /// <summary>
        /// Closes the target inventory attached to a player.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        object DidClose(IPlayer player);

        bool PauseInventoryUpdates { get; set; }
    }
}
