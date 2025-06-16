using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Allows adding behaviors for block entities. Remember, block entities are specific instances of blocks placed within the world.
    /// </summary>
    /// <example>
    /// <code language="json">
    ///"entityClass": "Brake",
	///"entityBehaviors": [
	///	{ "name": "MPBrake" },
	///	{ "name": "Animatable" }
	///],
    /// </code>
    /// </example>
    [DocumentAsJson]
    public class BlockEntityBehaviorType
    {
        /// <summary>
        /// <!--<jsonoptional>Required</jsonoptional>-->
        /// The ID for this block entity behavior.
        /// </summary>
        [JsonProperty]
        public string Name;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// A set of properties specific to the block entity behavior class.
        /// </summary>
        [JsonProperty, JsonConverter(typeof(JsonAttributesConverter))]
        public JsonObject properties;
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
        /// Alias of Blockentity.Pos
        /// </summary>
        public BlockPos Pos => Blockentity.Pos;

        /// <summary>
        /// Alias of BlockEntity.Block
        /// </summary>
        public Block Block => Blockentity.Block;

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
        /// <param name="api"></param>
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

        public virtual void OnBlockBroken(IPlayer byPlayer = null)
        {
            
        }

        public virtual void OnBlockPlaced(ItemStack byItemStack = null)
        {
            
        }

        public virtual void ToTreeAttributes(ITreeAttribute tree)
        {
            
        }

        public virtual void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve)
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

        [Obsolete("Use the variant with resolveImports parameter")]
        public virtual void OnLoadCollectibleMappings(IWorldAccessor worldForNewMappings, Dictionary<int, AssetLocation> oldBlockIdMapping, Dictionary<int, AssetLocation> oldItemIdMapping, int schematicSeed)
        {
            OnLoadCollectibleMappings(worldForNewMappings, oldItemIdMapping, oldItemIdMapping, schematicSeed, true);
        }

        public virtual void OnLoadCollectibleMappings(IWorldAccessor worldForNewMappings, Dictionary<int, AssetLocation> oldBlockIdMapping, Dictionary<int, AssetLocation> oldItemIdMapping, int schematicSeed, bool resolveImports)
        {
            
        }

        public virtual bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator)
        {
            return false;
        }

        public virtual void OnPlacementBySchematic(ICoreServerAPI api, IBlockAccessor blockAccessor, BlockPos pos, Dictionary<int, Dictionary<int, int>> replaceBlocks, int centerrockblockid, Block layerBlock, bool resolveImports)
        {
            
        }
    }
}
