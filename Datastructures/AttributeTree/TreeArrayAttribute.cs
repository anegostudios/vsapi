using System.IO;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    public class TreeArrayAttribute : ArrayAttribute<TreeAttribute>, IAttribute
    {
        public TreeArrayAttribute()
        {

        }

        public TreeArrayAttribute(TreeAttribute[] value)
        {
            this.value = value;
        }

        public void ToBytes(BinaryWriter stream)
        {
            stream.Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                value[i].ToBytes(stream);
            }

        }

        public void FromBytes(BinaryReader stream)
        {
            int quantity = stream.ReadInt32();
            value = new TreeAttribute[quantity];
            for (int i = 0; i < quantity; i++)
            {
                value[i] = new TreeAttribute();
                value[i].FromBytes(stream);
            }

        }

        public int GetAttributeId()
        {
            return 14;
        }

        public IAttribute Clone()
        {
            var newlist = new TreeAttribute[value.Length];
            for (int i = 0; i < newlist.Length; i++) newlist[i] = value[i].Clone() as TreeAttribute;

            return new TreeArrayAttribute(newlist);
        }

    }
}
