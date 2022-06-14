using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// Represents a vector of 2 doubles. Go bug Tyron of you need more utility methods in this class.
    /// </summary>
    public class Vec2d
    {
        public double X;
        public double Y;
        

        public Vec2d()
        {

        }

        public Vec2d(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vec2d Set(double x, double z)
        {
            this.X = x;
            this.Y = z;
            return this;
        }


        public double Dot(Vec2d a)
        {
            return X * a.X + Y * a.Y;
        }

        public double Dot(double x, double y)
        {
            return X * x + Y * y;
        }


        public double Length()
        {
            return GameMath.Sqrt(X * X + Y * Y);
        }

        public double LengthSq()
        {
            return X * X + Y * Y;
        }

        public Vec2d Normalize()
        {
            double length = Length();
            if (length > 0)
            {
                X /= length;
                Y /= length;
            }

            return this;
        }

        public double DistanceTo(Vec2d pos)
        {
            return DistanceTo(pos.X, pos.Y);
        }

        public double DistanceTo(double targetX, double targetY)
        {
            var dx = X - targetX;
            var dy = Y - targetY;

            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
