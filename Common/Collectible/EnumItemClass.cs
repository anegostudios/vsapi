
#nullable disable
namespace Vintagestory.API.Common
{
    /// <summary>
    /// The type of collectible in an itemstack.
    /// </summary>
    [DocumentAsJson]
    public enum EnumItemClass
    {
        /// <summary>
        /// This itemstack holds a block.
        /// </summary>
        Block = 0,
        /// <summary>
        /// This itemstack holds an item.
        /// </summary>
        Item = 1
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
