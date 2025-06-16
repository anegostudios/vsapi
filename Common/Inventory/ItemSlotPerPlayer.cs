using System;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Vintagestory.API.Common;

public class ItemSlotPerPlayer : ItemSlot
{
    public new InventoryPerPlayer Inventory => (InventoryPerPlayer)inventory;

    public int Slotid;

    public override bool DrawUnavailable {
        get
        {
            var world = ((ICoreClientAPI)Inventory.Api).World;
            if (world.Player.WorldData.CurrentGameMode == EnumGameMode.Survival)
                return false;

            ;
            return Inventory.GetPlayerRemaining(world.Player.PlayerUID, Slotid) <= 0;
        }
        set { }
    }

    public ItemSlotPerPlayer(InventoryBase inventory, int slotid) : base(inventory)
    {
        Slotid = slotid;
    }

    public override bool CanTakeFrom(ItemSlot sourceSlot, EnumMergePriority priority = EnumMergePriority.AutoMerge)
    {
        return false;
    }

    public override bool CanHold(ItemSlot sourceSlot)
    {
        return false;
    }

    public override bool CanTake()
    {
        return false;
    }

    public override ItemStack? TakeOutWhole()
    {
        return null;
    }

    public override ItemStack? TakeOut(int quantity)
    {
        return null;
    }

    public override int TryPutInto(IWorldAccessor world, ItemSlot sinkSlot, int quantity = 1)
    {
        return 0;
    }

    public override int TryPutInto(ItemSlot sinkSlot, ref ItemStackMoveOperation op)
    {
        if (!sinkSlot.CanTakeFrom(this) || itemstack == null)
        {
            return 0;
        }

        if (sinkSlot.Inventory?.CanContain(sinkSlot, this) == false) return 0;

        var remaining = Inventory.GetPlayerRemaining(op.ActingPlayer.PlayerUID, Slotid);
        // Fill the destination slot with as many items as we can
        if (sinkSlot.Itemstack == null)
        {
            int q = GameMath.Min(sinkSlot.GetRemainingSlotSpace(itemstack), op.RequestedQuantity, remaining);

            if (q > 0)
            {
                sinkSlot.Itemstack = Itemstack.GetEmptyClone();
                sinkSlot.Itemstack.StackSize = q;
                // Has to be above the modified calls because e.g. when moving stuff into the ground slot this will eject the item
                // onto the ground and sinkSlot.StackSize is 0 right after
                op.MovedQuantity = op.MovableQuantity = Math.Min(sinkSlot.StackSize, q);
                remaining -= op.MovedQuantity;
                Inventory.AddPlayerUsage(op.ActingPlayer.PlayerUID, Slotid, op.MovedQuantity);

                if (op.World is IClientWorldAccessor)
                {
                    Itemstack.StackSize = remaining;
                    if (remaining == 0)
                        Itemstack = null;
                }

                sinkSlot.OnItemSlotModified(sinkSlot.Itemstack);
                OnItemSlotModified(sinkSlot.Itemstack);
                Inventory.MarkDirty();
            }

            return op.MovedQuantity;
        }

        ItemStackMergeOperation mergeop = op.ToMergeOperation(sinkSlot, this);
        op = mergeop;
        int origRequestedQuantity = op.RequestedQuantity;
        op.RequestedQuantity = GameMath.Min(sinkSlot.GetRemainingSlotSpace(itemstack), op.RequestedQuantity, remaining);
        // keep our own stack unchanged
        var ownStack = Itemstack.Clone();
        sinkSlot.Itemstack.Collectible.TryMergeStacks(mergeop);
        if(op.World is IServerWorldAccessor)
        {
            Itemstack = ownStack;
        }
        else
        {
            if (remaining == 0)
                Itemstack = null;
        }

        sinkSlot.OnItemSlotModified(sinkSlot.Itemstack);
        OnItemSlotModified(sinkSlot.Itemstack);

        op.RequestedQuantity = origRequestedQuantity; //ensures op.NotMovedQuantity will be correct in calling code if used with slots with limited slot maxStackSize, e.g. InventorySmelting with a cooking container has slots with maxStackSize == 6
        Inventory.AddPlayerUsage(op.ActingPlayer.PlayerUID, Slotid, mergeop.MovedQuantity);
        Inventory.MarkDirty();

        return mergeop.MovedQuantity;
    }

    public override void ActivateSlot(ItemSlot sourceSlot, ref ItemStackMoveOperation op)
    {
        if (Empty && sourceSlot.Empty) return;

        switch (op.MouseButton)
        {
            case EnumMouseButton.Left:
                ActivateSlotLeftClick(sourceSlot, ref op);
                return;

            case EnumMouseButton.Right:
                ActivateSlotRightClick(sourceSlot, ref op);
                return;
        }
    }

    protected override void ActivateSlotLeftClick(ItemSlot sinkSlot, ref ItemStackMoveOperation op)
    {
        if (!sinkSlot.Empty && op.ActingPlayer.WorldData.CurrentGameMode == EnumGameMode.Creative)
        {
            itemstack = sinkSlot.Itemstack.Clone();
            Inventory.Quantities[Slotid] = itemstack.StackSize;
            MarkDirty();
            Inventory.MarkDirty();
            return;
        }

        if (sinkSlot.Empty && op.ActingPlayer.WorldData.CurrentGameMode == EnumGameMode.Creative && op.CtrlDown)
        {
            itemstack.StackSize = 0;
            itemstack = null;
            Inventory.Quantities[Slotid] = 0;
            MarkDirty();
            Inventory.MarkDirty();
            return;
        }

        // 2. Current slot non empty, source slot empty: Put items
        if (sinkSlot.Empty)
        {
            if (Inventory.CanTake(this, op))
            {
                op.RequestedQuantity = Inventory.GetPlayerRemaining(op.ActingPlayer.PlayerUID, Slotid);
                TryPutInto(sinkSlot, ref op);
            }
            return;
        }

        // 3. Both slots not empty, and they are the same: Fill source slot
        if (sinkSlot.Itemstack.Equals(op.World, Itemstack, GlobalConstants.IgnoredStackAttributes) &&
            Inventory.CanTake(this, op))
        {
            op.RequestedQuantity = 1;
            var mergeop = op.ToMergeOperation(sinkSlot, this);
            op = mergeop;

            var ownStack = Itemstack.Clone();

            sinkSlot.Itemstack.Collectible.TryMergeStacks(mergeop);

            // Ignore any changes made to our slot
            if(op.World is IServerWorldAccessor)
            {
                Itemstack = ownStack;
            }
            Inventory.AddPlayerUsage(op.ActingPlayer.PlayerUID, Slotid, mergeop.MovedQuantity);
            Inventory.MarkDirty();
        }
    }

    protected override void ActivateSlotRightClick(ItemSlot sourceSlot, ref ItemStackMoveOperation op)
    {
        if (sourceSlot.Empty)
        {
            if (Inventory.CanTake(this, op))
            {
                var remaining = Inventory.GetPlayerRemaining(op.ActingPlayer.PlayerUID, Slotid);
                op.RequestedQuantity = remaining / 2;
                TryPutInto(sourceSlot, ref op);
            }
        }
    }

    public override bool TryFlipWith(ItemSlot itemSlot)
    {
        return false;
    }

    protected override void FlipWith(ItemSlot withslot)
    {
    }
}
