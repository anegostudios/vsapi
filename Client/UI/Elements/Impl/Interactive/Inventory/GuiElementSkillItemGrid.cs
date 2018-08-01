using System;
using System.Collections.Generic;
using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class GuiElementSkillItemGrid : GuiElement
    {
        Dictionary<int, SkillItem> skillItems;
        int cols;
        int rows;

        public Action<int> OnSlotClick;
        public Action<int> OnSlotOver;

        public int selectedIndex = -1;
        int hoverTextureId;


        public override bool Focusable
        {
            get { return true; }
        }

        public GuiElementSkillItemGrid(ICoreClientAPI capi, Dictionary<int, SkillItem> skillItems, int cols, int rows, Action<int> OnSlotClick, ElementBounds bounds) : base(capi, bounds)
        {
            this.skillItems = skillItems;
            this.cols = cols;
            this.rows = rows;
            this.OnSlotClick = OnSlotClick;

            this.Bounds.fixedHeight = rows * (GuiElementItemSlotGrid.unscaledSlotPadding + GuiElementPassiveItemSlot.unscaledSlotSize);
            this.Bounds.fixedWidth = cols * (GuiElementItemSlotGrid.unscaledSlotPadding + GuiElementPassiveItemSlot.unscaledSlotSize);
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

                    RoundRectangle(ctx, Bounds.drawX + posX, Bounds.drawY + posY, slotWidth, slotHeight, ElementGeometrics.ElementBGRadius);
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
            RoundRectangle(ctx, 1, 1, slotWidth, slotHeight, ElementGeometrics.ElementBGRadius);
            ctx.Fill();

            generateTexture(surface, ref hoverTextureId);

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

            for (int i = 0; i < rows*cols; i++)
            {
                int row = i / cols;
                int col = i % cols;

                double posX = col * (slotWidth + slotPadding);
                double posY = row * (slotHeight + slotPadding);

                bool over = dx >= posX && dy >= posY && dx < posX + slotWidth + slotPadding && dy < posY + slotHeight + slotPadding;

                if (over || i == selectedIndex)
                {
                    api.Render.Render2DTexturePremultipliedAlpha(hoverTextureId, Bounds.renderX + posX, Bounds.renderY + posY, slotWidth, slotHeight);
                    if (over) OnSlotOver?.Invoke(i);
                }

                SkillItem skillItem = null;
                skillItems.TryGetValue(i, out skillItem);
                if (skillItem == null) { continue; }

                if (skillItem.Texture != null)
                {
                    api.Render.Render2DTexturePremultipliedAlpha(skillItem.Texture.TextureId, Bounds.renderX + posX + 1, Bounds.renderY + posY + 1, slotWidth, slotHeight);
                }

                skillItem.RenderHandler?.Invoke(skillItem.Code, deltaTime, Bounds.renderX + posX + 1, Bounds.renderY + posY + 1);
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

            OnSlotClick?.Invoke(index);
        }
        
    }

    public static partial class GuiComposerHelpers
    {

        public static GuiComposer AddSkillItemGrid(this GuiComposer composer, Dictionary<int, SkillItem> skillItems, int cols, int rows, Action<int> OnSlotClick, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementSkillItemGrid(composer.Api, skillItems, cols, rows, OnSlotClick, bounds), key);
            }
            return composer;
        }

        public static GuiElementSkillItemGrid GetSkillItemGrid(this GuiComposer composer, string key)
        {
            return (GuiElementSkillItemGrid)composer.GetElement(key);
        }
    }

}
