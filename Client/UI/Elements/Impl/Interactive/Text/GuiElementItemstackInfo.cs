using System;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Common;
using Vintagestory.API.Client;
using Vintagestory.API.Config;

namespace Vintagestory.API.Client
{
    public delegate string InfoTextDelegate(ItemSlot slot);

    

    public class GuiElementItemstackInfo : GuiElementTextBase
    {
        class BufferedElements { public GuiElementRichtext titleElement; public GuiElementRichtext descriptionElement; public LoadedTexture texture; }

        public static double ItemStackSize = GuiElementPassiveItemSlot.unscaledItemSize * 2.5;
        public static int MarginTop = 24;
        public static int BoxWidth = 415;
        public static int MinBoxHeight = 80;

        static double[] backTint = GuiStyle.DialogStrongBgColor;
        static double[] textTint = GuiStyle.DialogDefaultTextColor;

        ItemSlot curSlot;
        ItemStack curStack;
        CairoFont titleFont;

        // Recomposing often is expensive, so we do it in a seperate thread. Then however we need double buffering to not flicker during recompose
        BufferedElements[] eles = new BufferedElements[] { new BufferedElements(), new BufferedElements() };
        int readyBufferIndex = 0;

        int compBufIndex => 1 - readyBufferIndex;

        double maxWidth;

        InfoTextDelegate OnRequireInfoText;

        ElementBounds scissorBounds;

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

            eles[0].texture = new LoadedTexture(capi);
            eles[1].texture = new LoadedTexture(capi);

            ElementBounds textBounds = bounds.CopyOnlySize();
            ElementBounds descBounds = textBounds.CopyOffsetedSibling(ItemStackSize + 50, MarginTop, -ItemStackSize - 50, 0);
            descBounds.WithParent(bounds);
            textBounds.WithParent(bounds);

            eles[0].descriptionElement = new GuiElementRichtext(capi, new RichTextComponentBase[0], descBounds);
            eles[0].descriptionElement.zPos = 1001;

            eles[1].descriptionElement = new GuiElementRichtext(capi, new RichTextComponentBase[0], descBounds);
            eles[1].descriptionElement.zPos = 1001;


            titleFont = Font.Clone();
            titleFont.FontWeight = FontWeight.Bold;

            eles[0].titleElement = new GuiElementRichtext(capi, new RichTextComponentBase[0], textBounds);
            eles[0].titleElement.zPos = 1001;

            eles[1].titleElement = new GuiElementRichtext(capi, new RichTextComponentBase[0], textBounds);
            eles[1].titleElement.zPos = 1001;

            maxWidth = bounds.fixedWidth;
        }


        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            Recompose();
        }

        void RecalcBounds(string title, string desc)
        {
            var buf = eles[compBufIndex];

            buf.descriptionElement.BeforeCalcBounds();
            buf.titleElement.BeforeCalcBounds();

            double currentWidth = Math.Max(buf.descriptionElement.MaxLineWidth, buf.descriptionElement.Bounds.InnerWidth) / RuntimeEnv.GUIScale + 10;
            double unscaledTotalHeight;

            currentWidth += 40 + scaled(GuiElementPassiveItemSlot.unscaledItemSize) * 3;
            currentWidth = Math.Max(currentWidth, buf.descriptionElement.MaxLineWidth / RuntimeEnv.GUIScale + 10);
            currentWidth = Math.Min(currentWidth, maxWidth);

            double descWidth = currentWidth - ItemStackSize - 50;

            Bounds.fixedWidth = currentWidth;
            buf.descriptionElement.Bounds.fixedWidth = descWidth;
            buf.titleElement.Bounds.fixedWidth = currentWidth;
            buf.descriptionElement.Bounds.CalcWorldBounds();

            // Height depends on the width
            double unscaledDescTextHeight = buf.descriptionElement.Bounds.fixedHeight;
            unscaledTotalHeight = Math.Max(unscaledDescTextHeight, 25 + GuiElementPassiveItemSlot.unscaledItemSize * 3);
            buf.titleElement.Bounds.fixedHeight = unscaledTotalHeight;
            buf.descriptionElement.Bounds.fixedHeight = unscaledTotalHeight;
            Bounds.fixedHeight = 25 + unscaledTotalHeight;
        }

        

        void Recompose()
        {
            if (curSlot?.Itemstack == null) return;

            var buf = eles[compBufIndex];

            string title = curSlot.GetStackName();
            string desc = OnRequireInfoText(curSlot);
            desc.TrimEnd();


            buf.titleElement.SetNewText(title, titleFont);
            buf.descriptionElement.SetNewText(desc, Font);

            RecalcBounds(title, desc);


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
                surface.Blur(8.2);

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
                shSurface.Blur(7);
                shSurface.Blur(7);
                shSurface.Blur(7);
                EmbossRoundRectangleElement(shCtx, 0, 0, w, h, true);


                ctx.SetSourceSurface(shSurface, (int)(textBounds.drawX), (int)(textBounds.drawY + scaled(MarginTop)));
                ctx.Rectangle(textBounds.drawX, textBounds.drawY + scaled(MarginTop), w, h);
                ctx.Fill();

                shCtx.Dispose();
                shSurface.Dispose();


                api.Event.EnqueueMainThreadTask(() =>
                {
                    buf.titleElement.Compose(false);
                    buf.descriptionElement.Compose(false);

                    generateTexture(surface, ref buf.texture);

                    ctx.Dispose();
                    surface.Dispose();

                    double offset = (int)(30 + ItemStackSize / 2);
                    scissorBounds = ElementBounds.Fixed(4 + offset - ItemStackSize, 2 + offset + MarginTop - ItemStackSize, ItemStackSize + 38, ItemStackSize + 38).WithParent(Bounds);
                    scissorBounds.CalcWorldBounds();

                    SwapBuffers();

                }, "genstackinfotexture");
            });
        }

        private void SwapBuffers()
        {
            readyBufferIndex = 1 - readyBufferIndex;
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            if (curSlot?.Itemstack == null)
            {
                return;
            }

            var buf = eles[readyBufferIndex];

            api.Render.Render2DTexturePremultipliedAlpha(buf.texture.TextureId, Bounds, 1000);

            buf.titleElement.RenderInteractiveElements(deltaTime);
            buf.descriptionElement.RenderInteractiveElements(deltaTime);

            double offset = (int)scaled(30 + ItemStackSize/2);

            api.Render.PushScissor(scissorBounds);

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
        public void SetSourceSlot(ItemSlot nowSlot)
        {
            bool recompose =
                ((this.curStack == null) != (nowSlot?.Itemstack == null))
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
                
                Recompose();
            }
        }


        public override void Dispose()
        {
            base.Dispose();

            eles[0].texture.Dispose();
            eles[1].texture.Dispose();
            eles[0].descriptionElement?.Dispose();
            eles[1].descriptionElement?.Dispose();
            eles[0].titleElement?.Dispose();
            eles[1].titleElement?.Dispose();
        }
    }
}
