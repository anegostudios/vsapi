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
        public CustomMeshDataPartFloat() : base() { }
        public CustomMeshDataPartFloat(int arraySize) : base(arraySize) { }

        public CustomMeshDataPartFloat Clone()
        {
            CustomMeshDataPartFloat cloned = new CustomMeshDataPartFloat();
            cloned.SetFrom(this);
            return cloned;
        }
    }
}
