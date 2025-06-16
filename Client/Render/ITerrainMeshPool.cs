
#nullable disable
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
        /// Add mesh data, but first transformed with tfMatrix
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tfMatrix"></param>
        /// <param name="lodLevel"></param>
        void AddMeshData(MeshData data, float[] tfMatrix, int lodLevel = 1);

        /// <summary>
        /// Requires xyz, uv, rgba, indices, flags and xyzFaces to be set
        /// </summary>
        /// <param name="data"></param>
        /// <param name="colorMapData"></param>
        /// <param name="lodLevel"></param>
        void AddMeshData(MeshData data, ColorMapData colorMapData, int lodLevel = 1);
    }
}
