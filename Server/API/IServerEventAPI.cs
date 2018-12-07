using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
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
        void CanPlaceOrBreakBlock(CanPlaceOrBreakDelegate handler);

        /// <summary>
        /// Registers a handler to be called every time a player uses a block. The methods return value determines if the player may place/break this block.
        /// </summary>
        /// <param name = "handler">Function to register. Required parameters: (int player, int x, int y, int z)</param>
        void CanUseBlock(CanUseDelegate handler);


        /// <summary>
        /// If you require neighbour chunk data during world generation, you have to register to this event to receive access to the chunk generator thread. This method is only called once during server startup.
        /// </summary>
        /// <param name="f"></param>
        void GetWorldgenBlockAccessor(WorldGenThreadDelegate f);

        /// <summary>
        /// Event that is triggered whenever a new column of chunks is being generated. It is always called before the ChunkGenerator event
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="worldPlayStyleFlags">For which worlds to use this generator</param>
        void MapChunkGeneration(MapChunkGeneratorDelegate handler, EnumPlayStyleFlag worldPlayStyleFlags = EnumPlayStyleFlag.WildernessSurvival | EnumPlayStyleFlag.SurviveAndAutomate | EnumPlayStyleFlag.SurviveAndBuild);

        /// <summary>
        /// Event that is triggered whenever a new 8x8 section of column of chunks is being generated. It is always called before the ChunkGenerator and before the MapChunkGeneration event
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="worldPlayStyleFlags">For which worlds to use this generator</param>
        void MapRegionGeneration(MapRegionGeneratorDelegate handler, EnumPlayStyleFlag worldPlayStyleFlags = EnumPlayStyleFlag.WildernessSurvival | EnumPlayStyleFlag.SurviveAndAutomate | EnumPlayStyleFlag.SurviveAndBuild);

        /// <summary>
        /// Vintagestory uses this method to generate the basic terrain (base terrain + rock strata + caves) in full columns. Only called once in pass EnumWorldGenPass.TerrainNoise. Register to this event if you need acces to a whole chunk column during inital generation.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="pass"></param>
        /// <param name="worldPlayStyleFlags">For which worlds to use this generator</param>
        void ChunkColumnGeneration(ChunkColumnGenerationDelegate handler, EnumWorldGenPass pass, EnumPlayStyleFlag worldPlayStyleFlags = EnumPlayStyleFlag.WildernessSurvival | EnumPlayStyleFlag.SurviveAndAutomate | EnumPlayStyleFlag.SurviveAndBuild);


        /// <summary>
        /// Called when a player interacts with an entity
        /// </summary>
        event OnInteractDelegate OnPlayerInteractEntity;

        /// <summary>
        /// Called when a new player joins 
        /// </summary>
        event PlayerDelegate PlayerCreate;

        /// <summary>
        /// Called when a player got respawned
        /// </summary>
        event PlayerDelegate PlayerRespawn;

        /// <summary>
        /// Called when a player joins
        /// </summary>
        event PlayerDelegate PlayerJoin;

        /// <summary>
        /// Called when a player intentionally leaves
        /// </summary>
        event PlayerDelegate PlayerLeave;

        /// <summary>
        /// Called whenever a player disconnects (timeout, leave, disconnect, kick, etc.). 
        /// </summary>
        event PlayerDelegate PlayerDisconnect;

        /// <summary>
        /// Called when a player wrote a chat message
        /// </summary>
        event PlayerChatDelegate PlayerChat;

        /// <summary>
        /// Called when a player died
        /// </summary>
        event PlayerDeathDelegate PlayerDeath;

        /// <summary>
        /// Whenever a player switched his game mode or has it switched for him
        /// </summary>
        event PlayerDelegate PlayerSwitchGameMode;

        /// <summary>
        /// Fired when a player changes their active hotbar slot.
        /// </summary>
        event PlayerChangeHotbarSlot ActiveHotbarSlotChanged;

        /// <summary>
        /// Triggered after the game world data has been loaded. At this point all blocks are loaded and the Map size is known.
        /// </summary>
        event Action SaveGameLoaded;

        /// <summary>
        /// Triggered before the game world data is being saved to disk 
        /// </summary>
        event Action GameWorldSave;

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
        /// Registers a method to be called every time a player places a block
        /// </summary>
        event BlockPlaceDelegate DidPlaceBlock;
        
        /// <summary>
        /// Registers a method to be called every time a player deletes a block
        /// </summary>
        event BlockBreakDelegate DidBreakBlock;

        /// <summary>
        /// Registers a method to be called every time a player uses a block
        /// </summary>
        event BlockUseDelegate DidUseBlock;

    }
}
