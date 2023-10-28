using System;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

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
        bool TrackSelection { get; }
        BlockPos PreviewPos { get; set; }

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
        float[] GetRenderTransformMatrix(float[] currentModelViewMatrix, Vec3d playerPos);
        void ReceiveClientChunk(long chunkIndex3d, IWorldChunk chunk, IWorldAccessor world);
        void SetSubDimensionId(int dimensionId);
        void AdjustPosForSubDimension(BlockPos pos);
    }
}
