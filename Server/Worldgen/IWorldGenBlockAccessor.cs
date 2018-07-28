using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Server
{
    public interface IWorldGenBlockAccessor : IBlockAccessor
    {
        /// <summary>
        /// Tells the server to produce a block update at this given position once the chunk is fully generated and world ticking has begun
        /// </summary>
        /// <param name="pos"></param>
        void ScheduleBlockUpdate(BlockPos pos);

        /// <summary>
        /// Tells the server to relight this position once RunScheduledBlockLightUpdates() is called
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="oldBlockid"></param>
        /// <param name="newBlockId"></param>
        void ScheduleBlockLightUpdate(BlockPos pos, ushort oldBlockid, ushort newBlockId);

        /// <summary>
        /// This will run all scheduled block light updates at once. Should be called after all lighting has been completed.
        /// </summary>
        void RunScheduledBlockLightUpdates();

        /// <summary>
        /// Adds given initialized entity to the world. Requires you to set the Pos and ServerPos fields.
        /// </summary>
        /// <param name="entity"></param>
        void AddEntity(Entity entity);
    }
}
