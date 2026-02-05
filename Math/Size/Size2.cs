
#nullable disable
namespace Vintagestory.API.MathTools
{

    /// <summary>
    /// Represents a 2D size
    /// </summary>
    public class Size2f
    {
        public float Width;
        public float Height;

        public Size2f()
        {

        }

        public Size2f(float width, float height)
        {
            this.Width = width;
            this.Height = height;
        }

        public Size2f Clone()
        {
            return new Size2f(Width, Height);
        }
    }

    /// <summary>
    /// Represents a 2D size
    /// </summary>
    public class Size2d
    {
        public double Width;
        public double Height;


        public Size2d()
        {

        }

        public Size2d(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }

        public Size2d Clone()
        {
            return new Size2d(Width, Height);
        }
    }



    /// <summary>
    /// Represents a 2D size
    /// </summary>
    public class Size2i
    {
        public int Width;
        public int Height;

        public Size2i()
        {

        }

        public Size2i(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public Size2i Clone()
        {
            return new Size2i(Width, Height);
        }
    }

    /// <summary>
    /// Represents a 2D size
    /// </summary>
    public class HorSize2i
    {
        public int Width;
        public int Length;

        public int Area => Width * Length;

        public HorSize2i()
        {

        }

        public HorSize2i(int width, int length)
        {
            this.Width = width;
            this.Length = length;
        }

        public HorSize2i Clone()
        {
            return new HorSize2i(Width, Length);
        }
    }
}
