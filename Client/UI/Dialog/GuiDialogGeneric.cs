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
        public string DialogTitle;


        public override bool UnregisterOnClose => true;
        public virtual ITreeAttribute Attributes => null;

        public GuiDialogGeneric(string DialogTitle, ICoreClientAPI capi) : base(capi)
        {
            this.DialogTitle = DialogTitle;
        }

        public override string ToggleKeyCombinationCode
        {
            get { return null; }
        }

        

        public virtual void Recompose()
        {
            foreach (GuiComposer composer in DialogComposers.Values)
            {
                composer.ReCompose();
            }
        }

        public virtual void UnfocusElements()
        {
            foreach (GuiComposer composer in DialogComposers.Values)
            {
                composer.UnfocusOwnElements();
            }
        }

        public virtual void FocusElement(int index)
        {
            SingleComposer.FocusElement(index);
        }


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
