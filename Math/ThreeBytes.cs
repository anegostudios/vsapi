namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// For performance, intended to compile equivalently to a byte[3], but more RAM efficient and more GC efficient
    /// </summary>
    public struct ThreeBytes
    {
        private int val;

        public ThreeBytes(int a)
        {
            val = a;
        }

        public ThreeBytes(byte[] a)
        {
            val = a[0] + (a[1] << 8) + (a[2] << 16);
        }

        public byte this[int i]
        {
            get { return i == 0 ? (byte)val : (i == 1 ? (byte)(val >> 8) : (byte)(val >> 16)); }
            set { if (i == 0) val = val & 0xFFFF00 + value; else if (i == 1) val = val & 0xFF00FF + (value << 8); else val = val & 0x00FFFF + (value << 16); }
        }

        public static implicit operator byte[](ThreeBytes a)     // For backwards compatibility for ModelTransform, mods should only require a recompile against 1.21 API
        {
            return new byte[] { (byte)a.val, (byte)(a.val >> 8), (byte)(a.val >> 16) };
        }

        public static implicit operator ThreeBytes(byte[] a)     // For backwards compatibility for ModelTransform, mods should only require a recompile against 1.21 API
        {
            return new ThreeBytes(a);
        }

        public ThreeBytes Clone()
        {
            return new ThreeBytes(this);
        }
    }
}
