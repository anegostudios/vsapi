
#nullable disable
namespace Vintagestory.API.Common
{
    /// <summary>
    /// A keyframe for model transformation.  
    /// </summary>
    public class ModelTransformKeyFrame
    {
        /// <summary>
        /// The frame number for the keyframe
        /// </summary>
        public int FrameNumber;

        /// <summary>
        /// The new transform set for the keyframe.
        /// </summary>
        public ModelTransform Transform;
    }
}
