using Vintagestory.API.Client;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Return true if the action/event should be "consumed" (e.g. mark a mouse click as handled)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t1"></param>
    /// <returns></returns>
    public delegate bool ActionConsumable<T>(T t1);

    /// <summary>
    /// Return true if the action/event should be "consumed" (e.g. mark a mouse click as handled)
    /// </summary>
    /// <returns></returns>
    public delegate bool ActionConsumable();

    /// <summary>
    /// Return true if the action/event was successfull
    /// </summary>
    /// <returns></returns>
    public delegate bool ActionBoolReturn();

    /// <summary>
    /// Return true if the action/event should be "consumed" (e.g. mark a mouse click as handled)
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <returns></returns>
    public delegate bool ActionConsumable<T1, T2>(T1 t1, T2 t2);

    /// <summary>
    /// A parameterless method
    /// </summary>
    public delegate void Action();
    
    public delegate void Action<T>(T t1);
    public delegate void Action<T1, T2>(T1 t1, T2 t2);
    public delegate void Action<T1, T2, T3>(T1 t1, T2 t2, T3 t3);
    public delegate void Action<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4);
    public delegate void Action<T1, T2, T3, T4, T5>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5);
    public delegate void Action<T1, T2, T3, T4, T5, T6>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
    public delegate void Action<T1, T2, T3, T4, T5, T6, T7>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);

    public delegate TResult Func<TResult>();
    public delegate TResult Func<T1, TResult>(T1 t1);
    public delegate TResult Func<T1, T2, TResult>(T1 t1, T2 t2);
    public delegate TResult Func<T1, T2, T3, TResult>(T1 t1, T2 t2, T3 t3);
    public delegate TResult Func<T1, T2, T3, T4, TResult>(T1 t1, T2 t2, T3 t3, T4 t4);
    public delegate TResult Func<T1, T2, T3, T4, T5, T6, T7, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);


    /// <summary>
    /// When the player wrote a chat message. Set consumed.value to true to prevent further processing of this chat message
    /// </summary>
    /// <param name="byPlayer"></param>
    /// <param name="channelId"></param>
    /// <param name="message"></param>
    /// <param name="consumed"></param>
    /// <returns></returns>
    public delegate string PlayerChat(IServerPlayer byPlayer, int channelId, string message, BoolRef consumed);

    public delegate void PlayerDeath(IServerPlayer byPlayer, DamageSource damageSource);

    public delegate void PlayerChangeGameMode(IServerPlayer player);

    public delegate void DialogClick(IServerPlayer byPlayer, string widgetId);

    public delegate void SpecialKey(IServerPlayer byPlayer, EnumSpecialKey key);

    public delegate void SelectedHotbarslotChanged(IServerPlayer byPlayer);

    public delegate void UpdateEntity(int chunkx, int chunky, int chunkz, int id);

    public delegate void UseEntity(IServerPlayer byPlayer, int chunkx, int chunky, int chunkz, int id);

    public delegate void HitEntity(IServerPlayer byPlayer, int chunkx, int chunky, int chunkz, int id);

    public delegate void PlayerJoinServer(IServerPlayer byPlayer);
    public delegate void PlayerDisconnectServer(IServerPlayer byPlayer);
    public delegate void PlayerLeave(IServerPlayer byPlayer);



    public delegate void ChunkColumnGeneration(IServerChunk[] chunks, int chunkX, int chunkZ);

    public delegate void ChunkGenerationPass(IServerChunk chunk, int chunkX, int chunkY, int chunkZ);

    public delegate void MapChunkGenerator(IMapChunk mapChunk, int chunkX, int chunkZ);

    public delegate void MapRegionGenerator(IMapRegion mapRegion, int regionX, int regionZ);

    public delegate void WorldGenThread(IChunkProviderThread chunkProvider);

    //public delegate void BlockUpdate(int x, int y, int z);

    public delegate void BlockUse(IServerPlayer byPlayer, BlockSelection blockSel);
    public delegate void BlockBreak(IServerPlayer byPlayer, ushort oldblockId, BlockSelection blockSel);
    public delegate void BlockPlace(IServerPlayer byPlayer, ushort oldblockId, BlockSelection blockSel, ItemStack withItemStack);
    
    public delegate bool CanUse(IServerPlayer byPlayer, BlockSelection blockSel);
    public delegate bool CanPlaceOrBreak(IServerPlayer byPlayer, BlockSelection blockSel);



    public delegate void LogEntryDelegate(EnumLogType logType, string message, params object[] args);
}