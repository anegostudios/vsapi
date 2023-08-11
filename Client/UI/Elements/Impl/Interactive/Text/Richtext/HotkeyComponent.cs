using Cairo;
using System.Collections.Generic;

namespace Vintagestory.API.Client
{
    public class HotkeyComponent : RichTextComponent
    {
        LoadedTexture hotkeyTexture;
        HotKey hotkey;

        public HotkeyComponent(ICoreClientAPI api, string hotkeycode, CairoFont font) : base(api, hotkeycode, font)
        {
            PaddingRight = 4;

            if (api.Input.HotKeys.TryGetValue(hotkeycode.ToLowerInvariant(), out hotkey))
            {
                DisplayText = hotkey.CurrentMapping.ToString();
            }

            init();
            hotkeyTexture = new LoadedTexture(api);
        }

        public override void ComposeElements(Context ctxUnused, ImageSurface surfaceUnused)
        {
            //base.ComposeElements(ctx, surface); - dont run default text generation

            
        }

        public void GenHotkeyTexture()
        {
            List<string> parts = new List<string>();
            
            if (hotkey != null) {
                if (hotkey.CurrentMapping.Ctrl) parts.Add("Ctrl");
                if (hotkey.CurrentMapping.Alt) parts.Add("Alt");
                if (hotkey.CurrentMapping.Shift) parts.Add("Shift");
                parts.Add(GlKeyNames.ToString((GlKeys)hotkey.CurrentMapping.KeyCode));
                if (hotkey.CurrentMapping.SecondKeyCode != null) parts.Add(GlKeyNames.ToString((GlKeys)hotkey.CurrentMapping.SecondKeyCode));
            } else
            {
                parts.Add("?");
            }

            CairoFont font = Font.Clone().WithFontSize((float)Font.UnscaledFontsize * 0.8f);

            double textWidth = 0;
            double pluswdith = font.GetTextExtents("+").Width;
            double symbolspacing = 3;
            double leftRightPad = 3;
            foreach (var part in parts)
            {
                var w = font.GetTextExtents(part).Width + GuiElement.scaled(symbolspacing + 2 * leftRightPad);
                if (textWidth > 0) w += GuiElement.scaled(symbolspacing) + pluswdith;
                textWidth += w;
            }
           
            double textHeight = font.GetFontExtents().Height;
            int lineheight = (int)textHeight;

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)textWidth + 3, lineheight + 5);
            Context ctx = new Context(surface);
            font.SetupContext(ctx);
            
            double hx = 0;
            double y = 0;
            foreach (string part in parts)
            {
                hx = DrawHotkey(api, part, hx, y, ctx, font, lineheight, textHeight, pluswdith, symbolspacing, leftRightPad, font.Color);
            }

            api.Gui.LoadOrUpdateCairoTexture(surface, true, ref hotkeyTexture);
            ctx.Dispose();
            surface.Dispose();
        }

        public override void RenderInteractiveElements(float deltaTime, double renderX, double renderY, double renderZ)
        {
            base.RenderInteractiveElements(deltaTime, renderX, renderY, renderZ);
            
            var textLine = Lines[Lines.Length-1];
            double offsetX = GetFontOrientOffsetX();

            var bounds = textLine.Bounds;
            renderY += hotkeyTexture.Height * 0.15f / 2;

            api.Render.Render2DTexture(hotkeyTexture.TextureId, (float)(renderX + offsetX + bounds.X), (int)(renderY + bounds.Y), hotkeyTexture.Width, hotkeyTexture.Height, (float)renderZ + 50);
        }


        public override EnumCalcBoundsResult CalcBounds(TextFlowPath[] flowPath, double currentLineHeight, double offsetX, double lineY, out double nextOffsetX)
        {
            GenHotkeyTexture();

            return base.CalcBounds(flowPath, currentLineHeight, offsetX, lineY, out nextOffsetX);
        }


        public override void Dispose()
        {
            base.Dispose();

            hotkeyTexture?.Dispose();
        }


        public static double DrawHotkey(ICoreClientAPI capi, string keycode, double x, double y, Context ctx, CairoFont font, double lineheight, double textHeight, double pluswdith, double symbolspacing, double leftRightPadding, double[] color)
        {
            if (x > 0)
            {
                capi.Gui.Text.DrawTextLine(ctx, font, "+", x + symbolspacing, y + (lineheight - textHeight) / 2 + 2);
                x += pluswdith + 2 * symbolspacing;
            }

            double width = font.GetTextExtents(keycode).Width;

            GuiElement.RoundRectangle(ctx, x + 1, y + 1, width + GuiElement.scaled(leftRightPadding*2), lineheight, 3.5);
            ctx.SetSourceRGBA(color);
            ctx.LineWidth = 1.5;
            ctx.StrokePreserve();

            ctx.SetSourceRGBA(new double[] { color[0], color[1], color[2], color[3] * 0.5 });
            ctx.Fill();
            ctx.SetSourceRGBA(new double[] { 1, 1, 1, 1 });

            capi.Gui.Text.DrawTextLine(ctx, font, keycode, x + 1 + GuiElement.scaled(leftRightPadding), y + (lineheight - textHeight) / 2 + 2);

            return x + symbolspacing + width + GuiElement.scaled(leftRightPadding * 2);
        }

    }
}
