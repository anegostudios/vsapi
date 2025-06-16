
#nullable disable
namespace Vintagestory.API.Client
{
    public interface IMeshPoolSupplier
    {
        /// <summary>
        /// Gets a mesh pool supplier for the given render pass.
        /// </summary>
        /// <param name="textureid"></param>
        /// <param name="forRenderPass">The given render pass.</param>
        /// <param name="lodLevel"></param>
        /// <returns>The mesh data for the render pass.</returns>
        MeshData GetMeshPoolForPass(int textureid, EnumChunkRenderPass forRenderPass, int lodLevel);
    }
}
