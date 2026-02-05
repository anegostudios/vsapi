
#nullable disable
namespace Vintagestory.API.MathTools
{

    /// <summary>
    /// Represents a vector of 3 floats. Go bug Tyron of you need more utility methods in this class.
    /// </summary>
    [DocumentAsJson]
    public class Size3f : ISize3
    {
        /// <summary>
        /// The X-dimension of this size.
        /// </summary>
        [DocumentAsJson("Recommended", "0")]
        public float Width;

        /// <summary>
        /// The Y-dimension for this size.
        /// </summary>
        [DocumentAsJson("Recommended", "0")]
        public float Height;

        /// <summary>
        /// The Z-dimension for this size.
        /// </summary>
        [DocumentAsJson("Recommended", "0")]
        public float Length;

        public float Volume => Width * Height * Length;

        public int WidthAsInt => (int)Width;
        public int HeightAsInt => (int)Height;
        public int LengthAsInt => (int)Length;
        public double WidthAsDouble => (double)Width;
        public double HeightAsDouble => (double)Height;
        public double LengthAsDouble => (double)Length;
        public float WidthAsFloat => (float)Width;
        public float HeightAsFloat => (float)Height;
        public float LengthAsFloat => (float)Length;

        public Size3f()
        {

        }

        public Size3f(float width, float height, float length)
        {
            this.Width = width;
            this.Height = height;
            this.Length = length;
        }

        public Size3f Clone()
        {
            return new Size3f(Width, Height, Length);
        }

        public bool CanContain(Size3f obj)
        {
            return Width >= obj.Width && Height >= obj.Height && Length >= obj.Length;
        }
    }
    
    /// <summary>
    /// Represents a vector of 3 doubles. Go bug Tyron of you need more utility methods in this class.
    /// </summary>
    public class Size3d : ISize3
    {
        public double Width;
        public double Height;
        public double Length;

        public int WidthAsInt => (int)Width;
        public int HeightAsInt => (int)Height;
        public int LengthAsInt => (int)Length;
        public double WidthAsDouble => (double)Width;
        public double HeightAsDouble => (double)Height;
        public double LengthAsDouble => (double)Length;
        public float WidthAsFloat => (float)Width;
        public float HeightAsFloat => (float)Height;
        public float LengthAsFloat => (float)Length;
        public double Volume => Width * Height * Length;
        public Size3d()
        {

        }

        public Size3d(double width, double height, double length)
        {
            this.Width = width;
            this.Height = height;
            this.Length = length;
        }

        public Size3d Clone()
        {
            return new Size3d(Width, Height, Length);
        }

        public bool CanContain(Size3d obj)
        {
            return Width >= obj.Width && Height >= obj.Height && Length >= obj.Length;
        }
    }



    /// <summary>
    /// Represents a vector of 3 integers. Go bug Tyron of you need more utility methods in this class.
    /// </summary>
    public class Size3i : ISize3
    {
        public int Width;
        public int Height;
        public int Length;

        public int WidthAsInt => (int)Width;
        public int HeightAsInt => (int)Height;
        public int LengthAsInt => (int)Length;
        public double WidthAsDouble => (double)Width;
        public double HeightAsDouble => (double)Height;
        public double LengthAsDouble => (double)Length;
        public float WidthAsFloat => (float)Width;
        public float HeightAsFloat => (float)Height;
        public float LengthAsFloat => (float)Length;

        public int Volume => Width * Height * Length;

        public Size3i()
        {

        }

        public Size3i(int width, int height, int length)
        {
            this.Width = width;
            this.Height = height;
            this.Length = length;
        }

        public Size3i Clone()
        {
            return new Size3i(Width, Height, Length);
        }

        public bool CanContain(Size3i obj)
        {
            return Width >= obj.Width && Height >= obj.Height && Length >= obj.Length;
        }
    }
}
