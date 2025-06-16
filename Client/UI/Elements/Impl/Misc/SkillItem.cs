using Cairo;
using System;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    public delegate void RenderSkillItemDelegate(AssetLocation code, float dt, double atPosX, double atPosY);

    public delegate void DrawSkillIconDelegate(Context cr, int x, int y, float width, float height, double[] rgba);

    public class SkillItem : IDisposable
    {
        public string Name;
        public string Description;
        public AssetLocation Code;
        public LoadedTexture Texture;
        public KeyCombination Hotkey;
        public bool Linebreak = false;
        public bool Enabled = true;

        public RenderSkillItemDelegate RenderHandler;

        public object Data;
        public bool TexturePremultipliedAlpha = true;

        public SkillItem WithIcon(ICoreClientAPI capi, DrawSkillIconDelegate onDrawIcon)
        {
            if (capi == null) return this;

            Texture = capi.Gui.Icons.GenTexture(48, 48, (ctx, surface) =>
            {
                onDrawIcon(ctx, 5, 5, 38, 38, ColorUtil.WhiteArgbDouble);
            });

            return this;
        }

        public SkillItem WithIcon(ICoreClientAPI capi, string iconCode)
        {
            if (capi == null) return this;

            Texture = capi.Gui.Icons.GenTexture(48, 48, (ctx, surface) =>
            {
                capi.Gui.Icons.DrawIcon(ctx, iconCode, 5, 5, 38, 38, ColorUtil.WhiteArgbDouble);
            });

            return this;
        }

        public SkillItem WithLetterIcon(ICoreClientAPI capi, string letter)
        {
            if (capi == null) return this;

            int isize = (int)GuiElement.scaled(48);

            Texture = capi.Gui.Icons.GenTexture(isize, isize, (ctx, surface) =>
            {
                var font = CairoFont.WhiteMediumText().WithColor(new double[] { 1, 1, 1, 1 });
                font.SetupContext(ctx);
                var size = font.GetTextExtents(letter);
                var asc = font.GetFontExtents().Ascent + GuiElement.scaled(2);
                capi.Gui.Text.DrawTextLine(ctx, font, letter, (isize - size.Width) / 2, (isize - asc) / 2);
            });

            return this;
        }


        public SkillItem WithIcon(ICoreClientAPI capi, LoadedTexture texture)
        {
            Texture = texture;
            return this;
        }


        public void Dispose()
        {
            Texture?.Dispose();
        }
    }
}
