using System.Collections.Generic;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Holds all chunk mesh pools of the current running game
    /// </summary>
    public class MeshDataPoolMasterManager
    {
        List<MeshDataPool> modelPools = new List<MeshDataPool>();
        ICoreClientAPI capi;
        public float currentDt;
        public float[] currentModelViewMatrix;
        public float[] shadowMVPMatrix;


        /// <summary>
        /// If true, RemoveLocation() only actually removes the location after 3 frames. Need to call OnFrame() to achieve that
        /// </summary>
        public bool DelayedPoolLocationRemoval;

        Queue<ModelDataPoolLocation[]>[] removalQueue = new Queue<ModelDataPoolLocation[]>[] {
            new Queue<ModelDataPoolLocation[]>(),
            new Queue<ModelDataPoolLocation[]>(),
            new Queue<ModelDataPoolLocation[]>(),
            new Queue<ModelDataPoolLocation[]>()
        };



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
            if (DelayedPoolLocationRemoval)
            {
                for (int i = 0; i < locations.Length; i++)
                {
                    locations[i].Hide = true;
                }
                removalQueue[0].Enqueue(locations);
                return;
            }

            RemoveLocationsNow(locations);
        }


        public void OnFrame(float dt, float[] modelviewMatrix, float[] shadowMVPMatrix)
        {
            this.currentDt = dt;
            this.currentModelViewMatrix = modelviewMatrix;
            this.shadowMVPMatrix = shadowMVPMatrix;
            while (removalQueue[3].Count > 0)
            {
                RemoveLocationsNow(removalQueue[3].Dequeue());
            }

            while (removalQueue[2].Count > 0)
            {
                removalQueue[3].Enqueue(removalQueue[2].Dequeue());
            }

            while (removalQueue[1].Count > 0)
            {
                removalQueue[2].Enqueue(removalQueue[1].Dequeue());
            }

            while (removalQueue[0].Count > 0)
            {
                removalQueue[1].Enqueue(removalQueue[0].Dequeue());
            }
        }

        private void RemoveLocationsNow(ModelDataPoolLocation[] locations)
        {
            for (int i = 0; i < locations.Length; i++)
            {
                if (locations[i] == null || modelPools[locations[i].PoolId] == null)
                {
                    capi.World.Logger.Error("Could not remove model data from the master pool. Something wonky is happening. Ignoring for now.");
                    continue;
                }

                var mpool = modelPools[locations[i].PoolId];
                mpool.RemoveLocation(locations[i]);
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
