using System;
using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class GuiElementDynamicText : GuiElementTextBase
    {
        EnumTextOrientation orientation;
        public float lineHeightMultiplier;

        int textureId;
        float[] strokeRGB = new float[] { 0, 0, 0, 1 };
        double strokeWidth = 0.5;

        public Action OnClick;
        public bool autoHeight;

        public GuiElementDynamicText(ICoreClientAPI capi, string text, CairoFont font, EnumTextOrientation orientation, ElementBounds bounds) : base(capi, text, font, bounds)
        {
            this.orientation = orientation;
            lineHeightMultiplier = 1f;
        }

        public override void ComposeTextElements(Context ctx, ImageSurface surface)
        {
            RecomposeMultiLine();
        }

     
        public void AutoHeight()
        {
            Bounds.fixedHeight = GetMultilineTextHeight(text, Bounds.InnerWidth, lineHeightMultiplier) / ClientSettingsApi.GUIScale;
            Bounds.CalcWorldBounds();
            autoHeight = true;
        }

        public void RecomposeMultiLine()
        {
            if (autoHeight) AutoHeight();

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)Bounds.InnerWidth, (int)Bounds.InnerHeight);
            Context ctx = genContext(surface);
            ShowMultilineText(ctx, text, 0, 0, Bounds.InnerWidth, orientation, lineHeightMultiplier);

            generateTexture(surface, ref textureId);
            ctx.Dispose();
            surface.Dispose();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            api.Render.Render2DTexturePremultipliedAlpha(textureId, (int)Bounds.renderX, (int)Bounds.renderY, (int)Bounds.InnerWidth, (int)Bounds.InnerHeight);
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseDownOnElement(api, args);

            OnClick?.Invoke();
        }
        


        internal void enableStroke()
        {
            textPathMode = true;
        }

        public void SetNewText(string text, bool autoHeight = false, bool forceRedraw = false)
        {
            if (this.text != text || forceRedraw)
            {
                this.text = text;
                Bounds.CalcWorldBounds();
                if (autoHeight) AutoHeight();
                
                RecomposeMultiLine();
            }
        }

        public void setStroke(float[] rgba, double thickness)
        {
            textPathMode = true;
            this.strokeRGB = rgba;
            this.strokeWidth = thickness;
        }


    }


    public static class GuiElementDynamicTextHelper
    {
        public static GuiComposer AddDynamicText(this GuiComposer composer, string text, CairoFont font, EnumTextOrientation orientation, ElementBounds bounds, float lineheightmultiplier = 1f, string key = null)
        {
            if (!composer.composed)
            {
                GuiElementDynamicText elem = new GuiElementDynamicText(composer.Api, text, font, orientation, bounds);
                elem.lineHeightMultiplier = lineheightmultiplier;
                composer.AddInteractiveElement(elem, key);
            }
            return composer;
        }



        public static GuiElementDynamicText GetDynamicText(this GuiComposer composer, string key)
        {
            return (GuiElementDynamicText)composer.GetElement(key);
        }



    }

}
