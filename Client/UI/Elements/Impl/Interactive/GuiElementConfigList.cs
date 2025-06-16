using System;
using System.Collections.Generic;
using Cairo;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.Client
{
    public enum EnumItemType
    {
        Item,
        Title
    }

    /// <summary>
    /// A config item for the GUIElementConfigList.
    /// </summary>
    public class ConfigItem
    {
        /// <summary>
        /// Item or title
        /// </summary>
        public EnumItemType Type = EnumItemType.Item;

        /// <summary>
        /// The name of the config item.
        /// </summary>
        public string Key;

        /// <summary>
        /// the value of the config item.  
        /// </summary>
        public string Value;

        /// <summary>
        /// The code of the config item.
        /// </summary>
        public string Code;

        /// <summary>
        /// Has this particular config item errored?
        /// </summary>
        public bool error;

        /// <summary>
        /// The y position of the config item.
        /// </summary>
        public double posY;

        /// <summary>
        /// The height of the config item.
        /// </summary>
        public double height;


        public object Data;
    }

    public delegate void ConfigItemClickDelegate(int index, int indexWithoutTitles);

    /// <summary>
    /// A configurable list of items.  An example of this is the controls in the settings menu.
    /// </summary>
    public class GuiElementConfigList : GuiElementTextBase
    {
        public static double unscaledPadding = 2;

        public double leftWidthRel = 0.65;
        public double rightWidthRel = 0.3;

        public List<ConfigItem> items;

        ConfigItemClickDelegate OnItemClick;

        int textureId;
        LoadedTexture hoverTexture;

        public ElementBounds innerBounds;

        public CairoFont errorFont;
        public CairoFont stdFont;
        public CairoFont titleFont;

        /// <summary>
        /// Creates a new dropdown configuration list.
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="items">The list of items in the configuration.</param>
        /// <param name="OnItemClick">The event fired when the particular item is clicked.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="bounds">the bounds of the element.</param>
        public GuiElementConfigList(ICoreClientAPI capi, List<ConfigItem> items, ConfigItemClickDelegate OnItemClick, CairoFont font, ElementBounds bounds) : base(capi, "", font, bounds)
        {
            hoverTexture = new LoadedTexture(capi);

            this.items = items;
            this.OnItemClick = OnItemClick;
            this.errorFont = font.Clone();
            this.stdFont = font;
            this.titleFont = font.Clone().WithWeight(FontWeight.Bold);
            this.titleFont.Color[3] = 0.6;
        }

        /// <summary>
        /// Automatically adjusts the height of the element.
        /// </summary>
        public void Autoheight() {
            double totalHeight = 9;
            double pad = scaled(unscaledPadding);

            Bounds.CalcWorldBounds();
            bool first = true;

            foreach (ConfigItem item in items)
            {
                double lineHeight = Math.Max(
                    textUtil.GetMultilineTextHeight(Font, item.Key, Bounds.InnerWidth * leftWidthRel),
                    textUtil.GetMultilineTextHeight(Font, item.Value, Bounds.InnerWidth * rightWidthRel)
                );

                if (!first && item.Type == EnumItemType.Title) lineHeight += scaled(20);

                totalHeight += pad + lineHeight + pad;

                first = false;
            }

            innerBounds = Bounds.FlatCopy();
            innerBounds.fixedHeight = totalHeight / RuntimeEnv.GUIScale; // Unscaled value!
            innerBounds.CalcWorldBounds();
        }

        public override void ComposeTextElements(Context ctxs, ImageSurface surfaces)
        {
            ImageSurface surface = new ImageSurface(Format.Argb32, 200, 10);
            Context ctx = genContext(surface);
            ctx.SetSourceRGBA(1, 1, 1, 0.4);
            ctx.Paint();
            generateTexture(surface, ref hoverTexture);
            ctx.Dispose();
            surface.Dispose();

            Refresh();
        }

        /// <summary>
        /// Refreshes the Config List.
        /// </summary>
        public void Refresh()
        {
            Autoheight();

            ImageSurface surface = new ImageSurface(Format.Argb32, innerBounds.OuterWidthInt, innerBounds.OuterHeightInt);
            Context ctx = genContext(surface);

            double height = 4;
            double pad = scaled(unscaledPadding);
            bool first = true;

            foreach (ConfigItem item in items)
            {
                if (item.error)
                {
                    Font = errorFont;
                } else
                {
                    Font = stdFont;
                }
                if (item.Type == EnumItemType.Title) Font = titleFont;


                double offY = !first && item.Type == EnumItemType.Title ? scaled(20) : 0;

                double leftHeight = textUtil.AutobreakAndDrawMultilineTextAt(ctx, Font, item.Key, 0, offY + height + pad, innerBounds.InnerWidth * leftWidthRel, EnumTextOrientation.Left);
                double rightHeight = textUtil.AutobreakAndDrawMultilineTextAt(ctx, Font, item.Value, innerBounds.InnerWidth * (1 - rightWidthRel), offY + height + pad, innerBounds.InnerWidth * rightWidthRel, EnumTextOrientation.Left);

                double itemHeight = offY + pad + Math.Max(leftHeight, rightHeight) + pad;

                item.posY = height;
                item.height = itemHeight;
                
                height += itemHeight;

                first = false;
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

                    if (item.Type != EnumItemType.Title && diff > 0 && diff < item.height)
                    {
                        api.Render.Render2DTexturePremultipliedAlpha(hoverTexture.TextureId, (int)innerBounds.absX, (int)innerBounds.absY + (int)item.posY, innerBounds.InnerWidth, item.height);
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
                int elemIndex = 0;
                int elemNoTitleIndex = 0;
                foreach (ConfigItem item in items)
                {
                    diff = mouseY - (item.posY + innerBounds.absY);

                    if (item.Type != EnumItemType.Title && diff > 0 && diff < item.height)
                    {
                        OnItemClick(elemIndex, elemNoTitleIndex);
                    }

                    elemIndex++;


                    if (item.Type != EnumItemType.Title)
                    {
                        elemNoTitleIndex++;
                    }
                }
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
        /// Adds a config List to the current GUI.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="items">The items to add.</param>
        /// <param name="onItemClick">The event fired when the item is clicked.</param>
        /// <param name="font">The font of the Config List.</param>
        /// <param name="bounds">The bounds of the config list.</param>
        /// <param name="key">The name of the config list.</param>
        public static GuiComposer AddConfigList(this GuiComposer composer, List<ConfigItem> items, ConfigItemClickDelegate onItemClick, CairoFont font, ElementBounds bounds, string key = null)
        {
            if (!composer.Composed)
            {
                GuiElementConfigList element = new GuiElementConfigList(composer.Api, items, onItemClick, font, bounds);

                composer.AddInteractiveElement(element, key);
            }
            return composer;
        }

        /// <summary>
        /// Gets the config list by name.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="key">The name of the config list.</param>
        /// <returns></returns>
        public static GuiElementConfigList GetConfigList(this GuiComposer composer, string key)
        {
            return (GuiElementConfigList)composer.GetElement(key);
        }
    }

}
