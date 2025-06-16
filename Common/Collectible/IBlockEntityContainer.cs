using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    public interface IBlockEntityContainer
    {
        /// <summary>
        /// The inventory attached to this block entity container
        /// </summary>
        IInventory Inventory { get; }

        /// <summary>
        /// The class name for the inventory.
        /// </summary>
        string InventoryClassName { get; }

        /// <summary>
        /// Called by EntityBlockFalling if required
        /// </summary>
        void DropContents(Vec3d atPos);
    }
}
