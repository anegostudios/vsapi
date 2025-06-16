using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    public class ColorMap
    {
        public const int ColorMapLoadedFlag = 2;
        public string Code;
        public CompositeTexture Texture;
        public int Padding;
        public bool LoadIntoBlockTextureAtlas;
        public int ExtraFlags;

        // Loaded by the game engine client
        public int[] Pixels;
        public Size2i OuterSize;
        public int BlockAtlasTextureSubId;
        public int RectIndex;
    }
}
