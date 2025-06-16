using System;
using Cairo;

#nullable disable

namespace Vintagestory.API.Client
{
    public class GuiElementSwitch : GuiElementControl
    {
        Action<bool> handler;

        LoadedTexture onTexture;

        /// <summary>
        /// Wether the switch has been toggled to On
        /// </summary>
        public bool On;

        internal double unscaledPadding;
        internal double unscaledSize;

        public override bool Focusable { get { return enabled; } }

        /// <summary>
        /// Creates a switch which can be toggled.
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="OnToggled">The event that happens when the switch is flipped.</param>
        /// <param name="bounds">The bounds of the element.</param>
        /// <param name="size">The size of the switch. (Default: 30)</param>
        /// <param name="padding">The padding on the outside of the switch (Default: 5)</param>
        public GuiElementSwitch(ICoreClientAPI capi, Action<bool> OnToggled, ElementBounds bounds, double size = 30, double padding = 4) : base(capi, bounds)
        {
            onTexture = new LoadedTexture(capi);

            bounds.fixedWidth = size;
            bounds.fixedHeight = size;

            this.unscaledPadding = padding;
            this.unscaledSize = size;

            this.handler = OnToggled;
        }

        public override void ComposeElements(Context ctxStatic, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            ctxStatic.SetSourceRGBA(0, 0, 0, 0.2);
            RoundRectangle(ctxStatic, Bounds.drawX, Bounds.drawY, Bounds.InnerWidth, Bounds.InnerHeight, 1);
            ctxStatic.Fill();
            EmbossRoundRectangleElement(ctxStatic, Bounds, true, 1, 1);

            genOnTexture();
        }

        private void genOnTexture()
        {
            double size = scaled(unscaledSize - 2 * unscaledPadding);

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)size, (int)size);
            Context ctx = genContext(surface);

            RoundRectangle(ctx, 0, 0, size, size, 1);
            if (enabled) ctx.SetSourceRGBA(0, 0, 0, 1);
            else ctx.SetSourceRGBA(0.15, 0.15, 0, 0.65);
            ctx.FillPreserve();
            fillWithPattern(api, ctx, waterTextureName, false, true, enabled ? 255 : 127, 0.5f);

            generateTexture(surface, ref onTexture);

            ctx.Dispose();
            surface.Dispose();
        }


        public override void RenderInteractiveElements(float deltaTime)
        {
            if (On)
            {
                double padding = scaled(unscaledPadding);
                api.Render.Render2DLoadedTexture(onTexture, (int)(Bounds.renderX + padding), (int)(Bounds.renderY + padding));
            }
            
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseDownOnElement(api, args);

            if (!enabled) return;
            On = !On;
            handler?.Invoke(On);
            api.Gui.PlaySound("toggleswitch");
        }


        public override void OnKeyDown(ICoreClientAPI api, KeyEvent args)
        {
            if (!HasFocus) return;
            if (args.KeyCode == (int)GlKeys.Enter || args.KeyCode == (int)GlKeys.Space)
            {
                args.Handled = true;
                On = !On;
                handler?.Invoke(On);
                api.Gui.PlaySound("toggleswitch");
            }
        }

        /// <summary>
        /// Sets the value of the switch on or off.
        /// </summary>
        /// <param name="on">on == true.</param>
        public void SetValue(bool on)
        {
            On = on;
        }

        public override void Dispose()
        {
            base.Dispose();

            onTexture.Dispose();
        }

    }

    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Adds a switch to the GUI.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="onToggle">The event that happens when the switch is toggled.</param>
        /// <param name="bounds">The bounds of the switch.</param>
        /// <param name="key">the name of the switch. (Default: null)</param>
        /// <param name="size">The size of the switch (Default: 30)</param>
        /// <param name="padding">The padding around the switch (Default: 5)</param>
        public static GuiComposer AddSwitch(this GuiComposer composer, Action<bool> onToggle, ElementBounds bounds, string key = null, double size = 30, double padding = 4)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementSwitch(composer.Api, onToggle, bounds, size, padding), key);
            }
            return composer;
        }

        /// <summary>
        /// Gets the switch by name.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="key">The internal name of the switch.</param>
        /// <returns>Returns the named switch.</returns>
        public static GuiElementSwitch GetSwitch(this GuiComposer composer, string key)
        {
            return (GuiElementSwitch)composer.GetElement(key);
        }
    }
}
