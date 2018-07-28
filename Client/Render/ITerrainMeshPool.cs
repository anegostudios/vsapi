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
        void AddMeshData(MeshData data);

        /// <summary>
        /// Requires xyz, uv, rgba, indices, flags and xyzFaces to be set
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tintColor"></param>
        void AddMeshData(MeshData data, int tintColor);
    }
}
