
#nullable disable
namespace Vintagestory.API.Common
{
    /// <summary>
    /// A single slot not attached to a given inventory.
    /// </summary>
    public class DummySlot : ItemSlot
    {
        public DummySlot(ItemStack stack) : base(null)
        {
            this.itemstack = stack;
        }

        public DummySlot() : base(null)
        {
        }

        public DummySlot(ItemStack stack, InventoryBase inv) : base(inv)
        {
            this.itemstack = stack;
        }

        public void Set(ItemStack stack)
        {
            this.itemstack = stack;
        }
    }
}
