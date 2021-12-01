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
    public delegate bool OnInventorySlot(ItemSlot slot);

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
    /// Returns true if the action/event was successfull.
    /// </summary>
    /// <typeparam name="T">The additional type to pass in.</typeparam>
    /// <param name="t">The arguments for the event.</param>
    /// <returns></returns>
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



    public delegate TResult Func<TResult>();
    public delegate TResult Func<T1, TResult>(T1 t1);
    public delegate TResult Func<T1, T2, TResult>(T1 t1, T2 t2);
    public delegate TResult Func<T1, T2, T3, TResult>(T1 t1, T2 t2, T3 t3);
    public delegate TResult Func<T1, T2, T3, T4, TResult>(T1 t1, T2 t2, T3 t3, T4 t4);
    public delegate TResult Func<T1, T2, T3, T4, T5, T6, T7, TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);


    /// <summary>
    /// When the player wrote a chat message. Set consumed.value to true to prevent further processing of this chat message
    /// </summary>
    /// <param name="byPlayer">The player that submitted the chat message</param>
    /// <param name="channelId">The chat group id from where the message was sent from</param>
    /// <param name="message">The chat message</param>
    /// <param name="consumed">If set, the even is considered consumed, i.e. should no longer be handled further by the game engine</param>
    /// <param name="data"></param>
    /// <returns>The resulting string.</returns>
    public delegate void PlayerChatDelegate(IServerPlayer byPlayer, int channelId, ref string message, ref string data, BoolRef consumed);

    /// <summary>
    /// When the player died, this delegate will fire.
    /// </summary>
    /// <param name="byPlayer">The player that died.</param>
    /// <param name="damageSource">The source of the damage.</param>
    public delegate void PlayerDeathDelegate(IServerPlayer byPlayer, DamageSource damageSource);

    /// <summary>
    /// The delegate for a dialogue click.
    /// </summary>
    /// <param name="byPlayer">The player that clicked the dialogue.</param>
    /// <param name="widgetId">The internal name of the Widget.</param>
    public delegate void DialogClickDelegate(IServerPlayer byPlayer, string widgetId);


    public delegate void SelectedHotbarSlotDelegate(IServerPlayer byPlayer);

    public delegate void UpdateEntityDelegate(int chunkx, int chunky, int chunkz, int id);

    public delegate void UseEntityDelegate(IServerPlayer byPlayer, int chunkx, int chunky, int chunkz, int id);

    public delegate void HitEntityDelegate(IServerPlayer byPlayer, int chunkx, int chunky, int chunkz, int id);

    public delegate void PlayerDelegate(IServerPlayer byPlayer);

    public delegate void EntityDelegate(Entity entity);

    public delegate bool TrySpawnEntityDelegate(ref EntityProperties properties, Vec3d spawnPosition, long herdId);

    public delegate void EntityDespawnDelegate(Entity entity, EntityDespawnReason reason);

    public delegate void EntityDeathDelegate(Entity entity, DamageSource damageSource);

    public delegate void OnInteractDelegate(Entity entity, IPlayer byPlayer, ItemSlot slot, Vec3d hitPosition, int mode, ref EnumHandling handling);

    public delegate void ChunkColumnGenerationDelegate(IServerChunk[] chunks, int chunkX, int chunkZ, ITreeAttribute chunkGenParams = null);

    public delegate void ChunkGenerationPassDelegate(IServerChunk chunk, int chunkX, int chunkY, int chunkZ);

    public delegate void MapChunkGeneratorDelegate(IMapChunk mapChunk, int chunkX, int chunkZ);

    public delegate void MapRegionGeneratorDelegate(IMapRegion mapRegion, int regionX, int regionZ);

    public delegate void WorldGenThreadDelegate(IChunkProviderThread chunkProvider);


    public delegate void BlockUsedDelegate(IServerPlayer byPlayer, BlockSelection blockSel);
    public delegate void BlockBrokenDelegate(IServerPlayer byPlayer, int oldblockId, BlockSelection blockSel);
    public delegate void BlockPlacedDelegate(IServerPlayer byPlayer, int oldblockId, BlockSelection blockSel, ItemStack withItemStack);

    public delegate void BlockBreakDelegate(IServerPlayer byPlayer, BlockSelection blockSel, ref float dropQuantityMultiplier, ref EnumHandling handling);

    public delegate bool CanUseDelegate(IServerPlayer byPlayer, BlockSelection blockSel);
    public delegate bool CanPlaceOrBreakDelegate(IServerPlayer byPlayer, BlockSelection blockSel);



    public delegate void LogEntryDelegate(EnumLogType logType, string message, params object[] args);

    public delegate void OnGetClimateDelegate(ref ClimateCondition climate, BlockPos pos, EnumGetClimateMode mode = EnumGetClimateMode.WorldGenValues, double totalDays = 0);

    public delegate void OnGetWindSpeedDelegate(Vec3d pos, ref Vec3d windSpeed);

    public delegate EnumSuspendState SuspendServerDelegate();
    public delegate void ResumeServerDelegate();
}

public enum EnumSuspendState
{
    Wait,
    Ready
}