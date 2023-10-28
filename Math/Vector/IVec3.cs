namespace Vintagestory.API.MathTools
{
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
