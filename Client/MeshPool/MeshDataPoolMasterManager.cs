using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Holds all chunk mesh pools of the current running game
    /// </summary>
    public class MeshDataPoolMasterManager
    {
        List<MeshDataPool> modelPools = new List<MeshDataPool>();
        ICoreClientAPI capi;

        /// <summary>
        /// Initializes the master mesh data pool.
        /// </summary>
        /// <param name="capi">The Client API.</param>
        public MeshDataPoolMasterManager(ICoreClientAPI capi)
        {
            this.capi = capi;
        }

        /// <summary>
        /// Removes the models with the given locations.
        /// </summary>
        /// <param name="locations">The locations of the model data.</param>
        public void RemoveDataPoolLocations(ModelDataPoolLocation[] locations)
        {
            for (int i = 0; i < locations.Length; i++)
            {
                if (locations[i] == null || modelPools[locations[i].poolId] == null)
                {
                    capi.World.Logger.Error("Could not remove model data from the master pool. Something wonky is happening. Ignoring for now.");
                    continue;
                }

                modelPools[locations[i].poolId].RemoveLocation(locations[i]);
            }
        }

        /// <summary>
        /// Adds a new pool to the master pool.
        /// </summary>
        /// <param name="pool">The mesh data pool to add.</param>
        public void AddModelDataPool(MeshDataPool pool)
        {
            pool.poolId = modelPools.Count;
            modelPools.Add(pool);
        }

        /// <summary>
        /// Cleans up and gets rid of all the pools.
        /// </summary>
        /// <param name="capi">The client API.</param>
        public void DisposeAllPools(ICoreClientAPI capi)
        {
            for (int i = 0; i < modelPools.Count; i++)
            {
                modelPools[i].Dispose(capi);
            }
        }

        /// <summary>
        /// Calculates the fragmentation.
        /// </summary>
        /// <returns>The resulting calculation.</returns>
        public float CalcFragmentation()
        {
            long totalUsed = 0;
            long totalAllocated = 0;
            for (int i = 0; i < modelPools.Count; i++)
            {
                totalUsed += modelPools[i].UsedVertices;
                totalAllocated += modelPools[i].VerticesPoolSize;
            }

            return (float)(1 - (double)totalUsed / (double)totalAllocated);
        }

        /// <summary>
        /// The number of model pools in this master manager.
        /// </summary>
        /// <returns>The number of model pools</returns>
        public int QuantityModelDataPools()
        {
            return modelPools.Count;
        }
    }
}
