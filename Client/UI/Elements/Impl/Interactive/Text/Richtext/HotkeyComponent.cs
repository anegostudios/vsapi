using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cairo;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{

    public class HotkeyComponent : RichTextComponent
    {
        LoadedTexture hotkeyTexture;
        HotKey hotkey;
        Vec4f col;

        public HotkeyComponent(ICoreClientAPI api, string hotkeycode, CairoFont font) : base(api, hotkeycode, font)
        {
            if (api.Input.HotKeys.TryGetValue(hotkeycode.ToLowerInvariant(), out hotkey))
            {
                DisplayText = hotkey.CurrentMapping.ToString();
            }

            init();
            hotkeyTexture = new LoadedTexture(api);

            col = new Vec4f((float)font.Color[0], (float)font.Color[1], (float)font.Color[2], (float)font.Color[3]);
        }

        public override void ComposeElements(Context ctxUnused, ImageSurface surfaceUnused)
        {
            //base.ComposeElements(ctx, surface); - dont run default text generation

            GenHotkeyTexture();
        }

        public void GenHotkeyTexture()
        {
            double[] color = (double[])GuiStyle.DialogDefaultTextColor.Clone();
            // Make a mix between white and light brown text
            /*color[0] = (color[0] + 1) / 2;
            color[1] = (color[1] + 1) / 2;
            color[2] = (color[2] + 1) / 2;*/

            string keycode = hotkey == null ? "?" : GlKeyNames.ToString((GlKeys)hotkey.CurrentMapping.KeyCode);

            var extents = Font.GetTextExtents(keycode);
            double textWidth = extents.Width;
            double textHeight = Font.GetFontExtents().Height;
            int lineheight = (int)textHeight;

            double x = 0, y = 0;

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)textWidth + 8, lineheight);
            Context ctx = new Context(surface);

            Font.SetupContext(ctx);
            GuiElement.RoundRectangle(ctx, x + 1, y + 1, textWidth + 6, lineheight - 2, 3.5);
            ctx.SetSourceRGBA(color);
            ctx.LineWidth = 1.5;
            ctx.StrokePreserve();

            ctx.SetSourceRGBA(new double[] { 1, 1, 1, 0.5 });
            ctx.Fill();
            ctx.SetSourceRGBA(new double[] { 1, 1, 1, 1 });


            api.Gui.Text.DrawTextLine(ctx, Font, keycode, x + 3, y + (lineheight - textHeight) / 2 + 2);


            api.Gui.LoadOrUpdateCairoTexture(surface, true, ref hotkeyTexture);
            
            ctx.Dispose();
            surface.Dispose();
        }

        public override void RenderInteractiveElements(float deltaTime, double renderX, double renderY, double renderZ)
        {
            /*for (int i = 0; i < BoundsPerLine.Length; i++)
            {
                var lbounds = BoundsPerLine[i];
                api.Render.RenderRectangle((float)renderX + (float)lbounds.X, (float)renderY + (float)lbounds.Y, 50, (float)lbounds.Width, (float)lbounds.Height, ColorUtil.ColorFromRgba(200, 255, 255, 128));
            }*/

            base.RenderInteractiveElements(deltaTime, renderX, renderY, renderZ);
            
            var textLine = Lines[Lines.Length-1];
            double offsetX = GetFontOrientOffsetX();

            var bounds = textLine.Bounds;
            renderY += hotkeyTexture.Height * 0.15f / 2;

            api.Render.Render2DTexture(hotkeyTexture.TextureId, (float)(renderX + offsetX + bounds.X), (int)(renderY + bounds.Y), hotkeyTexture.Width * 0.85f, hotkeyTexture.Height * 0.85f, (float)renderZ + 50, col);
        }


        public override EnumCalcBoundsResult CalcBounds(TextFlowPath[] flowPath, double currentLineHeight, double offsetX, double lineY, out double nextOffsetX)
        {
            return base.CalcBounds(flowPath, currentLineHeight, offsetX, lineY, out nextOffsetX);
        }


        public override void Dispose()
        {
            base.Dispose();

            hotkeyTexture?.Dispose();
        }

    }
}
