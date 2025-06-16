using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Server
{
    public interface IWorldGenBlockAccessor : IBlockAccessor
    {
        /// <summary>
        /// Returns a special IWorldAccessor which wraps the standard one with one difference: it returns this IWorldGenBlockAccessor as its BlockAccessor, in place of the general BlockAccessor for this world
        /// </summary>
        IServerWorldAccessor WorldgenWorldAccessor { get; }

        /// <summary>
        /// Tells the server to produce a block update at this given position once the chunk is fully generated and world ticking has begun
        /// </summary>
        /// <param name="pos"></param>
        void ScheduleBlockUpdate(BlockPos pos);

        /// <summary>
        /// Tells the server to relight this position once RunScheduledBlockLightUpdates() is called
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="oldBlockid">currently unused but might be used in the future to make sure old light is removed properly</param>
        /// <param name="newBlockId"></param>
        void ScheduleBlockLightUpdate(BlockPos pos, int oldBlockid, int newBlockId);

        /// <summary>
        /// This will run all scheduled block light updates at once. Should be called after all lighting has been completed.
        /// </summary>
        void RunScheduledBlockLightUpdates(int chunkx, int chunkz);

        /// <summary>
        /// Adds given initialized entity to the world. Requires you to set the Pos and ServerPos fields.
        /// </summary>
        /// <param name="entity"></param>
        void AddEntity(Entity entity);

        void BeginColumn();
    }
}
