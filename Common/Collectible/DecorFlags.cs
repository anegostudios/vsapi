
#nullable disable
namespace Vintagestory.API.Common
{
    public class DecorFlags
    {
        /// <summary>Set to 1 for all decor blocks, or else they will not be rendered. (This intentionally prevents unloaded / unknown decor blocks from rendering)</summary>
        public const int IsDecor = 1;
        /// <summary>If true, do not cull even if parent face was culled (used e.g. for medium carpet, which stick out beyond the parent face)</summary>
        public const int DrawIfCulled = 2;
        /// <summary>If true, alternates z-offset vertexflag by 1 in odd/even XZ positions to reduce z-fighting (used e.g. for medium carpets overlaying neighbours)</summary>
        public const int AlternateZOffset = 4;
        /// <summary>IF true, this decor is NOT (at least) a full opaque face so that the parent block face still needs to be drawn</summary>
        public const int NotFullFace = 8;
        /// <summary>If true, this decor is removable using the players hands, without breaking the parent block</summary>
        public const int Removable = 16;
        /// <summary>If true, this decor supplies its own different models for NSEWUD placement, if false the code will auto-rotate the model</summary>
        public const int HasSidedVariants = 32;
    }
}
