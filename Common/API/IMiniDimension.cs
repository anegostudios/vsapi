using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Provides read/write access to the blocks of a movable mini-dimension.
    /// </summary>
    public interface IMiniDimension : IBlockAccessor
    {
        int subDimensionId { get; set; }
        EntityPos CurrentPos { get; set; }
        bool Dirty { get; set; }
        bool TrackSelection { get; set; }
        BlockPos selectionTrackingOriginalPos { get; set; }
        /// <summary>
        /// If this matches the subDimensionId, it indicates that this is a blocks preview minidimension
        /// </summary>
        int BlocksPreviewSubDimension_Server { get; set; }

        /// <summary>
        /// Sends dirty chunks to nearby clients
        /// </summary>
        void CollectChunksForSending(IPlayer[] players);

        /// <summary>
        /// Clears this mini-dimension (and empties any chunks but does not unload them) ready for re-use
        /// </summary>
        void ClearChunks();
        /// <summary>
        /// Unload any chunks which are still empty after the dimension has been re-used
        /// </summary>
        void UnloadUnusedServerChunks();

        /// <summary>
        /// Used when rendering
        /// </summary>
        FastVec3d GetRenderOffset(float dt);

        void SetRenderOffsetY(int offsetY);
        float[] GetRenderTransformMatrix(float[] currentModelViewMatrix, Vec3d playerPos);
        void ReceiveClientChunk(long chunkIndex3d, IWorldChunk chunk, IWorldAccessor world);
        void SetSubDimensionId(int dimensionId);
        void SetSelectionTrackingSubId_Server(int dimensionId);
        void AdjustPosForSubDimension(BlockPos pos);
    }
}
