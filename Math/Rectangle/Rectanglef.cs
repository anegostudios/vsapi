namespace Vintagestory.API
{
    public class RectangleFloat
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public float Bottom()
        {
            return Y + Height;
        }

        public RectangleFloat()
        {

        }

        public RectangleFloat(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public static RectangleFloat Create(float x, float y, float width, float height)
        {
            RectangleFloat r = new RectangleFloat();
            r.X = x;
            r.Y = y;
            r.Width = width;
            r.Height = height;
            return r;
        }
    }




}
