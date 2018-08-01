using System;
using System.Collections.Generic;
using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.Config;

namespace Vintagestory.API.Client
{
    public class ConfigItem
    {
        public string key;
        public string value;

        public string code;

        public bool error;

        public double posY;
        public double height;
    }

    public class GuiElementConfigList : GuiElementTextBase
    {
        public static double unscaledPadding = 2;

        public double leftWidthRel = 0.65;
        public double rightWidthRel = 0.3;

        public List<ConfigItem> items;

        Action<int> OnItemClick;

        int textureId;
        int hoverTextureId;

        public ElementBounds innerBounds;

        public CairoFont errorFont;
        public CairoFont stdFont;

        public GuiElementConfigList(ICoreClientAPI capi, List<ConfigItem> items, Action<int> OnItemClick, CairoFont font, ElementBounds bounds) : base(capi, "", font, bounds)
        {
            this.items = items;
            this.OnItemClick = OnItemClick;
            this.errorFont = font.Clone();
            this.stdFont = font;

            ImageSurface surface = new ImageSurface(Format.Argb32, 200, 10);
            Context ctx = genContext(surface);
            ctx.SetSourceRGBA(1, 1, 1, 0.4);
            ctx.Paint();
            generateTexture(surface, ref hoverTextureId);
            ctx.Dispose();
            surface.Dispose();
        }

        public void Autoheight() {
            double totalHeight = 0;
            double pad = scaled(unscaledPadding);

            Bounds.CalcWorldBounds();

            foreach (ConfigItem item in items)
            {
                double lineHeight = Math.Max(
                    GetMultilineTextHeight(item.key, Bounds.InnerWidth * leftWidthRel), 
                    GetMultilineTextHeight(item.value, Bounds.InnerWidth * rightWidthRel)
                );

                totalHeight += pad + lineHeight + pad;
            }

            innerBounds = Bounds.FlatCopy();
            innerBounds.fixedHeight = totalHeight / RuntimeEnv.GUIScale; // Unscaled value!
            innerBounds.CalcWorldBounds();
        }

        public override void ComposeTextElements(Context ctx, ImageSurface surface)
        {
            Refresh();
        }


        public void Refresh()
        {
            Autoheight();

            ImageSurface surface = new ImageSurface(Format.Argb32, innerBounds.OuterWidthInt, innerBounds.OuterHeightInt);
            Context ctx = genContext(surface);

            double height = 0;
            double pad = scaled(unscaledPadding);


            foreach (ConfigItem item in items)
            {
                if (item.error)
                {
                    Font = errorFont;
                } else
                {
                    Font = stdFont;
                }

                double leftHeight = ShowMultilineText(ctx, item.key, 0, height + pad, innerBounds.InnerWidth * leftWidthRel, EnumTextOrientation.Left);
                double rightHeight = ShowMultilineText(ctx, item.value, innerBounds.InnerWidth * (1 - rightWidthRel), height + pad, innerBounds.InnerWidth * rightWidthRel, EnumTextOrientation.Left);

                double itemHeight = pad + Math.Max(leftHeight, rightHeight) + pad;

                item.posY = height;
                item.height = itemHeight;

                height += itemHeight;
            }

            //surface.WriteToPng("configlist.png");

            generateTexture(surface, ref textureId);



            surface.Dispose();
            ctx.Dispose();
        }


        public override void RenderInteractiveElements(float deltaTime)
        {
            api.Render.Render2DTexturePremultipliedAlpha(textureId, innerBounds);

            int mouseX = api.Input.MouseX;
            int mouseY = api.Input.MouseY;

            double diff;

            if (innerBounds.PointInside(mouseX, mouseY))
            {
                foreach (ConfigItem item in items)
                {
                    diff = mouseY - (item.posY + innerBounds.absY);

                    if (diff > 0 && diff < item.height)
                    {
                        api.Render.Render2DTexturePremultipliedAlpha(hoverTextureId, (int)innerBounds.absX, (int)innerBounds.absY + (int)item.posY, innerBounds.InnerWidth, item.height);
                    }
                }
            }
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            if (!innerBounds.ParentBounds.PointInside(args.X, args.Y)) return;

            int mouseX = api.Input.MouseX;
            int mouseY = api.Input.MouseY; // - (int)bounds.fixedY + 5
            double diff;

            if (innerBounds.PointInside(mouseX, mouseY))
            {
                int i = 0;
                foreach (ConfigItem item in items)
                {
                    diff = mouseY - (item.posY + innerBounds.absY);

                    if (diff > 0 && diff < item.height)
                    {
                        OnItemClick(i);
                    }
                    i++;
                }
            }
        }

    }

    public static partial class GuiComposerHelpers
    {
        public static GuiComposer AddConfigList(this GuiComposer composer, List<ConfigItem> items, Action<int> OnItemClick, CairoFont font, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                GuiElementConfigList element = new GuiElementConfigList(composer.Api, items, OnItemClick, font, bounds);

                composer.AddInteractiveElement(element, key);
            }
            return composer;
        }

        public static GuiElementConfigList GetConfigList(this GuiComposer composer, string key)
        {
            return (GuiElementConfigList)composer.GetElement(key);
        }
    }

}
