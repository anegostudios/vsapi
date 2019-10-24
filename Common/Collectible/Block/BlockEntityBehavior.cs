using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class BlockEntityBehaviorType
    {
        [JsonProperty]
        public string Name;

        [JsonConverter(typeof(JsonObjectConverter))]
        public JsonObject Properties = new JsonObject(new JObject());
    }

    /// <summary>
    /// Basic class for block entities - a data structures to hold custom information for blocks, e.g. for chests to hold it's contents
    /// </summary>
    public abstract class BlockEntityBehavior
    {
        /// <summary>
        /// The block for this behavior instance.
        /// </summary>
        public BlockEntity Blockentity;

        /// <summary>
        /// The properties of this block behavior.
        /// </summary>
        public JsonObject properties;

        public ICoreAPI Api;
        
        public BlockEntityBehavior(BlockEntity blockentity)
        {
            this.Blockentity = blockentity;
        }

        /// <summary>
        /// Called right after the block behavior was created
        /// </summary>
        /// <param name="properties"></param>
        public virtual void Initialize(ICoreAPI api, JsonObject properties)
        {
            this.Api = api;
        }

        public virtual void OnBlockRemoved()
        {
            
        }

        public virtual void OnBlockUnloaded()
        {
            
        }

        public virtual void OnBlockBroken()
        {
            
        }

        public virtual void OnBlockPlaced()
        {
            
        }

        public virtual void ToTreeAttributes(ITreeAttribute tree)
        {
            
        }

        public virtual void FromTreeAtributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve)
        {
            
        }

        public virtual void OnReceivedClientPacket(IPlayer fromPlayer, int packetid, byte[] data)
        {
            
        }

        public virtual void OnReceivedServerPacket(int packetid, byte[] data)
        {
            
        }

        public virtual void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
        {
            
        }

        public virtual void OnStoreCollectibleMappings(Dictionary<int, AssetLocation> blockIdMapping, Dictionary<int, AssetLocation> itemIdMapping)
        {
            
        }

        public virtual void OnLoadCollectibleMappings(IWorldAccessor worldForNewMappings, Dictionary<int, AssetLocation> oldBlockIdMapping, Dictionary<int, AssetLocation> oldItemIdMapping)
        {
            
        }

        public virtual bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator)
        {
            return false;
        }
    }
}
