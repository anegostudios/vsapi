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
        /// <param name="tessThreadTesselator">If you need to tesselate something, you should use this tesselator, since using the main thread tesselator can cause race conditions and crash the game</param>
        bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator);
    }
}
