using System;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Common;
using Vintagestory.API.Client;
using Vintagestory.API.Config;

namespace Vintagestory.API.Client
{
    public delegate string InfoTextDelegate(ItemSlot slot);

    class GuiElementItemstackInfo : GuiElementTextBase
    {
        public static double ItemStackSize = GuiElementPassiveItemSlot.unscaledItemSize * 2.5;
        public static int MarginTop = 24;
        public static int BoxWidth = 400;
        public static int MinBoxHeight = 80;

        static double[] backTint = ElementGeometrics.DialogStrongBgColor;
        static double[] textTint = ElementGeometrics.DialogDefaultTextColor;

        ItemSlot curSlot;
        ItemStack curStack;


        GuiElementStaticText titleElement;
        GuiElementStaticText descriptionElement;

        LoadedTexture texture;
        double maxWidth;

        InfoTextDelegate OnRequireInfoText;

        public GuiElementItemstackInfo(ICoreClientAPI capi, ElementBounds bounds, InfoTextDelegate OnRequireInfoText) : base(capi, "", CairoFont.WhiteSmallText(), bounds)
        {
            this.OnRequireInfoText = OnRequireInfoText;

            texture = new LoadedTexture(capi);

            ElementBounds textBounds = bounds.CopyOnlySize();

            descriptionElement = new GuiElementStaticText(capi, "", EnumTextOrientation.Left, textBounds.CopyOffsetedSibling(ItemStackSize + 50, MarginTop, -ItemStackSize - 50, 0), Font);

            CairoFont titleFont = Font.Clone();
            titleFont.FontWeight = FontWeight.Bold;
            titleElement = new GuiElementStaticText(capi, "", EnumTextOrientation.Left, textBounds, titleFont);

            maxWidth = bounds.fixedWidth;
        }


        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            Recompose();
        }

        void RecalcBounds(string title, string desc)
        {
            double currentWidth = 0;
            double currentHeight = 0;
            string[] lines = desc.Split(new char[] { '\n' });
            
            

            for (int i = 0; i < lines.Length; i++)
            {
                currentWidth = Math.Max(currentWidth, descriptionElement.Font.GetTextExtents(lines[i]).Width / RuntimeEnv.GUIScale + 10);
            }

            currentWidth += 40 + scaled(GuiElementPassiveItemSlot.unscaledItemSize) * 3;
            currentWidth = Math.Max(currentWidth, titleElement.Font.GetTextExtents(title).Width / RuntimeEnv.GUIScale + 10);
            currentWidth = Math.Min(currentWidth, maxWidth);

            double descWidth = currentWidth - scaled(ItemStackSize) - 50;

            Bounds.fixedWidth = currentWidth;
            descriptionElement.Bounds.fixedWidth = descWidth;
            titleElement.Bounds.fixedWidth = currentWidth;
            descriptionElement.Bounds.CalcWorldBounds();

            // Height depends on the width
            double lineheight = descriptionElement.GetMultilineTextHeight();
            currentHeight = Math.Max(lineheight, 50 + scaled(GuiElementPassiveItemSlot.unscaledItemSize) * 3);
            titleElement.Bounds.fixedHeight = currentHeight;
            descriptionElement.Bounds.fixedHeight = currentHeight;
            Bounds.fixedHeight = currentHeight / RuntimeEnv.GUIScale;
        }


        void Recompose()
        {
            if (curSlot?.Itemstack == null) return;

            string title = curSlot.GetStackName();
            string desc = OnRequireInfoText(curSlot);
            desc.TrimEnd();


            titleElement.SetValue(title);
            descriptionElement.SetValue(desc);

            RecalcBounds(title, desc);


            Bounds.CalcWorldBounds();

            ElementBounds textBounds = Bounds.CopyOnlySize();
            textBounds.CalcWorldBounds();


            ImageSurface surface = new ImageSurface(Format.Argb32, Bounds.OuterWidthInt, Bounds.OuterHeightInt);
            Context ctx = genContext(surface);

            ctx.SetSourceRGBA(0, 0, 0, 0);
            ctx.Paint();

            ctx.SetSourceRGBA(backTint[0], backTint[1], backTint[2], backTint[3]);
            RoundRectangle(ctx, textBounds.bgDrawX, textBounds.bgDrawY, textBounds.OuterWidthInt, textBounds.OuterHeightInt, ElementGeometrics.DialogBGRadius);
            ctx.FillPreserve();
            ctx.SetSourceRGBA(backTint[0] / 2, backTint[1] / 2, backTint[2] / 2, backTint[3]);
            ctx.Stroke();

            ctx.SetSourceRGBA(ElementGeometrics.DialogAlternateBgColor);
            RoundRectangle(ctx, textBounds.drawX, textBounds.drawY + scaled(MarginTop), scaled(ItemStackSize) + scaled(40), scaled(ItemStackSize) + scaled(40), 0);
            ctx.Fill();


            titleElement.ComposeElements(ctx, surface);

            descriptionElement.ComposeElements(ctx, surface);

            generateTexture(surface, ref texture);

            ctx.Dispose();
            surface.Dispose();
        }


        public override void RenderInteractiveElements(float deltaTime)
        {
            if (curSlot?.Itemstack == null) return;

            api.Render.Render2DTexturePremultipliedAlpha(texture.TextureId, Bounds, 1000);

            double offset = (int)scaled(30 + ItemStackSize/2);

            api.Render.RenderItemstackToGui(
                curSlot.Itemstack,
                (int)Bounds.renderX + offset,
                (int)Bounds.renderY + offset + (int)scaled(MarginTop), 
                1000 + scaled(GuiElementPassiveItemSlot.unscaledItemSize) * 2, 
                (float)scaled(ItemStackSize), 
                ColorUtil.WhiteArgb,
                true,
                true,
                false
            );
        }



        
        


        public ItemSlot GetSlot()
        {
            return curSlot;
        }

        public void SetSourceSlot(ItemSlot nowSlot)
        {
            bool recompose = this.curStack == null || (nowSlot?.Itemstack != null && !nowSlot.Itemstack.Equals(curStack));

            this.curSlot = nowSlot;
            this.curStack = nowSlot?.Itemstack;

            if (nowSlot?.Itemstack == null)
            {
                Bounds.fixedHeight = 0;
            }

            if (recompose) Recompose();
        }


        public override void Dispose()
        {
            base.Dispose();

            texture.Dispose();
        }
    }
}
