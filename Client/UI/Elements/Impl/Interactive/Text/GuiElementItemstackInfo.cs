using System;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.Client
{
    public delegate string InfoTextDelegate(ItemSlot slot);

    public class GuiElementItemstackInfo : GuiElementTextBase
    {
        public bool Dirty;
        public bool Render = true;
        public GuiElementRichtext titleElement; 
        public GuiElementRichtext descriptionElement; 
        public LoadedTexture texture;

        public static double ItemStackSize = GuiElementPassiveItemSlot.unscaledItemSize * 2.5;
        public static int MarginTop = 24;
        public static int BoxWidth = 415;
        public static int MinBoxHeight = 80;

        static double[] backTint = GuiStyle.DialogStrongBgColor;

        public ItemSlot curSlot;
        ItemStack curStack;
        CairoFont titleFont;

        double maxWidth;

        InfoTextDelegate OnRequireInfoText;

        ElementBounds scissorBounds;
        public Action onRenderStack;

        public string[] RecompCheckIgnoredStackAttributes;

        /// <summary>
        /// Creates an ItemStackInfo element.
        /// </summary>
        /// <param name="capi">The client API</param>
        /// <param name="bounds">The bounds of the object.</param>
        /// <param name="OnRequireInfoText">The function that is called when an item information is called.</param>
        public GuiElementItemstackInfo(ICoreClientAPI capi, ElementBounds bounds, InfoTextDelegate OnRequireInfoText) : base(capi, "", CairoFont.WhiteSmallText(), bounds)
        {
            this.OnRequireInfoText = OnRequireInfoText;

            texture = new LoadedTexture(capi);

            ElementBounds textBounds = bounds.CopyOnlySize();
            ElementBounds descBounds = textBounds.CopyOffsetedSibling(ItemStackSize + 50, MarginTop, -ItemStackSize - 50, 0);
            descBounds.WithParent(bounds);
            textBounds.WithParent(bounds);

            descriptionElement = new GuiElementRichtext(capi, Array.Empty<RichTextComponentBase>(), descBounds);
            descriptionElement.zPos = 1001;

            titleFont = Font.Clone();
            titleFont.FontWeight = FontWeight.Bold;

            titleElement = new GuiElementRichtext(capi, Array.Empty<RichTextComponentBase>(), textBounds);
            titleElement.zPos = 1001;

            maxWidth = bounds.fixedWidth;

            onRenderStack = () =>
            {
                double offset = (int)scaled(30 + ItemStackSize / 2);
                api.Render.RenderItemstackToGui(
                    curSlot,
                    (int)Bounds.renderX + offset,
                    (int)Bounds.renderY + offset + (int)scaled(MarginTop),
                    1000 + scaled(GuiElementPassiveItemSlot.unscaledItemSize) * 2,
                    (float)scaled(ItemStackSize),
                    ColorUtil.WhiteArgb,
                    true,
                    true,
                    false
                );
            };
        }


        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
        }

        void RecalcBounds()
        {
            descriptionElement.BeforeCalcBounds();
            titleElement.BeforeCalcBounds();

            double currentWidth = Math.Max(
                titleElement.MaxLineWidth / RuntimeEnv.GUIScale, 
                descriptionElement.MaxLineWidth / RuntimeEnv.GUIScale + 10 + 40 + GuiElementPassiveItemSlot.unscaledItemSize * 3
            );

            currentWidth = Math.Min(currentWidth, maxWidth);

            double descWidth = currentWidth - ItemStackSize - 50;

            Bounds.fixedWidth = currentWidth;
            descriptionElement.Bounds.fixedWidth = descWidth;
            titleElement.Bounds.fixedWidth = currentWidth;
            descriptionElement.Bounds.CalcWorldBounds();

            // Height depends on the width
            double unscaledDescTextHeight = descriptionElement.Bounds.fixedHeight;
            double unscaledTotalHeight = Math.Max(unscaledDescTextHeight, 25 + GuiElementPassiveItemSlot.unscaledItemSize * 3);
            titleElement.Bounds.fixedHeight = unscaledTotalHeight;
            descriptionElement.Bounds.fixedHeight = unscaledTotalHeight;
            Bounds.fixedHeight = 25 + unscaledTotalHeight;
        }

        

        public void AsyncRecompose()
        {
            if (curSlot?.Itemstack == null) return;

            Dirty = true;

            string title = curSlot.GetStackName();
            string desc = OnRequireInfoText(curSlot);
            desc.TrimEnd();

            titleElement.Bounds.fixedWidth = maxWidth - 10;
            descriptionElement.Bounds.fixedWidth = maxWidth - 40 - scaled(GuiElementPassiveItemSlot.unscaledItemSize) * 3 - 10;
            descriptionElement.Bounds.CalcWorldBounds();
            titleElement.Bounds.CalcWorldBounds();

            titleElement.SetNewTextWithoutRecompose(title, titleFont, null, true);
            descriptionElement.SetNewTextWithoutRecompose(desc, Font, null, true);

            RecalcBounds();
            Bounds.CalcWorldBounds();

            ElementBounds textBounds = Bounds.CopyOnlySize();
            textBounds.CalcWorldBounds();

            TyronThreadPool.QueueTask(() =>
            {
                ImageSurface surface = new ImageSurface(Format.Argb32, Bounds.OuterWidthInt, Bounds.OuterHeightInt);
                Context ctx = genContext(surface);

                ctx.SetSourceRGBA(0, 0, 0, 0);
                ctx.Paint();

                ctx.SetSourceRGBA(backTint[0], backTint[1], backTint[2], backTint[3]);
                RoundRectangle(ctx, textBounds.bgDrawX, textBounds.bgDrawY, textBounds.OuterWidthInt, textBounds.OuterHeightInt, GuiStyle.DialogBGRadius);
                ctx.FillPreserve();

                ctx.SetSourceRGBA(GuiStyle.DialogLightBgColor[0] * 1.4, GuiStyle.DialogStrongBgColor[1] * 1.4, GuiStyle.DialogStrongBgColor[2] * 1.4, 1);
                ctx.LineWidth = 3 * 1.75;
                ctx.StrokePreserve();
                surface.BlurFull(8.2);

                ctx.SetSourceRGBA(backTint[0] / 2, backTint[1] / 2, backTint[2] / 2, backTint[3]);
                ctx.Stroke();


                int w = (int)(scaled(ItemStackSize) + scaled(40));
                int h = (int)(scaled(ItemStackSize) + scaled(40));

                ImageSurface shSurface = new ImageSurface(Format.Argb32, w, h);
                Context shCtx = genContext(shSurface);

                shCtx.SetSourceRGBA(GuiStyle.DialogSlotBackColor);
                RoundRectangle(shCtx, 0, 0, w, h, 0);
                shCtx.FillPreserve();

                shCtx.SetSourceRGBA(GuiStyle.DialogSlotFrontColor);
                shCtx.LineWidth = 5;
                shCtx.Stroke();
                shSurface.BlurFull(7);
                shSurface.BlurFull(7);
                shSurface.BlurFull(7);
                EmbossRoundRectangleElement(shCtx, 0, 0, w, h, true);


                ctx.SetSourceSurface(shSurface, (int)(textBounds.drawX), (int)(textBounds.drawY + scaled(MarginTop)));
                ctx.Rectangle(textBounds.drawX, textBounds.drawY + scaled(MarginTop), w, h);
                ctx.Fill();

                shCtx.Dispose();
                shSurface.Dispose();

                api.Event.EnqueueMainThreadTask(() =>
                {
                    titleElement.Compose(false);
                    descriptionElement.Compose(false);

                    generateTexture(surface, ref texture);

                    ctx.Dispose();
                    surface.Dispose();

                    double offset = (int)(30 + ItemStackSize / 2);
                    scissorBounds = ElementBounds.Fixed(4 + offset - ItemStackSize, 2 + offset + MarginTop - ItemStackSize, ItemStackSize + 38, ItemStackSize + 38).WithParent(Bounds);
                    scissorBounds.CalcWorldBounds();

                    Dirty = false;

                }, "genstackinfotexture");
            });
        }


        public override void RenderInteractiveElements(float deltaTime)
        {
            if (curSlot?.Itemstack == null || Dirty || !Render)
            {
                return;
            }

            api.Render.Render2DTexturePremultipliedAlpha(texture.TextureId, Bounds, 1000);

            titleElement.RenderInteractiveElements(deltaTime);
            descriptionElement.RenderInteractiveElements(deltaTime);

            api.Render.PushScissor(scissorBounds);

            onRenderStack();

            api.Render.PopScissor();
        }


        




        /// <summary>
        /// Gets the item slot for this stack info.
        /// </summary>
        /// <returns></returns>
        public ItemSlot GetSlot()
        {
            return curSlot;
        }

        /// <summary>
        /// Sets the source slot for stacks.
        /// </summary>
        /// <param name="nowSlot"></param>
        /// <param name="forceRecompose"></param>
        /// <returns>True if recomposed</returns>
        public bool SetSourceSlot(ItemSlot nowSlot, bool forceRecompose = false)
        {
            bool recompose = forceRecompose 
                || ((this.curStack == null) != (nowSlot?.Itemstack == null))
                || (nowSlot?.Itemstack != null && !nowSlot.Itemstack.Equals(api.World, curStack, RecompCheckIgnoredStackAttributes))
            ;

            if (nowSlot?.Itemstack == null)
            {
                this.curSlot = null;
            }

            if (recompose)
            {
                this.curSlot = nowSlot;
                this.curStack = nowSlot?.Itemstack?.Clone();

                if (nowSlot?.Itemstack == null)
                {
                    Bounds.fixedHeight = 0;
                }
                
                AsyncRecompose();
            }

            return recompose;
        }


        public override void Dispose()
        {
            base.Dispose();
            texture.Dispose();
            descriptionElement?.Dispose();
            titleElement?.Dispose();
        }
    }
}
