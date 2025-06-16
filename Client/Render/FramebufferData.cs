
#nullable disable
namespace Vintagestory.API.Client
{
    public enum EnumTextureFilter
    {
        Linear = 9729,
        Nearest = 9728
    }

    public enum EnumTextureWrap
    {
        Repeat = 10497,
        ClampToEdge = 33071
    }

    public enum EnumTextureInternalFormat
    {
        DepthComponent32 = 33191,
        Rgba16f = 34842,
        Rgba8 = 32856,
        R16f = 33325
    }

    public enum EnumTexturePixelFormat
    {
        DepthComponent = 6402,
        Rgba = 6408,
        Red = 6403
    }

    public enum EnumFramebufferAttachment
    {
        ColorAttachment0 = 36064,
        ColorAttachment1 = 36065,
        ColorAttachment2 = 36066,
        ColorAttachment3 = 36067,
        ColorAttachment4 = 36068,
        DepthAttachment = 36096,
    }

    public class RawTexture
    {
        public EnumTextureFilter MinFilter = EnumTextureFilter.Linear;
        public EnumTextureFilter MagFilter = EnumTextureFilter.Linear;

        public EnumTextureWrap WrapS = EnumTextureWrap.ClampToEdge;
        public EnumTextureWrap WrapT = EnumTextureWrap.ClampToEdge;

        public EnumTextureInternalFormat PixelInternalFormat = EnumTextureInternalFormat.Rgba8;
        public EnumTexturePixelFormat PixelFormat = EnumTexturePixelFormat.Rgba;

        public int Width;
        public int Height;

        public int TextureId = 0;
    }

    public class FramebufferAttrsAttachment
    {
        public RawTexture Texture;

        public EnumFramebufferAttachment AttachmentType;
        
    }

    public class FramebufferAttrs
    {
        public string Name;

        public FramebufferAttrsAttachment[] Attachments;

        public int Width;
        public int Height;

        public FramebufferAttrs(string name, int width, int height)
        {
            this.Name = name;
            this.Width = width;
            this.Height = height;
        }
    }
}
