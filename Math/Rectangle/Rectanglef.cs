
#nullable disable
namespace Vintagestory.API.Datastructures
{
    public class Rectanglef
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public float Bottom()
        {
            return Y + Height;
        }

        public Rectanglef()
        {

        }

        public Rectanglef(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public static Rectanglef Create(float x, float y, float width, float height)
        {
            Rectanglef r = new Rectanglef();
            r.X = x;
            r.Y = y;
            r.Width = width;
            r.Height = height;
            return r;
        }
    }




}
