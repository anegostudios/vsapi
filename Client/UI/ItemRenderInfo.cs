
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Holds information on how a collectible should be rendered in hands, on the ground or in gui.
    /// In collectible.OnBeforeRender() the values are prefilled with the default ones.
    /// </summary>
    public class ItemRenderInfo
    {
        /// <summary>
        /// The model to be rendered
        /// </summary>
        public MeshRef ModelRef;
        /// <summary>
        /// The transform to be applied when rendered
        /// </summary>
        public ModelTransform Transform;
        /// <summary>
        /// Wether or not to cull back faces
        /// </summary>
        public bool CullFaces;
        /// <summary>
        /// The texture to be used when rendering. Should probalby be the texture id of the block or item texture atlas
        /// </summary>
        public int TextureId;
        /// <summary>
        /// For discarding fragments with alpha value below this threshold
        /// </summary>
        public float AlphaTest;
        /// <summary>
        /// (Currently) not used.
        /// </summary>
        public bool HalfTransparent;
    }

}
