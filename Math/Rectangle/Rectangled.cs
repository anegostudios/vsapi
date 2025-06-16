
#nullable disable
namespace Vintagestory.API.Datastructures
{


    public class Rectangled
    {
        public double X;
        public double Y;
        public double Width;
        public double Height;

        public double Bottom()
        {
            return Y + Height;
        }

        public Rectangled()
        {

        }

        public Rectangled(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }

        public Rectangled(double X, double Y, double width, double height)
        {
            this.X = X;
            this.Y = Y;
            this.Width = width;
            this.Height = height;
        }

        internal Rectangled Clone()
        {
            return new Rectangled(X, Y, Width, Height);
        }

        public bool PointInside(double x, double y)
        {
            return
                x >= this.X &&
                y >= this.Y &&
                x < this.X + this.Width &&
                y < this.Y + this.Height
            ;
        }
    }

}