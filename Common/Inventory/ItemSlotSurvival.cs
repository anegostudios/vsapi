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



        public override bool CanTakeFrom(ItemSlot sourceSlot, EnumMergePriority priority = EnumMergePriority.AutoMerge)
        {
            if (CollectibleObject.IsBackPack(sourceSlot.Itemstack) && !CollectibleObject.IsEmptyBackPack(sourceSlot.Itemstack)) return false;
            return base.CanTakeFrom(sourceSlot, priority);
        }

        public override bool CanHold(ItemSlot sourceSlot)
        {
            return 
                base.CanHold(sourceSlot) &&
               (!CollectibleObject.IsBackPack(sourceSlot.Itemstack) || CollectibleObject.IsEmptyBackPack(sourceSlot.Itemstack))
               && inventory.CanContain(this, sourceSlot)
            ;
        }
    }
}
