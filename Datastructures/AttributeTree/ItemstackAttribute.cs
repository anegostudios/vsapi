using System.IO;
using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    public class ItemstackAttribute : IAttribute
    {
        public ItemStack value;

        public ItemstackAttribute()
        {

        }

        public ItemstackAttribute(ItemStack value)
        {
            this.value = value;
        }

        public int GetAttributeId()
        {
            return 7;
        }

        public object GetValue()
        {
            return value;
        }

        public void SetValue(ItemStack newval)
        {
            value = newval;
        }


        public void FromBytes(BinaryReader stream)
        {
            bool isNull = stream.ReadBoolean();
            if (!isNull)
            {
                value = new ItemStack();
                value.FromBytes(stream);
            }
        }


        public void ToBytes(BinaryWriter stream)
        {
            stream.Write(value == null);
            if (value != null) value.ToBytes(stream);
        }

        public bool Equals(IWorldAccessor worldForResolve, IAttribute attr)
        {
            return Equals(worldForResolve, attr, null);
        }

        internal bool Equals(IWorldAccessor worldForResolve, IAttribute attr, string[] ignorePaths)
        {
            if (!(attr is ItemstackAttribute)) return false;

            ItemstackAttribute stackAttr = (ItemstackAttribute)attr;

            return
                (stackAttr.value == null && value == null) ||
                (stackAttr.value != null && stackAttr.value.Equals(worldForResolve, value, ignorePaths))
            ;
        }

        public string ToJsonToken()
        {
            if (value?.Collectible == null)
            {
                return "";
            }

            if (value.Attributes == null || value.Attributes.Count == 0)
            {
                return string.Format("{{ \"type\": \"{0}\", code: \"{1}\"}}", value.Collectible.ItemClass.ToString().ToLowerInvariant(), value.Collectible.Code.ToShortString());
            } else
            {
                return string.Format("{{ \"type\": \"{0}\", \"code\": \"{1}\", \"attributes\": {2}}}", value.Collectible.ItemClass.ToString().ToLowerInvariant(), value.Collectible.Code.ToShortString(), value.Attributes.ToJsonToken());
            }
        }

        public override int GetHashCode()
        {
            return value?.GetHashCode() ?? 0;
        }

        public IAttribute Clone()
        {
            return new ItemstackAttribute(value?.Clone());
        }

    }
}
