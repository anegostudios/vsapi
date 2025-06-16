using System;
using Cairo;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    public class GuiElementVerticalTabs : GuiElementTextBase
    {
        protected Action<int, GuiTab> handler;
        protected GuiTab[] tabs;
        protected LoadedTexture baseTexture;
        protected LoadedTexture[] hoverTextures;
        protected int[] tabWidths;
        protected CairoFont selectedFont;
        protected double unscaledTabSpacing = 5;
        protected double unscaledTabHeight = 25;
        protected double unscaledTabPadding = 3;
        protected double tabHeight;
        protected double textOffsetY;

        public int ActiveElement = 0;
        public bool Right;

        /// <summary>
        /// If true, more than one tab can be active
        /// </summary>
        public bool ToggleTabs = false;

        public override bool Focusable { get { return enabled; } }

        /// <summary>
        /// Creates a new vertical tab group.
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="tabs">The collection of individual tabs.</param>
        /// <param name="font">The font for the group of them all.</param>
        /// <param name="selectedFont"></param>
        /// <param name="bounds">The bounds of the tabs.</param>
        /// <param name="onTabClicked">The event fired when the tab is clicked.</param>
        public GuiElementVerticalTabs(ICoreClientAPI capi, GuiTab[] tabs, CairoFont font, CairoFont selectedFont, ElementBounds bounds, Action<int, GuiTab> onTabClicked) : base(capi, "", font, bounds)
        {
            this.selectedFont = selectedFont;
            this.tabs = tabs;
            handler = onTabClicked;
            hoverTextures = new LoadedTexture[tabs.Length];
            for (int i = 0; i < tabs.Length; i++) hoverTextures[i] = new LoadedTexture(capi);
            baseTexture = new LoadedTexture(capi);

            tabWidths = new int[tabs.Length];
            if (tabs.Length > 0) tabs[0].Active = true;
        }


        public override void ComposeTextElements(Context ctxStatic, ImageSurface surfaceStatic)
        {
            Bounds.CalcWorldBounds();

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)Bounds.InnerWidth+1, (int)Bounds.InnerHeight+1);
            Context ctx = new Context(surface);
            
            double radius = scaled(1);
            double spacing = scaled(unscaledTabSpacing);
            double padding = scaled(unscaledTabPadding);

            tabHeight = scaled(unscaledTabHeight);

            double xpos;
            double ypos = 0;
            

            Font.Color[3] = 0.85;
            Font.SetupContext(ctx);
			textOffsetY = (tabHeight + 1 - Font.GetFontExtents().Height) / 2;

            double maxWidth = 0;
            for (int i = 0; i < tabs.Length; i++)
            {
                double w = (int)(ctx.TextExtents(tabs[i].Name).Width + 1 + 2 * padding);

                maxWidth = Math.Max(w, maxWidth);
            }


            for (int i = 0; i < tabs.Length; i++)
            {
                tabWidths[i] = (int)maxWidth + 1;

                if (Right)
                {
                    xpos = 1;
                    ypos += tabs[i].PaddingTop;

                    ctx.NewPath();
                    ctx.MoveTo(xpos, ypos + tabHeight);
                    ctx.LineTo(xpos, ypos);
                    ctx.LineTo(xpos + tabWidths[i] + radius, ypos);
                    ctx.ArcNegative(xpos + tabWidths[i], ypos + radius, radius, 270 * GameMath.DEG2RAD, 180 * GameMath.DEG2RAD);
                    ctx.ArcNegative(xpos + tabWidths[i], ypos - radius + tabHeight, radius, 180 * GameMath.DEG2RAD, 90 * GameMath.DEG2RAD);

                } else {

                    xpos = (int)Bounds.InnerWidth + 1;
                    ypos += tabs[i].PaddingTop;

                    ctx.NewPath();
                    ctx.MoveTo(xpos, ypos + tabHeight);
                    ctx.LineTo(xpos, ypos);
                    ctx.LineTo(xpos - tabWidths[i] + radius, ypos);
                    ctx.ArcNegative(xpos - tabWidths[i], ypos + radius, radius, 270 * GameMath.DEG2RAD, 180 * GameMath.DEG2RAD);
                    ctx.ArcNegative(xpos - tabWidths[i], ypos - radius + tabHeight, radius, 180 * GameMath.DEG2RAD, 90 * GameMath.DEG2RAD);

                }

                ctx.ClosePath();
                double[] color = GuiStyle.DialogDefaultBgColor;
                ctx.SetSourceRGBA(color[0], color[1], color[2], color[3]);

                ctx.FillPreserve();

                ShadePath(ctx, 2);

                Font.SetupContext(ctx);

                DrawTextLineAt(ctx, tabs[i].Name, xpos - (Right ? 0 : tabWidths[i]) + padding, ypos + textOffsetY);

                ypos += tabHeight + spacing;
            }

            Font.Color[3] = 1;

            ComposeOverlays();

            generateTexture(surface, ref baseTexture);

            ctx.Dispose();
            surface.Dispose();
        }


        private void ComposeOverlays()
        {
            double radius = scaled(1);
            double padding = scaled(unscaledTabPadding);
            double width;

            for (int i = 0; i < tabs.Length; i++)
            {
                ImageSurface surface = new ImageSurface(Format.Argb32, tabWidths[i]+1, (int)tabHeight + 1);
                Context ctx = genContext(surface);

                width = tabWidths[i]+1;

                ctx.SetSourceRGBA(1, 1, 1, 0);
                ctx.Paint();

                ctx.NewPath();
                ctx.MoveTo(width, tabHeight + 1);
                ctx.LineTo(width, 0);
                ctx.LineTo(radius, 0);
                ctx.ArcNegative(0, radius, radius, 270 * GameMath.DEG2RAD, 180 * GameMath.DEG2RAD);
                ctx.ArcNegative(0, tabHeight - radius, radius, 180 * GameMath.DEG2RAD, 90 * GameMath.DEG2RAD);
                ctx.ClosePath();

                double[] color = GuiStyle.DialogDefaultBgColor;
                ctx.SetSourceRGBA(color[0], color[1], color[2], color[3]);
                ctx.Fill();

                ctx.NewPath();
                if (Right)
                {
                    ctx.LineTo(1, 1);
                    ctx.LineTo(width, 1);
                    ctx.LineTo(width, 1 + tabHeight - 1);
                    ctx.LineTo(1, tabHeight - 1);
                } else
                {
                    ctx.LineTo(1 + width, 1);
                    ctx.LineTo(1, 1);
                    ctx.LineTo(1, tabHeight - 1);
                    ctx.LineTo(1 + width, 1 + tabHeight - 1);
                }
                
                

                float strokeWidth = 2;
                ctx.SetSourceRGBA(GuiStyle.DialogLightBgColor[0] * 1.6, GuiStyle.DialogStrongBgColor[1] * 1.6, GuiStyle.DialogStrongBgColor[2] * 1.6, 1);
                ctx.LineWidth = strokeWidth * 1.75;
                ctx.StrokePreserve();
                surface.BlurPartial(8, 16);

                ctx.SetSourceRGBA(new double[] { 45 / 255.0, 35 / 255.0, 33 / 255.0, 1 });
                ctx.LineWidth = strokeWidth;
                ctx.Stroke();


                selectedFont.SetupContext(ctx);

                DrawTextLineAt(ctx, tabs[i].Name, padding+2, textOffsetY);


                generateTexture(surface, ref hoverTextures[i]);

                ctx.Dispose();
                surface.Dispose();
            }
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            api.Render.Render2DTexture(baseTexture.TextureId, (int)Bounds.renderX, (int)Bounds.renderY, (int)Bounds.InnerWidth + 1, (int)Bounds.InnerHeight + 1);

            double spacing = scaled(unscaledTabSpacing);

            int mouseRelX = api.Input.MouseX - (int)Bounds.absX;
            int mouseRelY = api.Input.MouseY - (int)Bounds.absY;

            double xposend = (int)Bounds.InnerWidth;
            double ypos = 0;

            for (int i = 0; i < tabs.Length; i++)
            {
                var tab = tabs[i];
                ypos += tab.PaddingTop;

                if (Right)
                {
                    if (tab.Active || (mouseRelX >= 0 && mouseRelX < xposend && mouseRelY > ypos && mouseRelY < ypos + tabHeight))
                    {
                        api.Render.Render2DTexturePremultipliedAlpha(hoverTextures[i].TextureId, (int)Bounds.renderX, (int)(Bounds.renderY + ypos), tabWidths[i] + 1, (int)tabHeight + 1);
                    }
                } else
                {
                    if (tab.Active || (mouseRelX > xposend - tabWidths[i] - 3 && mouseRelX < xposend && mouseRelY > ypos && mouseRelY < ypos + tabHeight))
                    {
                        api.Render.Render2DTexturePremultipliedAlpha(hoverTextures[i].TextureId, (int)(Bounds.renderX + xposend - tabWidths[i] - 1), (int)(Bounds.renderY + ypos), tabWidths[i] + 1, (int)tabHeight + 1);
                    }
                }

                ypos += tabHeight + spacing;
            }
        }


        public override void OnKeyDown(ICoreClientAPI api, KeyEvent args)
        {
            if (!HasFocus) return;
            if (args.KeyCode == (int)GlKeys.Down)
            {
                args.Handled = true;
                SetValue((ActiveElement + 1) % tabs.Length);
            }

            if (args.KeyCode == (int)GlKeys.Up)
            {
                SetValue(GameMath.Mod(ActiveElement - 1, tabs.Length));
                args.Handled = true;
            }
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            double spacing = scaled(unscaledTabSpacing);
            double xposend = Bounds.InnerWidth + 1;
            double ypos = 0;

            int mouseRelX = api.Input.MouseX - (int)Bounds.absX;
            int mouseRelY = api.Input.MouseY - (int)Bounds.absY;

            for (int i = 0; i < tabs.Length; i++)
            {
                ypos += tabs[i].PaddingTop;

                bool inx = mouseRelX > xposend - tabWidths[i] - 3 && mouseRelX < xposend;
                bool iny = mouseRelY > ypos && mouseRelY < ypos + tabHeight + spacing;

                if (inx && iny)
                {
                    SetValue(i);
                    args.Handled = true;
                    break;
                }

                ypos += tabHeight + spacing;
            }
        }

        /// <summary>
        /// Switches to a different tab.
        /// </summary>
        /// <param name="index">The tab to switch to.</param>
        public void SetValue(int index)
        {
            api.Gui.PlaySound("menubutton_wood");
            if (!ToggleTabs) foreach (var tab in tabs) tab.Active = false;
            tabs[index].Active = !tabs[index].Active;

            handler(index, tabs[index]);
            ActiveElement = index;

        }

        /// <summary>
        /// Switches to a different tab.
        /// </summary>
        /// <param name="index">The tab to switch to.</param>
        /// <param name="triggerHandler">Whether or not the handler triggers.</param>
        public void SetValue(int index, bool triggerHandler)
        {
            if (!ToggleTabs) foreach (var tab in tabs) tab.Active = false;
            tabs[index].Active = !tabs[index].Active;

            if (triggerHandler)
            {
                handler(index, tabs[index]);
                api.Gui.PlaySound("menubutton_wood");
            }

            ActiveElement = index;
        }

        public override void Dispose()
        {
            base.Dispose();

            for (int i = 0; i < hoverTextures.Length; i++) hoverTextures[i].Dispose();
            baseTexture.Dispose();
        }
    }

    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Adds multiple tabs to a group of vertical tabs.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="tabs">The tabs being added.</param>
        /// <param name="bounds">The boundaries of the tab group.</param>
        /// <param name="onTabClicked">The event fired when any of the tabs are clicked.</param>
        /// <param name="key">The name of this tab group.</param>
        public static GuiComposer AddVerticalToggleTabs(this GuiComposer composer, GuiTab[] tabs, ElementBounds bounds, Action<int, GuiTab> onTabClicked, string key = null)
        {
            if (!composer.Composed)
            {
                CairoFont font = CairoFont.WhiteDetailText().WithFontSize(17);
                CairoFont selectedFont = CairoFont.WhiteDetailText().WithFontSize(17).WithColor(GuiStyle.ActiveButtonTextColor);
                var tabsElem = new GuiElementVerticalTabs(composer.Api, tabs, font, selectedFont, bounds, onTabClicked);
                tabsElem.ToggleTabs = true;
                composer.AddInteractiveElement(tabsElem, key);
            }

            return composer;
        }

        /// <summary>
        /// Adds multiple tabs to a group of vertical tabs.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="tabs">The tabs being added.</param>
        /// <param name="bounds">The boundaries of the tab group.</param>
        /// <param name="onTabClicked">The event fired when any of the tabs are clicked.</param>
        /// <param name="key">The name of this tab group.</param>
        public static GuiComposer AddVerticalTabs(this GuiComposer composer, GuiTab[] tabs, ElementBounds bounds, Action<int, GuiTab> onTabClicked, string key = null)
        {
            if (!composer.Composed)
            {
                CairoFont font = CairoFont.WhiteDetailText().WithFontSize(17);
                CairoFont selectedFont = CairoFont.WhiteDetailText().WithFontSize(17).WithColor(GuiStyle.ActiveButtonTextColor);
                composer.AddInteractiveElement(new GuiElementVerticalTabs(composer.Api, tabs, font, selectedFont, bounds, onTabClicked), key);
            }

            return composer;
        }

        /// <summary>
        /// Gets the vertical tab group as declared by name.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="key">The name of the vertical tab group to get.</param>
        public static GuiElementVerticalTabs GetVerticalTab(this GuiComposer composer, string key)
        {
            return (GuiElementVerticalTabs)composer.GetElement(key);
        }
    }
}