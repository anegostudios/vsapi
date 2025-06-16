
#nullable disable
namespace Vintagestory.API.Client
{
    /// <summary>
    /// Types that determine how block faces should be culled.
    /// </summary>
    [DocumentAsJson]
    public enum EnumFaceCullMode
    {
        /// <summary>
        /// Culls faces if they are opaque faces adjacent to opaque faces
        /// </summary>
        Default = 0,

        /// <summary>
        /// Never culls any faces
        /// </summary>
        NeverCull = 1,

        /// <summary>
        /// Culls all faces that are adjacent to opaque faces and faces adjacent to blocks of the same id
        /// </summary>
        Merge = 2,

        /// <summary>
        /// Calls method Block.ShouldMergeFace() to determine whether to cull the face or not
        /// </summary>
        Callback = 7,

        /// <summary>
        /// Culls all faces that are adjacent to opaque faces and the bottom, east or south faces adjacent to blocks of the same id
        /// This causes to still leave one single face inbetween instead of 2, eliminating any z-fighting.
        /// </summary>
        Collapse = 3,

        /// <summary>
        /// Same as Merge but checks for equal material
        /// </summary>
        MergeMaterial = 4,

        /// <summary>
        /// Same as Collapse but checks for equal material
        /// </summary>
        CollapseMaterial = 5,

        /// <summary>
        /// Same as CollapseMaterial but also culls faces towards opaque blocks
        /// </summary>
        Liquid = 6,

        /// <summary>
        /// 
        /// </summary>
        MergeSnowLayer = 8,

        /// <summary>
        /// Used for blocks similar to Farmland or StonePath, which are not themselves opaque except on the base, but can cull horizontal sides if adjacent block is opaque (or the same id)
        /// </summary>
        FlushExceptTop = 9,

        /// <summary>
        /// Culls non-opaque faces if the same block is adjacent e.g. a wide staircase made from several Stairs blocks side-by-side  (caution: in future this cull mode will not work with corner stairs shapes)
        /// </summary>
        Stairs = 10
    }
}
