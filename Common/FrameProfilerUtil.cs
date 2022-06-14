using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Vintagestory.API.Common;

namespace Vintagestory.API.Common
{
    public class ProfileEntry
    {
        public int ElapsedTicks;
        public int CallCount;

        public ProfileEntry()
        { }

        public ProfileEntry(int elaTicks, int callCount)
        {
            this.ElapsedTicks = elaTicks;
            this.CallCount = callCount;
        }
    }

    public class ProfileEntryRange
    {
        public string Code;
        public long Start;
        public long LastMark;
        public int CallCount = 1;

        public long ElapsedTicks;

        public Dictionary<string, ProfileEntry> Marks = null;

        public Dictionary<string, ProfileEntryRange> ChildRanges;
        public ProfileEntryRange ParentRange;
    }

    public class FrameProfilerUtil
    {
        public bool Enabled = false;

        public bool PrintSlowTicks;
        public int PrintSlowTicksThreshold = 40;
        public ProfileEntryRange PrevRootEntry;
        public string summary;


        Stopwatch stopwatch = new Stopwatch();
        ILogger logger;


        ProfileEntryRange rootEntry;
        ProfileEntryRange currentEntry;

        public FrameProfilerUtil(ILogger logger)
        {
            this.logger = logger;

            stopwatch.Start();
        }


        /// <summary>
        /// Called by the game engine for each render frame or server tick
        /// </summary>
        public void Begin()
        {
            if (!Enabled && !PrintSlowTicks) return;

            currentEntry = null;
            rootEntry = Enter("all");
        }


        public ProfileEntryRange Enter(string code)
        {
            if (!Enabled && !PrintSlowTicks) return null;

            long elapsedTicks = stopwatch.ElapsedTicks;
            if (currentEntry == null) return currentEntry = new ProfileEntryRange()
            {
                Code = code,
                Start = elapsedTicks,
                LastMark = elapsedTicks,
                CallCount = 0
            };

            ProfileEntryRange entry;
            if (currentEntry.ChildRanges == null) currentEntry.ChildRanges = new Dictionary<string, ProfileEntryRange>();
            if (!currentEntry.ChildRanges.TryGetValue(code, out entry))
            {
                currentEntry.ChildRanges[code] = entry = new ProfileEntryRange()
                {
                    Code = code,
                    Start = elapsedTicks,
                    LastMark = elapsedTicks,
                    CallCount = 0
                };

                entry.ParentRange = currentEntry;
            } else
            {
                entry.Start = elapsedTicks;
                entry.LastMark = elapsedTicks;
            }

            currentEntry = entry;
            entry.CallCount++;
            return entry;
        }

        /// <summary>
        /// Same as <see cref="Mark(string, long)"/> when <see cref="Enter(string)"/> was called before.
        /// </summary>
        public void Leave()
        {
            if (!Enabled && !PrintSlowTicks) return;

            long elapsedTicks = stopwatch.ElapsedTicks;
            currentEntry.ElapsedTicks += elapsedTicks - currentEntry.Start;
            currentEntry.LastMark = elapsedTicks;

            currentEntry = currentEntry.ParentRange;

            // Set the LastMark in all parents
            ProfileEntryRange parent = currentEntry;
            while (parent != null)
            {
                parent.LastMark = elapsedTicks;
                parent = parent.ParentRange;
            }
        }

        /// <summary>
        /// Use this method to add a frame profiling marker, will set or add the time ellapsed since the previous mark to the frame profiling reults.
        /// </summary>
        /// <param name="code"></param>
        public void Mark(string code)
        {
            if (!Enabled && !PrintSlowTicks || currentEntry == null) return;
            if (code == null) throw new ArgumentNullException("marker name may not be null!");

            var entry = currentEntry;

            if (entry.Marks == null) entry.Marks = new Dictionary<string, ProfileEntry>();

            ProfileEntry ms;
            if (!entry.Marks.TryGetValue(code, out ms))
            {
                ms = new ProfileEntry();
                entry.Marks[code] = ms;
            }

            long ticks = stopwatch.ElapsedTicks;

            ms.ElapsedTicks += (int)(ticks - entry.LastMark);
            ms.CallCount++;
            entry.LastMark = ticks;
        }

        /// <summary>
        /// Called by the game engine at the end of the render frame or server tick
        /// </summary>
        public void End()
        {
            if (!Enabled && !PrintSlowTicks) return;

            Mark("end");
            Leave();

            PrevRootEntry = rootEntry;

            double ms = (double)rootEntry.ElapsedTicks / Stopwatch.Frequency * 1000;
            if (PrintSlowTicks && ms > PrintSlowTicksThreshold)
            {
                StringBuilder strib = new StringBuilder();
                strib.AppendLine(string.Format("A tick took {0:0.##} ms", ms));

                slowTicksToString(rootEntry, strib);

                summary = "Stopwatched total= " + ms + "ms";

                logger.Notification(strib.ToString());
            }
        }


        void slowTicksToString(ProfileEntryRange entry, StringBuilder strib, double thresholdMs = 0.35, string indent = "")
        {
            double timeMS = (double)entry.ElapsedTicks / Stopwatch.Frequency * 1000;
            if (timeMS < thresholdMs) return;

            if (entry.CallCount > 1)
            {
                strib.AppendLine(
                    indent + string.Format("{0:0.00}ms, {1:####} calls, avg {2:0.00} us/call: {3:0.00}",
                    timeMS, entry.CallCount, (timeMS * 1000 / Math.Max(entry.CallCount, 1)), entry.Code)
                );
            } else
            {
                strib.AppendLine(
                    indent + string.Format("{0:0.00}ms, {1:####} call : {2}",
                    timeMS, entry.CallCount, entry.Code)
                );
            }

            List<ProfileEntryRange> profiles = new List<ProfileEntryRange>();

            if (entry.Marks != null) profiles.AddRange(entry.Marks.Select(e => new ProfileEntryRange() { ElapsedTicks = e.Value.ElapsedTicks, Code = e.Key, CallCount = e.Value.CallCount }));
            if (entry.ChildRanges != null) profiles.AddRange(entry.ChildRanges.Values);

            var profsordered = profiles.OrderByDescending((prof) => prof.ElapsedTicks);

            int i = 0;
            foreach (var prof in profsordered)
            {
                if (i++ > 8) return;
                slowTicksToString(prof, strib, thresholdMs, indent + "  ");
            }
        }
    }
}
