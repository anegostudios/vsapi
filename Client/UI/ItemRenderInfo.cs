
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

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
        public Size2i TextureSize = new Size2i();
        /// <summary>
        /// For discarding fragments with alpha value below this threshold
        /// </summary>
        public float AlphaTest;
        /// <summary>
        /// (Currently) not used.
        /// </summary>
        public bool HalfTransparent;

        public bool NormalShaded;

        public LoadedTexture OverlayTexture;

        public float OverlayOpacity;


        public void SetRotOverlay(ICoreClientAPI capi, float opacity)
        {
            if (OverlayTexture == null) OverlayTexture = new LoadedTexture(capi);
            
            capi.Render.GetOrLoadTexture(new AssetLocation("textures/gui/rot.png"), ref OverlayTexture);

            this.OverlayOpacity = opacity;
        }
    }

}
