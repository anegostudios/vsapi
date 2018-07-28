using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Vintagestory.API.Client
{
    public delegate void RenderSkillItemDelegate(AssetLocation code, float dt, double atPosX, double atPosY);

    public class SkillItem
    {
        public string name;
        public AssetLocation code;
        public LoadedTexture texture;
        public KeyCombination hotkey;

        public RenderSkillItemDelegate renderHandler;

        public object data;
    }
}
