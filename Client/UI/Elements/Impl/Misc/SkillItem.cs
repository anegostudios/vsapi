using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Vintagestory.API.Client
{
    public delegate void RenderSkillItemDelegate(AssetLocation code, float dt, double atPosX, double atPosY);

    public class SkillItem
    {
        public string Name;
        public string Description;
        public AssetLocation Code;
        public LoadedTexture Texture;
        public KeyCombination Hotkey;

        public RenderSkillItemDelegate RenderHandler;

        public object Data;
    }
}
