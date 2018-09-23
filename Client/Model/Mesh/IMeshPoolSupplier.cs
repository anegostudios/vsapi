using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public interface IMeshPoolSupplier
    {
        /// <summary>
        /// Gets a mesh pool supplier for the given render pass.
        /// </summary>
        /// <param name="forRenderPass">The given render pass.</param>
        /// <returns>The mesh data for the render pass.</returns>
        MeshData GetMeshPoolForPass(EnumChunkRenderPass forRenderPass);
    }
}
