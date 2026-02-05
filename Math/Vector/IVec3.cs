
#nullable disable
namespace Vintagestory.API.MathTools
{
    public interface ISize3
    {
        int WidthAsInt { get; }

        int HeightAsInt { get; }

        int LengthAsInt { get; }

        double WidthAsDouble { get; }

        double HeightAsDouble { get; }

        double LengthAsDouble { get; }

        float WidthAsFloat { get; }

        float HeightAsFloat { get; }

        float LengthAsFloat { get; }
    }

    public interface IVec3
    {

        int XAsInt { get; }

        int YAsInt { get; }

        int ZAsInt { get; }

        double XAsDouble { get; }

        double YAsDouble { get; }

        double ZAsDouble { get; }

        float XAsFloat { get; }

        float YAsFloat { get; }

        float ZAsFloat { get; }

        Vec3i AsVec3i { get; }

    }
}
