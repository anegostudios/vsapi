using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;

namespace Vintagestory.API.Common
{
    public interface IBlockShapeSupplier
    {
        /// <summary>
        /// Let's you add your own meshes to a chunk. Don't reuse the meshdata instance anywhere in your code.
        /// Return false to use the default mesh instead.
        /// WARNING!
        /// The Tesselator runs in a seperate thread, so you have to make sure the fields and methods you access inside this method are thread safe.
        /// </summary>
        /// <param name="mesher">The chunk mesh, add your stuff here</param>
        /// <param name="tesselator">If you need to tesselate something, I suggest you use this tesselator, since using the main thread tesselator can cause race conditions</param>
        bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator);
    }
}
