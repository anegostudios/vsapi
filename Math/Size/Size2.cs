
#nullable disable
namespace Vintagestory.API.MathTools
{

    /// <summary>
    /// Represents a vector of 2 doubles. Go bug Tyron of you need more utility methods in this class.
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
    /// Represents a vector of 2 doubles. Go bug Tyron of you need more utility methods in this class.
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
    /// Represents a vector of 2 doubles. Go bug Tyron of you need more utility methods in this class.
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
}
