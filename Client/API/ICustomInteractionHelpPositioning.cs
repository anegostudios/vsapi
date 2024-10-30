using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    public interface ICustomInteractionHelpPositioning
    {
        Vec3d GetInteractionHelpPosition();
        bool TransparentCenter { get; }
    }
}
