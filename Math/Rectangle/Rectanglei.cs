using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Vintagestory.API
{
    public class Rectanglei
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public int Bottom()
        {
            return Y + Height;
        }

        public Rectanglei()
        {

        }

        public Rectanglei(int width, int height)
        {
            this.Width = width;
            this.Height = height;
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
            X -= size;
            Y -= size;
            Width += size * 2;
            Height += size * 2;
            return this;
        }

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
    }
}
