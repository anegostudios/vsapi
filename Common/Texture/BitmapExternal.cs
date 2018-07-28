using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    public class BitmapExternal : BitmapRef
    {
        public Bitmap bmp;

        public override int Height
        {
            get { return bmp.Height; }
        }

        public override int Width
        {
            get { return bmp.Width; }
        }

        public override void Dispose()
        {
            bmp.Dispose();
        }

        public override void Save(string filename)
        {
            bmp.Save(filename);
        }

        /// <summary>
        /// Retrives the ARGB value from given coordinate
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override Color GetPixel(int x, int y)
        {
            return bmp.GetPixel(x, y);
        }

        /// <summary>
        /// Retrives the ARGB value from given coordinate using normalized coordinates (0..1)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override Color GetPixelRel(float x, float y)
        {
            return bmp.GetPixel((int)Math.Min(bmp.Width - 1, x * bmp.Width), (int)Math.Min(bmp.Height - 1, (y * bmp.Height)));
        }
    }
}
