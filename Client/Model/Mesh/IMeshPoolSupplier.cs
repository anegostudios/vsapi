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
        MeshData GetMeshPoolForPass(EnumChunkRenderPass forRenderPass);
    }
}
