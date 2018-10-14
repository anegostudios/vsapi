using System;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class GuiElementVerticalTabs : GuiElementTextControl
    {
        API.Common.Action<int> handler;

        internal GuiTab[] tabs;

        LoadedTexture baseTexture;
        LoadedTexture[] hoverTextures;
        int[] tabWidths;

        public int activeElement = 0;

        double unscaledTabSpacing = 5;
        double unscaledTabHeight = 25;

        double tabHeight;

        public override bool Focusable { get { return true; } }

        /// <summary>
        /// Creates a new vertical tab group.
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="tabs">The collection of individual tabs.</param>
        /// <param name="font">The font for the group of them all.</param>
        /// <param name="bounds">The bounds of the tabs.</param>
        /// <param name="onTabClicked">The event fired when the tab is clicked.</param>
        public GuiElementVerticalTabs(ICoreClientAPI capi, GuiTab[] tabs, CairoFont font, ElementBounds bounds, API.Common.Action<int> onTabClicked) : base(capi, "", font, bounds)
        {
            this.tabs = tabs;
            handler = onTabClicked;
            hoverTextures = new LoadedTexture[tabs.Length];
            for (int i = 0; i < tabs.Length; i++) hoverTextures[i] = new LoadedTexture(capi);
            baseTexture = new LoadedTexture(capi);

            tabWidths = new int[tabs.Length];
        }


        public override void ComposeTextElements(Context ctxStatic, ImageSurface surfaceStatic)
        {
            Bounds.CalcWorldBounds();

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)Bounds.InnerWidth+1, (int)Bounds.InnerHeight+1);
            Context ctx = new Context(surface);


            double radius = scaled(3);
            double spacing = scaled(unscaledTabSpacing);
            double padding = scaled(3);

            tabHeight = scaled(unscaledTabHeight);

            double xpos = 0; // bounds.drawX + spacing;
            double ypos = 0; // bounds.drawY;
            

            Font.Color[3] = 0.75;
            Font.SetupContext(ctx);



            for (int i = 0; i < tabs.Length; i++)
            {
                tabWidths[i] = (int)(ctx.TextExtents(tabs[i].name).Width + 1 + 2 * padding);

                xpos = (int)Bounds.InnerWidth + 1;

                ctx.NewPath();
                ctx.MoveTo(xpos, ypos + tabHeight);
                ctx.LineTo(xpos, ypos);
                ctx.LineTo(xpos - tabWidths[i] + radius, ypos);
                ctx.ArcNegative(xpos - tabWidths[i], ypos + radius, radius, 270 * GameMath.DEG2RAD, 180 * GameMath.DEG2RAD);
                ctx.ArcNegative(xpos - tabWidths[i], ypos - radius + tabHeight, radius, 180 * GameMath.DEG2RAD, 90 * GameMath.DEG2RAD);
                ctx.ClosePath();

                double[] color = ElementGeometrics.DialogDefaultBgColor;
                ctx.SetSourceRGBA(color[0], color[1], color[2], color[3] * 0.75);

                ctx.FillPreserve();

                ShadePath(ctx, 2);

                Font.SetupContext(ctx);

                ShowTextCorrectly(ctx, tabs[i].name, xpos - tabWidths[i] + padding, ypos + 2);

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
            double radius = scaled(3);
            double spacing = scaled(unscaledTabSpacing);
            double padding = scaled(3);
            double width;

            for (int i = 0; i < tabs.Length; i++)
            {
                ImageSurface surface = new ImageSurface(Format.Argb32, tabWidths[i]+1, (int)tabHeight + 2);
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

                double[] color = ElementGeometrics.DialogDefaultBgColor;
                ctx.SetSourceRGBA(color[0], color[1], color[2], color[3] * 0.75);
                ctx.Fill();

                ctx.NewPath();
                ctx.MoveTo(width, tabHeight);
                ctx.LineTo(width, 0);
                ctx.LineTo(radius, 0);
                ctx.ArcNegative(0, radius, radius, 270 * GameMath.DEG2RAD, 180 * GameMath.DEG2RAD);
                ctx.ArcNegative(0, tabHeight - radius, radius, 180 * GameMath.DEG2RAD, 90 * GameMath.DEG2RAD);
                ctx.Clip();

                ShadePath(ctx, 2);


                Font.SetupContext(ctx);

                ShowTextCorrectly(ctx, tabs[i].name, padding+3, 2);


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
                if (i == activeElement || (mouseRelX > xposend - tabWidths[i] - 3 && mouseRelX < xposend && mouseRelY > ypos && mouseRelY < ypos + tabHeight))
                {
                    api.Render.Render2DTexturePremultipliedAlpha(hoverTextures[i].TextureId, (int)(Bounds.renderX + xposend - tabWidths[i] - 1), (int)(Bounds.renderY + ypos), tabWidths[i], (int)tabHeight + 2);
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
                SetValue((activeElement + 1) % tabs.Length);
            }

            if (args.KeyCode == (int)GlKeys.Up)
            {
                SetValue(GameMath.Mod(activeElement - 1, tabs.Length));
                args.Handled = true;
            }
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseDownOnElement(api, args);

            double spacing = scaled(unscaledTabSpacing);
            double xposend = Bounds.InnerWidth + 1;
            double ypos = 0;

            int mouseRelX = api.Input.MouseX - (int)Bounds.absX;
            int mouseRelY = api.Input.MouseY - (int)Bounds.absY;

            for (int i = 0; i < tabs.Length; i++)
            {
                bool inx = mouseRelX > xposend - tabWidths[i] - 3 && mouseRelX < xposend;
                bool iny = mouseRelY > i * (int)(tabHeight + spacing) && mouseRelY < (i + 1) * (int)(tabHeight + spacing);

                if (inx && iny)
                {
                    SetValue(i);
                    break;
                }

                ypos += tabHeight + spacing;
            }
        }

        /// <summary>
        /// Switches to a different tab.
        /// </summary>
        /// <param name="selectedIndex">The tab to switch to.</param>
        public void SetValue(int selectedIndex)
        {
            handler(tabs[selectedIndex].index);
            activeElement = selectedIndex;
        }

        /// <summary>
        /// Switches to a different tab.
        /// </summary>
        /// <param name="selectedIndex">The tab to switch to.</param>
        /// <param name="triggerHandler">Whether or not the handler triggers.</param>
        public void SetValue(int selectedIndex, bool triggerHandler)
        {
            if (triggerHandler) handler(tabs[selectedIndex].index);
            activeElement = selectedIndex;
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
        /// <param name="tabs">The tabs being added.</param>
        /// <param name="bounds">The boundaries of the tab group.</param>
        /// <param name="OnTabClicked">The event fired when any of the tabs are clicked.</param>
        /// <param name="key">The name of this tab group.</param>
        public static GuiComposer AddVerticalTabs(this GuiComposer composer, GuiTab[] tabs, ElementBounds bounds, API.Common.Action<int> OnTabClicked, string key = null)
        {
            if (!composer.composed)
            {
                CairoFont font = CairoFont.WhiteDetailText().WithFontSize(17);
                composer.AddInteractiveElement(new GuiElementVerticalTabs(composer.Api, tabs, font, bounds, OnTabClicked), key);
            }

            return composer;
        }

        /// <summary>
        /// Gets the vertical tab group as declared by name.
        /// </summary>
        /// <param name="key">The name of the vertical tab group to get.</param>
        public static GuiElementVerticalTabs GetVerticalTab(this GuiComposer composer, string key)
        {
            return (GuiElementVerticalTabs)composer.GetElement(key);
        }
    }
}
