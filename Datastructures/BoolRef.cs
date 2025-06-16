
#nullable disable
namespace Vintagestory.API.Datastructures
{
    public class IntRef
    {
        public static IntRef Create(int value_)
        {
            IntRef intref = new IntRef();
            intref.value = value_;
            return intref;
        }
        internal int value;
        public int GetValue() { return value; }
        public void SetValue(int value_) { value = value_; }
    }

    public class BoolRef
    {
        public bool value;
        public bool GetValue() { return value; }
        public void SetValue(bool value_) { value = value_; }
    }

    public class Bools
    {
        private int data;
        public bool this[int i] { get => (data & 1 << i) != 0; set { if (value) data |= 1 << i; else data &= ~(1 << i); }  }
        public Bools(bool a, bool b)
        {
            data = (b ? 2 : 0) + (a ? 1 : 0);
        }

        internal bool Parity()
        {
            return (data + 1) / 2 != 1;  // true if this is 0 or 3, i.e. both bits are equal
        }
    }
}
