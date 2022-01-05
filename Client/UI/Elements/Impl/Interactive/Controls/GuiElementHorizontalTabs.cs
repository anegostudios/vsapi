using System;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class GuiTab
    {
        public int DataInt;
        public string Name;
        public double PaddingTop; // For vertical tabs
    }

    public class GuiElementHorizontalTabs : GuiElementTextBase
    {
        Action<int> handler;

        internal GuiTab[] tabs;

        LoadedTexture baseTexture;
        LoadedTexture[] hoverTextures;
        LoadedTexture[] notifyTextures;
        int[] tabWidths;
        CairoFont selectedFont;

        public int activeElement = 0;

        public double unscaledTabSpacing = 5;
        public double unscaledTabPadding = 4;

        public bool AlarmTabs;
        int alarmTabIndex = -1;

        float fontHeight;

        public override bool Focusable { get { return true; } }

        /// <summary>
        /// Creates a collection of horizontal tabs.
        /// </summary>
        /// <param name="capi">The client API</param>
        /// <param name="tabs">A collection of GUI tabs.</param>
        /// <param name="font">The font for the name of each tab.</param>
        /// <param name="bounds">The bounds of each tab.</param>
        /// <param name="onTabClicked">The event fired whenever the tab is clicked.</param>
        public GuiElementHorizontalTabs(ICoreClientAPI capi, GuiTab[] tabs, CairoFont font, CairoFont selectedFont, ElementBounds bounds, Action<int> onTabClicked) : base(capi, "", font, bounds)
        {
            this.selectedFont = selectedFont;
            this.tabs = tabs;
            handler = onTabClicked;
            hoverTextures = new LoadedTexture[tabs.Length];
            for (int i = 0; i < tabs.Length; i++) hoverTextures[i] = new LoadedTexture(capi);
            
            tabWidths = new int[tabs.Length];
            baseTexture = new LoadedTexture(capi);
        }

        CairoFont notifyFont;

        public void SetAlarmTab(int tabIndex)
        {
            alarmTabIndex = tabIndex;
        }

        public void WithAlarmTabs(CairoFont notifyFont)
        {
            this.notifyFont = notifyFont;
            notifyTextures = new LoadedTexture[tabs.Length];
            for (int i = 0; i < tabs.Length; i++) notifyTextures[i] = new LoadedTexture(this.api);
            AlarmTabs = true;
            ComposeOverlays(true);
        }


        public override void ComposeTextElements(Context ctxStatic, ImageSurface surfaceStatic)
        {
            ImageSurface surface = new ImageSurface(Format.Argb32, (int)Bounds.InnerWidth + 1, (int)Bounds.InnerHeight + 1);
            Context ctx = new Context(surface);

            Font.SetupContext(ctx);

            fontHeight = (float)Font.GetFontExtents().Height;

            double radius = scaled(1);
            double spacing = scaled(unscaledTabSpacing);
            double padding = scaled(unscaledTabPadding);

            double xpos = spacing;

            Font.Color[3] = 0.5;

            for (int i = 0; i < tabs.Length; i++)
            {
                tabWidths[i] = (int)(ctx.TextExtents(tabs[i].Name).Width + 2 * padding + 1);
                
                ctx.NewPath();
                ctx.MoveTo(xpos, Bounds.InnerHeight);
                ctx.LineTo(xpos, radius);
                ctx.Arc(xpos + radius, radius, radius, 180 * GameMath.DEG2RAD, 270 * GameMath.DEG2RAD);
                ctx.Arc(xpos + tabWidths[i] - radius, radius, radius, -90 * GameMath.DEG2RAD, 0 * GameMath.DEG2RAD);
                ctx.LineTo(xpos + tabWidths[i], Bounds.InnerHeight);
                ctx.ClosePath();

                double[] color = GuiStyle.DialogDefaultBgColor;
                ctx.SetSourceRGBA(color[0], color[1], color[2], color[3] * 0.75);

                ctx.FillPreserve();

                ShadePath(ctx, 2);

                if (AlarmTabs)
                {
                    notifyFont.SetupContext(ctx);
                } else {
                    Font.SetupContext(ctx);
                }

                DrawTextLineAt(ctx, tabs[i].Name, xpos + padding, (surface.Height - fontHeight) / 2);

                xpos += tabWidths[i] + spacing;
            }

            Font.Color[3] = 1;

            ComposeOverlays();

            generateTexture(surface, ref baseTexture);


            ctx.Dispose();
            surface.Dispose();
        }

        private void ComposeOverlays(bool isNotifyTabs = false)
        {
            double radius = scaled(1);
            double spacing = scaled(unscaledTabSpacing);
            double padding = scaled(unscaledTabPadding);

            for (int i = 0; i < tabs.Length; i++)
            {
                ImageSurface surface = new ImageSurface(Format.Argb32, tabWidths[i], (int)Bounds.InnerHeight + 1);
                Context ctx = genContext(surface);

                double degrees = Math.PI / 180.0;

                ctx.SetSourceRGBA(1, 1, 1, 0);
                ctx.Paint();

                ctx.NewPath();
                ctx.MoveTo(0, Bounds.InnerHeight + 1);
                ctx.LineTo(0, radius);
                ctx.Arc(radius, radius, radius, 180 * degrees, 270 * degrees);
                ctx.Arc(tabWidths[i] - radius, radius, radius, -90 * degrees, 0 * degrees);
                ctx.LineTo(tabWidths[i], surface.Height);
                ctx.ClosePath();

                double[] color = GuiStyle.DialogDefaultBgColor;
                ctx.SetSourceRGBA(color[0], color[1], color[2], color[3] * 0.75);
                ctx.FillPreserve();

                ctx.SetSourceRGBA(color[0] * 1.6, color[1] * 1.6, color[2] * 1.6, 1);
                ctx.LineWidth = 2 * 1.75;
                ctx.StrokePreserve();
                surface.Blur(5.2, 0, 0, surface.Width, surface.Height);

                ctx.SetSourceRGBA(color[0], color[1], color[2], color[3] * 0.75);
                ctx.LineWidth = 1;
                ctx.StrokePreserve();

                ctx.NewPath();
                ctx.MoveTo(0, Bounds.InnerHeight);
                ctx.LineTo(0, radius);
                ctx.Arc(radius, radius, radius, 180 * degrees, 270 * degrees);
                ctx.Arc(tabWidths[i] - radius, radius, radius, -90 * degrees, 0 * degrees);
                ctx.LineTo(tabWidths[i], Bounds.InnerHeight);

                ShadePath(ctx, 2);

                if (isNotifyTabs)
                {
                    notifyFont.SetupContext(ctx);
                } else
                {
                    selectedFont.SetupContext(ctx);
                }

                ctx.Operator = Operator.Clear;
                ctx.Rectangle(0, surface.Height - 1, surface.Width, 1);
                ctx.Fill();
                ctx.Operator = Operator.Over;

                DrawTextLineAt(ctx, tabs[i].Name, padding, (surface.Height - fontHeight) / 2);

                if (isNotifyTabs)
                {
                    generateTexture(surface, ref notifyTextures[i]);
                } else
                {
                    generateTexture(surface, ref hoverTextures[i]);
                }
                

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

            double xpos = spacing;

            for (int i = 0; i < tabs.Length; i++)
            {
                if (i == activeElement || mouseRelX > xpos && mouseRelX < xpos + tabWidths[i] && mouseRelY > 0 && mouseRelY < Bounds.InnerHeight)
                {
                    api.Render.Render2DTexturePremultipliedAlpha(hoverTextures[i].TextureId, (int)(Bounds.renderX + xpos), (int)Bounds.renderY, tabWidths[i], (int)Bounds.InnerHeight + 1);
                }

                if (alarmTabIndex == i)
                {
                    api.Render.Render2DTexturePremultipliedAlpha(notifyTextures[i].TextureId, (int)(Bounds.renderX + xpos), (int)Bounds.renderY, tabWidths[i], (int)Bounds.InnerHeight + 1);
                }

                xpos += tabWidths[i] + spacing;
            }
        }

        public override void OnKeyDown(ICoreClientAPI api, KeyEvent args)
        {
            if (!HasFocus) return;
            if (args.KeyCode == (int)GlKeys.Right)
            {
                args.Handled = true;
                SetValue((activeElement + 1) % tabs.Length);
            }

            if (args.KeyCode == (int)GlKeys.Left)
            {
                SetValue(GameMath.Mod(activeElement - 1, tabs.Length));
                args.Handled = true;
            }
        }


        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseDownOnElement(api, args);

            double spacing = scaled(unscaledTabSpacing);
            double xpos = spacing;

            int mouseRelX = api.Input.MouseX - (int)Bounds.absX;
            int mouseRelY = api.Input.MouseY - (int)Bounds.absY;

            for (int i = 0; i < tabs.Length; i++)
            {
                if (mouseRelX > xpos && mouseRelX < xpos + tabWidths[i] && mouseRelY > 0 && mouseRelY < Bounds.InnerHeight)
                {
                    SetValue(i);
                    break;
                }

                xpos += tabWidths[i] + spacing;
            }
        }

        /// <summary>
        /// Sets the current tab to the given index.
        /// </summary>
        /// <param name="selectedIndex">The current index of the tab.</param>
        public void SetValue(int selectedIndex, bool callhandler=true)
        {
            if (callhandler)
            {
                handler(tabs[selectedIndex].DataInt);
                api.Gui.PlaySound("menubutton_wood");
            }

            activeElement = selectedIndex;
        }

        public override void Dispose()
        {
            base.Dispose();

            baseTexture?.Dispose();
            for (int i = 0; i < hoverTextures.Length; i++)
            {
                hoverTextures[i].Dispose();
                if (notifyTextures != null) notifyTextures[i].Dispose();
            }
        }
    }

    public static partial class GuiComposerHelpers
    {

        /// <summary>
        /// Adds a set of horizontal tabs to the GUI.
        /// </summary>
        /// <param name="tabs">The collection of tabs.</param>
        /// <param name="bounds">The bounds of the horizontal tabs.</param>
        /// <param name="OnTabClicked">The event fired when the tab is clicked.</param>
        /// <param name="font">The font of the tabs.</param>
        /// <param name="key">The key for the added horizontal tabs.</param>
        public static GuiComposer AddHorizontalTabs(this GuiComposer composer, GuiTab[] tabs, ElementBounds bounds, Action<int> OnTabClicked, CairoFont font, CairoFont selectedFont, string key = null)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementHorizontalTabs(composer.Api, tabs, font, selectedFont, bounds, OnTabClicked), key);
            }

            return composer;
        }

        /// <summary>
        /// Gets the HorizontalTabs element from the GUI by name.
        /// </summary>
        /// <param name="key">The key for the horizontal tabs you want to get.</param>
        public static GuiElementHorizontalTabs GetHorizontalTabs(this GuiComposer composer, string key)
        {
            return (GuiElementHorizontalTabs)composer.GetElement(key);
        }
    }
}
