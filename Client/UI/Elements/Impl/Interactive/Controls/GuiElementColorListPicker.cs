using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Creates a toggle button for the GUI.
    /// </summary>
    public class GuiElementColorListPicker : GuiElementControl
    {
        Common.Action<bool> handler;

        /// <summary>
        /// Is this button on?
        /// </summary>
        public bool On;

        LoadedTexture activeTexture;

        int color;


        /// <summary>
        /// Is this element capable of being in the focus?
        /// </summary>
        public override bool Focusable { get { return true; } }

        /// <summary>
        /// Constructor for the button
        /// </summary>
        /// <param name="capi">The core client API.</param>
        /// <param name="color"></param>
        /// <param name="OnToggled">The action that happens when the button is toggled.</param>
        /// <param name="bounds">The bounding box of the button.</param>
        public GuiElementColorListPicker(ICoreClientAPI capi, int color, Common.Action<bool> OnToggled, ElementBounds bounds) : base(capi, bounds)
        {
            activeTexture = new LoadedTexture(capi);

            this.color = color;
            handler = OnToggled;
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
            double[] dcolor = ColorUtil.ToRGBADoubles(color);
            ctx.SetSourceRGBA(dcolor[2], dcolor[1], dcolor[0], 1);
            RoundRectangle(ctx, Bounds.drawX, Bounds.drawY, Bounds.InnerWidth, Bounds.InnerHeight, 1);
            ctx.Fill();

            ComposeActiveButton();
        }

        void ComposeActiveButton()
        {
            ImageSurface surface = new ImageSurface(Format.Argb32, (int)Bounds.InnerWidth + 6, (int)Bounds.InnerHeight + 6);
            Context ctx = genContext(surface);

            ctx.SetSourceRGBA(0, 0, 0, 0);
            ctx.Paint();

            ctx.SetSourceRGBA(1, 1, 1, 0.65);
            RoundRectangle(ctx, 3, 3, Bounds.InnerWidth + 3, Bounds.InnerHeight + 3, 1);
            ctx.LineWidth = 3;
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
        }
    }


    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Gets the toggle button by name in the GUIComposer.
        /// </summary>
        /// <param name="key">The name of the button.</param>
        /// <returns>A button.</returns>
        public static GuiElementColorListPicker GetColorListPicker(this GuiComposer composer, string key)
        {
            return (GuiElementColorListPicker)composer.GetElement(key);
        }


        /// <summary>
        /// Toggles the given button.
        /// </summary>
        /// <param name="key">The name of the button that was set.</param>
        /// <param name="selectedIndex">the index of the button.</param>
        public static void ColorListPickerSetValue(this GuiComposer composer, string key, int selectedIndex)
        {
            int i = 0;
            GuiElementColorListPicker btn;
            while ((btn = composer.GetColorListPicker(key + "-" + i)) != null)
            {
                btn.SetValue(i == selectedIndex);
                i++;
            }
        }


        /// <summary>
        /// Adds multiple buttons with Text.
        /// </summary>
        /// <param name="texts">The texts on all the buttons.</param>
        /// <param name="font">The font for the buttons</param>
        /// <param name="onToggle">The event fired when the button is pressed.</param>
        /// <param name="bounds">The bounds of the buttons.</param>
        /// <param name="key">The key given to the bundle of buttons.</param>
        public static GuiComposer AddColorListPicker(this GuiComposer composer, int[] colors, Common.Action<int> onToggle, ElementBounds startBounds, int maxLineWidth, string key = null)
        {
            if (!composer.composed)
            {
                if (key == null) key = "colorlistpicker";

                int quantityButtons = colors.Length;
                double lineWidth = 0;

                for (int i = 0; i < colors.Length; i++)
                {
                    int index = i;

                    if (lineWidth > maxLineWidth)
                    {
                        startBounds.fixedX -= lineWidth;
                        startBounds.fixedY += startBounds.fixedHeight + 5;
                        lineWidth = 0;
                    }

                    composer.AddInteractiveElement(
                        new GuiElementColorListPicker(composer.Api, colors[i], (on) => {
                            if (on)
                            {
                                onToggle(index);
                                for (int j = 0; j < quantityButtons; j++)
                                {
                                    if (j == index) continue;
                                    composer.GetColorListPicker(key + "-" + j).SetValue(false);
                                }
                            }
                            else
                            {
                                composer.GetColorListPicker(key + "-" + index).SetValue(true);
                            }
                        }, startBounds.FlatCopy()),
                        key + "-" + i
                    );

                    startBounds.fixedX += startBounds.fixedWidth + 5;

                    lineWidth += startBounds.fixedWidth + 5;

                }
            }
            return composer;
        }




    }

}
