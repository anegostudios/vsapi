using Vintagestory.API.MathTools;

namespace Vintagestory.API.MathTools
{
    public struct Sphere
    {
        public float x;
        public float y;
        public float z;
        public float radius;

        public const float sqrt3half = 0.8660254037844386f;

        public static Sphere BoundingSphereForCube(float x, float y, float z, float size)
        {
            return new Sphere()
            {
                x = x + size / 2,
                y = y + size / 2,
                z = z + size / 2,
                radius = sqrt3half * size
            };
        }
    }
}
