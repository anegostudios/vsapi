using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;

namespace Vintagestory.API.Common;

public class InventoryPerPlayer : InventoryGeneric
{
    public override bool PutLocked { get; set; } = true;
    public Dictionary<string, int[]> PlayerQuantities;
    public int[] Quantities;

    public InventoryPerPlayer(int quantitySlots, string? invId, ICoreAPI? api, NewSlotDelegate? onNewSlot = null) : base(quantitySlots, invId, api, onNewSlot)
    {
        PlayerQuantities = new Dictionary<string, int[]>();
        Quantities = new int[quantitySlots];
    }

    public bool CanTake(ItemSlot fromSlot, ItemStackMoveOperation op)
    {
        return GetPlayerRemaining(op.ActingPlayer.PlayerUID,GetSlotId(fromSlot)) > 0;
    }

    public void AddPlayerUsage(string playerUid, int slotId, int value)
    {
        if (PlayerQuantities.TryGetValue(playerUid, out var quantities))
        {
            quantities[slotId] += value;
        }
    }

    public int GetPlayerRemaining(string playerUid, int slotId)
    {
        if (PlayerQuantities.TryGetValue(playerUid, out var quantities))
        {
            return Math.Max(0, Quantities[slotId] - quantities[slotId]);
        }

        quantities = new int[Quantities.Length];
        // Array.Copy(Quantities, quantities, Quantities.Length);
        PlayerQuantities.Add(playerUid, quantities);
        return Math.Max(0, Quantities[slotId] - quantities[slotId]);
    }

    public override void MarkSlotDirty(int slotId)
    {
        // prevent marking the slot dirty since it will stay the same on the server
        // if we do not prevent it we will see item flicker client side when the player takes out items
    }

    /// <summary>
    /// allow the server to resend changed PlayerQuantities
    /// </summary>
    public void MarkDirty()
    {
        if (Api is ICoreServerAPI sapi)
        {
            sapi.World.BlockAccessor.GetBlockEntity(Pos).MarkDirty();
        }
    }

    protected override ItemSlot NewSlot(int slotId)
    {
        return new ItemSlotPerPlayer(this, slotId);
    }

    public void OnPlacementBySchematic()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            Quantities[i] = this[i].StackSize;
        }
    }

    public override void FromTreeAttributes(ITreeAttribute treeAttribute)
    {
        base.FromTreeAttributes(treeAttribute);
        var tree = treeAttribute["PlayerQuantities"] as TreeAttribute;
        if (tree != null)
        {
            if (tree.Count > 0)
            {
                foreach (var (playerUid, attribute) in tree)
                {
                    PlayerQuantities[playerUid] = (attribute as IntArrayAttribute)?.value ?? new int[Count];
                }
            }
            else
            {
                PlayerQuantities.Clear();
            }
        }

        Quantities = (treeAttribute["Quantities"] as IntArrayAttribute)?.value ?? new int[Count];

        // show in creative the stack as unavailable and in survival the stack will not exist client side once looted
        if (Api is ICoreClientAPI capi && PlayerQuantities.ContainsKey(capi.World.Player.PlayerUID))
        {
            for (int i = 0; i < Quantities.Length; i++)
            {
                if (this[i].Itemstack != null)
                {
                    var remaining = GetPlayerRemaining(capi.World.Player.PlayerUID, i);
                    if (remaining == 0)
                    {
                        if (capi.World.Player.WorldData.CurrentGameMode == EnumGameMode.Survival)
                        {
                            this[i].Itemstack = null;
                        }
                    }
                    else
                    {
                        this[i].Itemstack.StackSize = remaining;
                    }
                }
            }
        }
    }

    public override void ToTreeAttributes(ITreeAttribute invtree)
    {
        var tree = new TreeAttribute();
        foreach (var (playerUid, quantities) in PlayerQuantities)
        {
            tree[playerUid] = new IntArrayAttribute(quantities);
        }

        invtree["PlayerQuantities"] = tree;

        invtree["Quantities"] = new IntArrayAttribute(Quantities);
        base.ToTreeAttributes(invtree);
    }
}
