using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

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
        public virtual AssetLocation OpenSound => null;

        /// <summary>
        /// Gets the opening sound for the dialog being opened, or null if none.
        /// </summary>
        public virtual AssetLocation CloseSound => null;


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

            capi.World.Player.InventoryManager.OpenInventory(inventory);
            capi.Gui.PlaySound(OpenSound, true);
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
                capi.Event.RegisterCallback((deltatime) => TryClose(), 0);
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
                EntityPlayer entityPlayer = capi.World.Player.Entity;
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
        /// We tunnel our packet through a block entity packet so the block entity can 
        /// handle all the network stuff
        /// </summary>
        /// <param name="packet"></param>
        protected void DoSendPacket(object p)
        {
            capi.Network.SendBlockEntityPacket(BlockEntityPosition.X, BlockEntityPosition.Y, BlockEntityPosition.Z, p);
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
            Inventory.Open(capi.World.Player);
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
            Inventory.Close(capi.World.Player);
            capi.World.Player.InventoryManager.CloseInventory(Inventory);

            capi.Network.SendBlockEntityPacket(BlockEntityPosition.X, BlockEntityPosition.Y, BlockEntityPosition.Z, (int)EnumBlockEntityPacketId.Close);
            
            capi.Gui.PlaySound(CloseSound, true);
        }

        public override bool PrefersUngrabbedMouse => false;

        /// <summary>
        /// Reloads the values of the GUI.
        /// </summary>
        public void ReloadValues()
        {
            
        }


        public EnumPosFlag GetFreePos(string code)
        {
            var values = Enum.GetValues(typeof(EnumPosFlag));

            int flags = 0;
            posFlagDict().TryGetValue(code, out flags);
            
            foreach (EnumPosFlag flag in values)
            {
                if ((flags & (int)flag) > 0) continue;

                return flag;
            }

            return 0;
        }

        public void OccupyPos(string code, EnumPosFlag pos)
        {
            int flags = 0;
            posFlagDict().TryGetValue(code, out flags);
            posFlagDict()[code] = flags | (int)pos;
        }

        public void FreePos(string code, EnumPosFlag pos)
        {
            int flags = 0;
            posFlagDict().TryGetValue(code, out flags);
            posFlagDict()[code] = flags & ~(int)pos;
        }

        Dictionary<string, int> posFlagDict()
        {
            object valObj;
            capi.ObjectCache.TryGetValue("dialogCount", out valObj);
            Dictionary<string, int> val = valObj as Dictionary<string, int>;
            if (val == null) capi.ObjectCache["dialogCount"] = val = new Dictionary<string, int>();
            return val;
        }

        protected bool IsRight(EnumPosFlag flag)
        {
            return flag == EnumPosFlag.RightBot || flag == EnumPosFlag.RightMid || flag == EnumPosFlag.RightTop;
        }

        protected float YOffsetMul(EnumPosFlag flag)
        {
            if (flag == EnumPosFlag.RightTop || flag == EnumPosFlag.LeftTop) return -1;
            if (flag == EnumPosFlag.RightBot || flag == EnumPosFlag.LeftBot) return 1;
            return 0;
        }


        [Flags]
        public enum EnumPosFlag
        {
            RightMid = 1,
            RightTop = 2,
            RightBot = 4,
            LeftMid = 8,
            LeftTop = 16,
            LeftBot = 32
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
