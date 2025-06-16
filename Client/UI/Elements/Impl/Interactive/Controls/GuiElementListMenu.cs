using Cairo;
using System;
using System.Collections.Generic;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    // Contains quite some ugly hacks to make the scrollbar work :<
    public class GuiElementListMenu : GuiElementTextBase
    {
        public string[] Values { get; set; }
        public string[] Names { get; set; }

        /// <summary>
        /// Max height of the expanded list
        /// </summary>
        public int MaxHeight = 350;

        protected double expandedBoxWidth = 0;
        protected double expandedBoxHeight = 0;

        protected double unscaledLineHeight = 30;

        /// <summary>
        /// The (first) currently selected element
        /// </summary>
        public int SelectedIndex {
            get
            {
                if (SelectedIndices != null && SelectedIndices.Length > 0)
                {
                    return SelectedIndices[0];
                }
                return 0;
            }
            set
            {
                if (value < 0)
                {
                    SelectedIndices = Array.Empty<int>();
                    return;
                }

                if (SelectedIndices != null && SelectedIndices.Length > 0)
                {
                    SelectedIndices[0] = value;
                    return;
                }

                SelectedIndices = new int[] { value };
            }
        }
        /// <summary>
        /// The element the user currently has the mouse over
        /// </summary>
        public int HoveredIndex { get; set; }
        /// <summary>
        /// On multi select mode, the list of all selected elements
        /// </summary>
        public int[] SelectedIndices { get; set; }

        // on/off on multiselect
        GuiElementSwitch[] switches = Array.Empty<GuiElementSwitch>();


        protected SelectionChangedDelegate onSelectionChanged;

        protected LoadedTexture hoverTexture;
        protected LoadedTexture dropDownTexture;
        protected LoadedTexture scrollbarTexture;

        protected bool expanded;
        protected bool multiSelect;

        protected double scrollOffY = 0;

        protected GuiElementCompactScrollbar scrollbar;
        protected GuiElementRichtext[] richtTextElem;

        protected ElementBounds visibleBounds;

        /// <summary>
        /// Is the current menu opened?
        /// </summary>
        public bool IsOpened
        {
            get { return expanded; }
        }

        public override double DrawOrder
        {
            get { return 0.5; }
        }

        public override bool Focusable
        {
            get { return enabled; }
        }

        /// <summary>
        /// Creates a new GUI Element List Menu
        /// </summary>
        /// <param name="capi">The Client API.</param>
        /// <param name="values">The values of the list.</param>
        /// <param name="names">The names for each of the values.</param>
        /// <param name="selectedIndex">The default selected index.</param>
        /// <param name="onSelectionChanged">The event fired when the selection is changed.</param>
        /// <param name="bounds">The bounds of the GUI element.</param>
        /// <param name="font"></param>
        /// <param name="multiSelect"></param>
        public GuiElementListMenu(ICoreClientAPI capi, string[] values, string[] names, int selectedIndex, SelectionChangedDelegate onSelectionChanged, ElementBounds bounds, CairoFont font, bool multiSelect) : base(capi, "", font, bounds)
        {
            if (values.Length != names.Length) throw new ArgumentException("Values and Names arrays must be of the same length!");

            hoverTexture = new LoadedTexture(capi);
            dropDownTexture = new LoadedTexture(capi);
            scrollbarTexture = new LoadedTexture(capi);

            this.Values = values;
            this.Names = names;
            this.SelectedIndex = selectedIndex;
            this.multiSelect = multiSelect;
            this.onSelectionChanged = onSelectionChanged;
            HoveredIndex = selectedIndex;
            
            ElementBounds scrollbarBounds = ElementBounds.Fixed(0, 0, 0, 0).WithEmptyParent();

            scrollbar = new GuiElementCompactScrollbar(api, OnNewScrollbarValue, scrollbarBounds);
            scrollbar.zOffset = 300;

            

            richtTextElem = new GuiElementRichtext[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                ElementBounds textBounds = ElementBounds.Fixed(0, 0, 700, 100).WithEmptyParent();
                richtTextElem[i] = new GuiElementRichtext(capi, Array.Empty<RichTextComponentBase>(), textBounds);
            }
        }

        private void OnNewScrollbarValue(float offY)
        {
            scrollOffY = (int)((offY / (30 * Scale)) * 30 * Scale);
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            ComposeDynamicElements();
        }

        /// <summary>
        /// Composes the list of elements dynamically.
        /// </summary>
        public void ComposeDynamicElements()
        {
            Bounds.CalcWorldBounds();

            if (multiSelect)
            {
                if (switches != null)
                {
                    foreach (var val in switches) val.Dispose();
                }

                switches = new GuiElementSwitch[Names.Length];
            }

            for (int i = 0; i < richtTextElem.Length; i++)
            {
                richtTextElem[i].Dispose();
            }

            richtTextElem = new GuiElementRichtext[Values.Length];
            for (int i = 0; i < Values.Length; i++)
            {
                ElementBounds textBounds = ElementBounds.Fixed(0, 0, 700, 100).WithEmptyParent();
                richtTextElem[i] = new GuiElementRichtext(api, Array.Empty<RichTextComponentBase>(), textBounds);
            }


            double scaleMul = Scale * RuntimeEnv.GUIScale;
            double lineHeight = unscaledLineHeight * scaleMul;

            // Expandable box with list of names
            expandedBoxWidth = Bounds.InnerWidth;
            expandedBoxHeight = Values.Length * lineHeight;

            double scrollbarWidth = 10;

            for (int i = 0; i < Values.Length; i++)
            {
                GuiElementRichtext elem = richtTextElem[i];
                elem.SetNewTextWithoutRecompose(Names[i], Font);
                elem.BeforeCalcBounds();
                
                expandedBoxWidth = Math.Max(expandedBoxWidth, elem.MaxLineWidth + 5 * scaleMul + scaled(scrollbarWidth + 5));
            }


            ImageSurface surface = new ImageSurface(Format.Argb32, (int)expandedBoxWidth, (int)expandedBoxHeight);
            Context ctx = genContext(surface);

            visibleBounds = Bounds.FlatCopy();
            visibleBounds.fixedHeight = Math.Min(MaxHeight, expandedBoxHeight / RuntimeEnv.GUIScale);
            visibleBounds.fixedWidth = expandedBoxWidth / RuntimeEnv.GUIScale;
            visibleBounds.fixedY += Bounds.InnerHeight / RuntimeEnv.GUIScale;
            visibleBounds.CalcWorldBounds();

            Font.SetupContext(ctx);

            ctx.SetSourceRGBA(GuiStyle.DialogStrongBgColor);
            RoundRectangle(ctx, 0, 0, expandedBoxWidth, expandedBoxHeight, 1);
            ctx.FillPreserve();
            ctx.SetSourceRGBA(0,0,0,0.5);
            ctx.LineWidth = 2;
            ctx.Stroke();

            double unscaledHeight = Font.GetFontExtents().Height / RuntimeEnv.GUIScale;
            double unscaledOffY = (unscaledLineHeight - unscaledHeight) / 2;
            double unscaledOffx = multiSelect ? unscaledHeight + 10 : 0;

            double scaledHeight = unscaledHeight * scaleMul;

            ctx.SetSourceRGBA(GuiStyle.DialogDefaultTextColor);

            ElementBounds switchParentBounds = Bounds.FlatCopy();
            switchParentBounds.IsDrawingSurface = true;
            switchParentBounds.CalcWorldBounds();

            

            for (int i = 0; i < Values.Length; i++)
            {
                int num = i;
                double y = ((int)unscaledOffY + i * unscaledLineHeight) * scaleMul;
                double x = unscaledOffx + 5 * scaleMul;

                double offy = (scaledHeight - Font.GetTextExtents(Names[i]).Height)/2;

                if (multiSelect)
                {
                    double pad = 2;
                    ElementBounds switchBounds = new ElementBounds()
                    {
                        ParentBounds = switchParentBounds,
                        fixedX = 4 * Scale,
                        fixedY = (y + offy) / RuntimeEnv.GUIScale,
                        fixedWidth = (unscaledHeight) * Scale,
                        fixedHeight = (unscaledHeight) * Scale,
                        fixedPaddingX = 0,
                        fixedPaddingY = 0
                    };

                    switches[i] = new GuiElementSwitch(api, (on) => toggled(on, num), switchBounds, switchBounds.fixedHeight, pad);
                    switches[i].ComposeElements(ctx, surface);

                    ctx.SetSourceRGBA(GuiStyle.DialogDefaultTextColor);
                }

                GuiElementRichtext elem = richtTextElem[i];

                elem.Bounds.fixedX = x;
                elem.Bounds.fixedY = (y + offy) / RuntimeEnv.GUIScale;
                elem.BeforeCalcBounds();
                elem.Bounds.CalcWorldBounds();
                elem.ComposeFor(elem.Bounds, ctx, surface);
            }

            generateTexture(surface, ref dropDownTexture);

            ctx.Dispose();
            surface.Dispose();


            // Scrollbar static stuff
            scrollbar.Bounds.WithFixedSize(scrollbarWidth, visibleBounds.fixedHeight - 3).WithFixedPosition(expandedBoxWidth / RuntimeEnv.GUIScale - 10, 0).WithFixedPadding(0, 2);
            scrollbar.Bounds.WithEmptyParent();
            scrollbar.Bounds.CalcWorldBounds();

            surface = new ImageSurface(Format.Argb32, (int)expandedBoxWidth, (int)scrollbar.Bounds.OuterHeight);
            ctx = genContext(surface);
            
            scrollbar.ComposeElements(ctx, surface);
            scrollbar.SetHeights((int)visibleBounds.InnerHeight , (int)expandedBoxHeight);

            generateTexture(surface, ref scrollbarTexture);

            ctx.Dispose();
            surface.Dispose();


            // Hover bar
            surface = new ImageSurface(Format.Argb32, (int)expandedBoxWidth, (int)(unscaledLineHeight * scaleMul));
            ctx = genContext(surface);

            double[] col = GuiStyle.DialogHighlightColor;
            col[3] = 0.5;
            ctx.SetSourceRGBA(col);
            RoundRectangle(ctx, 0, 0, expandedBoxWidth, unscaledLineHeight * scaleMul, 0);
            ctx.Fill();
            generateTexture(surface, ref hoverTexture);

            ctx.Dispose();
            surface.Dispose();
            
        }

        private void toggled(bool on, int num)
        {
            List<int> selected = new List<int>();
            for (int i = 0; i < switches.Length; i++)
            {
                if (switches[i].On) selected.Add(i);
            }

            SelectedIndices = selected.ToArray();
        }

        public override bool IsPositionInside(int posX, int posY)
        {
            if (!IsOpened) return false;

            return
                posX >= Bounds.absX &&
                posX <= Bounds.absX + expandedBoxWidth &&
                posY >= Bounds.absY &&
                posY <= Bounds.absY + expandedBoxHeight
            ;
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            if (expanded)
            {
                double scaleMul = Scale * RuntimeEnv.GUIScale;

                api.Render.PushScissor(visibleBounds);
                
                api.Render.Render2DTexture(
                    dropDownTexture.TextureId, 
                    (int)Bounds.renderX,
                    (int)Bounds.renderY + (int)Bounds.InnerHeight - (int)scrollOffY, 
                    (int)expandedBoxWidth, 
                    (int)expandedBoxHeight,
                    110 + 200
                );


                if (multiSelect)
                {
                    api.Render.GlPushMatrix();
                    api.Render.GlTranslate(0, Bounds.InnerHeight - (int)scrollOffY, 350);
                    for (int i = 0; i < switches.Length; i++)
                    {
                        //switches[i].Bounds.fixedOffsetY = () / RuntimeEnv.GUIScale;
                        //switches[i].Bounds.CalcWorldBounds();
                        switches[i].RenderInteractiveElements(deltaTime);
                    }
                    api.Render.GlPopMatrix();
                }

                if (HoveredIndex >= 0)
                {
                    api.Render.Render2DTexturePremultipliedAlpha(
                        hoverTexture.TextureId, 
                        (int)Bounds.renderX + 1, 
                        (int)(Bounds.renderY + Bounds.InnerHeight + unscaledLineHeight * scaleMul * HoveredIndex - (int)scrollOffY + 1), 
                        (int)expandedBoxWidth - scaled(10), 
                        (int)unscaledLineHeight * scaleMul - 2,
                        111 + 200
                    );
                }

                api.Render.PopScissor();

                if (api.Render.ScissorStack.Count > 0)
                {
                    api.Render.GlScissorFlag(false);
                }

                api.Render.GlPushMatrix();
                api.Render.GlTranslate(0, 0, 200);
                api.Render.Render2DTexturePremultipliedAlpha(
                    scrollbarTexture.TextureId,
                    (int)visibleBounds.renderX,
                    (int)visibleBounds.renderY,
                    scrollbarTexture.Width,
                    scrollbarTexture.Height,
                    116 + 200
                );

                scrollbar.Bounds.WithParent(Bounds);
                scrollbar.Bounds.absFixedY = Bounds.InnerHeight;
                scrollbar.RenderInteractiveElements(deltaTime);

                api.Render.GlPopMatrix();

                if (api.Render.ScissorStack.Count > 0)
                {
                    api.Render.GlScissorFlag(true);
                }
            }

        }

        public override void OnKeyDown(ICoreClientAPI api, KeyEvent args)
        {
            if (!hasFocus) return;

            if ((args.KeyCode == (int)GlKeys.Enter || args.KeyCode == (int)GlKeys.KeypadEnter) && expanded)
            {
                expanded = false;
                SelectedIndex = HoveredIndex;
                onSelectionChanged?.Invoke(Values[SelectedIndex], true);
                args.Handled = true;
                return;
            }

            if (args.KeyCode == (int)GlKeys.Up || args.KeyCode == (int)GlKeys.Down)
            {
                args.Handled = true;

                if (!expanded)
                {
                    expanded = true;
                    HoveredIndex = SelectedIndex;
                    return;
                }

                if (args.KeyCode == (int)GlKeys.Up)
                {
                    HoveredIndex = GameMath.Mod((HoveredIndex - 1), Values.Length);
                }
                else
                {
                    HoveredIndex = GameMath.Mod((HoveredIndex + 1), Values.Length);
                }
            }
        }


        public override void OnMouseMove(ICoreClientAPI api, MouseEvent args)
        {
            if (!expanded) return;

            int mouseX = api.Input.MouseX;
            int mouseY = api.Input.MouseY;
            double scaleMul = Scale * RuntimeEnv.GUIScale;

            if (!(mouseX >= Bounds.renderX && mouseX <= Bounds.renderX + expandedBoxWidth)) return;

            if (scrollbar.mouseDownOnScrollbarHandle)
            {
                if (scrollbar.mouseDownOnScrollbarHandle || Bounds.renderX + expandedBoxWidth - args.X < scaled(10))
                {
                    scrollbar.OnMouseMove(api, args);
                    return;
                }
            }

            int num = (int)((mouseY - Bounds.renderY - Bounds.InnerHeight + scrollOffY) / (unscaledLineHeight * scaleMul));
            if (num >= 0 && num < Values.Length)
            {
                HoveredIndex = num;
                args.Handled = true;
            }
        }


        public override void OnMouseUp(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseUp(api, args);

            if (expanded)
            {
                scrollbar.OnMouseUp(api, args);
            }
        }


        /// <summary>
        /// Opens the menu.
        /// </summary>
        public void Open()
        {
            expanded = true;
        }

        internal void Close()
        {
            expanded = false;
        }

        public override void OnMouseDown(ICoreClientAPI api, MouseEvent args)
        {
            if (!expanded) return;
            if (!(args.X >= Bounds.renderX && args.X <= Bounds.renderX + expandedBoxWidth)) return;

            double scaleMul = Scale * RuntimeEnv.GUIScale;

            if (Bounds.renderX + expandedBoxWidth - args.X < scaled(10))
            {
                scrollbar.OnMouseDown(api, args);
                return;
            }

            double dy = args.Y - Bounds.renderY - unscaledLineHeight * scaleMul;

            if (dy < 0 || dy > visibleBounds.OuterHeight)
            {
                expanded = false;
                args.Handled = true;
                api.Gui.PlaySound("menubutton");
                return;
            }

            int mouseY = api.Input.MouseY;
            int selectedElement = (int)((mouseY - Bounds.renderY - Bounds.InnerHeight + scrollOffY) / (unscaledLineHeight * scaleMul));

            if (selectedElement >= 0 && selectedElement < Values.Length)
            {
                if (multiSelect)
                {
                    switches[selectedElement].OnMouseDownOnElement(api, args);
                    onSelectionChanged?.Invoke(Values[selectedElement], switches[selectedElement].On);
                }
                else
                {
                    SelectedIndex = selectedElement;
                    onSelectionChanged?.Invoke(Values[SelectedIndex], true);
                }

                api.Gui.PlaySound("toggleswitch");

                if (!multiSelect) expanded = false;
                args.Handled = true;
            }
        }
        

        public override void OnMouseWheel(ICoreClientAPI api, MouseWheelEventArgs args)
        {
            if (expanded && visibleBounds.PointInside(api.Input.MouseX, api.Input.MouseY))
            {
                scrollbar.OnMouseWheel(api, args);
            }
        }


        public override void OnFocusLost()
        {
            base.OnFocusLost();
            expanded = false;
        }

        /// <summary>
        /// Sets the selected index.
        /// </summary>
        /// <param name="selectedIndex">The index to be set to.</param>
        public void SetSelectedIndex(int selectedIndex)
        {
            this.SelectedIndex = selectedIndex;
        }
        
        /// <summary>
        /// Sets the selected index to the given value.
        /// </summary>
        /// <param name="value">The value to be set to.</param>
        public void SetSelectedValue(params string[] value)
        {
            if (value == null)
            {
                this.SelectedIndices = Array.Empty<int>();
                return;
            }

            List<int> selectedIndices = new List<int>();
                
            for (int i = 0; i < Values.Length; i++)
            {
                if (multiSelect) switches[i].On = false;

                for (int j = 0; j < value.Length; j++)
                {
                    if (Values[i] == value[j])
                    {
                        selectedIndices.Add(i);

                        if (multiSelect) switches[i].On = true;
                    }
                }
            }

            this.SelectedIndices = selectedIndices.ToArray();

            
        }

        /// <summary>
        /// Sets the list for the GUI Element list value.
        /// </summary>
        /// <param name="values">The values of the list.</param>
        /// <param name="names">The names of the values.</param>
        public void SetList(string[] values, string[] names)
        {
            this.Values = values;
            this.Names = names;
            ComposeDynamicElements();
        }


        public override void Dispose()
        {
            base.Dispose();

            hoverTexture.Dispose();
            dropDownTexture.Dispose();
            scrollbarTexture.Dispose();

            scrollbar?.Dispose();
        }


    }
}
