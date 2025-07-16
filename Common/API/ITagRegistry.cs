using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;

namespace Vintagestory.API.Common;

public interface ITagRegistry
{
    /// <summary>
    /// Registers new entity tags. Should be called only on server side. Should be called no later than 'AssetsFinalize' stage.
    /// </summary>
    /// <param name="tags"></param>
    void RegisterEntityTags(params string[] tags);

    /// <summary>
    /// Registers new item tags. Should be called only on server side. Should be called no later than 'AssetsFinalize' stage.
    /// </summary>
    /// <param name="tags"></param>
    void RegisterItemTags(params string[] tags);

    /// <summary>
    /// Registers new block tags. Should be called only on server side. Should be called no later than 'AssetsFinalize' stage.
    /// </summary>
    /// <param name="tags"></param>
    void RegisterBlockTags(params string[] tags);



    /// <summary>
    /// Converts a list of entity tags to their corresponding tag IDs.<br/>
    /// If removeUnknownTags is true, any unknown tags will be removed from the list.<br/>
    /// Result is sorted in ascending order.
    /// </summary>
    /// <param name="tags">List of entity tags</param>
    /// <param name="removeUnknownTags">If set to true, not registered tags wont be included in the result</param>
    /// <returns>List of tag ids sorted in ascending order</returns>
    ushort[] EntityTagsToTagIds(string[] tags, bool removeUnknownTags = false);

    /// <summary>
    /// Converts a list of item tags to their corresponding tag IDs.<br/>
    /// If removeUnknownTags is true, any unknown tags will be removed from the list.<br/>
    /// Result is sorted in ascending order.
    /// </summary>
    /// <param name="tags">List of item tags</param>
    /// <param name="removeUnknownTags">If set to true, not registered tags wont be included in the result</param>
    /// <returns>List of tag ids sorted in ascending order</returns>
    ushort[] ItemTagsToTagIds(string[] tags, bool removeUnknownTags = false);

    /// <summary>
    /// Converts a list of block tags to their corresponding tag IDs.<br/>
    /// If removeUnknownTags is true, any unknown tags will be removed from the list.<br/>
    /// Result is sorted in ascending order.
    /// </summary>
    /// <param name="tags">List of block tags</param>
    /// <param name="removeUnknownTags">If set to true, not registered tags wont be included in the result</param>
    /// <returns>List of tag ids sorted in ascending order</returns>
    ushort[] BlockTagsToTagIds(string[] tags, bool removeUnknownTags = false);



    /// <summary>
    /// Converts list of entity tags to tags array. Unknown tags are ignored.
    /// </summary>
    /// <param name="tags">List of entity tags</param>
    /// <returns>Tag array suitable for quick comparisons</returns>
    public EntityTagArray EntityTagsToTagArray(params string[] tags);

    /// <summary>
    /// Converts list of item tags to tags array. Unknown tags are ignored.
    /// </summary>
    /// <param name="tags">List of item tags</param>
    /// <returns>Tag array suitable for quick comparisons</returns>
    public ItemTagArray ItemTagsToTagArray(params string[] tags);

    /// <summary>
    /// Converts list of block tags to tags array. Unknown tags are ignored.
    /// </summary>
    /// <param name="tags">List of block tags</param>
    /// <returns>Tag array suitable for quick comparisons</returns>
    public BlockTagArray BlockTagsToTagArray(params string[] tags);



    /// <summary>
    /// Returns tag id of the entity tag. If the tag is not registered, it will return 0.
    /// </summary>
    /// <param name="tag">Entity tag</param>
    /// <returns>tag id</returns>
    ushort EntityTagToTagId(string tag);

    /// <summary>
    /// Returns tag id of the item tag. If the tag is not registered, it will return 0.
    /// </summary>
    /// <param name="tag">Item tag</param>
    /// <returns>tag id</returns>
    ushort ItemTagToTagId(string tag);

    /// <summary>
    /// Returns tag id of the block tag. If the tag is not registered, it will return 0.
    /// </summary>
    /// <param name="tag">Block tag</param>
    /// <returns>tag id</returns>
    ushort BlockTagToTagId(string tag);



    /// <summary>
    /// Returns tag by entity tag id. If the tag id is not registered, it will return empty string.
    /// </summary>
    /// <param name="id">Entity tag id</param>
    /// <returns>tag</returns>
    string EntityTagIdToTag(ushort id);

    /// <summary>
    /// Returns tag by item tag id. If the tag id is not registered, it will return empty string.
    /// </summary>
    /// <param name="id">Item tag id</param>
    /// <returns>tag</returns>
    string ItemTagIdToTag(ushort id);

    /// <summary>
    /// Returns tag by block tag id. If the tag id is not registered, it will return empty string.
    /// </summary>
    /// <param name="id">Block tag id</param>
    /// <returns>tag</returns>
    string BlockTagIdToTag(ushort id);

    /// <summary>
    /// Loads tags from 'TagRegistry.TagsAssetPath' assets from all domains. Is only used by 'RegistryObjectTypeLoader'. Not meant to be called by mods.
    /// </summary>
    /// <param name="api"></param>
    void LoadTagsFromAssets(ICoreServerAPI api);
}
