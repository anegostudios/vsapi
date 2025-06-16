using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

#nullable disable

namespace Vintagestory.API.Server
{
    /// <summary>
    /// API for general Server features
    /// </summary>
    public interface IServerAPI
    {
        /// <summary>
        /// The ip adress the server is listening at
        /// </summary>
        string ServerIp { get; }

        /// <summary>
        /// All players known to the server (which joined at least once while the server was running)
        /// </summary>
        IServerPlayer[] Players { get; }

        /// <summary>
        /// The servers current configuration as configured in the serverconfig.json. You can set the values but you need to call MarkDirty() to have them saved
        /// </summary>
        IServerConfig Config { get; }

        /// <summary>
        /// Marks the config dirty for saving
        /// </summary>
        void MarkConfigDirty();


        /// <summary>
        /// Returns the servers current run phase
        /// </summary>
        /// <value></value>
        EnumServerRunPhase CurrentRunPhase { get; }

        /// <summary>
        /// Returns whether the current server a dedicated server
        /// </summary>
        /// <value></value>
        bool IsDedicated { get; }


        /// <summary>
        /// Determines if the server process has been asked to terminate.
        /// Use this when you need to save data in a method registered using RegisterOnSave() before server quits.
        /// </summary>
        /// <value><i>true</i>
        ///   if server is about to shutdown</value>
        bool IsShuttingDown { get; }

        /// <summary>
        /// Gracefully shuts down the server
        /// </summary>
        /// <returns></returns>
        void ShutDown();

        /// <summary>
        /// Allows mods to add a ServerThread.  Useful for off-thread tasks which must be run continuously (at specified intervals) while the server is running.  Calling code simply needs to implement IAsyncServerSystem
        /// </summary>
        /// <param name="threadname"></param>
        /// <param name="system"></param>
        void AddServerThread(string threadname, IAsyncServerSystem system);

        /// <summary>
        /// Does a blocking wait until given thread is paused. Returns true if the thread was paused within given time
        /// </summary>
        /// <param name="threadname"></param>
        /// <param name="waitTimeoutMs"></param>
        /// <returns></returns>
        bool PauseThread(string threadname, int waitTimeoutMs = 5000);

        /// <summary>
        /// Resumes a previously paused thread
        /// </summary>
        /// <param name="threadname"></param>
        void ResumeThread(string threadname);

        /// <summary>
        /// If true, code should generally aim to reduce the number of CPU threads used on the server, even at the cost of slight performance delays
        /// </summary>
        bool ReducedServerThreads { get; }



        long TotalReceivedBytes { get; }
        long TotalSentBytes { get; }

        /// <summary>
        /// Returns the number of seconds the server has been running since last restart
        /// </summary>
        /// <value>Server uptime in seconds</value>
        int ServerUptimeSeconds { get; }

        /// <summary>
        /// Server uptime in milliseconds
        /// </summary>
        /// <value></value>
        long ServerUptimeMilliseconds { get; }

        /// <summary>
        /// Returns the number of seconds the current world has been running. This is the playtime displayed on the singleplayer world list.
        /// </summary>
        int TotalWorldPlayTime { get; }


        /// <summary>
        /// Returns a logging interface to log any log level message
        /// </summary>
        /// <returns></returns>
        ILogger Logger { get; }


        /// <summary>
        /// Log given message with type = EnumLogType.Chat
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void LogChat(string message, params object[] args);


        /// <summary>
        /// Log given message with type = EnumLogType.Build
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void LogBuild(string message, params object[] args);

        /// <summary>
        /// Log given message with type = EnumLogType.VerboseDebug
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void LogVerboseDebug(string message, params object[] args);

        /// <summary>
        /// Log given message with type = EnumLogType.Debug
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void LogDebug(string message, params object[] args);

        /// <summary>
        /// Log given message with type = EnumLogType.Notification
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void LogNotification(string message, params object[] args);

        /// <summary>
        /// Log given message with type = EnumLogType.Warning
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void LogWarning(string message, params object[] args);

        /// <summary>
        /// Log given message with type = EnumLogType.Error
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void LogError(string message, params object[] args);

        /// <summary>
        /// Log given message with type = EnumLogType.Fatal
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void LogFatal(string message, params object[] args);

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void LogEvent(string message, params object[] args);

        /// <summary>
        /// Add the specified dimension to the LoadedMiniDimensions, and return its allocated subdimension index.
        /// <br/>A mini dimension is a small set of blocks up to 4096x4096x4096 used for schematic previews, vehicles etc
        /// </summary>
        int LoadMiniDimension(IMiniDimension blocks);
        /// <summary>
        /// Set the specified mini-dimension at the specified subdimension index, and return its index
        /// <br/>A mini dimension is a small set of blocks up to 4096x4096x4096 used for schematic previews, vehicles etc
        /// </summary>
        int SetMiniDimension(IMiniDimension miniDimension, int subId);

        /// <summary>
        /// Get the mini-dimension at the specified subdimension index; returns null if none exists
        /// <br/>A mini dimension is a small set of blocks up to 4096x4096x4096 used for schematic previews, vehicles etc
        /// </summary>
        IMiniDimension GetMiniDimension(int subId);

        /// <summary>
        /// Remove an entity from the physics ticking system on the server.
        /// </summary>
        /// <param name="entityBehavior"></param>
        public void AddPhysicsTickable(IPhysicsTickable entityBehavior);

        /// <summary>
        /// Add an entity to the physics ticking system on the server.
        /// </summary>
        /// <param name="entityBehavior"></param>
        public void RemovePhysicsTickable(IPhysicsTickable entityBehavior);
    }
}
