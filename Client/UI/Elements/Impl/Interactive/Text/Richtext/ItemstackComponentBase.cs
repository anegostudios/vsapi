using System;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Draws an itemstack 
    /// </summary>
    public class ItemstackComponentBase : RichTextComponentBase
    {
        static int tooltipOffsetX = 10;
        static int tooltipOffsetY = 40;

        protected ItemSlot renderedTooltipSlot;
        protected GuiElementItemstackInfo stackInfo;
        protected ICoreClientAPI capi;

        bool bottomOverlap = false;
        bool rightOverlap = false;
        bool recalcAlignmentOffset = false;
        protected ElementBounds stackInfoBounds;

        protected ElementBounds parentBounds;

        public double offY = 0;
        public double offX = 0;
        protected DummyInventory dummyInv;

        long lastHoverslotInfoTextUpdateTotalMs;

        public ItemstackComponentBase(ICoreClientAPI capi) : base(capi)
        {
            this.capi = capi;

            dummyInv = new DummyInventory(capi);
            dummyInv.OnAcquireTransitionSpeed += (transType, stack, mul) =>
            {
                return 0;
            };
            renderedTooltipSlot = new DummySlot(null, dummyInv);

            stackInfoBounds =
                ElementBounds
                .FixedSize(EnumDialogArea.None, GuiElementItemstackInfo.BoxWidth, 0)
                .WithFixedPadding(6 + 4 * RuntimeEnv.GUIScale)
                .WithFixedPosition(12 + 8 / RuntimeEnv.GUIScale, 28 + 12 / RuntimeEnv.GUIScale)
            ;

            parentBounds = ElementBounds.Fixed(0, 0, 1, 1);
            parentBounds.WithParent(ElementBounds.Empty);
            stackInfoBounds.WithParent(parentBounds);

            stackInfo = new GuiElementItemstackInfo(capi, stackInfoBounds, OnRequireInfoText);
            stackInfo.SetSourceSlot(renderedTooltipSlot);
            stackInfo.ComposeElements(null, null);
            stackInfo.RecompCheckIgnoredStackAttributes = GlobalConstants.IgnoredStackAttributes;
        }

        protected virtual string OnRequireInfoText(ItemSlot slot)
        {
            return slot.GetStackDescription(capi.World, capi.Settings.Bool["extendedDebugInfo"]);
        }

        public void RenderItemstackTooltip(ItemSlot slot, double renderX, double renderY, float dt)
        {
            parentBounds.fixedX = renderX / RuntimeEnv.GUIScale;
            parentBounds.fixedY = renderY / RuntimeEnv.GUIScale;
            parentBounds.CalcWorldBounds();

            renderedTooltipSlot.Itemstack = slot.Itemstack;
            renderedTooltipSlot.BackgroundIcon = slot.BackgroundIcon;

            if (capi.ElapsedMilliseconds - lastHoverslotInfoTextUpdateTotalMs > 1000)
            {
                stackInfo.SetSourceSlot(null);
            }

            stackInfo.SetSourceSlot(renderedTooltipSlot);
            lastHoverslotInfoTextUpdateTotalMs = capi.ElapsedMilliseconds;

            bool newRightOverlap = capi.Input.MouseX + stackInfoBounds.OuterWidth > capi.Render.FrameWidth - 5;
            bool newBottomOverlap = capi.Input.MouseY + stackInfoBounds.OuterHeight > capi.Render.FrameHeight - 5;

            if (recalcAlignmentOffset || bottomOverlap != newBottomOverlap || newRightOverlap != rightOverlap)
            {
                stackInfoBounds.WithFixedAlignmentOffset(
                    newRightOverlap ? -stackInfoBounds.OuterWidth / RuntimeEnv.GUIScale - tooltipOffsetX : 0,
                    newBottomOverlap ? -stackInfoBounds.OuterHeight / RuntimeEnv.GUIScale - tooltipOffsetY : 0
                );

                stackInfoBounds.CalcWorldBounds();
                stackInfoBounds.fixedOffsetY += Math.Max(0, -stackInfoBounds.renderY);

                stackInfoBounds.CalcWorldBounds();
                bottomOverlap = newBottomOverlap;
                rightOverlap = newRightOverlap;
                recalcAlignmentOffset = false;
            }

            if (capi.Render.ScissorStack.Count > 0)
            {
                capi.Render.GlScissorFlag(false);
                stackInfo.RenderInteractiveElements(dt);
                capi.Render.GlScissorFlag(true);
            } else
            {
                stackInfo.RenderInteractiveElements(dt);
            }
        }


        public override void Dispose()
        {
            base.Dispose();

            stackInfo.Dispose();
        }
    }
}
