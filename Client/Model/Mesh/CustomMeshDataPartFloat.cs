using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Holds arbitrary float data for meshes to be used in the shader
    /// </summary>
    public class CustomMeshDataPartFloat : CustomMeshDataPart<float>
    {
        /// <summary>
        /// Empty Constructor.
        /// </summary>
        public CustomMeshDataPartFloat() : base() { }

        /// <summary>
        /// Size initialization constructor.
        /// </summary>
        /// <param name="arraySize"></param>
        public CustomMeshDataPartFloat(int arraySize) : base(arraySize) { }

        /// <summary>
        /// Creates a clone of this collection of data parts.
        /// </summary>
        /// <returns>A clone of this collection of data parts.</returns>
        public CustomMeshDataPartFloat Clone()
        {
            CustomMeshDataPartFloat cloned = new CustomMeshDataPartFloat();
            cloned.SetFrom(this);
            return cloned;
        }
    }
}
