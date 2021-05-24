namespace Vintagestory.API.Common
{
    public enum EnumItemClass
    {
        Block,
        Item
    }

    public static class ItemClassMethods
    {
        public static string Name(this EnumItemClass s1)
        {
            switch (s1)
            {
                case EnumItemClass.Block:
                    return "block";
                case EnumItemClass.Item:
                    return "item";
            }

            return null;
        }
    }
}