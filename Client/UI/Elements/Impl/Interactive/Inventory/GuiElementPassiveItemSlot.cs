using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Displays a single slot from given inventory, cannot be directly interacted with. Currently used for the mouse slot
    /// </summary>
    public class GuiElementPassiveItemSlot : GuiElement
    {
        public static double unscaledItemSize = 32 * 0.8f;

        public static double unscaledSlotSize = 48;

        ItemSlot slot;
        IInventory inventory;

        bool drawBackground;

        GuiElementStaticText textComposer;

        /// <summary>
        /// Creates a new passive item slot.
        /// </summary>
        /// <param name="capi">The client API</param>
        /// <param name="bounds">the bounds of the Slot.</param>
        /// <param name="inventory">the attached inventory for the slot.</param>
        /// <param name="slot">The slot of the slot.</param>
        /// <param name="drawBackground">Do we draw the background for this slot? (Default: true)</param>
        public GuiElementPassiveItemSlot(ICoreClientAPI capi, ElementBounds bounds, IInventory inventory, ItemSlot slot, bool drawBackground = true) : base(capi, bounds)
        {
            this.slot = slot;
            this.inventory = inventory;
            this.drawBackground = drawBackground;

            bounds.fixedWidth = unscaledSlotSize;
            bounds.fixedHeight = unscaledSlotSize;
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            if (drawBackground)
            {
                ctx.SetSourceRGBA(1, 1, 1, 0.6);
                ElementRoundRectangle(ctx, Bounds);
                ctx.Fill();
                EmbossRoundRectangleElement(ctx, Bounds, true);
            }

            double absSlotSize = scaled(unscaledSlotSize);

            ElementBounds textBounds = ElementBounds
              .Fixed(0, unscaledSlotSize - GuiStyle.SmallFontSize - 2, unscaledSlotSize - 5, unscaledSlotSize - 5)
              .WithEmptyParent();

            CairoFont font = CairoFont.WhiteSmallText();
            font.FontWeight = FontWeight.Bold;
            textComposer = new GuiElementStaticText(api, "", EnumTextOrientation.Right, textBounds, font);
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            if (slot.Itemstack != null)
            {
                double absSlotWidth = scaled(unscaledSlotSize);
                double absItemstackSize = scaled(unscaledItemSize);
                double offset = absSlotWidth / 2;
                api.Render.RenderItemstackToGui(slot, Bounds.renderX + offset, Bounds.renderY + offset, 450, (float)scaled(unscaledItemSize), ColorUtil.WhiteArgb);
            }
        }
    }


    public static partial class GuiComposerHelpers
    {

        /// <summary>
        /// Adds a passive item slot to the GUI.
        /// </summary>
        /// <param name="bounds">The bounds of the Slot</param>
        /// <param name="inventory">The inventory attached to the slot.</param>
        /// <param name="slot">The internal slot of the slot.</param>
        /// <param name="drawBackground">Do we draw the background for this slot? (Default: true)</param>
        public static GuiComposer AddPassiveItemSlot(this GuiComposer composer, ElementBounds bounds, IInventory inventory, ItemSlot slot, bool drawBackground = true)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementPassiveItemSlot(composer.Api, bounds, inventory, slot, drawBackground));
            }

            return composer;
        }
    }
}
