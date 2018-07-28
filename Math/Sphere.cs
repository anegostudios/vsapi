using Vintagestory.API.MathTools;

namespace Vintagestory.API.MathTools
{
    public class Sphere
    {
        public float x;
        public float y;
        public float z;
        public float radius;

        public static float sqrt3half = GameMath.Sqrt(3) * 0.5f;

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
