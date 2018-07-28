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

        public MeshDataPoolMasterManager(ICoreClientAPI capi)
        {
            this.capi = capi;
        }

        public void RemoveModelData(ModelDataPoolLocation[] locations)
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

        public void AddModelDataPool(MeshDataPool pool)
        {
            pool.poolId = modelPools.Count;
            modelPools.Add(pool);
        }

        public void DisposeAllPools(ICoreClientAPI capi)
        {
            for (int i = 0; i < modelPools.Count; i++)
            {
                modelPools[i].Dispose(capi);
            }
        }

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

        public int QuantityModelDataPools()
        {
            return modelPools.Count;
        }
    }
}
