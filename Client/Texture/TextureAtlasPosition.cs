
#nullable disable
namespace Vintagestory.API.Client
{
    /// <summary>
    /// The position of a texture inside an atlas
    /// </summary>
    public class TextureAtlasPosition
    {
        public const int RndColorsLength = 30;

        /// <summary>
        /// The OpenGL textureid
        /// </summary>
        public int atlasTextureId;

        /// <summary>
        /// A sequential number in which atlas this position is in. Atlasses for a given type are sequentially numbered if more than one atlas was required to hold all the textures
        /// </summary>
        public byte atlasNumber;

        /// <summary>
        /// A sequential number that goes up with every texture atlas reload, used to see if this texpos is still fresh
        /// </summary>
        public short reloadIteration;

        public int AvgColor;

        public int[] RndColors;

        /// <summary>
        /// The x coordinate of the texture origin point
        /// </summary>
        public float x1;

        /// <summary>
        /// The y coordinate of the texture origin point
        /// </summary>
        public float y1;

        /// <summary>
        /// The x coordinate of the texture end point
        /// </summary>
        public float x2;

        /// <summary>
        /// The y coordinate of the texture end point
        /// </summary>
        public float y2;

        public TextureAtlasPosition Clone()
        {
            return new TextureAtlasPosition() { atlasTextureId = atlasTextureId, atlasNumber = atlasNumber, reloadIteration = reloadIteration, AvgColor = AvgColor, RndColors = RndColors, x1 = x1, y1 = y1, x2 = x2, y2 = y2 };
        }
    }
}
