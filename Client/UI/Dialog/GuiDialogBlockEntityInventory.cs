using System;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// A block entity inventory system for things like a campfire, or other things like that.
    /// </summary>
    public class GuiDialogBlockEntityInventory : GuiDialogBlockEntity
    {
        int cols;
        EnumPosFlag screenPos;

        public override double DrawOrder => 0.2;

        public GuiDialogBlockEntityInventory(string dialogTitle, InventoryBase inventory, BlockPos blockEntityPos, int cols, ICoreClientAPI capi)
            : base(dialogTitle, inventory, blockEntityPos, capi)
        {
            if (IsDuplicate) return;
            this.cols = cols;

            
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

            screenPos = GetFreePos("smallblockgui");

            if (visibleRows < rows)
            {
                // 2a. The scrollable bounds is also the clipping bounds. Needs it's parent to be set.
                ElementBounds clippingBounds = slotGridBounds.CopyOffsetedSibling();
                clippingBounds.fixedHeight -= 3; // Why?

                // 3. Around all that is the dialog centered to screen middle, with some extra spacing right for the scrollbar
                ElementBounds dialogBounds = insetBounds
                    .ForkBoundingParent(elemToDlgPad, elemToDlgPad + 30, elemToDlgPad + 20, elemToDlgPad)
                    .WithFixedAlignmentOffset(IsRight(screenPos) ? -GuiStyle.DialogToScreenPadding : GuiStyle.DialogToScreenPadding, 0)
                    .WithAlignment(IsRight(screenPos) ? EnumDialogArea.RightMiddle :EnumDialogArea.LeftMiddle)
                ;

                if (!capi.Settings.Bool["immersiveMouseMode"])
                {
                    dialogBounds.fixedOffsetY += (dialogBounds.fixedHeight + 10) * YOffsetMul(screenPos);
                    dialogBounds.fixedOffsetX += (dialogBounds.fixedWidth + 10) * XOffsetMul(screenPos);
                }

                // 4. Right of the slot grid is the scrollbar
                ElementBounds scrollbarBounds = ElementStdBounds.VerticalScrollbar(insetBounds).WithParent(dialogBounds);

                SingleComposer = capi.Gui
                    .CreateCompo("blockentityinventory" + blockEntityPos, dialogBounds)
                    .AddShadedDialogBG(ElementBounds.Fill)
                    .AddDialogTitleBar(dialogTitle, CloseIconPressed)
                    .AddInset(insetBounds)
                    .AddVerticalScrollbar(OnNewScrollbarvalue, scrollbarBounds, "scrollbar")
                    .BeginClip(clippingBounds)
                    .AddItemSlotGrid(inventory, DoSendPacket, cols, fullGridBounds, "slotgrid")
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
                    .WithFixedAlignmentOffset(IsRight(screenPos) ? -GuiStyle.DialogToScreenPadding : GuiStyle.DialogToScreenPadding, 0)
                    .WithAlignment(IsRight(screenPos) ? EnumDialogArea.RightMiddle : EnumDialogArea.LeftMiddle)
                ;

                if (!capi.Settings.Bool["immersiveMouseMode"])
                {
                    dialogBounds.fixedOffsetY += (dialogBounds.fixedHeight + 10) * YOffsetMul(screenPos);
                    dialogBounds.fixedOffsetX += (dialogBounds.fixedWidth + 10) * XOffsetMul(screenPos);
                }

                SingleComposer = capi.Gui
                    .CreateCompo("blockentityinventory"+blockEntityPos, dialogBounds)
                    .AddShadedDialogBG(ElementBounds.Fill)
                    .AddDialogTitleBar(dialogTitle, CloseIconPressed)
                    .AddInset(insetBounds)
                    .AddItemSlotGrid(inventory, DoSendPacket, cols, slotGridBounds, "slotgrid")
                    .Compose();
            }

            SingleComposer.UnfocusOwnElements();
        }


        public override void OnGuiClosed()
        {
            base.OnGuiClosed();
            FreePos("smallblockgui", screenPos);
        }

        public override void OnGuiOpened()
        {
            base.OnGuiOpened();

            if (capi.Gui.GetDialogPosition(SingleComposer.DialogName) == null)
            {
                OccupyPos("smallblockgui", screenPos);
            }
        }
    }
}
