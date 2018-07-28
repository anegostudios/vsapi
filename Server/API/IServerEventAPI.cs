using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Common;
using Action = Vintagestory.API.Common.Action;

namespace Vintagestory.API.Server
{
    /// <summary>
    /// Contains methods to hook into various server processes
    /// </summary>
    public interface IServerEventAPI : IEventAPI
    {
        /// <summary>
        /// Returns the list of currently registered map chunk generator handlers for given playstyle. Returns an array of handler lists. Each element in the array represents all the handlers for one worldgenpass (see EnumWorldGenPass)       
        /// </summary>
        /// <param name="playstyle"></param>
        /// <returns></returns>
        IWorldGenHandler GetRegisteredWorldGenHandlers(EnumPlayStyle playstyle);
        

        /// <summary>
        /// Registers a handler to be called every time a player places a block. The methods return value determines if the player may place/break this block.
        /// </summary>
        /// <param name = "handler">Function to register. Required parameters: (int player, int x, int y, int z)</param>
        void CanPlaceOrBreakBlock(CanPlaceOrBreak handler);

        /// <summary>
        /// Registers a handler to be called every time a player uses a block. The methods return value determines if the player may place/break this block.
        /// </summary>
        /// <param name = "handler">Function to register. Required parameters: (int player, int x, int y, int z)</param>
        void CanUseBlock(CanUse handler);


        /// <summary>
        /// If you require neighbour chunk data during world generation, you have to register to this event to receive access to the chunk generator thread. This method is only called once during server startup.
        /// </summary>
        /// <param name="f"></param>
        void GetWorldgenBlockAccessor(WorldGenThread f);

        /// <summary>
        /// Event that is triggered whenever a new column of chunks is being generated. It is always called before the ChunkGenerator event
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="worldPlayStyleFlags">For which worlds to use this generator</param>
        void MapChunkGeneration(MapChunkGenerator handler, EnumPlayStyleFlag worldPlayStyleFlags = EnumPlayStyleFlag.WildernessSurvival | EnumPlayStyleFlag.SurviveAndAutomate | EnumPlayStyleFlag.SurviveAndBuild);

        /// <summary>
        /// Event that is triggered whenever a new 8x8 section of column of chunks is being generated. It is always called before the ChunkGenerator and before the MapChunkGeneration event
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="worldPlayStyleFlags">For which worlds to use this generator</param>
        void MapRegionGeneration(MapRegionGenerator handler, EnumPlayStyleFlag worldPlayStyleFlags = EnumPlayStyleFlag.WildernessSurvival | EnumPlayStyleFlag.SurviveAndAutomate | EnumPlayStyleFlag.SurviveAndBuild);

        /// <summary>
        /// Vintagestory uses this method to generate the basic terrain (base terrain + rock strata + caves) in full columns. Only called once in pass EnumWorldGenPass.TerrainNoise. Register to this event if you need acces to a whole chunk column during inital generation.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="pass"></param>
        /// <param name="worldPlayStyleFlags">For which worlds to use this generator</param>
        void ChunkColumnGeneration(ChunkColumnGeneration handler, EnumWorldGenPass pass, EnumPlayStyleFlag worldPlayStyleFlags = EnumPlayStyleFlag.WildernessSurvival | EnumPlayStyleFlag.SurviveAndAutomate | EnumPlayStyleFlag.SurviveAndBuild);

        /// <summary>
        /// Called when a player joins
        /// </summary>
        /// <param name="handler"></param>
        void PlayerJoin(PlayerJoinServer handler);

        /// <summary>
        /// Called when a player intentionally leaves
        /// </summary>
        /// <param name="handler"></param>
        void PlayerLeave(PlayerLeave handler);

        /// <summary>
        /// Called whenever a player disconnects (timeout, leave, disconnect, kick, etc.). 
        /// </summary>
        /// <param name="handler"></param>
        void PlayerDisconnect(PlayerDisconnectServer handler);

        /// <summary>
        /// Called when a player wrote a chat message
        /// </summary>
        /// <param name="handler"></param>
        void PlayerChat(PlayerChat handler);

        /// <summary>
        /// Called when a player died
        /// </summary>
        /// <param name="handler"></param>
        void PlayerDeath(PlayerDeath handler);

        /// <summary>
        /// Whenever a player switched his game mode or has it switched for him
        /// </summary>
        /// <param name="handler"></param>
        void PlayerSwitchGameMode(PlayerChangeGameMode handler);

        /// <summary>
        /// Triggered after the game world data has been loaded. At this point all blocks are loaded and the Map size is known.
        /// </summary>
        /// <param name="handler"></param>
        void SaveGameLoaded(Action handler);

        /// <summary>
        /// Triggered before the game world data is being saved to disk 
        /// </summary>
        /// <param name="handler"></param>
        void GameWorldSave(Action handler);

        /// <summary>
        /// Triggered whenever the server enters a new run phase. Since mods are only loaded during run phase "LoadGamePre" registering to any earlier event will get triggered.
        /// </summary>
        /// <param name="runPhase"></param>
        /// <param name="handler"></param>
        void ServerRunPhase(EnumServerRunPhase runPhase, Action handler);

        /// <summary>
        /// Registers a method to be called every given interval
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="interval"></param>
        void Timer(Action handler, double interval);

        /// <summary>
        /// Registers the given method to be called every time a player presses a "SpecialKey" (currenly only the Respawn key is in use)
        /// </summary>
        /// <param name = "handler">Method to execute. Must have certain format: void Name(int player, SpecialKey key);</param>
        void SpecialKey(SpecialKey handler);

        /// <summary>
        /// Registers the given method to be called each time the player changes their current hotbar slot
        /// </summary>
        /// <param name = "handler">Method to execute. Must have certain format: void Name(int player)</param>
        void ChangedActiveHotbarSlot(SelectedHotbarslotChanged handler);

        /// <summary>
        /// Registers a method to be called every time a player places a block
        /// </summary>
        /// <param name = "handler"></param>
        void DidBuildBlock(BlockPlace handler);
        
        /// <summary>
        /// Registers a method to be called every time a player deletes a block
        /// </summary>
        /// <param name = "handler"></param>
        void DidBreakBlock(BlockBreak handler);

        /// <summary>
        /// Registers a method to be called every time a player uses a block
        /// </summary>
        /// <param name = "handler"></param>
        void DidUseBlock(BlockUse handler);

    }
}
