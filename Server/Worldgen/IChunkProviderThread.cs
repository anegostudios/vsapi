
#nullable disable
namespace Vintagestory.API.Server
{
    public interface IChunkProviderThread
    {
        /// <summary>
        /// Retrieve a customized interface to access blocks for generating chunks
        /// </summary>
        /// <param name="updateHeightmap">Whether or not SetBlock should update the heightmap</param>
        /// <returns></returns>
        IWorldGenBlockAccessor GetBlockAccessor(bool updateHeightmap);
    }
}
