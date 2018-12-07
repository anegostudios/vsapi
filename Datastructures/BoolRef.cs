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

}
