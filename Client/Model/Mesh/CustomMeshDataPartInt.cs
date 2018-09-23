using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Holds arbitrary int data for meshes to be used in the shader
    /// </summary>
    public class CustomMeshDataPartInt : CustomMeshDataPart<int>
    {
        /// <summary>
        /// Empty constructor.
        /// </summary>
        public CustomMeshDataPartInt() : base() { }

        /// <summary>
        /// Size initialization constructor.
        /// </summary>
        /// <param name="arraySize"></param>
        public CustomMeshDataPartInt(int size) : base(size) { }

        public DataConversion Conversion = DataConversion.Integer;

        /// <summary>
        /// Creates a clone of this collection of data parts.
        /// </summary>
        /// <returns>A clone of this collection of data parts.</returns>
        public CustomMeshDataPartInt Clone()
        {
            CustomMeshDataPartInt cloned = new CustomMeshDataPartInt();
            cloned.SetFrom(this);
            return cloned;
        }
    }
}
