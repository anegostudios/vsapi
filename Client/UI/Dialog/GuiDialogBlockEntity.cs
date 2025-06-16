using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Base class for dialogs bound to block entities.
    /// </summary>
    public abstract class GuiDialogBlockEntity : GuiDialogGeneric
    {
        public bool IsDuplicate { get; }

        public InventoryBase Inventory { get; }

        public BlockPos BlockEntityPosition { get; }

        /// <summary>
        /// Gets the opening sound for the dialog being opened, or null if none.
        /// </summary>
        public virtual AssetLocation OpenSound { get; set; } = null;

        /// <summary>
        /// Gets the opening sound for the dialog being opened, or null if none.
        /// </summary>
        public virtual AssetLocation CloseSound { get; set; } = null;


        /// <summary>
        /// Gets the Y offset of the dialog in-world if floaty GUIs is turned on.
        /// 0.5 is the center of the block and larger means it will float higher up.
        /// </summary>
        protected virtual double FloatyDialogPosition => 0.75;

        /// <summary>
        /// Gets the Y align of the dialog if floaty GUIs is turned on.
        /// 0.5 means the dialog is centered on <see cref="FloatyDialogPosition"/>.
        /// 0 is top-aligned while 1 is bottom-aligned.
        /// </summary>
        protected virtual double FloatyDialogAlign => 0.75;


        /// <param name="dialogTitle">The title of this dialogue. Ex: "Chest"</param>
        /// <param name="inventory">The inventory associated with this block entity.</param>
        /// <param name="blockEntityPos">The position of this block entity.</param>
        /// <param name="capi">The Client API</param>
        public GuiDialogBlockEntity(string dialogTitle, InventoryBase inventory, BlockPos blockEntityPos, ICoreClientAPI capi)
            : base(dialogTitle, capi)
        {
            IsDuplicate = capi.World.Player.InventoryManager.Inventories.ContainsValue(inventory);
            if (IsDuplicate) return;

            Inventory = inventory;
            BlockEntityPosition = blockEntityPos;
        }


        /// <param name="dialogTitle">The title of this dialogue. Ex: "Chest"</param>
        /// <param name="blockEntityPos">The position of this block entity.</param>
        /// <param name="capi">The Client API</param>
        public GuiDialogBlockEntity(string dialogTitle, BlockPos blockEntityPos, ICoreClientAPI capi)
            : base(dialogTitle, capi)
        {
            IsDuplicate = capi.OpenedGuis.FirstOrDefault(dlg => (dlg as GuiDialogBlockEntity)?.BlockEntityPosition == blockEntityPos) != null;
            if (IsDuplicate) return;

            BlockEntityPosition = blockEntityPos;
        }


        /// <summary>
        /// This occurs right before the frame is pushed to the screen.
        /// </summary>
        /// <param name="dt">The time elapsed.</param>
        public override void OnFinalizeFrame(float dt)
        {
            base.OnFinalizeFrame(dt);

            if (!IsInRangeOfBlock(BlockEntityPosition))
            {
                // Because we cant do it in here
                capi.Event.EnqueueMainThreadTask(() => TryClose(), "closedlg");
            }
        }

        /// <summary>
        /// Render's the object in Orthographic mode.
        /// </summary>
        /// <param name="deltaTime">The time elapsed.</param>
        public override void OnRenderGUI(float deltaTime)
        {
            if (capi.Settings.Bool["immersiveMouseMode"])
            {
                Vec3d aboveHeadPos = new Vec3d(BlockEntityPosition.X + 0.5, BlockEntityPosition.Y + FloatyDialogPosition, BlockEntityPosition.Z + 0.5);
                Vec3d pos = MatrixToolsd.Project(aboveHeadPos, capi.Render.PerspectiveProjectionMat, capi.Render.PerspectiveViewMat, capi.Render.FrameWidth, capi.Render.FrameHeight);

                // Z negative seems to indicate that the name tag is behind us \o/
                if (pos.Z < 0) return;

                SingleComposer.Bounds.Alignment = EnumDialogArea.None;
                SingleComposer.Bounds.fixedOffsetX = 0;
                SingleComposer.Bounds.fixedOffsetY = 0;
                SingleComposer.Bounds.absFixedX = pos.X - SingleComposer.Bounds.OuterWidth / 2;
                SingleComposer.Bounds.absFixedY = capi.Render.FrameHeight - pos.Y - SingleComposer.Bounds.OuterHeight * FloatyDialogAlign;
                SingleComposer.Bounds.absMarginX = 0;
                SingleComposer.Bounds.absMarginY = 0;
            }

            base.OnRenderGUI(deltaTime);
        }



        /// <summary>
        /// We tunnel our packet through a block entity packet so the block entity can handle all the network stuff
        /// </summary>
        /// <param name="p"></param>
        protected void DoSendPacket(object p)
        {
            capi.Network.SendBlockEntityPacket(BlockEntityPosition.X, BlockEntityPosition.InternalY, BlockEntityPosition.Z, p);
        }

        /// <summary>
        /// Called whenever the scrollbar or mouse wheel is used.
        /// </summary>
        /// <param name="value">The new value of the scrollbar.</param>
        protected void OnNewScrollbarvalue(float value)
        {
            ElementBounds bounds = SingleComposer.GetSlotGrid("slotgrid").Bounds;
            bounds.fixedY = 10 - GuiElementItemSlotGrid.unscaledSlotPadding - value;

            bounds.CalcWorldBounds();
        }

        /// <summary>
        /// Occurs whenever the X icon in the top right corner of the GUI (not the window) is pressed.
        /// </summary>
        protected void CloseIconPressed()
        {
            TryClose();
        }

        /// <summary>
        /// Called whenver the GUI is opened.
        /// </summary>
        public override void OnGuiOpened()
        {
            if (Inventory != null) capi.World.Player.InventoryManager.OpenInventory(Inventory);

            capi.Gui.PlaySound(OpenSound, true);
        }

        /// <summary>
        /// Attempts to open this gui.
        /// </summary>
        /// <returns>Whether the attempt was successful.</returns>
        public override bool TryOpen()
        {
            if (IsDuplicate) return false;
            return base.TryOpen();
        }

        /// <summary>
        /// Called when the GUI is closed.
        /// </summary>
        public override void OnGuiClosed()
        {
            if (Inventory != null) capi.World.Player.InventoryManager.CloseInventoryAndSync(Inventory);

            capi.Network.SendBlockEntityPacket(BlockEntityPosition, (int)EnumBlockEntityPacketId.Close);

            capi.Gui.PlaySound(CloseSound, true);
        }

        public override bool PrefersUngrabbedMouse => false;

        /// <summary>
        /// Reloads the values of the GUI.
        /// </summary>
        public void ReloadValues()
        {

        }



    }

    /// <summary>
    /// Packet IDs for block entities.
    /// </summary>
    public enum EnumBlockEntityPacketId
    {
        Open = 1000,
        Close = 1001
    }
}
