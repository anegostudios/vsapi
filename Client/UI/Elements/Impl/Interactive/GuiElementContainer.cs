using Cairo;
using System;
using System.Collections.Generic;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{

    public class GuiElementContainer : GuiElement
    {
        /// <summary>
        /// The cells in the list.  See IGuiElementCell for how it's supposed to function.
        /// </summary>
        public List<GuiElement> Elements = new List<GuiElement>();

        /// <summary>
        /// the space between the cells.  Default: 10
        /// </summary>
        public int unscaledCellSpacing = 10;

        /// <summary>
        /// The padding on the vertical axis of the cell.  Default: 2
        /// </summary>
        public int UnscaledCellVerPadding = 4;

        /// <summary>
        /// The padding on the horizontal axis of the cell.  Default: 7
        /// </summary>
        public int UnscaledCellHorPadding = 7;

        public bool Tabbable = false;
        bool renderFocusHighlight;

        public override bool Focusable { get { return Tabbable; } }


        LoadedTexture listTexture;

        ElementBounds insideBounds;

        protected int currentFocusableElementKey;



        /// <summary>
        /// Creates a new list in the current GUI.
        /// </summary>
        /// <param name="capi">The Client API.</param>
        /// <param name="bounds">The bounds of the list.</param>
        public GuiElementContainer(ICoreClientAPI capi, ElementBounds bounds) : base(capi, bounds)
        {
            listTexture = new LoadedTexture(capi);
            bounds.IsDrawingSurface = true;
        }

        public override void BeforeCalcBounds()
        {
            base.BeforeCalcBounds();

            insideBounds = new ElementBounds().WithFixedPadding(unscaledCellSpacing).WithEmptyParent();
            insideBounds.CalcWorldBounds();
            CalcTotalHeight();
        }

        internal void ReloadCells()
        {
            CalcTotalHeight();
            ComposeList();
        }

        /// <summary>
        /// Calculates the total height for the list.
        /// </summary>
        public void CalcTotalHeight()
        {
            double height = 0;
            foreach (GuiElement cell in Elements)
            {
                cell.BeforeCalcBounds();
                height = Math.Max(height, cell.Bounds.fixedY + cell.Bounds.fixedHeight);
            }

            Bounds.fixedHeight = height + unscaledCellSpacing;
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            insideBounds = new ElementBounds().WithFixedPadding(unscaledCellSpacing).WithEmptyParent();
            insideBounds.CalcWorldBounds();

            Bounds.CalcWorldBounds();
            ComposeList();
        }



        void ComposeList()
        {
            ImageSurface surface = new ImageSurface(Format.Argb32, (int)Bounds.OuterWidth, (int)Bounds.OuterHeight);
            Context ctx = genContext(surface);

            CalcTotalHeight();
            Bounds.CalcWorldBounds();

            foreach (GuiElement elem in Elements)
            {
                elem.ComposeElements(ctx, surface);
            }

            generateTexture(surface, ref listTexture);

            ctx.Dispose();
            surface.Dispose();
        }

        /// <summary>
        /// Gets the currently tabbed index element, if there is one currently focused.
        /// </summary>
        public GuiElement CurrentTabIndexElement
        {
            get
            {
                foreach (GuiElement element in Elements)
                {
                    if (element.Focusable && element.HasFocus)
                    {
                        return element;
                    }
                }

                return null;
            }
        }

        public GuiElement FirstTabbableElement
        {
            get
            {
                foreach (GuiElement element in Elements)
                {
                    if (element.Focusable)
                    {
                        return element;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the maximum tab index of the components.
        /// </summary>
        public int MaxTabIndex
        {
            get
            {
                int tabIndex = -1;
                foreach (GuiElement element in Elements)
                {
                    if (element.Focusable)
                    {
                        tabIndex = Math.Max(tabIndex, element.TabIndex);
                    }
                }

                return tabIndex;
            }
        }

        /// <summary>
        /// marks an element as in focus.  
        /// </summary>
        /// <param name="tabIndex">The tab index to focus at.</param>
        /// <returns>Whether or not the focus could be done.</returns>
        public bool FocusElement(int tabIndex)
        {
            GuiElement newFocusedElement = null;

            foreach (GuiElement element in Elements)
            {
                if (element.Focusable && element.TabIndex == tabIndex)
                {
                    newFocusedElement = element;
                    break;
                }
            }

            if (newFocusedElement != null)
            {
                UnfocusOwnElementsExcept(newFocusedElement);
                newFocusedElement.OnFocusGained();
                return true;
            }

            return false;
        }

        public void UnfocusOwnElements()
        {
            UnfocusOwnElementsExcept(null);
        }

        /// <summary>
        /// Unfocuses all elements except one specific element.
        /// </summary>
        /// <param name="elem">The element to remain in focus.</param>
        public void UnfocusOwnElementsExcept(GuiElement elem)
        {
            foreach (GuiElement element in Elements)
            {
                if (element == elem) continue;

                if (element.Focusable && element.HasFocus)
                {
                    element.OnFocusLost();
                }
            }
        }

        public void Clear()
        {
            Elements.Clear();
            Bounds.ChildBounds.Clear();
            currentFocusableElementKey = 0;
        }

        /// <summary>
        /// Adds a cell to the list.
        /// </summary>
        /// <param name="elem">The cell to add.</param>
        /// <param name="afterPosition">The position of the cell to add after.  (Default: -1)</param>
        public void Add(GuiElement elem, int afterPosition = -1)
        {
            if (afterPosition == -1)
            {
                Elements.Add(elem);
            }
            else
            {
                Elements.Insert(afterPosition, elem);
            }

            if (elem.Focusable)
            {
                elem.TabIndex = currentFocusableElementKey++;
            }
            else
            {
                elem.TabIndex = -1;
            }

            elem.InsideClipBounds = InsideClipBounds;

            Bounds.WithChild(elem.Bounds);
        }

        /// <summary>
        /// Removes a cell at a specified position.
        /// </summary>
        /// <param name="position">The position of the cell to remove.</param>
        public void RemoveCell(int position)
        {
            Elements.RemoveAt(position);
        }

        public override void OnMouseUp(ICoreClientAPI api, MouseEvent args)
        {
            foreach (GuiElement element in Elements)
            {
                element.OnMouseUp(api, args);
            }

            if (!args.Handled) base.OnMouseUp(api, args);
        }

        public override void OnMouseDown(ICoreClientAPI api, MouseEvent args)
        {
            bool beforeHandled = false;
            bool nowHandled = false;
            renderFocusHighlight = false;

            foreach (GuiElement element in Elements)
            {
                if (!beforeHandled)
                {
                    element.OnMouseDown(api, args);
                    nowHandled = args.Handled;
                }

                if (!beforeHandled && nowHandled)
                {
                    if (element.Focusable && !element.HasFocus)
                    {
                        element.OnFocusGained();
                    }
                }
                else
                {
                    if (element.Focusable && element.HasFocus)
                    {
                        element.OnFocusLost();
                    }
                }

                beforeHandled = nowHandled;
            }

            if (!args.Handled) base.OnMouseDown(api, args);
        }


        public override void OnMouseMove(ICoreClientAPI api, MouseEvent args)
        {
            foreach (GuiElement element in Elements)
            {
                element.OnMouseMove(api, args);
                if (args.Handled)
                {
                    break;
                }
            }

            if (!args.Handled) base.OnMouseMove(api, args);
        }


        bool tabPressed = false;
        bool shiftTabPressed = false;
        public override void OnKeyDown(ICoreClientAPI api, KeyEvent args)
        {
            tabPressed = args.KeyCode == (int)GlKeys.Tab;
            shiftTabPressed = tabPressed && args.ShiftPressed;

            if (!HasFocus) return;

            base.OnKeyDown(api, args);

            foreach (GuiElement element in Elements)
            {
                element.OnKeyDown(api, args);
                if (args.Handled) break;
            }

            if (!args.Handled && args.KeyCode == (int)GlKeys.Tab && Tabbable)
            {
                renderFocusHighlight = true;
                GuiElement elem = CurrentTabIndexElement;
                if (elem != null && MaxTabIndex > 0)
                {
                    int dir = args.ShiftPressed ? -1 : 1;
                    int tb = elem.TabIndex + dir;
                    if (tb < 0 || tb > MaxTabIndex || args.CtrlPressed) return;
                    FocusElement(tb);
                    args.Handled = true;
                }
                else if (MaxTabIndex > 0)
                {
                    FocusElement(args.ShiftPressed ? GameMath.Mod(-1, MaxTabIndex + 1) : 0);
                    args.Handled = true;
                }
            }

            // Hardcoded element class type :/
            if (!args.Handled && (args.KeyCode == (int)GlKeys.Enter || args.KeyCode == (int)GlKeys.KeypadEnter) && CurrentTabIndexElement is GuiElementEditableTextBase)
            {
                UnfocusOwnElementsExcept(null);
            }
        }

        public override void OnKeyUp(ICoreClientAPI api, KeyEvent args)
        {
            tabPressed = false;
            shiftTabPressed = false;

            if (!HasFocus) return;

            base.OnKeyUp(api, args);

            foreach (GuiElement element in Elements)
            {
                element.OnKeyUp(api, args);
                if (args.Handled) break;
            }
        }

        public override void OnKeyPress(ICoreClientAPI api, KeyEvent args)
        {
            if (!HasFocus) return;

            base.OnKeyPress(api, args);

            foreach (GuiElement element in Elements)
            {
                element.OnKeyPress(api, args);
                if (args.Handled) break;
            }
        }

        public override void OnMouseWheel(ICoreClientAPI api, MouseWheelEventArgs args)
        {
            if (!Bounds.ParentBounds.PointInside(api.Input.MouseX, api.Input.MouseY)) return;

            // Prefer an element that is currently hovered 
            foreach (var element in Elements)
            {
                if (element.IsPositionInside(api.Input.MouseX, api.Input.MouseY))
                {
                    element.OnMouseWheel(api, args);
                }

                if (args.IsHandled) return;
            }

            foreach (GuiElement element in Elements)
            {
                element.OnMouseWheel(api, args);
                if (args.IsHandled) break;
            }
        }

        public override void OnFocusGained()
        {
            base.OnFocusGained();

            if (CurrentTabIndexElement != null) return;

            renderFocusHighlight = tabPressed;
            if (shiftTabPressed) FocusElement(MaxTabIndex);
            else FocusElement(FirstTabbableElement.TabIndex);
        }

        public override void OnFocusLost()
        {
            base.OnFocusLost();

            renderFocusHighlight = false;
            UnfocusOwnElements();
        }


        public override void RenderInteractiveElements(float deltaTime)
        {
            api.Render.Render2DTexturePremultipliedAlpha(listTexture.TextureId, Bounds);

            MouseOverCursor = null;
            foreach (GuiElement element in Elements)
            {
                element.RenderInteractiveElements(deltaTime);

                if (element.IsPositionInside(api.Input.MouseX, api.Input.MouseY))
                {
                    MouseOverCursor = element.MouseOverCursor;
                }
            }

            ElementBounds tempClipBounds;
            foreach (GuiElement element in Elements)
            {
                // Seperate due to clipping
                if (element.HasFocus && renderFocusHighlight)
                {
                    if (InsideClipBounds != null)
                    {
                        tempClipBounds = element.InsideClipBounds;
                        element.InsideClipBounds = null;
                        element.RenderFocusOverlay(deltaTime);
                        element.InsideClipBounds = tempClipBounds;
                    }
                    else element.RenderFocusOverlay(deltaTime);
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            listTexture.Dispose();

            foreach (var val in Elements)
            {
                val.Dispose();
            }
        }

    }

    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Adds a container to the current GUI. Can be used to add any gui element within a scrollable window.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="bounds">The bounds of the cell.</param>
        /// <param name="key">The identifier for the list.</param>
        public static GuiComposer AddContainer(this GuiComposer composer, ElementBounds bounds, string key = null)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementContainer(composer.Api, bounds), key);
            }

            return composer;
        }

        /// <summary>
        /// Gets the container by key
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="key">The name of the list to get.</param>
        /// <returns></returns>
        public static GuiElementContainer GetContainer(this GuiComposer composer, string key)
        {
            return (GuiElementContainer)composer.GetElement(key);
        }
    }

}
