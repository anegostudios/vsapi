using System;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Vintagestory.API.Common
{
    public class HistoryState
    {
        public static HistoryState Empty = new HistoryState() { BlockUpdates = new BlockUpdate[0], DecorUpdates = new DecorUpdate[0] };

        public BlockUpdate[] BlockUpdates;
        public DecorUpdate[] DecorUpdates;
        public object Data;
        public BlockPos OldStartMarker;
        public BlockPos OldEndMarker;

        public BlockPos NewStartMarker;
        public BlockPos NewEndMarker;
    }


    /// <summary>
    /// Provides read/write access to the blocks of a world. 
    /// </summary>
    public interface IBlockAccessorRevertable : IBulkBlockAccessor
    {
        event Action<HistoryState> OnStoreHistoryState;
        event Action<HistoryState, int> OnRestoreHistoryState;

        /// <summary>
        /// Whether or not to do relighting on the chunk
        /// </summary>
        bool Relight { get; set; }

        /// <summary>
        /// 0 = working on latest version, 1 = undo used one time, 2 = undo used 2 times, etc.
        /// </summary>
        int CurrentyHistoryState { get; }

        /// <summary>
        /// 1 = perform 1 undo 
        /// -1 = perform 1 redo
        /// </summary>
        void ChangeHistoryState(int quantity = 1);

        /// <summary>
        /// Maximum Amount of undos you can perform. More states means more memory usage.
        /// </summary>
        int QuantityHistoryStates { get; set; }

        /// <summary>
        /// Amount of currently stored history states
        /// </summary>
        int AvailableHistoryStates { get; }

        /// <summary>
        /// Manually set the history state of a block for the to-be-comitted history state
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="oldBlockId"></param>
        /// <param name="newBlockId"></param>
        void SetHistoryStateBlock(int posX, int posY, int posZ, int oldBlockId, int newBlockId);

        void CommitBlockEntityData();

        void BeginMultiEdit();
        void EndMultiEdit();

        void StoreHistoryState(HistoryState state);

    }
}
