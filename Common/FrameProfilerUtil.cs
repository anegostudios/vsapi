using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

#nullable disable

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
        public string OutputPrefix = "";

        public static ConcurrentQueue<string> offThreadProfiles = new ConcurrentQueue<string>();
        public static bool PrintSlowTicks_Offthreads;
        public static int PrintSlowTicksThreshold_Offthreads = 40;

        Stopwatch stopwatch = new Stopwatch();
        ProfileEntryRange rootEntry;
        ProfileEntryRange currentEntry;
        string beginText;
        Action<string> onLogoutputHandler;

        public FrameProfilerUtil(Action<string> onLogoutputHandler)
        {
            this.onLogoutputHandler = onLogoutputHandler;
            stopwatch.Start();
        }

        /// <summary>
        /// Used to create a FrameProfilerUtil on threads other than the main thread
        /// </summary>
        /// <param name="outputPrefix"></param>
        public FrameProfilerUtil(string outputPrefix) : this((text) => offThreadProfiles.Enqueue(text))
        {
            this.OutputPrefix = outputPrefix;
        }


        /// <summary>
        /// Called by the game engine for each render frame or server tick
        /// </summary>
        public void Begin(string beginText = null, params object[] args)
        {
            if (!Enabled && !PrintSlowTicks) return;

            this.beginText = beginText == null ? null : String.Format(beginText, args);
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

            if (currentEntry.ChildRanges == null) currentEntry.ChildRanges = new Dictionary<string, ProfileEntryRange>();
            if (!currentEntry.ChildRanges.TryGetValue(code, out ProfileEntryRange entry))
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
        /// Same as <see cref="Mark(string)"/> when <see cref="Enter(string)"/> was called before.
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
        /// Use this method like .Mark where the string has a second component
        /// (For performance, this avoids concatenating the strings unnecessarily, where the profiler is not enabled)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="param"></param>
        public void Mark(string code, object param)
        {
            if (!Enabled && !PrintSlowTicks) return;
            if (code == null) throw new ArgumentNullException("marker name may not be null!");

            MarkInternal(code + param);
        }

        /// <summary>
        /// Use this method to add a frame profiling marker, will set or add the time ellapsed since the previous mark to the frame profiling reults.
        /// </summary>
        /// <param name="code"></param>
        public void Mark(string code)
        {
            if (!Enabled && !PrintSlowTicks) return;
            if (code == null) throw new ArgumentNullException("marker name may not be null!");

            MarkInternal(code);
        }

        private void MarkInternal(string code)
        {
            try
            {
                var entry = currentEntry;
                if (entry == null) return;
                var marks = entry.Marks;
                if (marks == null) entry.Marks = marks = new Dictionary<string, ProfileEntry>();

                if (!marks.TryGetValue(code, out ProfileEntry ms))
                {
                    ms = new ProfileEntry();
                    marks[code] = ms;
                }

                long ticks = stopwatch.ElapsedTicks;

                ms.ElapsedTicks += (int)(ticks - entry.LastMark);
                ms.CallCount++;
                entry.LastMark = ticks;
            }
            catch (Exception)
            {
                // ignored
            }
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
                if (beginText != null) strib.Append(beginText).Append(' ');
                strib.AppendLine(string.Format("{0}A tick took {1:0.##} ms", OutputPrefix, ms));

                slowTicksToString(rootEntry, strib);

                summary = "Stopwatched total= " + ms + "ms";

                onLogoutputHandler(strib.ToString());
            }
        }

        public void OffThreadEnd()
        {
            End();
            Enabled = PrintSlowTicks = PrintSlowTicks_Offthreads;
            PrintSlowTicksThreshold = PrintSlowTicksThreshold_Offthreads;
        }


        void slowTicksToString(ProfileEntryRange entry, StringBuilder strib, double thresholdMs = 0.35, string indent = "")
        {
            try
            {
                double timeMS = (double)entry.ElapsedTicks / Stopwatch.Frequency * 1000;
                if (timeMS < thresholdMs) return;

                if (entry.CallCount > 1)
                {
                    strib.AppendLine(
                        indent + string.Format("{0:0.00}ms, {1:####} calls, avg {2:0.00} us/call: {3:0.00}",
                        timeMS, entry.CallCount, (timeMS * 1000 / Math.Max(entry.CallCount, 1)), entry.Code)
                    );
                }
                else
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
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
