using System.Collections.Generic;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.Common;
using System;
using System.Linq;

namespace Vintagestory.API.Client
{
    public class StacklistElement
    {
        public ItemStack Stack;
        public LoadedTexture Texture;
        public bool Visible;
        public string TextCache;

        public void RenderTo(ICoreClientAPI capi, double x, double y)
        {
            float size = (float)GuiElement.scaled(25);
            float pad = (float)GuiElement.scaled(10);
            capi.Render.RenderItemstackToGui(Stack, x + pad + size/2 , y + size / 2, 100, size, ColorUtil.WhiteArgb, true, false, false);

            if (Texture == null)
            {
                Texture = new TextTextureUtil(capi).GenTextTexture(Stack.GetName(), CairoFont.WhiteSmallText());
            }

            capi.Render.Render2DTexturePremultipliedAlpha(
                Texture.TextureId,
                (x + size + GuiElement.scaled(25)), 
                y + size / 4 - 3,
                Texture.Width,
                Texture.Height,
                50
            );
        }

        public void Dispose() { Texture?.Dispose(); }
    }

    public class GuiElementStacklist : GuiElement
    {
        public List<StacklistElement> Elements = new List<StacklistElement>();

        public int unscaledCellSpacing = 5;
        public int unscaledCellHeight = 40;

        public API.Common.Action<int> onLeftClick;

        LoadedTexture hoverOverlayTexture;
        public ElementBounds insideBounds;

        public GuiElementStacklist(ICoreClientAPI capi, ElementBounds bounds, API.Common.Action<int> onLeftClick, List<StacklistElement> elements = null) : base(capi, bounds)
        {
            hoverOverlayTexture = new LoadedTexture(capi);

            insideBounds = new ElementBounds().WithFixedPadding(unscaledCellSpacing).WithEmptyParent();
            insideBounds.CalcWorldBounds();

            this.onLeftClick = onLeftClick;
            if (elements != null)
            {
                Elements = elements;
            }

            CalcTotalHeight();
        }
        

        public void CalcTotalHeight()
        {
            double height = Elements.Where(e => e.Visible).Count() * (unscaledCellHeight + unscaledCellSpacing);
            insideBounds.fixedHeight = height + unscaledCellSpacing;
        }

        public override void ComposeElements(Context ctxStatic, ImageSurface surfaceStatic)
        {
            insideBounds = new ElementBounds().WithFixedPadding(unscaledCellSpacing).WithEmptyParent();
            insideBounds.CalcWorldBounds();
            CalcTotalHeight();
            Bounds.CalcWorldBounds();

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)Bounds.InnerWidth, (int)GuiElement.scaled(unscaledCellHeight));
            Context ctx = new Context(surface);

            ctx.SetSourceRGBA(1, 1, 1, 0.5);
            ctx.Paint();

            generateTexture(surface, ref hoverOverlayTexture);

            ctx.Dispose();
            surface.Dispose();
        }



        public override void OnMouseUpOnElement(ICoreClientAPI api, MouseEvent args)
        {
            if (!Bounds.ParentBounds.PointInside(args.X, args.Y)) return;

            int i = 0;

            int mx = api.Input.MouseX;
            int my = api.Input.MouseY;
            double posY = insideBounds.absY;


            foreach (StacklistElement element in Elements)
            {
                if (!element.Visible)
                {
                    i++;
                    continue;
                }

                float y = (float)(5 + Bounds.absY + posY);

                if (mx > Bounds.absX && mx <= Bounds.absX + Bounds.InnerWidth && my >= y - 8 && my <= y + scaled(unscaledCellHeight) - 8)
                {
                    api.Gui.PlaySound("menubutton_press");
                    onLeftClick?.Invoke(i);
                    args.Handled = true;
                    return;
                }
                
                posY += scaled(unscaledCellHeight + unscaledCellSpacing);
                i++;
            }
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            int mx = api.Input.MouseX;
            int my = api.Input.MouseY;

            double posY = insideBounds.absY;

            foreach (StacklistElement element in Elements)
            {
                if (!element.Visible) continue;

                float y = (float)(5 + Bounds.absY + posY);

                if (mx > Bounds.absX && mx <= Bounds.absX + Bounds.InnerWidth && my >= y-8 && my <= y + scaled(unscaledCellHeight)-8)
                {
                    api.Render.Render2DLoadedTexture(hoverOverlayTexture, (float)Bounds.absX, y-8);
                }

                if (posY > -50 && posY < Bounds.OuterHeight + 50)
                {
                    element.RenderTo(api, Bounds.absX, y);
                }

                posY += scaled(unscaledCellHeight + unscaledCellSpacing);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            hoverOverlayTexture.Dispose();

            foreach (var val in Elements)
            {
                val.Dispose();
            }
        }

    }

    public static partial class GuiComposerHelpers
    {

        public static GuiComposer AddStacklist(this GuiComposer composer, ElementBounds bounds, API.Common.Action<int> onleftClick = null, List<StacklistElement> stacks = null, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementStacklist(composer.Api, bounds, onleftClick, stacks), key);
            }

            return composer;
        }

        public static GuiElementStacklist GetStacklist(this GuiComposer composer, string key)
        {
            return (GuiElementStacklist)composer.GetElement(key);
        }
    }

}
