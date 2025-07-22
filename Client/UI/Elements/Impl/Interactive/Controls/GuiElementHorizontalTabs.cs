using Cairo;
using System;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    public class GuiTab
    {
        public int DataInt;
        public string Name;
        public double PaddingTop; // For vertical tabs
        public bool Active;
    }

    public class GuiElementHorizontalTabs : GuiElementTextBase
    {
        Action<int> handler;

        internal GuiTab[] tabs;

        LoadedTexture baseTexture;
        LoadedTexture[] hoverTextures;
        LoadedTexture[] notifyTextures;
        LoadedTexture[] arrowTextures;
        int[] tabWidths;
        double[] tabOffsets;
        CairoFont selectedFont;

        double totalWidth;
        double currentScrollOffset = 0;

        double maxScrollOffset => Math.Max(0, totalWidth - Bounds.InnerWidth + scaled(unscaledTabSpacing));

        bool displayLeftArrow => currentScrollOffset > scaled(unscaledTabSpacing);
        bool displayRightArrow => currentScrollOffset < maxScrollOffset - scaled(unscaledTabSpacing);

        public int activeElement = 0;

        public double unscaledTabSpacing = 5;
        public double unscaledTabPadding = 4;
        public bool[] TabHasAlarm { get; set; }

        public bool AlarmTabs;

        float fontHeight;

        public override bool Focusable { get { return enabled; } }

        /// <summary>
        /// Creates a collection of horizontal tabs.
        /// </summary>
        /// <param name="capi">The client API</param>
        /// <param name="tabs">A collection of GUI tabs.</param>
        /// <param name="font">The font for the name of each tab.</param>
        /// <param name="selectedFont"></param>
        /// <param name="bounds">The bounds of each tab.</param>
        /// <param name="onTabClicked">The event fired whenever the tab is clicked.</param>
        public GuiElementHorizontalTabs(ICoreClientAPI capi, GuiTab[] tabs, CairoFont font, CairoFont selectedFont, ElementBounds bounds, Action<int> onTabClicked) : base(capi, "", font, bounds)
        {
            this.selectedFont = selectedFont;
            this.tabs = tabs;
            TabHasAlarm = new bool[tabs.Length];
            handler = onTabClicked;
            hoverTextures = new LoadedTexture[tabs.Length];
            for (int i = 0; i < tabs.Length; i++) hoverTextures[i] = new LoadedTexture(capi);

            arrowTextures = new LoadedTexture[2];
            arrowTextures[0] = new LoadedTexture(capi);
            arrowTextures[1] = new LoadedTexture(capi);

            tabWidths = new int[tabs.Length];
            tabOffsets = new double[tabs.Length];
            baseTexture = new LoadedTexture(capi);
        }

        CairoFont notifyFont;

        [Obsolete("Use TabHasAlarm[] property instead. Used by the chat window to mark a tab/chat as unread")]
        public void SetAlarmTab(int tabIndex)
        {

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

            totalWidth = 0;

            for (int i = 0; i < tabs.Length; i++)
            {
                tabWidths[i] = (int)(ctx.TextExtents(tabs[i].Name).Width + 2 * padding + 1);
                totalWidth += spacing + tabWidths[i];
            }

            ctx.Dispose();
            surface.Dispose();
            surface = new ImageSurface(Format.Argb32, (int)totalWidth + 1, (int)Bounds.InnerHeight + 1);
            ctx = new Context(surface);

            double xpos = spacing;

            Font.Color[3] = 0.5;

            for (int i = 0; i < tabs.Length; i++)
            {
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

                tabOffsets[i] = xpos;
            }

            Font.Color[3] = 1;

            ComposeOverlays();
            ComposeArrows();

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
                surface.BlurPartial(5.2, 10);

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

        private void ComposeArrows()
        {
            ImageSurface surface = new ImageSurface(Format.Argb32, (int)((Bounds.InnerHeight - 2) / 2), (int)Bounds.InnerHeight - 2);
            Context ctx = genContext(surface);

            // Arrow left
            ctx.SetSourceRGBA(ColorUtil.Hex2Doubles("#a88b6c", 1));
            RoundRectangle(ctx, 0, 0, surface.Width, surface.Height, 1);
            ctx.Fill();

            EmbossRoundRectangleElement(ctx, 0, 0, surface.Width, surface.Height, false, 2, 1);

            ctx.NewPath();
            ctx.LineTo(scaled(1) * Scale + 1, surface.Height - scaled(9.5) * Scale);
            ctx.LineTo((surface.Width - scaled(2) - 1) * Scale, surface.Height - scaled(14.25) * Scale);
            ctx.LineTo((surface.Width - scaled(2) - 1) * Scale, surface.Height - scaled(4.75) * Scale);
            ctx.ClosePath();
            ctx.SetSourceRGBA(1, 1, 1, 1);
            ctx.Fill();

            generateTexture(surface, ref arrowTextures[0]);

            surface.Dispose();
            ctx.Dispose();
            surface = new ImageSurface(Format.Argb32, (int)((Bounds.InnerHeight - 2) / 2), (int)Bounds.InnerHeight - 2);
            ctx = genContext(surface);

            // Arrow right
            ctx.SetSourceRGBA(ColorUtil.Hex2Doubles("#a88b6c", 1));
            RoundRectangle(ctx, 0, 0, surface.Width, surface.Height, 1);
            ctx.Fill();

            EmbossRoundRectangleElement(ctx, 0, 0, surface.Width, surface.Height, false, 2, 1);

            ctx.NewPath();
            ctx.LineTo((surface.Width - scaled(2) - 1) * Scale, surface.Height - scaled(9.5) * Scale);
            ctx.LineTo(scaled(1) * Scale + 1, surface.Height - scaled(14.25) * Scale);
            ctx.LineTo(scaled(1) * Scale + 1, surface.Height - scaled(4.75) * Scale);
            ctx.ClosePath();
            ctx.SetSourceRGBA(1, 1, 1, 1);
            ctx.Fill();

            generateTexture(surface, ref arrowTextures[1]);

            ctx.Dispose();
            surface.Dispose();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            /*double xoffset = 0;
            for (int i = 0; i < activeElement; i++)
            {
                xoffset += tabWidths[i] + scaled(unscaledTabSpacing);
            }
            currentScrollOffset = (xoffset + tabWidths[activeElement] > Bounds.InnerWidth) ? totalWidth - Bounds.InnerWidth + scaled(unscaledTabSpacing) : 0;*/

            api.Render.PushScissor(Bounds, true);
            api.Render.Render2DTexture(baseTexture.TextureId, (int)(Bounds.renderX - currentScrollOffset), (int)Bounds.renderY, (int)totalWidth + 1, (int)Bounds.InnerHeight + 1);
            api.Render.PopScissor();

            double spacing = scaled(unscaledTabSpacing);

            int mouseRelX = api.Input.MouseX - (int)Bounds.absX;
            int mouseRelY = api.Input.MouseY - (int)Bounds.absY;

            double xpos = spacing;

            for (int i = 0; i < tabs.Length; i++)
            {
                if (i == activeElement || mouseRelX > (xpos - currentScrollOffset) && mouseRelX < (xpos + tabWidths[i] - currentScrollOffset) && mouseRelY > 0 && mouseRelY < Bounds.InnerHeight && mouseRelX > 0 && mouseRelX < Bounds.InnerWidth)
                {
                    if (i == activeElement || !(displayLeftArrow && mouseRelX > 0 && mouseRelX < arrowTextures[0].Width) && !(displayRightArrow && mouseRelX > Bounds.InnerWidth - arrowTextures[1].Width && mouseRelX < Bounds.InnerWidth))
                    {
                        api.Render.PushScissor(Bounds, true);
                        api.Render.Render2DTexturePremultipliedAlpha(hoverTextures[i].TextureId, (int)((int)(Bounds.renderX - currentScrollOffset) + xpos), (int)Bounds.renderY, tabWidths[i], (int)Bounds.InnerHeight + 1);
                        api.Render.PopScissor();
                    }
                }

                if (TabHasAlarm[i])
                {
                    api.Render.PushScissor(Bounds, true);
                    api.Render.Render2DTexturePremultipliedAlpha(notifyTextures[i].TextureId, (int)((int)(Bounds.renderX - currentScrollOffset) + xpos), (int)Bounds.renderY, tabWidths[i], (int)Bounds.InnerHeight + 1);
                    api.Render.PopScissor();
                }

                xpos += tabWidths[i] + spacing;
            }

            if (displayLeftArrow) api.Render.Render2DTexturePremultipliedAlpha(arrowTextures[0].TextureId, (int)Bounds.renderX, (int)Bounds.renderY + 1, (int)((Bounds.InnerHeight - 2) / 2), (int)Bounds.InnerHeight - 2);
            if (displayRightArrow) api.Render.Render2DTexturePremultipliedAlpha(arrowTextures[1].TextureId, (int)Bounds.renderX + Bounds.InnerWidth - arrowTextures[1].Width, (int)Bounds.renderY + 1, (int)((Bounds.InnerHeight - 2) / 2), (int)Bounds.InnerHeight - 2);
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

        public override void OnMouseWheel(ICoreClientAPI api, MouseWheelEventArgs args)
        {
            if (!enabled) return;

            if (!Bounds.PointInside(api.Input.MouseX, api.Input.MouseY)) return;
            args.SetHandled(true);

            double dir = args.deltaPrecise * scaled(10.0);
            if (currentScrollOffset <= 0 && dir < 0 || currentScrollOffset >= maxScrollOffset && dir > 0) return;
            currentScrollOffset = Math.Clamp(currentScrollOffset + dir, 0, maxScrollOffset);
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseDownOnElement(api, args);

            double spacing = scaled(unscaledTabSpacing);
            double xpos = spacing;

            int mouseRelX = api.Input.MouseX - (int)Bounds.absX;
            int mouseRelY = api.Input.MouseY - (int)Bounds.absY;

            if (displayLeftArrow && mouseRelX > 0 && mouseRelX < arrowTextures[0].Width)
            {
                currentScrollOffset -= scaled(5);
                return;
            }

            if (displayRightArrow && mouseRelX > Bounds.InnerWidth - arrowTextures[1].Width && mouseRelX < Bounds.InnerWidth)
            {
                currentScrollOffset += scaled(5);
                return;
            }

            for (int i = 0; i < tabs.Length; i++)
            {
                if (mouseRelX > (xpos - currentScrollOffset) && mouseRelX < (xpos + tabWidths[i] - currentScrollOffset) && mouseRelY > 0 && mouseRelY < Bounds.InnerHeight)
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
        /// <param name="callhandler"></param>
        public void SetValue(int selectedIndex, bool callhandler=true)
        {
            if (callhandler)
            {
                handler(tabs[selectedIndex].DataInt);
                api.Gui.PlaySound("menubutton_wood");
            }

            activeElement = selectedIndex;

            double newOffset = tabOffsets[activeElement] - Bounds.InnerWidth + arrowTextures[1].Width;
            if (currentScrollOffset < newOffset)
            {
                currentScrollOffset = Math.Clamp(newOffset, 0, maxScrollOffset);
            }

            newOffset = tabOffsets[activeElement] - tabWidths[activeElement] - scaled(unscaledTabSpacing) - arrowTextures[0].Width;
            if (currentScrollOffset > newOffset)
            {
                currentScrollOffset = Math.Clamp(newOffset, 0, maxScrollOffset);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            baseTexture?.Dispose();
            foreach (var texture in arrowTextures)
            {
                texture.Dispose();
            }

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
        /// <param name="composer"></param>
        /// <param name="tabs">The collection of tabs.</param>
        /// <param name="bounds">The bounds of the horizontal tabs.</param>
        /// <param name="onTabClicked">The event fired when the tab is clicked.</param>
        /// <param name="font">The font of the tabs.</param>
        /// <param name="selectedFont"></param>
        /// <param name="key">The key for the added horizontal tabs.</param>
        public static GuiComposer AddHorizontalTabs(this GuiComposer composer, GuiTab[] tabs, ElementBounds bounds, Action<int> onTabClicked, CairoFont font, CairoFont selectedFont, string key = null)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementHorizontalTabs(composer.Api, tabs, font, selectedFont, bounds, onTabClicked), key);
            }

            return composer;
        }

        /// <summary>
        /// Gets the HorizontalTabs element from the GUI by name.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="key">The key for the horizontal tabs you want to get.</param>
        public static GuiElementHorizontalTabs GetHorizontalTabs(this GuiComposer composer, string key)
        {
            return (GuiElementHorizontalTabs)composer.GetElement(key);
        }
    }
}
