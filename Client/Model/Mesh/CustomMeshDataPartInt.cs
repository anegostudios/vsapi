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
        public CustomMeshDataPartInt() : base() { }
        public CustomMeshDataPartInt(int size) : base(size) { }

        public DataConversion Conversion = DataConversion.Integer;

        public CustomMeshDataPartInt Clone()
        {
            CustomMeshDataPartInt cloned = new CustomMeshDataPartInt();
            cloned.SetFrom(this);
            return cloned;
        }
    }
}
