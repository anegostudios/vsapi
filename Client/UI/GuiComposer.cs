using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{

    public interface IGuiComposerManager
    {
        void UnfocusElements();       
    }

    /// <summary>
    /// Composes a dialog which are made from a set of elements
    /// The composed dialog is cached, so to recompose you have to Recompose All elements or instantiate a new composer with doCache set to false
    /// The caching allows the dialog using the composer to not worry about performance and just call compose whenever it has to display a new composed dialog
    /// </summary>
    public class GuiComposer
    {
        public static int Outlines = 0;

        internal IGuiComposerManager composerManager;

        internal Dictionary<string, GuiElement> staticElements = new Dictionary<string, GuiElement>();
        internal Dictionary<string, GuiElement> interactiveElements = new Dictionary<string, GuiElement>();

        List<GuiElement> interactiveElementsInDrawOrder = new List<GuiElement>();

        int currentElementKey; // Default key index if no custom key is set
        int currentFocusableElementKey; // Default key index if no custom key is set

        internal string dialogName;
        int staticElementsTextureId;

        ElementBounds bounds;

        Stack<ElementBounds> parentBoundsForNextElement = new Stack<ElementBounds>();
        Stack<bool> conditionalAdds = new Stack<bool>();

        ElementBounds lastAddedElementBounds;

        internal bool composed = false;
        internal bool recomposeOnRender = false;
        internal bool onlyDynamicRender = false;
        internal bool InsideClip;

        public ICoreClientAPI Api;


        public Action<bool> OnFocusChanged;

        /// <summary>
        /// A unique number assigned to each element
        /// </summary>
        public int CurrentElementKey
        {
            get { return currentElementKey; }
        }


        public ElementBounds Bounds
        {
            get { return bounds; }
        }


        internal GuiComposer(ICoreClientAPI api, ElementBounds bounds, string dialogName)
        {
            this.dialogName = dialogName;
            this.bounds = bounds;
            this.Api = api;
            parentBoundsForNextElement.Push(bounds);
        }

        public static GuiComposer CreateEmpty(ICoreClientAPI api)
        {
            return new GuiComposer(api, ElementBounds.Empty, null).Compose();
        }

        public GuiComposer AddIf(bool condition)
        {
            conditionalAdds.Push(condition);
            return this;
        }
        

        public GuiComposer EndIf()
        {
            if (conditionalAdds.Count > 0) conditionalAdds.Pop();
            return this;
        }

        public GuiComposer BeginChildElements(ElementBounds bounds)
        {
            if (conditionalAdds.Count > 0 && !conditionalAdds.Peek()) return this;

            parentBoundsForNextElement.Peek().WithChild(bounds);
            parentBoundsForNextElement.Push(bounds);

            string key = "element-" + (++currentElementKey);
            staticElements.Add(key, new GuiElementParent(Api, bounds));

            return this;
        }


        public GuiComposer BeginChildElements()
        {
            parentBoundsForNextElement.Push(lastAddedElementBounds);
            return this;
        }

        public GuiComposer EndChildElements()
        {
            if (parentBoundsForNextElement.Count > 1)
            {
                parentBoundsForNextElement.Pop();
            }
             
            return this;
        }


        public GuiComposer OnlyDynamic()
        {
            onlyDynamicRender = true;
            return this;
        }

        
        public void ReCompose()
        {
            composed = false;
            Compose(false);
        }

        internal void UnFocusElements()
        {
            composerManager.UnfocusElements();
            OnFocusChanged?.Invoke(false);
        }


        public GuiElement CurrentTabIndexElement
        {
            get {
                foreach (GuiElement element in interactiveElements.Values)
                {
                    if (element.Focusable && element.HasFocus)
                    {
                        return element;
                    }
                }

                return null;
            }
        }

        public int MaxTabIndex
        {
            get
            {
                int tabIndex = -1;
                foreach (GuiElement element in interactiveElements.Values)
                {
                    if (element.Focusable)
                    {
                        tabIndex = Math.Max(tabIndex, element.TabIndex);
                    }
                }

                return tabIndex;
            }
        }


        public bool FocusElement(int tabIndex)
        {
            GuiElement newFocusedElement = null;

            foreach (GuiElement element in interactiveElements.Values)
            {
                if (element.Focusable && element.TabIndex == tabIndex)
                {
                    newFocusedElement = element;
                    break;
                }
            }

            if (newFocusedElement != null) {
                UnfocusOwnElementsExcept(newFocusedElement);
                newFocusedElement.OnFocusGained();
                OnFocusChanged?.Invoke(true);
                return true;
            }

            return false;
        }

        public void UnfocusOwnElements()
        {
            UnfocusOwnElementsExcept(null);
        }

        public void UnfocusOwnElementsExcept(GuiElement elem)
        {
            foreach (GuiElement element in interactiveElements.Values)
            {
                if (element == elem) continue;

                if (element.Focusable && element.HasFocus)
                {
                    element.OnFocusLost();
                    OnFocusChanged?.Invoke(false);
                }
            }
        }

        public GuiComposer Compose(bool focusFirstElement = true)
        {
            if (composed)
            {
                if (focusFirstElement && MaxTabIndex >= 0) FocusElement(0);
                return this;
            }

            bounds.Initialized = false;
            try
            {
                bounds.CalcWorldBounds();
            } catch (Exception e)
            {
                Api.World.Logger.Error("Exception thrown when trying to calculate world bounds for gui composite " + dialogName + ": " + e);
            }
            
            bounds.IsDrawingSurface = true;

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)bounds.OuterWidth, (int)bounds.OuterHeight);
            Context ctx = new Context(surface);
            ctx.SetSourceRGBA(0, 0, 0, 0);
            ctx.Paint();
            ctx.Antialias = Antialias.Best;

            foreach (GuiElement element in staticElements.Values)
            {
                element.ComposeElements(ctx, surface);
            }

            interactiveElementsInDrawOrder.Clear();

            foreach (GuiElement element in interactiveElements.Values)
            {
                int insertPos = 0;
                foreach (GuiElement addedElem in interactiveElementsInDrawOrder)
                {
                    if (element.DrawOrder >= addedElem.DrawOrder) insertPos++;
                    else break;
                }

                interactiveElementsInDrawOrder.Insert(insertPos, element);
            }

            int oldTexId = staticElementsTextureId;

            //surface.WriteToPng(dialogName+".png");
           
            staticElementsTextureId = Api.Gui.LoadCairoTexture(surface, true);
            if (oldTexId > 0) Api.Gui.DeleteTexture(oldTexId);

            

            ctx.Dispose();
            surface.Dispose();

            composed = true;

            if (focusFirstElement && MaxTabIndex >= 0) FocusElement(0);

            return this;
        }


        public void OnMouseUp(ICoreClientAPI api, MouseEvent mouse)
        {
            foreach (GuiElement element in interactiveElements.Values)
            {
                element.OnMouseUp(api, mouse);
            }
            
        }

        public void OnMouseDown(ICoreClientAPI api, MouseEvent mouse)
        {
            bool beforeHandled = false;
            bool nowHandled = false;
            foreach (GuiElement element in interactiveElements.Values)
            {
                if (!beforeHandled)
                {
                    element.OnMouseDown(api, mouse);
                    nowHandled = mouse.Handled;
                }

                if (!beforeHandled && nowHandled)
                {
                    if (element.Focusable && !element.HasFocus)
                    {
                        element.OnFocusGained();
                        if (element.HasFocus)
                        {
                            OnFocusChanged?.Invoke(true);
                        }
                    }
                } else
                {
                    if (element.Focusable && element.HasFocus)
                    {
                        element.OnFocusLost();
                    }
                }

                beforeHandled = nowHandled;

                //if (nowHandled) break; - why is this here? it needs to loop through all so that it can call focuslost on other elems
            }
            
        }

        public void OnMouseMove(ICoreClientAPI api, MouseEvent mouse)
        {
            foreach (GuiElement element in interactiveElements.Values)
            {
                element.OnMouseMove(api, mouse);
                if (mouse.Handled)
                {
                    break;
                }
            }
        }

        public void OnMouseWheel(ICoreClientAPI api, MouseWheelEventArgs mouse) {
            // Prefer an element that is currently hovered 
            foreach (GuiElement element in interactiveElements.Values)
            {
                if (element.IsPositionInside(api.Input.MouseX, api.Input.MouseY)) {
                    element.OnMouseWheel(api, mouse);
                }
                
                if (mouse.IsHandled) return;
            }


            foreach (GuiElement element in interactiveElements.Values)
            {
                element.OnMouseWheel(api, mouse);
                if (mouse.IsHandled) break;
            }
        }


        public void OnKeyDown(ICoreClientAPI api, KeyEvent args, bool haveFocus)
        {
            foreach (GuiElement element in interactiveElements.Values)
            {
                element.OnKeyDown(api, args);
                if (args.Handled) break;
            }

            if (haveFocus && !args.Handled && args.KeyCode == (int)GlKeys.Tab)
            {
                GuiElement elem = CurrentTabIndexElement;
                if (elem != null && MaxTabIndex > 0)
                {
                    int dir = args.ShiftPressed ? -1 : 1;
                    int tb = GameMath.Mod(elem.TabIndex + dir, MaxTabIndex+1);
                    FocusElement(tb);
                    args.Handled = true;
                }
            }

            // Hardcoded element class type :/
            if (!args.Handled && args.KeyCode == (int)GlKeys.Enter && CurrentTabIndexElement is GuiElementEditableTextBase)
            {
                UnfocusOwnElementsExcept(null);
            }
        }

        public void OnKeyPress(ICoreClientAPI api, KeyEvent args) {

            foreach (GuiElement element in interactiveElements.Values)
            {
                element.OnKeyPress(api, args);
                if (args.Handled) break;
            }
        }

        public void PostRender(float deltaTime)
        {
            // Window size goes 0 when minimized
            if (Api.Render.FrameWidth == 0 || Api.Render.FrameHeight == 0)
            {
                return;
            }

            if (bounds.ParentBounds.RequiresRecalculation)
            {
                Api.Logger.Notification("Window probably resized, recalculating dialog bounds and recomposing " + dialogName + "...");
                bounds.MarkDirtyRecursive();
                bounds.ParentBounds.CalcWorldBounds();

                // Check here to because it would crash otherwise when trying to calc bounds and child bounds
                if (bounds.ParentBounds.OuterWidth == 0 || bounds.ParentBounds.OuterHeight == 0)
                {
                    return;
                }

                
                bounds.CalcWorldBounds();

                ReCompose();
            }

            foreach (GuiElement element in interactiveElementsInDrawOrder)
            {
                element.PostRenderInteractiveElements(deltaTime);
            }

        }

        public void Render(float deltaTime)
        {
            if (recomposeOnRender)
            {
                composed = false;
                Compose();
                recomposeOnRender = false;
            }

            if (!onlyDynamicRender)
            {
                Api.Render.Render2DTexture(staticElementsTextureId, bounds);
            }

            foreach (GuiElement element in interactiveElementsInDrawOrder)
            {
                element.RenderInteractiveElements(deltaTime);
            }


            foreach (GuiElement element in interactiveElementsInDrawOrder)
            {
                // Seperate due to clipping
                if (element.HasFocus)
                {
                    element.RenderFocusOverlay(deltaTime);
                }
            }

            if (Outlines == 1)
            {
                Api.Render.RenderRectangle((int)bounds.renderX, (int)bounds.renderY, (int)bounds.OuterWidth, 500, (int)bounds.OuterHeight, 255 + (255 << 8) + (255 << 16) + (255 << 24));

                int i = 0;
                foreach (GuiElement elem in staticElements.Values)
                {
                    Api.Render.RenderRectangle((int)elem.Bounds.renderX, (int)elem.Bounds.renderY, 500, (int)elem.Bounds.OuterWidth, (int)elem.Bounds.OuterHeight, elem.OutlineColor());

                    i++;
                }
            }

            if (Outlines == 2) { 
                int i = 0;
                foreach (GuiElement elem in interactiveElements.Values)
                {
                    Api.Render.RenderRectangle((int)elem.Bounds.renderX, (int)elem.Bounds.renderY, 500, (int)elem.Bounds.OuterWidth, (int)elem.Bounds.OuterHeight, elem.OutlineColor());

                    i++;
                }
            }
        }



        internal static double scaled(double value)
        {
            return value * RuntimeEnv.GUIScale;
        }


        public GuiComposer AddInteractiveElement(GuiElement element, string key = null)
        {
            if (conditionalAdds.Count > 0 && !conditionalAdds.Peek()) return this;

            if (key == null)
            {
                key = "element-" + (++currentElementKey);
            }

            interactiveElements.Add(key, element);
            staticElements.Add(key, element);

            if (element.Focusable)
            {
                element.TabIndex = currentFocusableElementKey++;
            } else
            {
                element.TabIndex = -1;
            }

            element.InsideClipElement = InsideClip;

            parentBoundsForNextElement.Peek().WithChild(element.Bounds);

            lastAddedElementBounds = element.Bounds;

            return this;
        }

        public GuiComposer AddStaticElement(GuiElement element, string key = null)
        {
            if (conditionalAdds.Count > 0 && !conditionalAdds.Peek()) return this;

            if (key == null)
            {
                key = "element-" + (++currentElementKey);
            }
            staticElements.Add(key, element);
            parentBoundsForNextElement.Peek().WithChild(element.Bounds);

            lastAddedElementBounds = element.Bounds;

            element.InsideClipElement = InsideClip;

            return this;
        }


        public GuiElement GetElement(string key)
        {
            if (interactiveElements.ContainsKey(key))
            {
                return interactiveElements[key];
            }

            if (staticElements.ContainsKey(key))
            {
                return staticElements[key];
            }

            return null;
        }



    }
}
