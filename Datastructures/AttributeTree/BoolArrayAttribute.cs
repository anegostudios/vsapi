using System.IO;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    public class BoolArrayAttribute : ArrayAttribute<bool>, IAttribute
    {
        public BoolArrayAttribute()
        {

        }

        public BoolArrayAttribute(bool[] value)
        {
            this.value = value;
        }

        public void ToBytes(BinaryWriter stream)
        {
            stream.Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                stream.Write(value[i]);
            }

        }

        public void FromBytes(BinaryReader stream)
        {
            int quantity = stream.ReadInt32();
            value = new bool[quantity];
            for (int i = 0; i < quantity; i++)
            {
                value[i] = stream.ReadBoolean();
            }

        }

        public int GetAttributeId()
        {
            return 16;
        }

        public IAttribute Clone()
        {
            return new BoolArrayAttribute((bool[])value.Clone());
        }
    }
}
