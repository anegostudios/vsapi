using System;

#nullable disable

namespace Vintagestory.API.MathTools
{
    public class HorRectanglei
    {
        public int X1;
        public int Z1;
        public int X2;
        public int Z2;

        public HorRectanglei()
        {
        }

        public HorRectanglei(int x1, int z1, int x2, int z2)
        {
            X1 = x1;
            Z1 = z1;
            X2 = x2;
            Z2 = z2;
        }

        public int MinX => Math.Min(X1, X2);
        public int MaxX => Math.Max(X1, X2);
        public int MaxZ => Math.Max(Z1, Z2);
        public int MinZ => Math.Min(Z1, Z2);
    }

    
    public struct Rectanglei
    {
        int X;
        int Y;
        int Width;
        int Height;

        /// <summary>
        /// Same as X
        /// </summary>
        public int X1 => X;

        /// <summary>
        /// Same as Y
        /// </summary>
        public int Y1 => Y;

        /// <summary>
        /// Same as X + Width
        /// </summary>
        public int X2
        {
            get { return X + Width; }
        }

        /// <summary>
        /// Same as Y + Height
        /// </summary>
        public int Y2
        {
            get { return Y + Height; }
        }

        public int Bottom()
        {
            return Y + Height;
        }

        public Rectanglei(int X, int Y, int width, int height)
        {
            this.X = X;
            this.Y = Y;
            this.Width = width;
            this.Height = height;
        }

        public Rectanglei GrowBy(int size)
        {
            return new Rectanglei(X - size, Y - size, Width + size * 2, Height + size * 2);
        }

        [Obsolete("Rectanglei is a struct and there is no point cloning a struct")]
        public Rectanglei Clone()
        {
            return new Rectanglei(X, Y, Width, Height);
        }

        public bool PointInside(int x, int y)
        {
            return
                x >= this.X &&
                y >= this.Y &&
                x <= this.X + this.Width &&
                y <= this.Y + this.Height
            ;
        }

        public bool PointInside(Vec2i pos)
        {
            return
                pos.X >= this.X &&
                pos.Y >= this.Y &&
                pos.X <= this.X + this.Width &&
                pos.Y <= this.Y + this.Height
            ;
        }

        /// <summary>
        /// If the given cuboid intersects with this cubiod
        /// </summary>
        public bool Intersects(Rectanglei with)
        {
            return with.X2 > this.X1 && with.X1 < this.X2 ? (with.Y2 > this.Y1 && with.Y1 < this.Y2 ? true : false) : false;
        }

        /// <summary>
        /// If the given cuboid intersects  with or is adjacent to this cubiod
        /// </summary>
        public bool IntersectsOrTouches(Rectanglei with)
        {
            if (with.X2 < this.X1) return false;
            if (with.X1 >= this.X2) return false;
            if (with.Y2 < this.Y1) return false;
            if (with.Y1 >= this.Y2) return false;
            return true;
        }

        public bool IntersectsOrTouches(int withX1, int withY1, int withX2, int withY2)
        {
            if (withX2 < this.X1) return false;
            if (withX1 >= this.X2) return false;
            if (withY2 < this.Y1) return false;
            if (withY1 >= this.Y2) return false;
            return true;
        }

        /// <summary>
        /// Returns if the given point is inside the cuboid
        /// </summary>
        public bool Contains(Vec2i pos)
        {
            return pos.X >= X1 && pos.X < X2 && pos.Y >= Y1 && pos.Y < Y2;
        }

        /// <summary>
        /// Returns if the given point is inside the cuboid
        /// </summary>
        public bool Contains(int x, int y)
        {
            return x >= X1 && x < X2 && y >= Y1 && y < Y2;
        }

    }
}
