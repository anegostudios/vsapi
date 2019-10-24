using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// For handling events on the event bus
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="handling">Set to EnumHandling.Last to stop further propagation of the event</param>
    /// <param name="data"></param>
    public delegate void EventBusListenerDelegate(string eventName, ref EnumHandling handling, IAttribute data);

    public enum EnumChunkDirtyReason
    {
        NewlyCreated,
        NewlyLoaded,
        MarkedDirty
    }

    /// <summary>
    /// For handling dirty chunks
    /// </summary>
    /// <param name="chunkCoord"></param>
    /// <param name="chunk"></param>
    /// <param name="reason"></param>
    public delegate void ChunkDirtyDelegate(Vec3i chunkCoord, IWorldChunk chunk, EnumChunkDirtyReason reason);


    /// <summary>
    /// Events that are available on the server and the client
    /// </summary>
    public interface IEventAPI
    {
        /// <summary>
        /// Triggered when a new entity spawned
        /// </summary>
        event EntityDelegate OnEntitySpawn;

        /// <summary>
        /// Triggered when a new entity spawned
        /// </summary>
        event EntityDespawnDelegate OnEntityDespawn;

        /// <summary>
        /// Called whenever a chunk was marked dirty (as in, its blocks or light values have been modified or it got newly loaded or newly created)
        /// </summary>
        event ChunkDirtyDelegate ChunkDirty;

        /// <summary>
        /// There's 2 global event busses, 1 on the client and 1 on the server. This pushes an event onto the bus.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        void PushEvent(string eventName, IAttribute data = null);


        /// <summary>
        /// Registers a listener on the event bus. This is intended for mods as the game engine itself does not push any events.
        /// </summary>
        /// <param name="OnEvent">The handler for the events</param>
        /// <param name="priority">Set this to a different value if you want to catch an event before/after another mod catches it</param>
        /// <param name="filterByEventName">If set, events only with given eventName are received</param>
        void RegisterEventBusListener(EventBusListenerDelegate OnEvent, double priority = 0.5, string filterByEventName = null);


        /// <summary>
        /// Calls given method after every given interval until unregistered. The engine may call your method slightly later since these event are handled only during fixed interval game ticks.
        /// </summary>
        /// <param name="OnGameTick"></param>
        /// <param name="millisecondInterval"></param>
        /// <returns>listenerId</returns>
        long RegisterGameTickListener(Action<float> OnGameTick, int millisecondInterval);


        /// <summary>
        /// Calls given method after every given interval until unregistered. The engine may call your method slightly later since these event are handled only during fixed interval game ticks.
        /// </summary>
        /// <param name="OnGameTick"></param>
        /// <param name="pos"></param>
        /// <param name="millisecondInterval"></param>
        /// <returns>listenerId</returns>
        long RegisterGameTickListener(Action<IWorldAccessor, BlockPos, float> OnGameTick, BlockPos pos, int millisecondInterval);




        /// <summary>
        /// Calls given method after supplied amount of milliseconds. The engine may call your method slightly later since these event are handled only during fixed interval game ticks.
        /// </summary>
        /// <param name="OnTimePassed"></param>
        /// <param name="millisecondDelay"></param>
        /// <returns>listenerId</returns>
        long RegisterCallback(Action<float> OnTimePassed, int millisecondDelay);

        /// <summary>
        /// Calls given method after supplied amount of milliseconds, lets you supply a block position to be passed to the method. The engine may call your method slightly later since these event are handled only during fixed interval game ticks.
        /// </summary>
        /// <param name="OnTimePassed"></param>
        /// <param name="pos"></param>
        /// <param name="millisecondDelay"></param>
        /// <returns>listenerId</returns>
        long RegisterCallback(Action<IWorldAccessor, BlockPos, float> OnTimePassed, BlockPos pos, int millisecondDelay);


        /// <summary>
        /// Removes a delayed callback
        /// </summary>
        /// <param name="listenerId"></param>
        void UnregisterCallback(long listenerId);


        /// <summary>
        /// Removes a game tick listener
        /// </summary>
        /// <param name="listenerId"></param>
        void UnregisterGameTickListener(long listenerId);


        /// <summary>
        /// Can be used to execute supplied method a frame later or can be called from a seperate thread to ensure some code is executed in the main thread
        /// </summary>
        /// <param name="action"></param>
        /// <param name="code">Task category identifier for the frame profiler</param>
        void EnqueueMainThreadTask(API.Common.Action action, string code);

    }
}
