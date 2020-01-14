using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    public delegate void SelectionChangedDelegate(string code, bool selected);

    /// <summary>
    /// Creates a drop-down list of items.
    /// </summary>
    public class GuiElementDropDown : GuiElementTextControl
    {
        public string SingularNameCode = "{0} item";
        public string PluralNameCode = "{0} items";
        public string PluralMoreNameCode = "+{0} more";
        public string SingularMoreNameCode = "+{0} more";

        public GuiElementListMenu listMenu;
        
        
        protected LoadedTexture highlightTexture;
        protected LoadedTexture currentValueTexture;

        protected LoadedTexture arrowDownButtonReleased;
        protected LoadedTexture arrowDownButtonPressed;

        protected ElementBounds highlightBounds;
        protected SelectionChangedDelegate onSelectionChanged;

        bool multiSelect;

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


        public string SelectedValue
        {
            get
            {
                if (listMenu.SelectedIndex < 0) return null;
                return listMenu.Values[listMenu.SelectedIndex];
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
        public GuiElementDropDown(ICoreClientAPI capi, string[] values, string[] names, int selectedIndex, SelectionChangedDelegate onSelectionChanged, ElementBounds bounds, bool multiSelect) : base(capi, "", CairoFont.WhiteSmallText(), bounds)
        {
            highlightTexture = new LoadedTexture(capi);
            currentValueTexture = new LoadedTexture(capi);
            arrowDownButtonReleased = new LoadedTexture(capi);
            arrowDownButtonPressed = new LoadedTexture(capi);

            listMenu = new GuiElementListMenu(capi, values, names, selectedIndex, didSelect, bounds, multiSelect)
            {
                HoveredIndex = selectedIndex   
            };

            this.onSelectionChanged = onSelectionChanged;
            this.multiSelect = multiSelect;
        }

        private void didSelect(string newvalue, bool on)
        {
            onSelectionChanged?.Invoke(newvalue, on);
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
            EmbossRoundRectangleElement(ctx, Bounds, true, 1, 1);

            listMenu.ComposeDynamicElements();
            ComposeDynamicElements();
        }

        private void ComposeDynamicElements()
        {
            // Released Button
            int btnWidth = (int)(scaled(20) * Scale);
            int btnHeight = (int)(Bounds.InnerHeight);

            ImageSurface surface = new ImageSurface(Format.Argb32, btnWidth, btnHeight);
            Context ctx = genContext(surface);

            ctx.SetSourceRGB(GuiStyle.DialogDefaultBgColor[0], GuiStyle.DialogDefaultBgColor[1], GuiStyle.DialogDefaultBgColor[2]);
            RoundRectangle(ctx, 0, 0, btnWidth, btnHeight, GuiStyle.ElementBGRadius);
            ctx.FillPreserve();
            ctx.SetSourceRGBA(1, 1, 1, 0.1);
            ctx.Fill();

            EmbossRoundRectangleElement(ctx, 0, 0, btnWidth, btnHeight, false, 2, 1);

            ctx.SetSourceRGBA(GuiStyle.DialogHighlightColor);
            RoundRectangle(ctx, 0, 0, btnWidth, btnHeight, 1);
            ctx.Fill();

            ctx.NewPath();
            ctx.LineTo(btnWidth - scaled(17) * Scale, scaled(10) * Scale);
            ctx.LineTo(btnWidth - scaled(3) * Scale, scaled(10) * Scale);
            ctx.LineTo(btnWidth - scaled(10) * Scale, scaled(20) * Scale);
            ctx.ClosePath();
            ctx.SetSourceRGBA(1, 1, 1, 0.6);
            ctx.Fill();

            generateTexture(surface, ref arrowDownButtonReleased);

            // Pressed Button
            ctx.Operator = Operator.Clear;
            ctx.SetSourceRGBA(0, 0, 0, 0);
            ctx.Paint();
            ctx.Operator = Operator.Over;

            ctx.SetSourceRGB(GuiStyle.DialogDefaultBgColor[0], GuiStyle.DialogDefaultBgColor[1], GuiStyle.DialogDefaultBgColor[2]);
            RoundRectangle(ctx, 0, 0, btnWidth, btnHeight, GuiStyle.ElementBGRadius);
            ctx.FillPreserve();
            ctx.SetSourceRGBA(0, 0, 0, 0.1);
            ctx.Fill();

            EmbossRoundRectangleElement(ctx, 0, 0, btnWidth, btnHeight, true, 2, 1);

            ctx.SetSourceRGBA(GuiStyle.DialogHighlightColor);
            RoundRectangle(ctx, 0, 0, btnWidth, btnHeight, 1);
            ctx.Fill();

            ctx.NewPath();
            ctx.LineTo(btnWidth - scaled(17) * Scale, scaled(10) * Scale);
            ctx.LineTo(btnWidth - scaled(3) * Scale, scaled(10) * Scale);
            ctx.LineTo(btnWidth - scaled(10) * Scale, scaled(20) * Scale);
            ctx.ClosePath();
            ctx.SetSourceRGBA(1, 1, 1, 0.4);
            ctx.Fill();

            generateTexture(surface, ref arrowDownButtonPressed);
            
            surface.Dispose();
            ctx.Dispose();



            // Highlight overlay
            ImageSurface surfaceHighlight = new ImageSurface(Format.Argb32, (int)Bounds.OuterWidth - btnWidth, (int)Bounds.OuterHeight);
            Context ctxHighlight = genContext(surfaceHighlight);

            ctxHighlight.SetSourceRGBA(1, 1, 1, 0.3);
            ctxHighlight.Paint();

            generateTexture(surfaceHighlight, ref highlightTexture);

            ctxHighlight.Dispose();
            surfaceHighlight.Dispose();

            highlightBounds = Bounds.CopyOffsetedSibling().WithFixedPadding(0, 0).FixedGrow(2 * Bounds.absPaddingX, 2 * Bounds.absPaddingY);
            highlightBounds.fixedWidth -= btnWidth;
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

            string text = "";
            double height = Font.GetFontExtents().Height;

            if (listMenu.SelectedIndices.Length > 1)
            {    
                for (int i = 0; i < listMenu.SelectedIndices.Length; i++)
                {
                    int index = listMenu.SelectedIndices[i];
                    string addText = "";

                    if (text.Length > 0) addText += ", ";
                    addText += listMenu.Names[index];

                    int cntleft = listMenu.SelectedIndices.Length - i;
                    int cnt = listMenu.SelectedIndices.Length;

                    string moreText =
                        text.Length > 0 ?
                        (" " + (cntleft == 1 ? Lang.Get(SingularMoreNameCode, cntleft) : Lang.Get(PluralMoreNameCode, cntleft))) :
                        (cnt == 1 ? Lang.Get(SingularNameCode, cnt) : Lang.Get(PluralNameCode, cnt))
                    ;

                    if (Font.GetTextExtents(text + addText + Lang.Get(PluralMoreNameCode, cntleft)).Width < width)
                    {
                        text += addText;
                    } else
                    {
                        text = text + moreText;
                        break;
                    }
                }

                DrawTextLineAt(ctx, text, 5, (valueHeight - height)/2);
            }

            if (listMenu.SelectedIndices.Length == 1)
            {
                DrawTextLineAt(ctx, listMenu.Names[listMenu.SelectedIndex], 5, (valueHeight - height) / 2);
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
            if (HasFocus)
            {
                api.Render.Render2DTexture(highlightTexture.TextureId, highlightBounds);
            }

            api.Render.Render2DTexturePremultipliedAlpha(
                currentValueTexture.TextureId, 
                (int)Bounds.renderX, 
                (int)Bounds.renderY,
                valueWidth,
                valueHeight
            );

            double renderX = Bounds.renderX + Bounds.InnerWidth - arrowDownButtonReleased.Width;
            double renderY = Bounds.renderY;


            if (listMenu.IsOpened)
            {
                api.Render.Render2DTexturePremultipliedAlpha(arrowDownButtonPressed.TextureId, renderX, renderY, arrowDownButtonReleased.Width, arrowDownButtonReleased.Height);
            } else
            {
                api.Render.Render2DTexturePremultipliedAlpha(arrowDownButtonReleased.TextureId, renderX, renderY, arrowDownButtonReleased.Width, arrowDownButtonReleased.Height);
            }
            


            listMenu.RenderInteractiveElements(deltaTime);
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

        public override void OnMouseUp(ICoreClientAPI api, MouseEvent args)
        {
            listMenu.OnMouseUp(api, args);

            args.Handled |= listMenu.IsPositionInside(args.X, args.Y) || IsPositionInside(args.X, args.Y);
        }


        public override void OnMouseDown(ICoreClientAPI api, MouseEvent args)
        {
            listMenu.OnMouseDown(api, args);
            
            if (!listMenu.IsOpened && listMenu.IsPositionInside(args.X, args.Y) && !args.Handled)
            {
                listMenu.Open();
                api.Gui.PlaySound("menubutton");
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
        public void SetSelectedValue(params string[] value)
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
            arrowDownButtonReleased.Dispose();
            arrowDownButtonPressed.Dispose();
        }

    }


    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Adds a multiple select dropdown to the current GUI instance.
        /// </summary>
        /// <param name="values">The values of the current drodown.</param>
        /// <param name="names">The names of those values.</param>
        /// <param name="selectedIndex">The default selected index.</param>
        /// <param name="onSelectionChanged">The event fired when the index is changed.</param>
        /// <param name="bounds">The bounds of the index.</param>
        /// <param name="key">The name of this dropdown.</param>
        public static GuiComposer AddMultiSelectDropDown(this GuiComposer composer, string[] values, string[] names, int selectedIndex, SelectionChangedDelegate onSelectionChanged, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementDropDown(composer.Api, values, names, selectedIndex, onSelectionChanged, bounds, true), key);
            }
            return composer;
        }



        /// <summary>
        /// Adds a dropdown to the current GUI instance.
        /// </summary>
        /// <param name="values">The values of the current drodown.</param>
        /// <param name="names">The names of those values.</param>
        /// <param name="selectedIndex">The default selected index.</param>
        /// <param name="onSelectionChanged">The event fired when the index is changed.</param>
        /// <param name="bounds">The bounds of the index.</param>
        /// <param name="key">The name of this dropdown.</param>
        public static GuiComposer AddDropDown(this GuiComposer composer, string[] values, string[] names, int selectedIndex, SelectionChangedDelegate onSelectionChanged, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementDropDown(composer.Api, values, names, selectedIndex, onSelectionChanged, bounds, false), key);
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
