
#nullable disable
namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// Not really a sphere, actually now an AABB centred on x,y,z, but we keep the name for API consistency
    /// </summary>
    public struct Sphere
    {
        public float x;
        public float y;
        public float z;
        public float radius;
        public float radiusY;
        public float radiusZ;

        public const float sqrt3half = 0.8660254037844386f;

        public Sphere(float x1, float y1, float z1, float dx, float dy, float dz)
        {
            x = x1;
            y = y1;
            z = z1;
            radius = sqrt3half * dx;
            radiusY = sqrt3half * dy;
            radiusZ = sqrt3half * dz;
        }

        public static Sphere BoundingSphereForCube(float x, float y, float z, float size)
        {
            return new Sphere()
            {
                x = x + size / 2,
                y = y + size / 2,
                z = z + size / 2,
                radius = sqrt3half * size,
                radiusY = sqrt3half * size,
                radiusZ = sqrt3half * size
            };
        }
    }
}
