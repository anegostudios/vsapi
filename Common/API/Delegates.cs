using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Return false to stop walking the inventory
    /// </summary>
    /// <param name="slot"></param>
    /// <returns></returns>
    public delegate bool OnInventorySlot(IItemSlot slot);

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

    public delegate bool ActionBoolReturn<T>(T t);

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
    /// <param name="byPlayer">The player that chatted.</param>
    /// <param name="channelId">The channel ID number</param>
    /// <param name="message">The message from the player.</param>
    /// <param name="consumed">Was the message consumed?</param>
    /// <returns>The resulting string.</returns>
    public delegate string PlayerChatDelegate(IServerPlayer byPlayer, int channelId, string message, BoolRef consumed);

    public delegate void PlayerDeathDelegate(IServerPlayer byPlayer, DamageSource damageSource);

    public delegate void DialogClickDelegate(IServerPlayer byPlayer, string widgetId);


    public delegate void SelectedHotbarSlotDelegate(IServerPlayer byPlayer);

    public delegate void UpdateEntityDelegate(int chunkx, int chunky, int chunkz, int id);

    public delegate void UseEntityDelegate(IServerPlayer byPlayer, int chunkx, int chunky, int chunkz, int id);

    public delegate void HitEntityDelegate(IServerPlayer byPlayer, int chunkx, int chunky, int chunkz, int id);

    public delegate void PlayerDelegate(IServerPlayer byPlayer);

    public delegate void PlayerChangeHotbarSlot(IServerPlayer byPlayer, ActiveHotbarSlotChangedEvent eventArgs);

    public delegate void EntityDelegate(Entity entity);

    public delegate void EntityDespawnDelegate(Entity entity, EntityDespawnReason reason);

    public delegate void OnInteractDelegate(Entity entity, IPlayer byPlayer, IItemSlot slot, Vec3d hitPosition, int mode, ref EnumHandling handling);

    public delegate void ChunkColumnGenerationDelegate(IServerChunk[] chunks, int chunkX, int chunkZ);

    public delegate void ChunkGenerationPassDelegate(IServerChunk chunk, int chunkX, int chunkY, int chunkZ);

    public delegate void MapChunkGeneratorDelegate(IMapChunk mapChunk, int chunkX, int chunkZ);

    public delegate void MapRegionGeneratorDelegate(IMapRegion mapRegion, int regionX, int regionZ);

    public delegate void WorldGenThreadDelegate(IChunkProviderThread chunkProvider);


    public delegate void BlockUseDelegate(IServerPlayer byPlayer, BlockSelection blockSel);
    public delegate void BlockBreakDelegate(IServerPlayer byPlayer, ushort oldblockId, BlockSelection blockSel);
    public delegate void BlockPlaceDelegate(IServerPlayer byPlayer, ushort oldblockId, BlockSelection blockSel, ItemStack withItemStack);
    
    public delegate bool CanUseDelegate(IServerPlayer byPlayer, BlockSelection blockSel);
    public delegate bool CanPlaceOrBreakDelegate(IServerPlayer byPlayer, BlockSelection blockSel);



    public delegate void LogEntryDelegate(EnumLogType logType, string message, params object[] args);
}