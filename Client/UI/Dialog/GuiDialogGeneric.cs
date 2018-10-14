using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Mainly used for block entity based guis
    /// </summary>
    public abstract class GuiDialogGeneric : GuiDialog
    {
        /// <summary>
        /// The title of the Dialog.
        /// </summary>
        public string DialogTitle;

        /// <summary>
        /// Should this Dialog de-register itself once closed?
        /// </summary>
        public override bool UnregisterOnClose => true;

        /// <summary>
        /// The tree attributes for this dialog.
        /// </summary>
        public virtual ITreeAttribute Attributes => null;

        /// <summary>
        /// Constructor for a generic Dialog.
        /// </summary>
        /// <param name="DialogTitle">The title of the dialog.</param>
        /// <param name="capi">The Client API</param>
        public GuiDialogGeneric(string DialogTitle, ICoreClientAPI capi) : base(capi)
        {
            this.DialogTitle = DialogTitle;
        }

        public override string ToggleKeyCombinationCode
        {
            get { return null; }
        }

        /// <summary>
        /// Recomposes the dialog with it's set of elements.
        /// </summary>
        public virtual void Recompose()
        {
            foreach (GuiComposer composer in DialogComposers.Values)
            {
                composer.ReCompose();
            }
        }

        /// <summary>
        /// Unfocuses the elements in each composer.
        /// </summary>
        public virtual void UnfocusElements()
        {
            foreach (GuiComposer composer in DialogComposers.Values)
            {
                composer.UnfocusOwnElements();
            }
        }

        /// <summary>
        /// Focuses a specific element in the single composer.
        /// </summary>
        /// <param name="index">Index of the element.</param>
        public virtual void FocusElement(int index)
        {
            SingleComposer.FocusElement(index);
        }

        /// <summary>
        /// Checks if the player is in range of the block.
        /// </summary>
        /// <param name="blockEntityPos">The block's position.</param>
        /// <returns>In range or no?</returns>
        public virtual bool IsInRangeOfBlock(BlockPos blockEntityPos)
        {
            Block block = capi.World.BlockAccessor.GetBlock(blockEntityPos);
            Cuboidf[] boxes = block.GetSelectionBoxes(capi.World.BlockAccessor, blockEntityPos);

            double dist = 99;
            for (int i = 0; boxes != null && i < boxes.Length; i++)
            {
                Cuboidf box = boxes[i];
                Vec3d playerEye = capi.World.Player.Entity.Pos.XYZ.Add(0, capi.World.Player.Entity.EyeHeight, 0);
                dist = Math.Min(dist, box.ToDouble().Translate(blockEntityPos.X, blockEntityPos.Y, blockEntityPos.Z).ShortestDistanceFrom(playerEye));
            }

            return dist <= capi.World.Player.WorldData.PickingRange + 0.5;
        }

        /// <summary>
        /// Positions the dialogue above a given point.
        /// </summary>
        /// <param name="aboveHeadPos">The given point.</param>
        public virtual void PositionDialogAbove(Vec3d aboveHeadPos)
        {
            EntityPlayer entityPlayer = capi.World.Player.Entity;
            Vec3d pos = MatrixToolsd.Project(aboveHeadPos, capi.Render.PerspectiveProjectionMat, capi.Render.PerspectiveViewMat, capi.Render.FrameWidth, capi.Render.FrameHeight);

            // Z negative seems to indicate that the name tag is behind us \o/
            if (pos.Z < 0)
            {
                return;
            }

            SingleComposer.Bounds.Alignment = EnumDialogArea.None;
            SingleComposer.Bounds.fixedOffsetX = 0;
            SingleComposer.Bounds.fixedOffsetY = 0;
            SingleComposer.Bounds.absFixedX = pos.X - SingleComposer.Bounds.OuterWidth / 2;
            SingleComposer.Bounds.absFixedY = capi.Render.FrameHeight - pos.Y;
            SingleComposer.Bounds.absMarginX = 0;
            SingleComposer.Bounds.absMarginY = 0;
        }
    }
}
