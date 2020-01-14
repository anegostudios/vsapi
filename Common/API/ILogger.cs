using System;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Interface to the clients and servers event/debug/error logging utilty
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// If true, will also print to Diagnostics.Debug
        /// </summary>
        bool TraceLog { get; set; }

        /// <summary>
        /// Fired every time a log entry has been added
        /// </summary>
        event LogEntryDelegate EntryAdded;

        /// <summary>
        /// Removes any handler that registered to the EntryAdded event. This method is called when the client leaves a world or server shuts down.
        /// </summary>
        void ClearWatchers();

        /// <summary>
        /// Add a log entry
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void Log(EnumLogType logType, string message, params object[] args);

        /// <summary>
        /// Adds a build log entry
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void Build(string message, params object[] args);

        /// <summary>
        /// Adds a chat log entry
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void Chat(string message, params object[] args);

        /// <summary>
        /// Adds a verbose debug log entry (these are only logged to file and not sent to console)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void VerboseDebug(string message, params object[] args);

        /// <summary>
        /// Adds a debug log entry
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void Debug(string message, params object[] args);

        /// <summary>
        /// Adds a notification log entry
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void Notification(string message, params object[] args);

        /// <summary>
        /// Adds a warning log entry
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void Warning(string message, params object[] args);

        /// <summary>
        /// Adds a error log entry
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void Error(string message, params object[] args);

        /// <summary>
        /// Adds a fatal error log entry
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void Fatal(string message, params object[] args);

        /// <summary>
        /// Adds an event log entry. These are showing to the player when he's starting a singpleplayer server
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void Event(string message, params object[] args);

        /// <summary>
        /// Adds an story event log entry. These are showing to the player when he's starting a singpleplayer server
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void StoryEvent(string message, params object[] args);
    }
}
