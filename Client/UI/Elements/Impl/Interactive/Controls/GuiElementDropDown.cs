using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    public class GuiElementDropDown : GuiElementTextControl
    {
        GuiElementListMenu listMenu;
        
        
        protected int highlightTextureId;
        int currentValueTextureId;
        protected ElementBounds highlightBounds;
        API.Common.Action<string> onSelectionChanged;

        public override double DrawOrder
        {
            get { return 0.5; }
        }

        public override bool Focusable
        {
            get { return true; }
        }

        public override double Scale
        {
            get
            {
                return base.Scale;
            }

            set
            {
                base.Scale = value;
                listMenu.Scale = value;
            }
        }

        public GuiElementDropDown(ICoreClientAPI capi, string[] values, string[] names, int selectedIndex, API.Common.Action<string> onSelectionChanged, ElementBounds bounds) : base(capi, "", CairoFont.WhiteSmallText(), bounds)
        {
            listMenu = new GuiElementListMenu(capi, values, names, selectedIndex, didSelect, bounds)
            {
                hoveredIndex = selectedIndex   
            };

            this.onSelectionChanged = onSelectionChanged;
        }

        private void didSelect(string newvalue)
        {
            onSelectionChanged?.Invoke(newvalue);
            ComposeCurrentValue();
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            ctx.SetSourceRGBA(0, 0, 0, 0.2);
            RoundRectangle(ctx, Bounds.drawX, Bounds.drawY, Bounds.InnerWidth, Bounds.InnerHeight, 3);
            ctx.Fill();
            EmbossRoundRectangleElement(ctx, Bounds, true, 1, 2);

            ctx.SetSourceRGBA(ElementGeometrics.DialogHighlightColor);
            RoundRectangle(ctx, Bounds.drawX + Bounds.InnerWidth - scaled(20 + 1) * Scale, Bounds.drawY, scaled(20) * Scale, Bounds.InnerHeight - scaled(1), 1);
            ctx.Fill();

            ctx.NewPath();
            ctx.LineTo(Bounds.drawX + Bounds.InnerWidth - scaled(17) * Scale, Bounds.drawY + scaled(10) * Scale);
            ctx.LineTo(Bounds.drawX + Bounds.InnerWidth - scaled(3) * Scale, Bounds.drawY + scaled(10) * Scale);
            ctx.LineTo(Bounds.drawX + Bounds.InnerWidth - scaled(10) * Scale, Bounds.drawY + scaled(20) * Scale);
            ctx.ClosePath();
            ctx.SetSourceRGBA(1, 1, 1, 0.4);
            ctx.Fill();

            listMenu.ComposeDynamicElements();
            ComposeDynamicElements();
        }

        private void ComposeDynamicElements()
        {
            // Highlight overlay
            ImageSurface surfaceHighlight = new ImageSurface(Format.Argb32, (int)Bounds.OuterWidth, (int)Bounds.OuterHeight);
            Context ctxHighlight = genContext(surfaceHighlight);

            ctxHighlight.SetSourceRGBA(0, 0, 0, 0.2);
            ctxHighlight.Paint();

            generateTexture(surfaceHighlight, ref highlightTextureId);

            ctxHighlight.Dispose();
            surfaceHighlight.Dispose();

            highlightBounds = Bounds.CopyOffsetedSibling().WithFixedPadding(0, 0).FixedGrow(2 * Bounds.absPaddingX, 2 * Bounds.absPaddingY);
            highlightBounds.CalcWorldBounds();

            ComposeCurrentValue();
        }

        int valueWidth;
        int valueHeight;

        void ComposeCurrentValue()
        {
            double width = Bounds.InnerWidth;

            valueWidth = (int)((Bounds.InnerWidth - scaled(20)) * Scale);
            valueHeight = (int)(scaled(30) * Scale);

            // Current value
            ImageSurface surface = new ImageSurface(Format.Argb32, valueWidth, valueHeight);
            Context ctx = genContext(surface);

            Font.SetupContext(ctx);
            ctx.SetSourceRGBA(ElementGeometrics.DialogDefaultTextColor);

            if (listMenu.selectedIndex >= 0)
            {
                double height = Font.GetFontExtents().Height;

                ShowTextCorrectly(ctx, listMenu.names[listMenu.selectedIndex], 5, (valueHeight - height)/2);
            }

            generateTexture(surface, ref currentValueTextureId);

            ctx.Dispose();
            surface.Dispose();
        }


        public override void RenderInteractiveElements(float deltaTime)
        {
            api.Render.Render2DTexturePremultipliedAlpha(
                currentValueTextureId, 
                (int)Bounds.renderX, 
                (int)Bounds.renderY, 
                (int)valueWidth, 
                (int)valueHeight
            );

            listMenu.RenderInteractiveElements(deltaTime);

            if (HasFocus)
            {
                api.Render.Render2DTexture(highlightTextureId, highlightBounds);
            }
        }

        public override void OnKeyDown(ICoreClientAPI api, KeyEvent args)
        {
            listMenu.OnKeyDown(api, args);
        }


        public override void OnMouseMove(ICoreClientAPI api, MouseEvent args)
        {
            listMenu.OnMouseMove(api, args);
        }

        public override void OnMouseWheel(ICoreClientAPI api, MouseWheelEventArgs args)
        {
            listMenu.OnMouseWheel(api, args);
        }


        public override void OnMouseDown(ICoreClientAPI api, MouseEvent args)
        {
            listMenu.OnMouseDown(api, args);
            //Console.WriteLine("{0}/{1}", args.X, args.Y);
            if (!listMenu.IsOpened && listMenu.IsPositionInside(args.X, args.Y))
            {
                listMenu.Open();
                args.Handled = true;
                return;
            }

        }

        public override void OnFocusLost()
        {
            base.OnFocusLost();
            listMenu.OnFocusLost();
        }

        public void SetSelectedIndex(int selectedIndex)
        {
            this.listMenu.SetSelectedIndex(selectedIndex);
            ComposeCurrentValue();
        }

        public void SetList(string[] values, string[] names)
        {
            this.listMenu.SetList(values, names);
        }
    }


    public static partial class GuiComposerHelpers
    {

        public static GuiComposer AddDropDown(this GuiComposer composer, string[] values, string[] names, int selectedIndex, API.Common.Action<string> onSelectionChanged, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementDropDown(composer.Api, values, names, selectedIndex, onSelectionChanged, bounds), key);
            }
            return composer;
        }

        public static GuiElementDropDown GetDropDown(this GuiComposer composer, string key)
        {
            return (GuiElementDropDown)composer.GetElement(key);
        }
    }
}
