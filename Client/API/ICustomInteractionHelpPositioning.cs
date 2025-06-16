using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    public interface ICustomInteractionHelpPositioning
    {
        Vec3d GetInteractionHelpPosition();
        bool TransparentCenter { get; }
    }
}
