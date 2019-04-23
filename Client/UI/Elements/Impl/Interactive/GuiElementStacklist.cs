using System.Collections.Generic;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.Common;
using System;
using System.Linq;
using VintagestoryAPI.Util;

namespace Vintagestory.API.Client
{
    public abstract class GuiListElement
    {
        public abstract string PageCode { get; }

        public abstract void RenderTo(ICoreClientAPI capi, double x, double y);
        public abstract void Dispose();
        public bool Visible;

        public abstract RichTextComponentBase[] GetPageText(ICoreClientAPI capi, ItemStack[] allStacks, Common.Action<string> openDetailPageFor);
        public abstract bool MatchesText(string text);
    }

    public class GroupedHandbookStacklistElement : HandbookStacklistElement
    {
        public List<ItemStack> Stacks = new List<ItemStack>();
        public string Name;

        public override string PageCode => Name;

        public override void RenderTo(ICoreClientAPI capi, double x, double y)
        {
            float size = (float)GuiElement.scaled(25);
            float pad = (float)GuiElement.scaled(10);

            int index = (int)((capi.ElapsedMilliseconds / 1000) % Stacks.Count);

            capi.Render.RenderItemstackToGui(Stacks[index], x + pad + size / 2, y + size / 2, 100, size, ColorUtil.WhiteArgb, true, false, false);

            if (Texture == null)
            {
                Texture = new TextTextureUtil(capi).GenTextTexture(Name, CairoFont.WhiteSmallText());
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

        public override RichTextComponentBase[] GetPageText(ICoreClientAPI capi, ItemStack[] allStacks, Common.Action<string> openDetailPageFor)
        {
            return Stacks[0].Collectible.GetHandbookInfo(Stacks[0], capi, allStacks, openDetailPageFor);
        }
    }

    public class HandbookStacklistElement : GuiListElement
    {
        public ItemStack Stack;
        public LoadedTexture Texture;
        public string TextCache;

        public int PageNumber;

        public override string PageCode => PageCodeForCollectible(Stack.Collectible);


        public static string PageCodeForCollectible(CollectibleObject collectible)
        {
            return (collectible is Block ? "block" : "item") + "-" + collectible.Code.ToShortString();
        }

        public override void RenderTo(ICoreClientAPI capi, double x, double y)
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

        public override void Dispose() { Texture?.Dispose(); }

        public override RichTextComponentBase[] GetPageText(ICoreClientAPI capi, ItemStack[] allStacks, Common.Action<string> openDetailPageFor)
        {
            return Stack.Collectible.GetHandbookInfo(Stack, capi, allStacks, openDetailPageFor);
        }

        public override bool MatchesText(string text)
        {
            return TextCache.CaseInsensitiveContains(text);
        }
    }

    public class GuiElementList : GuiElement
    {
        public List<GuiListElement> Elements = new List<GuiListElement>();

        public int unscaledCellSpacing = 5;
        public int unscaledCellHeight = 40;

        public API.Common.Action<int> onLeftClick;

        LoadedTexture hoverOverlayTexture;
        public ElementBounds insideBounds;

        public GuiElementList(ICoreClientAPI capi, ElementBounds bounds, API.Common.Action<int> onLeftClick, List<GuiListElement> elements = null) : base(capi, bounds)
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


            foreach (GuiListElement element in Elements)
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

            foreach (GuiListElement element in Elements)
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

        public static GuiComposer AddList(this GuiComposer composer, ElementBounds bounds, API.Common.Action<int> onleftClick = null, List<GuiListElement> stacks = null, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementList(composer.Api, bounds, onleftClick, stacks), key);
            }

            return composer;
        }

        public static GuiElementList GetList(this GuiComposer composer, string key)
        {
            return (GuiElementList)composer.GetElement(key);
        }
    }

}
