using Cairo;
using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    public delegate void RenderSkillItemDelegate(AssetLocation code, float dt, double atPosX, double atPosY);

    public delegate void DrawSkillIconDelegate(Context cr, int x, int y, float width, float height, double[] rgba);

    public class SkillItem
    {
        public string Name;
        public string Description;
        public AssetLocation Code;
        public LoadedTexture Texture;
        public KeyCombination Hotkey;

        public RenderSkillItemDelegate RenderHandler;

        public object Data;


        public SkillItem WithIcon(ICoreClientAPI capi, DrawSkillIconDelegate onDrawIcon)
        {
            Texture = capi.Gui.Icons.GenTexture(48, 48, (ctx, surface) =>
            {
                onDrawIcon(ctx, 5, 5, 38, 38, ColorUtil.WhiteArgbDouble);
            });

            return this;
        }

        public void Dispose()
        {
            Texture?.Dispose();
        }
    }
}
