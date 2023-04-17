using System;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Interface to the client's and server's event, debug and error logging utilty.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// If true, will also print to Diagnostics.Debug.
        /// </summary>
        bool TraceLog { get; set; }

        /// <summary>
        /// Fired each time a new log entry has been added.
        /// </summary>
        event LogEntryDelegate EntryAdded;

        /// <summary>
        /// Removes any handler that registered to the EntryAdded event.
        /// This method is called when the client leaves a world or server shuts down.
        /// </summary>
        void ClearWatchers();

        /// <summary>
        /// Adds a new log entry with the specified log type, format string and arguments.
        /// </summary>
        void Log(EnumLogType logType, string format, params object[] args);
        /// <summary>
        /// Adds a new log entry with the specified log type and message.
        /// </summary>
        void Log(EnumLogType logType, string message);

        /// <summary>
        /// Adds a new <see cref="EnumLogType.Chat"/> log entry with the specified format string and arguments.
        /// </summary>
        void Chat(string format, params object[] args);

        /// <summary>
        /// Adds a new <see cref="EnumLogType.Chat"/> log entry with the specified message.
        /// </summary>
        void Chat(string message);

        /// <summary>
        /// Adds a new <see cref="EnumLogType.Event"/> log entry with the specified format string and arguments.
        /// </summary>
        void Event(string format, params object[] args);
        /// <summary>
        /// Adds a new <see cref="EnumLogType.Event"/> log entry with the specified message.
        /// </summary>
        void Event(string message);

        /// <summary>
        /// Adds a new <see cref="EnumLogType.StoryEvent"/> log entry with the specified format string and arguments.
        /// </summary>
        void StoryEvent(string format, params object[] args);
        /// <summary>
        /// Adds a new <see cref="EnumLogType.StoryEvent"/> log entry with the specified message.
        /// </summary>
        void StoryEvent(string message);

        /// <summary>
        /// Adds a new <see cref="EnumLogType.Build"/> log entry with the specified format string and arguments.
        /// </summary>
        void Build(string format, params object[] args);
        /// <summary>
        /// Adds a new <see cref="EnumLogType.Build"/> log entry with the specified message.
        /// </summary>
        void Build(string message);

        /// <summary>
        /// Adds a new <see cref="EnumLogType.VerboseDebug"/> log entry with the specified format string and arguments.
        /// </summary>
        void VerboseDebug(string format, params object[] args);
        /// <summary>
        /// Adds a new <see cref="EnumLogType.VerboseDebug"/> log entry with the specified message.
        /// </summary>
        void VerboseDebug(string message);

        /// <summary>
        /// Adds a new <see cref="EnumLogType.Debug"/> log entry with the specified format string and arguments.
        /// </summary>
        void Debug(string format, params object[] args);
        /// <summary>
        /// Adds a new <see cref="EnumLogType.Debug"/> log entry with the specified message.
        /// </summary>
        void Debug(string message);

        /// <summary>
        /// Adds a new <see cref="EnumLogType.Notification"/> log entry with the specified format string and arguments.
        /// </summary>
        void Notification(string format, params object[] args);
        /// <summary>
        /// Adds a new <see cref="EnumLogType.Notification"/> log entry with the specified message.
        /// </summary>
        void Notification(string message);

        /// <summary>
        /// Adds a new <see cref="EnumLogType.Warning"/> log entry with the specified format string and arguments.
        /// </summary>
        void Warning(string format, params object[] args);
        /// <summary>
        /// Adds a new <see cref="EnumLogType.Warning"/> log entry with the specified message.
        /// </summary>
        void Warning(string message);

        /// <summary>
        /// Adds a new <see cref="EnumLogType.Error"/> log entry with the specified format string and arguments.
        /// </summary>
        void Error(string format, params object[] args);
        /// <summary>
        /// Adds a new <see cref="EnumLogType.Error"/> log entry with the specified message.
        /// </summary>
        void Error(string message);

        /// <summary>
        /// Adds a new <see cref="EnumLogType.Fatal"/> log entry with the specified format string and arguments.
        /// </summary>
        void Fatal(string format, params object[] args);
        /// <summary>
        /// Adds a new <see cref="EnumLogType.Fatal"/> log entry with the specified message.
        /// </summary>
        void Fatal(string message);

        /// <summary>
        /// Adds a new <see cref="EnumLogType.Audit"/> log entry with the specified format string and arguments.
        /// </summary>
        void Audit(string format, params object[] args);
        /// <summary>
        /// Adds a new <see cref="EnumLogType.Audit"/> log entry with the specified message.
        /// </summary>
        void Audit(string message);

    }

    /// <summary>
    /// Base implementation for <see cref="ILogger"/> which implements all
    /// methods besides a new abstract method <see cref="LoggerBase.LogImpl"/>.
    /// </summary>
    public abstract class LoggerBase : ILogger
    {
        // Reusable empty argument array to avoid unnecessary allocation.
        private static readonly object[] _emptyArgs = new object[0];

        public bool TraceLog { get; set; }

        public event LogEntryDelegate EntryAdded;
        public void ClearWatchers() => EntryAdded = null;

        /// <summary>
        /// This is the only method necessary to be overridden by the
        /// implementing class, actually does the logging as necessary.
        /// </summary>
        protected abstract void LogImpl(EnumLogType logType, string format, params object[] args);

        public void Log(EnumLogType logType, string format, params object[] args)
        {
            LogImpl(logType, format, args);
            EntryAdded?.Invoke(logType, format, args);
        }
        public void Log(EnumLogType logType, string message)
            => Log(logType, message, _emptyArgs);

        public void Chat(string format, params object[] args)
            => Log(EnumLogType.Chat, format, args);
        public void Chat(string message)
            => Log(EnumLogType.Chat, message, _emptyArgs);

        public void Event(string format, params object[] args)
            => Log(EnumLogType.Event, format, args);
        public void Event(string message)
            => Log(EnumLogType.Event, message, _emptyArgs);

        public void StoryEvent(string format, params object[] args)
            => Log(EnumLogType.StoryEvent, format, args);
        public void StoryEvent(string message)
            => Log(EnumLogType.StoryEvent, message, _emptyArgs);

        public void Build(string format, params object[] args)
            => Log(EnumLogType.Build, format, args);
        public void Build(string message)
            => Log(EnumLogType.Build, message, _emptyArgs);

        public void VerboseDebug(string format, params object[] args)
            => Log(EnumLogType.VerboseDebug, format, args);
        public void VerboseDebug(string message)
            => Log(EnumLogType.VerboseDebug, message, _emptyArgs);

        public void Debug(string format, params object[] args)
            => Log(EnumLogType.Debug, format, args);
        public void Debug(string message)
            => Log(EnumLogType.Debug, message, _emptyArgs);

        public void Notification(string format, params object[] args)
            => Log(EnumLogType.Notification, format, args);
        public void Notification(string message)
            => Log(EnumLogType.Notification, message, _emptyArgs);

        public void Warning(string format, params object[] args)
            => Log(EnumLogType.Warning, format, args);
        public void Warning(string message)
            => Log(EnumLogType.Warning, message, _emptyArgs);

        public void Error(string format, params object[] args)
            => Log(EnumLogType.Error, format, args);
        public void Error(string message)
            => Log(EnumLogType.Error, message, _emptyArgs);

        public void Fatal(string format, params object[] args)
            => Log(EnumLogType.Fatal, format, args);
        public void Fatal(string message)
            => Log(EnumLogType.Fatal, message, _emptyArgs);

        public void Audit(string format, params object[] args)
            => Log(EnumLogType.Audit, format, args);
        public void Audit(string message)
            => Log(EnumLogType.Audit, message, _emptyArgs);

        public void Worldgen(string format, params object[] args)
            => Log(EnumLogType.Worldgen, format, args);
        public void Worldgen(string message)
            => Log(EnumLogType.Worldgen, message, _emptyArgs);
    }
}
