using System.IO;
using Vintagestory.API.Datastructures;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Represents a stack of items or blocks
    /// </summary>
    public interface IItemStack
    {
        /// <summary>
        /// The base class the Item/Block inherits from
        /// </summary>
        CollectibleObject Collectible { get; }

        /// <summary>
        /// Is it a Block or Item?
        /// </summary>
        EnumItemClass Class { get; }

        /// <summary>
        /// The Item if ItemClass==Item, otherwise null
        /// </summary>
        Item Item { get; }

        /// <summary>
        /// The Block if ItemClass==Block, otherwise null
        /// </summary>
        Block Block { get; }

        /// <summary>
        /// Amount of items or blocks in this stack
        /// </summary>
        int StackSize { get; set; }

        /// <summary>
        /// The items or blocks unique id
        /// </summary>
        int Id { get; }
        
        /// <summary>
        /// Attributes assigned to this itemstack. Modifiable.
        /// </summary>
        ITreeAttribute Attributes { get; set; }

        /// <summary>
        /// Checks if this item stack is of the same class, id and has the same stack attributes. Ignores stack size
        /// </summary>
        /// <param name="worldForResolve"></param>
        /// <param name="sourceStack"></param>
        /// <param name="ignoreAttributeSubTrees"></param>
        /// <returns></returns>
        bool Equals(IWorldAccessor worldForResolve, ItemStack sourceStack, params string[] ignoreAttributeSubTrees);
        
        /// <summary>
        /// Serializes this itemstack into a byte stream
        /// </summary>
        /// <param name="stream"></param>
        void ToBytes(BinaryWriter stream);

        /// <summary>
        /// Deserializes an itemstack from given byte stream
        /// </summary>
        /// <param name="stream"></param>
        void FromBytes(BinaryReader stream);

        /// <summary>
        /// Checks if the contained item or block name contains given searchtext
        /// </summary>
        /// <param name="world"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        bool MatchesSearchText(IWorldAccessor world, string searchText);

        /// <summary>
        /// Returns the name displayed in the players inventory
        /// </summary>
        /// <returns></returns>
        string GetName();

        /// <summary>
        /// Returns a multiline description text of the item
        /// </summary>
        /// <param name="world"></param>
        /// <param name="inSlot"></param>
        /// <param name="debug">Whether to show additional debug info</param>
        /// <returns></returns>
        string GetDescription(IWorldAccessor world, ItemSlot inSlot, bool debug = false);

        /// <summary>
        /// Creates a deep copy of the itemstack
        /// </summary>
        /// <returns></returns>
        ItemStack Clone();
    }
}
