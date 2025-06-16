using Cairo;
using System;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Creates a toggle button for the GUI.
    /// </summary>
    public abstract class GuiElementElementListPickerBase<T> : GuiElementControl
    {
        public Action<bool> handler;

        /// <summary>
        /// Is this button on?
        /// </summary>
        public bool On;

        LoadedTexture activeTexture;

        T elem;

        public bool ShowToolTip;
        public string TooltipText
        {
            set { hoverText.SetNewText(value); }
        }

        GuiElementHoverText hoverText;


        /// <summary>
        /// Is this element capable of being in the focus?
        /// </summary>
        public override bool Focusable { get { return enabled; } }

        /// <summary>
        /// Constructor for the button
        /// </summary>
        /// <param name="capi">The core client API.</param>
        /// <param name="elem"></param>
        /// <param name="bounds">The bounding box of the button.</param>
        public GuiElementElementListPickerBase(ICoreClientAPI capi, T elem, ElementBounds bounds) : base(capi, bounds)
        {
            activeTexture = new LoadedTexture(capi);

            this.elem = elem;
            //handler = OnToggled;

            hoverText = new GuiElementHoverText(capi, "", CairoFont.WhiteSmallText(), 200, Bounds.CopyOnlySize());
            hoverText.Bounds.ParentBounds = bounds;
            hoverText.SetAutoWidth(true);
            bounds.ChildBounds.Add(hoverText.Bounds);
        }

        /// <summary>
        /// Composes the element in both the pressed, and released states.
        /// </summary>
        /// <param name="ctx">The context of the element.</param>
        /// <param name="surface">The surface of the element.</param>
        /// <remarks>Neither the context, nor the surface is used in this function.</remarks>
        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            DrawElement(elem, ctx, surface);

            ComposeActiveButton();

            hoverText.ComposeElements(ctx, surface);
        }

        public abstract void DrawElement(T elem, Context ctx, ImageSurface surface);


        void ComposeActiveButton()
        {
            ImageSurface surface = new ImageSurface(Format.Argb32, (int)Bounds.InnerWidth + 6, (int)Bounds.InnerHeight + 6);
            Context ctx = genContext(surface);

            ctx.SetSourceRGBA(0, 0, 0, 0);
            ctx.Paint();

            ctx.SetSourceRGBA(1, 1, 1, 0.65);
            RoundRectangle(ctx, 3, 3, Bounds.InnerWidth + 1, Bounds.InnerHeight + 1, 1);
            ctx.LineWidth = 2;
            ctx.Stroke();

            generateTexture(surface, ref activeTexture);

            ctx.Dispose();
            surface.Dispose();
        }

        /// <summary>
        /// Renders the button.
        /// </summary>
        /// <param name="deltaTime">The time elapsed.</param>
        public override void RenderInteractiveElements(float deltaTime)
        {
            if (On) {
                api.Render.Render2DTexturePremultipliedAlpha(activeTexture.TextureId, Bounds.renderX - 3, Bounds.renderY - 3, activeTexture.Width, activeTexture.Height);
            }

            if (ShowToolTip)
            {
                hoverText.RenderInteractiveElements(deltaTime);
            }
        }

        /// <summary>
        /// Handles the mouse button press while the mouse is on this button.
        /// </summary>
        /// <param name="api">The client API</param>
        /// <param name="args">The mouse event arguments.</param>
        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseDownOnElement(api, args);

            On = !On;
            handler?.Invoke(On);
            api.Gui.PlaySound("toggleswitch");
        }

        /// <summary>
        /// Handles the mouse button release while the mouse is on this button.
        /// </summary>
        /// <param name="api">The client API</param>
        /// <param name="args">The mouse event arguments</param>
        public override void OnMouseUpOnElement(ICoreClientAPI api, MouseEvent args)
        {
            //if (!Toggleable) On = false;
        }

        /// <summary>
        /// Handles the event fired when the mouse is released.
        /// </summary>
        /// <param name="api">The client API</param>
        /// <param name="args">Mouse event arguments</param>
        public override void OnMouseUp(ICoreClientAPI api, MouseEvent args)
        {
            //if (!Toggleable) On = false;
            base.OnMouseUp(api, args);
        }

        /// <summary>
        /// Sets the value of the button.
        /// </summary>
        /// <param name="on">Am I on or off?</param>
        public void SetValue(bool on)
        {
            On = on;
        }

        /// <summary>
        /// Disposes of the button.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            activeTexture.Dispose();
            hoverText?.Dispose();
        }
    }


    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Adds multiple buttons with Text.
        /// </summary>
        /// <param name="elems"></param>
        /// <param name="onToggle">The event fired when the button is pressed.</param>
        /// <param name="startBounds">The bounds of the buttons.</param>
        /// <param name="maxLineWidth"></param>
        /// <param name="key">The key given to the bundle of buttons.</param>
        /// <param name="composer"></param>
        /// <param name="pickertype"></param>
        public static GuiComposer AddElementListPicker<T>(this GuiComposer composer, Type pickertype, T[] elems, Action<int> onToggle, ElementBounds startBounds, int maxLineWidth, string key)
        {
            if (!composer.Composed)
            {
                if (key == null) key = "elementlistpicker";

                int quantityButtons = elems.Length;
                double lineWidth = 0;

                for (int i = 0; i < elems.Length; i++)
                {
                    int index = i;

                    if (lineWidth > maxLineWidth)
                    {
                        startBounds.fixedX -= lineWidth;
                        startBounds.fixedY += startBounds.fixedHeight + 5;
                        lineWidth = 0;
                    }

                    var elem = Activator.CreateInstance(pickertype, composer.Api, elems[i], startBounds.FlatCopy()) as GuiElement;
                    composer.AddInteractiveElement(elem, key + "-" + i);

                    (composer[key + "-" + i] as GuiElementElementListPickerBase<T>).handler = (on) =>
                    {
                        if (on)
                        {
                            onToggle(index);
                            for (int j = 0; j < quantityButtons; j++)
                            {
                                if (j == index) continue;
                                (composer[(key + "-" + j)] as GuiElementElementListPickerBase<T>).SetValue(false);
                            }
                        }
                        else
                        {
                            (composer[key + "-" + index] as GuiElementElementListPickerBase<T>).SetValue(true);
                        }
                    };

                    startBounds.fixedX += startBounds.fixedWidth + 5;

                    lineWidth += startBounds.fixedWidth + 5;

                }
            }
            return composer;
        }




    }

}
