using Cairo;
using System;
using System.Collections.Generic;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// A slot for item skills.
    /// </summary>
    public class GuiElementSkillItemGrid : GuiElement
    {
        List<SkillItem> skillItems;
        int cols;
        int rows;

        public Action<int> OnSlotClick;
        public Action<int> OnSlotOver;

        public int selectedIndex = -1;
        LoadedTexture hoverTexture;


        public override bool Focusable
        {
            get { return true; }
        }

        /// <summary>
        /// Creates a Skill Item Grid.
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="skillItems">The items with skills.</param>
        /// <param name="columns">The columns of the Item Grid</param>
        /// <param name="rows">The Rows of the Item Grid.</param>
        /// <param name="OnSlotClick">The event fired when the slot is clicked.</param>
        /// <param name="bounds">The bounds of the Item Grid.</param>
        public GuiElementSkillItemGrid(ICoreClientAPI capi, List<SkillItem> skillItems, int columns, int rows, Action<int> OnSlotClick, ElementBounds bounds) : base(capi, bounds)
        {
            hoverTexture = new LoadedTexture(capi);

            this.skillItems = skillItems;
            this.cols = columns;
            this.rows = rows;
            this.OnSlotClick = OnSlotClick;

            this.Bounds.fixedHeight = rows * (GuiElementItemSlotGrid.unscaledSlotPadding + GuiElementPassiveItemSlot.unscaledSlotSize);
            this.Bounds.fixedWidth = columns * (GuiElementItemSlotGrid.unscaledSlotPadding + GuiElementPassiveItemSlot.unscaledSlotSize);
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            ComposeSlots(ctx, surface);
            ComposeHover();
        }

        void ComposeSlots(Context ctx, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            double slotPadding = scaled(GuiElementItemSlotGrid.unscaledSlotPadding);
            double slotWidth = scaled(GuiElementPassiveItemSlot.unscaledSlotSize);
            double slotHeight = scaled(GuiElementPassiveItemSlot.unscaledSlotSize);

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    double posX = col * (slotWidth + slotPadding);
                    double posY = row * (slotHeight + slotPadding);

                    ctx.SetSourceRGBA(1, 1, 1, 0.2);

                    RoundRectangle(ctx, Bounds.drawX + posX, Bounds.drawY + posY, slotWidth, slotHeight, GuiStyle.ElementBGRadius);
                    ctx.Fill();
                    EmbossRoundRectangleElement(ctx, Bounds.drawX + posX, Bounds.drawY + posY, slotWidth, slotHeight, true);
                }
            }
        }

        void ComposeHover()
        {
            double slotWidth = scaled(GuiElementPassiveItemSlot.unscaledSlotSize);
            double slotHeight = scaled(GuiElementPassiveItemSlot.unscaledSlotSize);

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)slotWidth - 2, (int)slotHeight - 2);
            Context ctx = genContext(surface);

            ctx.SetSourceRGBA(1, 1, 1, 0.7);
            RoundRectangle(ctx, 1, 1, slotWidth, slotHeight, GuiStyle.ElementBGRadius);
            ctx.Fill();

            generateTexture(surface, ref hoverTexture);

            ctx.Dispose();
            surface.Dispose();
        }


        public override void RenderInteractiveElements(float deltaTime)
        {
            double slotPadding = scaled(GuiElementItemSlotGrid.unscaledSlotPadding);
            double slotWidth = scaled(GuiElementPassiveItemSlot.unscaledSlotSize);
            double slotHeight = scaled(GuiElementPassiveItemSlot.unscaledSlotSize);

            int dx = api.Input.MouseX - (int)Bounds.absX;
            int dy = api.Input.MouseY - (int)Bounds.absY;

            var scale = RuntimeEnv.GUIScale;

            for (int i = 0; i < rows*cols; i++)
            {
                int row = i / cols;
                int col = i % cols;

                double posX = col * (slotWidth + slotPadding);
                double posY = row * (slotHeight + slotPadding);

                bool over = dx >= posX && dy >= posY && dx < posX + slotWidth + slotPadding && dy < posY + slotHeight + slotPadding;

                if (over || i == selectedIndex)
                {
                    api.Render.Render2DTexture(hoverTexture.TextureId, (float)(Bounds.renderX + posX),(float)(Bounds.renderY + posY), (float)slotWidth, (float)slotHeight);
                    if (over) OnSlotOver?.Invoke(i);
                }

                if (skillItems.Count <= i) continue;
                SkillItem skillItem = skillItems[i];

                if (skillItem == null) continue;

                var sb = ElementBounds.Fixed((Bounds.renderX + posX + 1) / scale, (Bounds.renderY + posY + 1) / scale, GuiElementPassiveItemSlot.unscaledSlotSize-2, GuiElementPassiveItemSlot.unscaledSlotSize-2).WithParent(api.Gui.WindowBounds);
                sb.CalcWorldBounds();

                api.Render.PushScissor(sb, true);

                if (skillItem.Texture != null)
                {
                    if (skillItem.TexturePremultipliedAlpha)
                    {
                        api.Render.Render2DTexturePremultipliedAlpha(skillItem.Texture.TextureId, Bounds.renderX + posX + 1, Bounds.renderY + posY + 1, slotWidth, slotHeight); 
                    } else
                    {
                        api.Render.Render2DTexture(skillItem.Texture.TextureId, (float)(Bounds.renderX + posX + 1), (float)(Bounds.renderY + posY + 1), (float)slotWidth, (float)slotHeight);
                    }
                    
                }

                skillItem.RenderHandler?.Invoke(skillItem.Code, deltaTime, Bounds.renderX + posX + 1, Bounds.renderY + posY + 1);

                api.Render.PopScissor();
            }
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseDownOnElement(api, args);

            int dx = api.Input.MouseX - (int)Bounds.absX;
            int dy = api.Input.MouseY - (int)Bounds.absY;

            double slotPadding = scaled(GuiElementItemSlotGrid.unscaledSlotPadding);
            double slotWidth = scaled(GuiElementPassiveItemSlot.unscaledSlotSize);
            double slotHeight = scaled(GuiElementPassiveItemSlot.unscaledSlotSize);

            int row = (int)(dy / (slotHeight + slotPadding));
            int col = (int)(dx / (slotWidth + slotPadding));

            int index = row * cols + col;

            if (index >= 0 && index < skillItems.Count)
            {
                OnSlotClick?.Invoke(index);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            hoverTexture.Dispose();
        }

    }

    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Adds a skill item grid to the GUI.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="skillItems">The items that represent skills.</param>
        /// <param name="columns">the columns in the skill item grid.</param>
        /// <param name="rows">The rows in the skill item grid.</param>
        /// <param name="onSlotClick">The effect when a slot is clicked.</param>
        /// <param name="bounds">The bounds of the item grid.</param>
        /// <param name="key">The name of the item grid to add.</param>
        public static GuiComposer AddSkillItemGrid(this GuiComposer composer, List<SkillItem> skillItems, int columns, int rows, Action<int> onSlotClick, ElementBounds bounds, string key = null)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementSkillItemGrid(composer.Api, skillItems, columns, rows, onSlotClick, bounds), key);
            }
            return composer;
        }

        /// <summary>
        /// Fetches the skill item grid by name
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="key">The name of the skill item grid to get.</param>
        /// <returns>The skill item grid to get.</returns>
        public static GuiElementSkillItemGrid GetSkillItemGrid(this GuiComposer composer, string key)
        {
            return (GuiElementSkillItemGrid)composer.GetElement(key);
        }
    }

}
