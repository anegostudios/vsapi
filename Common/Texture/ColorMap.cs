using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class ColorMap
    {
        public string Code;
        public CompositeTexture Texture;
        public int Padding;
        public bool LoadIntoBlockTextureAtlas;

        // Loaded by the game engine client
        public int[] Pixels;
        public Size2i OuterSize;
        public int BlockAtlasTextureSubId;
        public int RectIndex;

    }
}
