using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Basic class for block entities - a data structures to hold custom information for blocks, e.g. for chests to hold it's contents
    /// </summary>
    public abstract class BlockEntity
    {
        protected List<long> TickHandlers;
        protected List<long> CallbackHandlers;

        /// <summary>
        /// The core API added to the block.  Accessable after initialization.
        /// </summary>
        public ICoreAPI Api;

        /// <summary>
        /// Position of the block for this block entity
        /// </summary>
        public BlockPos Pos;

        /// <summary>
        /// The block type at the position of the block entity. This poperty is updated by the engine if ExchangeBlock is called
        /// </summary>
        public Block Block { get; set; }

        /// <summary>
        /// List of block entity behaviors associated with this block entity
        /// </summary>
        public List<BlockEntityBehavior> Behaviors = new List<BlockEntityBehavior>();

        public ItemStack stackForWorldgen;

        /// <summary>
        /// Creats an empty instance. Use initialize to initialize it with the api.
        /// </summary>
        public BlockEntity()
        {
        }

        public T GetBehavior<T>() where T : class
        {
            for (int i = 0; i < Behaviors.Count; i++)
            {
                if (Behaviors[i] is T)
                {
                    return Behaviors[i] as T;
                }
            }

            return null;
        }


        /// <summary>
        /// This method is called right after the block entity was spawned or right after it was loaded from a newly loaded chunk. You do have access to the world and its blocks at this point.
        /// However if this block entity already existed then FromTreeAttributes is called first!
        /// You should still call the base method to sets the this.api field
        /// </summary>
        /// <param name="api"></param>
        public virtual void Initialize(ICoreAPI api)
        {
            this.Api = api;

            if (api.World.FrameProfiler?.Enabled == true)
            {
                foreach (var val in Behaviors)
                {
                    val.Initialize(api, val.properties);
                    api.World.FrameProfiler.Mark("initbebehavior-", val.GetType());
                }
            } else
            {
                foreach (var val in Behaviors) val.Initialize(api, val.properties);
            }

            if (stackForWorldgen != null)
            {
                try
                {
                    OnBlockPlaced(stackForWorldgen);
                }
                catch (Exception e)   // Just in case modded BlockEntity are not expecting to be handled in this way
                {
                    api.Logger.Error(e);
                }
                stackForWorldgen = null;
            }
        }


        public virtual void CreateBehaviors(Block block, IWorldAccessor worldForResolve)
        {
            this.Block = block;

            foreach (var beht in block.BlockEntityBehaviors)
            {
                if (worldForResolve.ClassRegistry.GetBlockEntityBehaviorClass(beht.Name) == null)
                {
                    worldForResolve.Logger.Warning(Lang.Get("Block entity behavior {0} for block {1} not found", beht.Name, block.Code));
                    continue;
                }

                if (beht.properties == null) beht.properties = new JsonObject(new JObject());
                BlockEntityBehavior behavior = worldForResolve.ClassRegistry.CreateBlockEntityBehavior(this, beht.Name);
                behavior.properties = beht.properties;

                Behaviors.Add(behavior);
            }
        }

        /// <summary>
        /// Registers a game tick listener that does the disposing for you when the Block is removed
        /// </summary>
        /// <param name="onGameTick"></param>
        /// <param name="millisecondInterval"></param>
        /// <param name="initialDelayOffsetMs"></param>
        /// <returns></returns>
        public virtual long RegisterGameTickListener(Action<float> onGameTick, int millisecondInterval, int initialDelayOffsetMs = 0)
        {
            if (Dimensions.ShouldNotTick(Pos, Api)) return 0L;
            long listenerId = Api.Event.RegisterGameTickListener(onGameTick, TickingExceptionHandler, millisecondInterval, initialDelayOffsetMs);
            if (TickHandlers == null) TickHandlers = new List<long>(1);
            TickHandlers.Add(listenerId);
            return listenerId;
        }

        /// <summary>
        /// Removes a registered game tick listener from the game.
        /// </summary>
        /// <param name="listenerId">the ID of the listener to unregister.</param>
        public virtual void UnregisterGameTickListener(long listenerId)
        {
            Api.Event.UnregisterGameTickListener(listenerId);
            TickHandlers?.Remove(listenerId);
        }

        public virtual void UnregisterAllTickListeners()
        {
            if (TickHandlers != null)
            {
                foreach (long handlerId in TickHandlers)
                {
                    Api.Event.UnregisterGameTickListener(handlerId);
                }
            }
        }

        /// <summary>
        /// Registers a delayed callback that does the disposing for you when the Block is removed
        /// </summary>
        /// <param name="OnDelayedCallbackTick"></param>
        /// <param name="millisecondInterval"></param>
        /// <returns></returns>
        public virtual long RegisterDelayedCallback(Action<float> OnDelayedCallbackTick, int millisecondInterval)
        {
            long listenerId = Api.Event.RegisterCallback(OnDelayedCallbackTick, millisecondInterval);
            if (CallbackHandlers == null) CallbackHandlers = new List<long>();
            CallbackHandlers.Add(listenerId);
            return listenerId;
        }


        /// <summary>
        /// Unregisters a callback.  This is usually done automatically.
        /// </summary>
        /// <param name="listenerId">The ID of the callback listiner.</param>
        public virtual void UnregisterDelayedCallback(long listenerId)
        {
            Api.Event.UnregisterCallback(listenerId);
            CallbackHandlers?.Remove(listenerId);
        }

        public virtual void TickingExceptionHandler(Exception e)
        {
            if (Api == null) throw new Exception("Api was null while ticking a BlockEntity: " + GetType().FullName);

            Api.Logger.Error("At position " + Pos + " for block " + (Block?.Code.ToShortString() ?? "(missing)") + " a " + GetType().Name + " threw an error when ticked:");
            Api.Logger.Error(e);
        }

        /// <summary>
        /// Called when the block at this position was removed in some way. Removes the game tick listeners, so still call the base method
        /// </summary>
        public virtual void OnBlockRemoved() {
            UnregisterAllTickListeners();

            if (CallbackHandlers != null) foreach (long handlerId in CallbackHandlers)
            {
                Api.Event.UnregisterCallback(handlerId);
            }

            foreach (var val in Behaviors)
            {
                val.OnBlockRemoved();
            }

            //api?.World.Logger.VerboseDebug("OnBlockRemoved(): {0}@{1}", this, pos);
        }

        //Adds/Removes block entity behaviors to reflect the new block properties.
        /// <summary>
        /// Called when blockAccessor.ExchangeBlock() is used to exchange this block. Make sure to call the base method when overriding.
        /// </summary>
        /// <param name="block"></param>
        public virtual void OnExchanged(Block block)
        {
            //var oldBlock = this.Block;
            if (block != this.Block) MarkDirty(true);
            this.Block = block;

            // Add new behaviors
            /*foreach (var beht in block.BlockEntityBehaviors)
            {
                if (Api.World.ClassRegistry.GetBlockEntityBehaviorClass(beht.Name) == null)
                {
                    Api.World.Logger.Warning(Lang.Get("Block entity behavior {0} for block {1} not found", beht.Name, block.Code));
                    continue;
                }

                if (Behaviors.FirstOrDefault(bh => bh.GetType() == Api.World.ClassRegistry.GetBlockEntityBehaviorClass(beht.Name)) != null) continue;

                if (beht.properties == null) beht.properties = new JsonObject(new JObject());
                BlockEntityBehavior behavior = Api.World.ClassRegistry.CreateBlockEntityBehavior(this, beht.Name);
                behavior.properties = beht.properties;

                Behaviors.Add(behavior);
            }

            // Remove old behaviors
            foreach (var oldbh in oldBlock.BlockEntityBehaviors)
            {
                if (block.BlockEntityBehaviors.FirstOrDefault(bh => bh.Name == oldbh.Name) == null)
                {
                    var type = Api.World.ClassRegistry.GetBlockEntityBehaviorClass(oldbh.Name);
                    this.Behaviors.RemoveAll(bh => bh.GetType() == type);
                }
            }*/

        }

        /// <summary>
        /// Called when the block was broken in survival mode or through explosions and similar. Generally in situations where you probably want
        /// to drop the block entity contents, if it has any
        /// </summary>
        public virtual void OnBlockBroken(IPlayer byPlayer = null)
        {
            foreach (var val in Behaviors)
            {
                val.OnBlockBroken(byPlayer);
            }
        }

        /// <summary>
        /// Called by the undo/redo system, after calling FromTreeAttributes
        /// </summary>
        public virtual void HistoryStateRestore()
        {

        }

        /// <summary>
        /// Called when the chunk the block entity resides in was unloaded. Removes the game tick listeners, so still call the base method
        /// </summary>
        public virtual void OnBlockUnloaded()
        {
            try
            {
                if (Api != null)
                {
                    UnregisterAllTickListeners();

                    if (CallbackHandlers != null) foreach (long handlerId in CallbackHandlers)
                    {
                        Api.Event.UnregisterCallback(handlerId);
                    }
                }

                foreach (var val in Behaviors)
                {
                    val.OnBlockUnloaded();
                }
            }
            catch (Exception)
            {
                Api.Logger.Error("At position " + Pos + " for block " + (Block?.Code.ToShortString() ?? "(missing)") + " a " + GetType().Name + " threw an error when unloaded");
                throw;
            }
        }

        /// <summary>
        /// Called when the block entity just got placed, not called when it was previously placed and the chunk is loaded. Always called after Initialize()
        /// </summary>
        public virtual void OnBlockPlaced(ItemStack byItemStack = null)
        {
            foreach (var val in Behaviors)
            {
                val.OnBlockPlaced(byItemStack);
            }
        }

        /// <summary>
        /// Called when saving the world or when sending the block entity data to the client. When overriding, make sure to still call the base method.
        /// </summary>
        /// <param name="tree"></param>
        public virtual void ToTreeAttributes(ITreeAttribute tree) {
            if (Api?.Side != EnumAppSide.Client && Block.IsMissing)
            {
                foreach (var val in missingBlockTree)
                {
                    tree[val.Key] = val.Value;
                }
                return;
            }

            tree.SetInt("posx", Pos.X);
            tree.SetInt("posy", Pos.InternalY);
            tree.SetInt("posz", Pos.Z);
            if (Block != null)
            {
                tree.SetString("blockCode", Block.Code.ToShortString());
            }

            foreach (var val in Behaviors)
            {
                val.ToTreeAttributes(tree);
            }
        }

        /// <summary>
        /// Called when loading the world or when receiving block entity from the server. When overriding, make sure to still call the base method.
        /// FromTreeAttributes is always called before Initialize() is called, so the this.api field is not yet set!
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="worldAccessForResolve">Use this api if you need to resolve blocks/items. Not suggested for other purposes, as the residing chunk may not be loaded at this point</param>
        public virtual void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve) {
            Pos = new BlockPos(
                tree.GetInt("posx"),
                tree.GetInt("posy"),
                tree.GetInt("posz")
            );

            foreach (var val in Behaviors)
            {
                val.FromTreeAttributes(tree, worldAccessForResolve);
            }

            if (worldAccessForResolve.Side == EnumAppSide.Server && Block.IsMissing) missingBlockTree = tree;
        }

        private ITreeAttribute missingBlockTree = null;

        /// <summary>
        /// Called whenever a blockentity packet at the blocks position has been received from the client
        /// </summary>
        /// <param name="fromPlayer"></param>
        /// <param name="packetid"></param>
        /// <param name="data"></param>
        public virtual void OnReceivedClientPacket(IPlayer fromPlayer, int packetid, byte[] data)
        {
            foreach (var val in Behaviors)
            {
                val.OnReceivedClientPacket(fromPlayer, packetid, data);
            }
        }

        /// <summary>
        /// Called whenever a blockentity packet at the blocks position has been received from the server
        /// </summary>
        /// <param name="packetid"></param>
        /// <param name="data"></param>
        public virtual void OnReceivedServerPacket(int packetid, byte[] data)
        {
            foreach (var val in Behaviors)
            {
                val.OnReceivedServerPacket(packetid, data);
            }
        }


        /// <summary>
        /// When called on Server: Will resync the block entity with all its TreeAttribute to the client, but will not resend or redraw the block unless specified.
        /// When called on Client: Triggers a block changed event on the client, but will not redraw the block unless specified.
        /// </summary>
        /// <param name="redrawOnClient">When true, the block is also marked dirty and thus redrawn. When called serverside a dirty block packet is sent to the client for it to be redrawn</param>
        /// <param name="skipPlayer"></param>
        public virtual void MarkDirty(bool redrawOnClient = false, IPlayer skipPlayer = null)
        {
            if (Api == null) return;

            Api.World.BlockAccessor.MarkBlockEntityDirty(Pos);

            if (redrawOnClient) {
                Api.World.BlockAccessor.MarkBlockDirty(Pos, skipPlayer);
            }
        }

        /// <summary>
        /// Called by the block info HUD for displaying additional information
        /// </summary>
        /// <param name="forPlayer"></param>
        /// <param name="dsc"></param>
        /// <returns></returns>
        public virtual void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
        {
            foreach (var val in Behaviors)
            {
                val.GetBlockInfo(forPlayer, dsc);
            }
        }


        /// <summary>
        /// Called by the worldedit schematic exporter so that it can also export the mappings of items/blocks stored inside blockentities
        /// </summary>
        /// <param name="blockIdMapping"></param>
        /// <param name="itemIdMapping"></param>
        public virtual void OnStoreCollectibleMappings(Dictionary<int, AssetLocation> blockIdMapping, Dictionary<int, AssetLocation> itemIdMapping)
        {
            foreach (var val in Behaviors)
            {
                val.OnStoreCollectibleMappings(blockIdMapping, itemIdMapping);
            }
        }

        /// <summary>
        /// Called by the blockschematic loader so that you may fix any blockid/itemid mappings against the mapping of the savegame, if you store any collectibles in this blockentity.
        /// Note: Some vanilla blocks resolve randomized contents in this method.
        /// Hint: Use itemstack.FixMapping() to do the job for you.
        /// </summary>
        /// <param name="worldForNewMappings"></param>
        /// <param name="oldBlockIdMapping"></param>
        /// <param name="oldItemIdMapping"></param>
        /// <param name="schematicSeed">If you need some sort of randomness consistency accross an imported schematic, you can use this value</param>
        [Obsolete("Use the variant with resolveImports parameter")]
        public virtual void OnLoadCollectibleMappings(IWorldAccessor worldForNewMappings, Dictionary<int, AssetLocation> oldBlockIdMapping, Dictionary<int, AssetLocation> oldItemIdMapping, int schematicSeed)
        {
            OnLoadCollectibleMappings(worldForNewMappings, oldBlockIdMapping, oldItemIdMapping, schematicSeed, true);
        }

        /// <summary>
        /// Called by the blockschematic loader so that you may fix any blockid/itemid mappings against the mapping of the savegame, if you store any collectibles in this blockentity.
        /// Note: Some vanilla blocks resolve randomized contents in this method.
        /// Hint: Use itemstack.FixMapping() to do the job for you.
        /// </summary>
        /// <param name="worldForNewMappings"></param>
        /// <param name="oldBlockIdMapping"></param>
        /// <param name="oldItemIdMapping"></param>
        /// <param name="schematicSeed">If you need some sort of randomness consistency accross an imported schematic, you can use this value</param>
        /// <param name="resolveImports">Turn it off to spawn structures as they are. For example, in this mode, instead of traders, their meta spawners will spawn</param>
        public virtual void OnLoadCollectibleMappings(IWorldAccessor worldForNewMappings, Dictionary<int, AssetLocation> oldBlockIdMapping, Dictionary<int, AssetLocation> oldItemIdMapping, int schematicSeed, bool resolveImports)
        {
            foreach (var val in Behaviors)
            {
                val.OnLoadCollectibleMappings(worldForNewMappings, oldBlockIdMapping, oldItemIdMapping, schematicSeed, resolveImports);
            }
        }


        /// <summary>
        /// Let's you add your own meshes to a chunk. Don't reuse the meshdata instance anywhere in your code. Return true to skip the default mesh.
        /// WARNING!
        /// The Tesselator runs in a seperate thread, so you have to make sure the fields and methods you access inside this method are thread safe.
        /// </summary>
        /// <param name="mesher">The chunk mesh, add your stuff here</param>
        /// <param name="tessThreadTesselator">If you need to tesselate something, you should use this tesselator, since using the main thread tesselator can cause race conditions and crash the game</param>
        /// <returns>True to skip default mesh, false to also add the default mesh</returns>
        public virtual bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator)
        {
            bool result = false;

            for (int i = 0; i < Behaviors.Count; i++)
            {
                result |= Behaviors[i].OnTesselation(mesher, tessThreadTesselator);
            }

            return result;
        }

        /// <summary>
        /// Called when this block entity was placed by a schematic, either through world edit or by worldgen
        /// </summary>
        /// <param name="api"></param>
        /// <param name="blockAccessor"></param>
        /// <param name="pos"></param>
        /// <param name="replaceBlocks"></param>
        /// <param name="centerrockblockid"></param>
        /// <param name="layerBlock">If block.CustomBlockLayerHandler is true and the block is below the surface, this value is set</param>
        /// <param name="resolveImports">Turn it off to spawn structures as they are. For example, in this mode, instead of traders, their meta spawners will spawn</param>
        public virtual void OnPlacementBySchematic(Server.ICoreServerAPI api, IBlockAccessor blockAccessor, BlockPos pos, Dictionary<int, Dictionary<int, int>> replaceBlocks, int centerrockblockid, Block layerBlock, bool resolveImports)
        {
            Pos = pos.Copy();

            for (int i = 0; i < Behaviors.Count; i++)
            {
                Behaviors[i].OnPlacementBySchematic(api, blockAccessor, pos, replaceBlocks, centerrockblockid, layerBlock, resolveImports);
            }
        }
    }

}
