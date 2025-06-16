
#nullable disable
namespace Vintagestory.API.Client
{
    public class FrameBufferRef
    {
        public FramebufferAttrs FbAttrs;
        public int FboId;
        public int DepthTextureId;
        public int[] ColorTextureIds;

        public int Width;
        public int Height;
    }
}