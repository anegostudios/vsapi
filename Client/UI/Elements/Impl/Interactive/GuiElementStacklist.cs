using System.Collections.Generic;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.Common;
using System;
using System.Linq;
using VintagestoryAPI.Util;
using Vintagestory.API.Datastructures;

namespace Vintagestory.API.Client
{
    public abstract class GuiHandbookPage
    {
        public abstract string PageCode { get; }

        public abstract void RenderTo(ICoreClientAPI capi, double x, double y);
        public abstract void Dispose();
        public bool Visible;

        public abstract RichTextComponentBase[] GetPageText(ICoreClientAPI capi, ItemStack[] allStacks, Common.Action<string> openDetailPageFor);
        public abstract bool MatchesText(string text);
    }

    public class GuiHandbookTextPage : GuiHandbookPage
    {
        public string pageCode;
        public string title;
        public string text;

        public LoadedTexture Texture;
        public override string PageCode => pageCode;
        public override void Dispose() { Texture?.Dispose(); }

        RichTextComponentBase[] comps;
        public int PageNumber;

        public GuiHandbookTextPage()
        {
            
        }

        public void Init(ICoreClientAPI capi)
        {
            if (text.Length < 255)
            {
                text = Lang.Get(text);
            }
            comps = VtmlUtil.Richtextify(capi, text, CairoFont.WhiteSmallText().WithLineHeightMultiplier(1.2));
        }

        public override RichTextComponentBase[] GetPageText(ICoreClientAPI capi, ItemStack[] allStacks, Common.Action<string> openDetailPageFor)
        {
            return comps;
        }

        public void Recompose(ICoreClientAPI capi)
        {
            Texture?.Dispose();
            Texture = new TextTextureUtil(capi).GenTextTexture(Lang.Get(title), CairoFont.WhiteSmallText());
        }

        public override bool MatchesText(string text)
        {
            return text.CaseInsensitiveContains(text);
        }

        public override void RenderTo(ICoreClientAPI capi, double x, double y)
        {
            float size = (float)GuiElement.scaled(25);
            float pad = (float)GuiElement.scaled(10);
            //capi.Render.RenderItemstackToGui(Stack, x + pad + size / 2, y + size / 2, 100, size, ColorUtil.WhiteArgb, true, false, false);

            if (Texture == null)
            {
                Recompose(capi);
            }

            capi.Render.Render2DTexturePremultipliedAlpha(
                Texture.TextureId,
                (x + pad),
                y + size / 4 - 3,
                Texture.Width,
                Texture.Height,
                50
            );
        }
    }

    public class GuiHandbookGroupedItemstackPage : GuiHandbookItemStackPage
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

    public class GuiHandbookItemStackPage : GuiHandbookPage
    {
        public ItemStack Stack;
        public LoadedTexture Texture;
        public string TextCache;

        public int PageNumber;

        public override string PageCode => PageCodeForStack(Stack);


        public static string PageCodeForStack(ItemStack stack)
        {
            if (stack.Attributes != null)
            {
                ITreeAttribute tree = stack.Attributes.Clone();
                foreach (var val in GlobalConstants.IgnoredStackAttributes) tree.RemoveAttribute(val);
                tree.RemoveAttribute("durability");

                return (stack.Class == EnumItemClass.Block ? "block" : "item") + "-" + stack.Collectible.Code.ToShortString() + tree.GetHashCode();
            } else
            {
                return (stack.Class == EnumItemClass.Block ? "block" : "item") + "-" + stack.Collectible.Code.ToShortString();
            }
        }

        public void Recompose(ICoreClientAPI capi)
        {
            Texture?.Dispose();
            Texture = new TextTextureUtil(capi).GenTextTexture(Stack.GetName(), CairoFont.WhiteSmallText());
        }

        public override void RenderTo(ICoreClientAPI capi, double x, double y)
        {
            float size = (float)GuiElement.scaled(25);
            float pad = (float)GuiElement.scaled(10);
            capi.Render.RenderItemstackToGui(Stack, x + pad + size/2 , y + size / 2, 100, size, ColorUtil.WhiteArgb, true, false, false);

            if (Texture == null)
            {
                Recompose(capi);
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

        public override void Dispose() {
            Texture?.Dispose();
            Texture = null;
        }

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
        public List<GuiHandbookPage> Elements = new List<GuiHandbookPage>();

        public int unscaledCellSpacing = 5;
        public int unscaledCellHeight = 40;

        public API.Common.Action<int> onLeftClick;

        LoadedTexture hoverOverlayTexture;
        public ElementBounds insideBounds;

        public GuiElementList(ICoreClientAPI capi, ElementBounds bounds, API.Common.Action<int> onLeftClick, List<GuiHandbookPage> elements = null) : base(capi, bounds)
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


            foreach (GuiHandbookPage element in Elements)
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

            foreach (GuiHandbookPage element in Elements)
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

        public static GuiComposer AddList(this GuiComposer composer, ElementBounds bounds, API.Common.Action<int> onleftClick = null, List<GuiHandbookPage> stacks = null, string key = null)
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
