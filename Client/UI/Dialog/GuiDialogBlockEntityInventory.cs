using System;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// A block entity inventory system for things like a campfire, or other things like that.
    /// </summary>
    public class GuiDialogBlockEntityInventory : GuiDialogBlockEntity
    {
        int cols;

        public override AssetLocation OpenSound => new AssetLocation("sounds/block/chestopen");
        public override AssetLocation CloseSound => new AssetLocation("sounds/block/chestclose");

        public GuiDialogBlockEntityInventory(string dialogTitle, InventoryBase inventory, BlockPos blockEntityPos, int cols, ICoreClientAPI capi)
            : base(dialogTitle, inventory, blockEntityPos, capi)
        {
            if (IsDuplicate) return;
            this.cols = cols;
            
            int openedchests = capi.OpenedGuis.OfType<GuiDialogBlockEntityInventory>().Count();
            double elemToDlgPad = GuiStyle.ElementToDialogPadding;
            double pad = GuiElementItemSlotGrid.unscaledSlotPadding;
            int rows = (int)Math.Ceiling(inventory.Count / (float)cols);
            int visibleRows = Math.Min(rows, 7);

            // 1. The bounds of the slot grid itself. It is offseted by slot padding. It determines the size of the dialog, so we build the dialog from the bottom up
            ElementBounds slotGridBounds = ElementStdBounds.SlotGrid(EnumDialogArea.None, pad, pad, cols, visibleRows);

            // 1a.) Determine the full size of scrollable area, required to calculate scrollbar handle size
            ElementBounds fullGridBounds = ElementStdBounds.SlotGrid(EnumDialogArea.None, 0, 0, cols, rows);

            // 2. Around that is the 3 wide inset stroke
            ElementBounds insetBounds = slotGridBounds.ForkBoundingParent(6, 6, 6, 6);

            if (visibleRows < rows)
            {
                // 2a. The scrollable bounds is also the clipping bounds. Needs it's parent to be set.
                ElementBounds clippingBounds = slotGridBounds.CopyOffsetedSibling();
                clippingBounds.fixedHeight -= 3; // Why?

                // 3. Around all that is the dialog centered to screen middle, with some extra spacing right for the scrollbar
                ElementBounds dialogBounds = insetBounds
                    .ForkBoundingParent(elemToDlgPad, elemToDlgPad + 70, elemToDlgPad + 40, elemToDlgPad)
                    .WithFixedAlignmentOffset(-GuiStyle.DialogToScreenPadding, 0)
                    .WithAlignment(openedchests >= 3 ? EnumDialogArea.LeftMiddle : EnumDialogArea.RightMiddle)
                ;

                if (!capi.Settings.Bool["floatyGuis"])
                {
                    if (openedchests % 3 == 1) dialogBounds.fixedOffsetY -= dialogBounds.fixedHeight + 10;
                    if (openedchests % 3 == 2) dialogBounds.fixedOffsetY += dialogBounds.fixedHeight + 10;
                }

                // 4. Right of the slot grid is the scrollbar
                ElementBounds scrollbarBounds = ElementStdBounds.VerticalScrollbar(insetBounds).WithParent(dialogBounds);

                SingleComposer = capi.Gui
                    .CreateCompo("blockentityinventory" + blockEntityPos, dialogBounds)
                    .AddDialogBG(ElementBounds.Fill)
                    .AddDialogTitleBar(dialogTitle, CloseIconPressed)
                    .AddInset(insetBounds, 8, 0.85f)
                    .AddVerticalScrollbar(OnNewScrollbarvalue, scrollbarBounds, "scrollbar")
                    .BeginClip(clippingBounds)
                    .AddItemSlotGrid(inventory, DoSendPacket, cols, slotGridBounds, "slotgrid")
                    .EndClip()
                    .Compose();

                SingleComposer.GetScrollbar("scrollbar").SetHeights(
                    (float)(slotGridBounds.fixedHeight),
                    (float)(fullGridBounds.fixedHeight + pad)
                );

            }
            else
            {
                // 3. Around all that is the dialog centered to screen middle, with some extra spacing right for the scrollbar
                ElementBounds dialogBounds = insetBounds
                    .ForkBoundingParent(elemToDlgPad, elemToDlgPad + 20, elemToDlgPad, elemToDlgPad)
                    .WithFixedAlignmentOffset(-GuiStyle.DialogToScreenPadding, 0)
                    .WithAlignment(openedchests >= 3 ? EnumDialogArea.LeftMiddle : EnumDialogArea.RightMiddle);

                if (!capi.Settings.Bool["floatyGuis"])
                {
                    if (openedchests % 3 == 1) dialogBounds.fixedOffsetY -= dialogBounds.fixedHeight + 10;
                    if (openedchests % 3 == 2) dialogBounds.fixedOffsetY += dialogBounds.fixedHeight + 10;
                }

                SingleComposer = capi.Gui
                    .CreateCompo("blockentityinventory"+blockEntityPos, dialogBounds)
                    .AddDialogBG(ElementBounds.Fill)
                    .AddDialogTitleBar(dialogTitle, CloseIconPressed)
                    .AddInset(insetBounds, 8, 0.85f)
                    .AddItemSlotGrid(inventory, DoSendPacket, cols, slotGridBounds, "slotgrid")
                    .Compose();
            }

            SingleComposer.UnfocusOwnElements();
        }
    }
}
