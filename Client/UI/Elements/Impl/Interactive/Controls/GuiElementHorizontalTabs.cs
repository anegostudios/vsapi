using System;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class GuiTab
    {
        public int index;
        public string name;
    }

    public class GuiElementHorizontalTabs : GuiElementTextBase
    {
        API.Common.Action<int> handler;

        internal GuiTab[] tabs;

        LoadedTexture[] hoverTextures;
        int[] tabWidths;

        public int activeElement = 0;

        double unscaledTabSpacing = 5;

        public override bool Focusable { get { return true; } }

        public GuiElementHorizontalTabs(ICoreClientAPI capi, GuiTab[] tabs, CairoFont font, ElementBounds bounds, API.Common.Action<int> onTabClicked) : base(capi, "", font, bounds)
        {
            this.tabs = tabs;
            handler = onTabClicked;
            hoverTextures = new LoadedTexture[tabs.Length];
            for (int i = 0; i < tabs.Length; i++) hoverTextures[i] = new LoadedTexture(capi);
            tabWidths = new int[tabs.Length];
        }


        public override void ComposeTextElements(Context ctx, ImageSurface surface)
        {
            double radius = scaled(3);
            double spacing = scaled(unscaledTabSpacing);
            double padding = scaled(3);

            double xpos = spacing;

            Font.Color[3] = 0.5;

            for (int i = 0; i < tabs.Length; i++)
            {

                tabWidths[i] = (int)(ctx.TextExtents(tabs[i].name).Width + 2 * padding + 1);
                
                ctx.NewPath();
                ctx.MoveTo(Bounds.drawX + xpos, Bounds.drawY + Bounds.InnerHeight);
                ctx.LineTo(Bounds.drawX + xpos, Bounds.drawY + radius);
                ctx.Arc(Bounds.drawX + xpos + radius, Bounds.drawY + radius, radius, 180 * GameMath.DEG2RAD, 270 * GameMath.DEG2RAD);
                ctx.Arc(Bounds.drawX + xpos + tabWidths[i] - radius, Bounds.drawY + radius, radius, -90 * GameMath.DEG2RAD, 0 * GameMath.DEG2RAD);
                ctx.LineTo(Bounds.drawX + xpos + tabWidths[i], Bounds.drawY + Bounds.InnerHeight);
                ctx.ClosePath();

                double[] color = ElementGeometrics.DialogDefaultBgColor;
                ctx.SetSourceRGBA(color[0], color[1], color[2], color[3] * 0.75);

                ctx.FillPreserve();

                ShadePath(ctx, 2);

                Font.SetupContext(ctx);

                ShowTextCorrectly(ctx, tabs[i].name, Bounds.drawX + xpos + padding, Bounds.drawY + 1);

                xpos += tabWidths[i] + spacing;
            }

            Font.Color[3] = 1;

            ComposeOverlays();
        }

        private void ComposeOverlays()
        {
            double radius = scaled(3);
            double spacing = scaled(unscaledTabSpacing);
            double padding = scaled(3);

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
                ctx.LineTo(tabWidths[i], Bounds.InnerHeight + 1);
                ctx.ClosePath();

                double[] color = ElementGeometrics.DialogDefaultBgColor;
                ctx.SetSourceRGBA(color[0], color[1], color[2], color[3] * 0.75);
                ctx.Fill();

                ctx.NewPath();
                ctx.MoveTo(0, Bounds.InnerHeight);
                ctx.LineTo(0, radius);
                ctx.Arc(radius, radius, radius, 180 * degrees, 270 * degrees);
                ctx.Arc(tabWidths[i] - radius, radius, radius, -90 * degrees, 0 * degrees);
                ctx.LineTo(tabWidths[i], Bounds.InnerHeight);

                ShadePath(ctx, 2);


                Font.SetupContext(ctx);

                ShowTextCorrectly(ctx, tabs[i].name, padding, 1);

              
                generateTexture(surface, ref hoverTextures[i]);

                ctx.Dispose();
                surface.Dispose();
            }
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
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

        public void SetValue(int selectedIndex)
        {
            handler(tabs[selectedIndex].index);
            activeElement = selectedIndex;
        }

        public override void Dispose()
        {
            base.Dispose();

            for (int i = 0; i < hoverTextures.Length; i++)
            {
                hoverTextures[i].Dispose();
            }
        }
    }

    public static partial class GuiComposerHelpers
    {
        public static GuiComposer AddHorizontalTabs(this GuiComposer composer, GuiTab[] tabs, ElementBounds bounds, API.Common.Action<int> OnTabClicked, CairoFont font, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementHorizontalTabs(composer.Api, tabs, font, bounds, OnTabClicked), key);
            }

            return composer;
        }

        public static GuiElementHorizontalTabs GetHorizontalTabs(this GuiComposer composer, string key)
        {
            return (GuiElementHorizontalTabs)composer.GetElement(key);
        }
    }
}
