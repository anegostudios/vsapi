using Vintagestory.API.Client;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// A collectible object that can be placed on the ground or on shelves or in display cases, but require custom code or rendering for it
    /// </summary>
    public interface ICollectibleDisplayable
    {
        /// <summary>
        /// Return a custom mesh to be used for the collectible. Return null to use default item/block mesh. It is recommended to cache the returned mesh in the ObjectCache for efficieny.
        /// </summary>
        /// <param name="inSlot"></param>
        /// <param name="displayType">e.g. "shelf" or "ground" or "displaycase"</param>
        /// <returns></returns>
        MeshData GetMeshDataForDisplay(ItemSlot inSlot, string displayType);


        /// <summary>
        /// This collectible was placed in-world and the chunk is about to get re-tesselated.
        /// </summary>
        /// <param name="byBlockEntity"></param>
        /// <param name="inSlot"></param>
        void NowOnDisplay(BlockEntity byBlockEntity, ItemSlot inSlot);
    }

    /// <summary>
    /// A collectible object that can be placed on the ground or on shelves or in display cases, but also can still accept interactions from the player
    /// </summary>
    public interface ICollectibleOnDisplayInteractable
    {
        bool OnInteractStart(ItemSlot inSlot, IPlayer byPlayer);

        bool OnInteractStep(float secondsUsed, ItemSlot inSlot, IPlayer byPlayer);

        void OnInteractStop(float secondsUsed, ItemSlot inSlot, IPlayer byPlayer);

        bool OnInteractCancel(float secondsUsed, ItemSlot inSlot, IPlayer byPlayer, EnumItemUseCancelReason cancelReason);
    }

}
