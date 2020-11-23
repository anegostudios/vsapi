using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace Vintagestory.API.Client
{
    public interface ITerrainMeshPool
    {
        /// <summary>
        /// Requires xyz, uv, rgba, indices, flags and xyzFaces to be set
        /// </summary>
        /// <param name="data"></param>
        /// <param name="lodLevel"></param>
        void AddMeshData(MeshData data, int lodLevel = 1);

        /// <summary>
        /// Requires xyz, uv, rgba, indices, flags and xyzFaces to be set
        /// </summary>
        /// <param name="data"></param>
        /// <param name="colorMapData"></param>
        /// <param name="lodLevel"></param>
        void AddMeshData(MeshData data, ColorMapData colorMapData, int lodLevel = 1);
    }
}
