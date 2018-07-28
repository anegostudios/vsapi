using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    class GuiElementClip : GuiElement
    {
        bool clip;

        public GuiElementClip(ICoreClientAPI capi, bool clip, ElementBounds bounds) : base(capi, bounds)
        {
            this.clip = clip;
        }

        public override void ComposeElements(Context ctxStatic, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            if (clip)
            {
               api.Render.BeginScissor(Bounds);
            } else
            {
                api.Render.EndScissor();
            }
        }

        public override int OutlineColor()
        {
            return (255 << 16) + (255 << 24);
        }


        public override void OnMouseDown(ICoreClientAPI api, MouseEvent mouse)
        {
            // Can't be interacted with
        }

    }



    public static class GuiElementClipHelpler
    {

        public static GuiComposer BeginClip(this GuiComposer composer, ElementBounds bounds)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementClip(composer.Api, true, bounds));
                composer.InsideClip = true;
                composer.BeginChildElements();
            }
            return composer;
        }

        public static GuiComposer EndClip(this GuiComposer composer)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementClip(composer.Api, false, ElementBounds.Empty));
                composer.InsideClip = false;
                composer.EndChildElements();
            }
            return composer;
        }


    }
}
