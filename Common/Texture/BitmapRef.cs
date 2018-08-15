using System;
using System.Drawing;

namespace Vintagestory.API.Common
{
    public abstract class BitmapRef : IDisposable
    {
        public abstract void Dispose();

        public abstract Color GetPixel(int x, int y);
        public abstract Color GetPixelRel(float x, float y);

        public abstract int Width { get; }
        public abstract int Height { get; }

        public abstract void Save(string filename);
        
    }

}
