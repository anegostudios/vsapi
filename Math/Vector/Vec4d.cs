
#nullable disable
namespace Vintagestory.API.MathTools
{
    public class Vec4d
    {
        public double X;
        public double Y;
        public double Z;
        public double W;

        public Vec4d()
        {

        }

        public Vec4d(double x, double y, double z, double w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        /// <summary>
        /// Returns the n-th coordinate
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double this[int index]
        {
            get { return 
                    index == 0 ? X : 
                    (index == 1 ? Y : 
                    (index == 2 ? Z : 
                    W));
            }
            set {
                if (index == 0) X = value;
                else if (index == 1) Y = value;
                else if (index == 2) Z = value;
                else W = value; }
        }

        public Vec3d XYZ => new Vec3d(X, Y, Z);

        public void Set(double x, double y, double z, double w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public float SquareDistanceTo(float x, float y, float z)
        {
            double dx = X - x;
            double dy = Y - y;
            double dz = Z - z;

            return (float)(dx * dx + dy * dy + dz * dz);
        }

        public float SquareDistanceTo(double x, double y, double z)
        {
            double dx = X - x;
            double dy = Y - y;
            double dz = Z - z;

            return (float)(dx * dx + dy * dy + dz * dz);
        }

        public float SquareDistanceTo(Vec3d pos)
        {
            double dx = X - pos.X;
            double dy = Y - pos.Y;
            double dz = Z - pos.Z;

            return (float)(dx * dx + dy * dy + dz * dz);
        }

        public float HorizontalSquareDistanceTo(Vec3d pos)
        {
            double dx = X - pos.X;
            double dz = Z - pos.Z;

            return (float)(dx * dx + dz * dz);
        }

        public float HorizontalSquareDistanceTo(double x, double z)
        {
            double dx = X - x;
            double dz = Z - z;

            return (float)(dx * dx + dz * dz);
        }
    }
}
