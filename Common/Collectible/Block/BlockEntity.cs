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
    /// <summary>
    /// Basic class for block entities - a data structures to hold custom information for blocks, e.g. for chests to hold it's contents
    /// </summary>
    public abstract class BlockEntity
    {
        List<long> TickHandlers = new List<long>();
        List<long> CallbackHandlers = new List<long>();

        /// <summary>
        /// The core API added to the block.  Accessable after initialization.
        /// </summary>
        public ICoreAPI api;

        /// <summary>
        /// Position of the block for this block entity
        /// </summary>
        public BlockPos pos;

        
        /// <summary>
        /// Uniquely identifies this block entity instance within the scope of its game world
        /// </summary>
        //public long id;

        /// <summary>
        /// Creats an empty instance. Use initialize to initialize it with the api.
        /// </summary>
        public BlockEntity()
        {
        }
        
        /// <summary>
        /// This method is called right after the block entity was spawned or right after it was loaded from a newly loaded chunk. You do have access to the world and its blocks at this point.
        /// However if this block entity already existed then FromTreeAttributes is called first!
        /// You should still call the base method to sets the this.api field
        /// </summary>
        /// <param name="api"></param>
        public virtual void Initialize(ICoreAPI api)
        {
            this.api = api;
        }

        /// <summary>
        /// Registers a game tick listener that does the disposing for you when the Block is removed
        /// </summary>
        /// <param name="OnGameTick"></param>
        /// <param name="millisecondInterval"></param>
        /// <returns></returns>
        protected virtual long RegisterGameTickListener(Action<float> OnGameTick, int millisecondInterval)
        {
            long listenerId = api.Event.RegisterGameTickListener(OnGameTick, millisecondInterval);
            TickHandlers.Add(listenerId);
            return listenerId;
        }

        /// <summary>
        /// Registers a delayed callback that does the disposing for you when the Block is removed
        /// </summary>
        /// <param name="OnDelayedCallbackTick"></param>
        /// <param name="millisecondInterval"></param>
        /// <returns></returns>
        protected virtual long RegisterDelayedCallback(Action<float> OnDelayedCallbackTick, int millisecondInterval)
        {
            long listenerId = api.Event.RegisterCallback(OnDelayedCallbackTick, millisecondInterval);
            CallbackHandlers.Add(listenerId);
            return listenerId;
        }

        /// <summary>
        /// Called when the block at this position was removed in some way. Removes the game tick listeners, so still call the base method
        /// </summary>
        public virtual void OnBlockRemoved() {
            foreach (long handlerId in TickHandlers)
            {
                api.Event.UnregisterGameTickListener(handlerId);
            }

            foreach (long handlerId in CallbackHandlers)
            {
                api.Event.UnregisterCallback(handlerId);
            }

            api?.World.Logger.VerboseDebug("OnBlockRemoved(): {0}@{1}", this, pos);
        }

        /// <summary>
        /// Called when the block was broken in survival mode or through explosions and similar. Generally in situations where you probably want 
        /// to drop the block entity contents, if it has any
        /// </summary>
        public virtual void OnBlockBroken()
        {

        }

        /// <summary>
        /// Called when the chunk the block entity resides in was unloaded. Removes the game tick listeners, so still call the base method
        /// </summary>
        public virtual void OnBlockUnloaded()
        {
            foreach (long handlerId in TickHandlers)
            {
                api.Event.UnregisterGameTickListener(handlerId);
            }

            foreach (long handlerId in CallbackHandlers)
            {
                api.Event.UnregisterCallback(handlerId);
            }
        }

        /// <summary>
        /// Called when the block entity just got placed, not called when it was previously placed and the chunk is loaded
        /// </summary>
        public virtual void OnBlockPlaced(ItemStack byItemStack = null)
        {
            
        }

        /// <summary>
        /// Called when saving the world or when sending the block entity data to the client. When overriding, make sure to still call the base method.
        /// </summary>
        /// <param name="tree"></param>
        public virtual void ToTreeAttributes(ITreeAttribute tree) {
            tree.SetInt("posx", pos.X);
            tree.SetInt("posy", pos.Y);
            tree.SetInt("posz", pos.Z);
        }

        /// <summary>
        /// Called when loading the world or when receiving block entity from the server. When overriding, make sure to still call the base method.
        /// FromTreeAtributes is always called before Initialize() is called, so the this.api field is not yet set!
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="worldAccessForResolve">Use this api if you need to resolve blocks/items. Not suggested for other purposes, as the residing chunk may not be loaded at this point</param>
        public virtual void FromTreeAtributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve) {
            pos = new BlockPos(
                tree.GetInt("posx"),
                tree.GetInt("posy"),
                tree.GetInt("posz")
            );
        }

        /// <summary>
        /// Called whenever a blockentity packet at the blocks position has been received from the client
        /// </summary>
        /// <param name="fromPlayer"></param>
        /// <param name="packetid"></param>
        /// <param name="data"></param>
        public virtual void OnReceivedClientPacket(IPlayer fromPlayer, int packetid, byte[] data)
        {
        }

        /// <summary>
        /// Called whenever a blockentity packet at the blocks position has been received from the server
        /// </summary>
        /// <param name="packetid"></param>
        /// <param name="data"></param>
        public virtual void OnReceivedServerPacket(int packetid, byte[] data)
        {
        }


        /// <summary>
        /// When called on Server: Will resync the block entity with all its TreeAttribute to the client, but will not resend or redraw the block unless specified.
        /// When called on Client: Triggers a block changed event on the client, but will not redraw the block unless specified.
        /// </summary>
        /// <param name="redrawOnClient">When true, the block is also marked dirty and thus redrawn. When called serverside a dirty block packet is sent to the client for it to be redrawn</param>
        public void MarkDirty(bool redrawOnClient = false)
        {
            api.World.BlockAccessor.MarkBlockEntityDirty(pos);

            if (redrawOnClient) {
                api.World.BlockAccessor.MarkBlockDirty(pos);
            }
        }

        /// <summary>
        /// Called by the block info HUD for displaying additional information
        /// </summary>
        /// <param name="forPlayer"></param>
        /// <returns></returns>
        public virtual string GetBlockInfo(IPlayer forPlayer)
        {
            return null;
        }


        /// <summary>
        /// Called by the worldedit schematic exporter so that it can also export the mappings of items/blocks stored inside blockentities
        /// </summary>
        /// <param name="blockIdMapping"></param>
        /// <param name="itemIdMapping"></param>
        public virtual void OnStoreCollectibleMappings(Dictionary<int, AssetLocation> blockIdMapping, Dictionary<int, AssetLocation> itemIdMapping)
        {

        }

        /// <summary>
        /// Called by the blockschematic loader so that you may fix any blockid/itemid mappings against the mapping of the savegame, if you store any collectibles in this blockentity.
        /// Hint: Use itemstack.FixMapping() to do the job for you.
        /// </summary>
        /// <param name="oldBlockIdMapping"></param>
        /// <param name="oldItemIdMapping"></param>
        public virtual void OnLoadCollectibleMappings(IWorldAccessor worldForNewMappings, Dictionary<int, AssetLocation> oldBlockIdMapping, Dictionary<int, AssetLocation> oldItemIdMapping)
        {

        }

    }

}
