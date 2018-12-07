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
    /// <summary>
    /// Creates a drop-down list of items.
    /// </summary>
    public class GuiElementDropDown : GuiElementTextControl
    {
        protected GuiElementListMenu listMenu;
        
        
        protected LoadedTexture highlightTexture;
        protected LoadedTexture currentValueTexture;


        protected ElementBounds highlightBounds;
        protected API.Common.Action<string> onSelectionChanged;

        /// <summary>
        /// The draw order of this GUI Element.
        /// </summary>
        public override double DrawOrder
        {
            get { return 0.5; }
        }

        /// <summary>
        /// Can this element be put into focus?
        /// </summary>
        public override bool Focusable
        {
            get { return true; }
        }

        /// <summary>
        /// The scale of this GUI element.
        /// </summary>
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

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="capi">The client API</param>
        /// <param name="values">The values of the strings.</param>
        /// <param name="names">The names of the strings.</param>
        /// <param name="selectedIndex">The default selected index.</param>
        /// <param name="onSelectionChanged">The event that occurs when the selection is changed.</param>
        /// <param name="bounds">The bounds of the drop down.</param>
        public GuiElementDropDown(ICoreClientAPI capi, string[] values, string[] names, int selectedIndex, API.Common.Action<string> onSelectionChanged, ElementBounds bounds) : base(capi, "", CairoFont.WhiteSmallText(), bounds)
        {
            highlightTexture = new LoadedTexture(capi);
            currentValueTexture = new LoadedTexture(capi);

            listMenu = new GuiElementListMenu(capi, values, names, selectedIndex, DidSelect, bounds)
            {
                hoveredIndex = selectedIndex   
            };

            this.onSelectionChanged = onSelectionChanged;
        }

        private void DidSelect(string newvalue)
        {
            onSelectionChanged?.Invoke(newvalue);
            ComposeCurrentValue();
        }

        /// <summary>
        /// Composes the element based on the context.
        /// </summary>
        /// <param name="ctx">The context of the element.</param>
        /// <param name="surface">The surface of the image. (Not used)</param>
        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            ctx.SetSourceRGBA(0, 0, 0, 0.2);
            RoundRectangle(ctx, Bounds.drawX, Bounds.drawY, Bounds.InnerWidth, Bounds.InnerHeight, 3);
            ctx.Fill();
            EmbossRoundRectangleElement(ctx, Bounds, true, 1, 2);

            ctx.SetSourceRGBA(GuiStyle.DialogHighlightColor);
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

            generateTexture(surfaceHighlight, ref highlightTexture);

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
            ctx.SetSourceRGBA(GuiStyle.DialogDefaultTextColor);

            if (listMenu.selectedIndex >= 0)
            {
                double height = Font.GetFontExtents().Height;

                DrawTextLineAt(ctx, listMenu.names[listMenu.selectedIndex], 5, (valueHeight - height)/2);
            }

            generateTexture(surface, ref currentValueTexture);

            ctx.Dispose();
            surface.Dispose();
        }

        /// <summary>
        /// Renders the dropdown's interactive elements.
        /// </summary>
        /// <param name="deltaTime">The change in time.</param>
        public override void RenderInteractiveElements(float deltaTime)
        {
            api.Render.Render2DTexturePremultipliedAlpha(
                currentValueTexture.TextureId, 
                (int)Bounds.renderX, 
                (int)Bounds.renderY,
                valueWidth,
                valueHeight
            );

            listMenu.RenderInteractiveElements(deltaTime);

            if (HasFocus)
            {
                api.Render.Render2DTexture(highlightTexture.TextureId, highlightBounds);
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

        /// <summary>
        /// Sets the current index to a newly selected index.
        /// </summary>
        /// <param name="selectedIndex">the index that is to be selected.</param>
        public void SetSelectedIndex(int selectedIndex)
        {
            this.listMenu.SetSelectedIndex(selectedIndex);
            ComposeCurrentValue();
        }

        /// <summary>
        /// Sets the current index to the value of the selected string.
        /// </summary>
        /// <param name="value">the string contained in the drop down.</param>
        public void SetSelectedValue(string value)
        {
            this.listMenu.SetSelectedValue(value);
            ComposeCurrentValue();
        }

        /// <summary>
        /// Sets the values of the list with their corresponding names.
        /// </summary>
        /// <param name="values">The values of the list.</param>
        /// <param name="names">The names of the list.</param>
        public void SetList(string[] values, string[] names)
        {
            this.listMenu.SetList(values, names);
        }

        public override void Dispose()
        {
            base.Dispose();

            highlightTexture.Dispose();
            currentValueTexture.Dispose();
            listMenu?.Dispose();
        }

    }


    public static partial class GuiComposerHelpers
    {

        /// <summary>
        /// Adds a dropdown to the current GUI instance.
        /// </summary>
        /// <param name="values">The values of the current drodown.</param>
        /// <param name="names">The names of those values.</param>
        /// <param name="selectedIndex">The default selected index.</param>
        /// <param name="onSelectionChanged">The event fired when the index is changed.</param>
        /// <param name="bounds">The bounds of the index.</param>
        /// <param name="key">The name of this dropdown.</param>
        public static GuiComposer AddDropDown(this GuiComposer composer, string[] values, string[] names, int selectedIndex, API.Common.Action<string> onSelectionChanged, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementDropDown(composer.Api, values, names, selectedIndex, onSelectionChanged, bounds), key);
            }
            return composer;
        }

        /// <summary>
        /// Gets the Drop Down element from the GUIComposer by their key.
        /// </summary>
        /// <param name="key">the name of the dropdown to fetch.</param>
        public static GuiElementDropDown GetDropDown(this GuiComposer composer, string key)
        {
            return (GuiElementDropDown)composer.GetElement(key);
        }
    }
}
