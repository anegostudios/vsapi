using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// An interface that can supply texture atlas positions 
    /// </summary>
    public interface ITexPositionSource
    {
        TextureAtlasPosition this[string textureCode] { get; }

        /// <summary>
        /// This returns the size of the atlas this texture resides in.
        /// </summary>
        Size2i AtlasSize { get; }
    }
}
