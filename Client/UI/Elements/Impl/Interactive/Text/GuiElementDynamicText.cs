using System;
using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.Config;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Text that can be changed after being composed
    /// </summary>
    public class GuiElementDynamicText : GuiElementTextBase
    {
        EnumTextOrientation orientation;

        LoadedTexture textTexture;
        
        public Action OnClick;
        public bool autoHeight;

        public int QuantityTextLines { get
            {
                return textUtil.GetQuantityTextLines(Font, text, Bounds.InnerWidth);
            }
        }


        /// <summary>
        /// Adds a new element that renders text dynamically.
        /// </summary>
        /// <param name="capi">The client API.</param>
        /// <param name="text">The starting text on the component.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="orientation">The orientation of the text.</param>
        /// <param name="bounds">the bounds of the text.</param>
        public GuiElementDynamicText(ICoreClientAPI capi, string text, CairoFont font, EnumTextOrientation orientation, ElementBounds bounds) : base(capi, text, font, bounds)
        {
            this.orientation = orientation;
            textTexture = new LoadedTexture(capi);
        }

        public override void ComposeTextElements(Context ctx, ImageSurface surface)
        {
            RecomposeText();
        }

     
        /// <summary>
        /// Automatically adjusts the height of the dynamic text.
        /// </summary>
        public void AutoHeight()
        {
            Bounds.fixedHeight = GetMultilineTextHeight() / RuntimeEnv.GUIScale;
            Bounds.CalcWorldBounds();
            autoHeight = true;
        }

        /// <summary>
        /// Recomposes the element for lines.
        /// </summary>
        public void RecomposeText()
        {
            if (autoHeight) AutoHeight();

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)Bounds.InnerWidth, (int)Bounds.InnerHeight);
            Context ctx = genContext(surface);
            DrawMultilineTextAt(ctx, 0, 0, orientation);
            
            generateTexture(surface, ref textTexture);

           // surface.WriteToPng("bla.png");

            ctx.Dispose();
            surface.Dispose();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            api.Render.Render2DTexturePremultipliedAlpha(textTexture.TextureId, (int)Bounds.renderX, (int)Bounds.renderY, (int)Bounds.InnerWidth, (int)Bounds.InnerHeight);
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseDownOnElement(api, args);

            OnClick?.Invoke();
        }
        

        
        /// <summary>
        /// Sets the text value of the element.
        /// </summary>
        /// <param name="text">The text of the component.</param>
        /// <param name="autoHeight">Whether the height of the component should be modified.</param>
        /// <param name="forceRedraw">Whether the element should be redrawn.</param>
        public void SetNewText(string text, bool autoHeight = false, bool forceRedraw = false)
        {
            if (this.text != text || forceRedraw)
            {
                this.text = text;
                Bounds.CalcWorldBounds();
                if (autoHeight) AutoHeight();
                
                RecomposeText();
            }
        }
        

        public override void Dispose()
        {
            textTexture?.Dispose();
        }

    }


    public static class GuiElementDynamicTextHelper
    {
        /// <summary>
        /// Adds dynamic text to the GUI.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="orientation"></param>
        /// <param name="bounds"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static GuiComposer AddDynamicText(this GuiComposer composer, string text, CairoFont font, EnumTextOrientation orientation, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                GuiElementDynamicText elem = new GuiElementDynamicText(composer.Api, text, font, orientation, bounds);
                composer.AddInteractiveElement(elem, key);
            }
            return composer;
        }


        /// <summary>
        /// Gets the Dynamic Text by name from the GUI.
        /// </summary>
        /// <param name="key">The name of the element.</param>
        public static GuiElementDynamicText GetDynamicText(this GuiComposer composer, string key)
        {
            return (GuiElementDynamicText)composer.GetElement(key);
        }



    }

}
