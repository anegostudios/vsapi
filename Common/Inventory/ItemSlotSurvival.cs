using Vintagestory.API.Common;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Standard survival mode slot that can hold everything except full backpacks
    /// </summary>
    public class ItemSlotSurvival : ItemSlot
    {
        public ItemSlotSurvival(InventoryBase inventory) : base(inventory)
        {
        }



        public override bool CanTakeFrom(IItemSlot sourceSlot)
        {
            if (CollectibleObject.IsBackPack(sourceSlot.Itemstack) && !CollectibleObject.IsEmptyBackPack(sourceSlot.Itemstack)) return false;
            return base.CanTakeFrom(sourceSlot);
        }

        public override bool CanHold(IItemSlot itemstackFromSourceSlot)
        {
            return 
                base.CanHold(itemstackFromSourceSlot) &&
               (!CollectibleObject.IsBackPack(itemstackFromSourceSlot.Itemstack) || CollectibleObject.IsEmptyBackPack(itemstackFromSourceSlot.Itemstack))
            ;
        }
    }
}
