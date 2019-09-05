using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Adds a tesselator to your block
    /// WARNING: please make sure whatever functions and fields you use with the OnTesselation event are THREAD SAFE!
    /// </summary>
    public interface IBlockShapeSupplier
    {
        /// <summary>
        /// Let's you add your own meshes to a chunk. Don't reuse the meshdata instance anywhere in your code.
        /// WARNING!
        /// The Tesselator runs in a seperate thread, so you have to make sure the fields and methods you access inside this method are thread safe.
        /// </summary>
        /// <param name="mesher">The chunk mesh, add your stuff here</param>
        /// <param name="tessThreadTesselator">If you need to tesselate something, you should use this tesselator, since using the main thread tesselator can cause race conditions and crash the game</param>
        /// <returns>True to skip default mesh, false to also add the default mesh</returns>
        bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator);
    }
}
