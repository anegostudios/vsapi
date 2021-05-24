using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Vintagestory.API.Common;

namespace Vintagestory.API.Common
{
    public class FrameProfilerUtil
    {
        public bool Enabled = false;

        public bool PrintSlowTicks;
        public int PrintSlowTicksThreshold = 40;
        public Dictionary<string, long> elemsPrevFrame = new Dictionary<string, long>();
        public string summary;

        long start;
        long frameStart;
        Dictionary<string, long> elems = new Dictionary<string, long>();
        Stopwatch stopwatch = new Stopwatch();
        ILogger logger;


        public FrameProfilerUtil(ILogger logger)
        {
            this.logger = logger;

            stopwatch.Start();
        }

        public double EllapsedMs {
            get
            {
                return ((double)(stopwatch.ElapsedTicks - frameStart)/ Stopwatch.Frequency) * 1000;
            }
        }

        /// <summary>
        /// Called by the game engine for each render frame or server tick
        /// </summary>
        public void Begin()
        {
            if (!Enabled && !PrintSlowTicks) return;

            long ticks = stopwatch.ElapsedTicks;
            elems["begin"] = ticks - start;
            start = ticks;
            frameStart = start;
        }

        /// <summary>
        /// Use this method to add a frame profiling marker, will set or add the time ellapsed since the previous mark to the frame profiling reults.
        /// </summary>
        /// <param name="code"></param>
        public void Mark(string code)
        {
            if (!Enabled && !PrintSlowTicks) return;
            if (code == null) throw new ArgumentNullException("marker name may not be null!");

            long ms = 0;
            elems.TryGetValue(code, out ms);

            long ticks = stopwatch.ElapsedTicks;
            elems[code] = ms + ticks - start;
            start = ticks;
        }

        /// <summary>
        /// Same as Mark(), but without actually taking note on where the time was spent
        /// </summary>
        public void Skip()
        {
            start = stopwatch.ElapsedTicks;
        }

        public void Set(string elem, long ellapsedTicks)
        {
            if (!Enabled && !PrintSlowTicks) return;

            elems[elem] = ellapsedTicks;
        }

        /// <summary>
        /// Called by the game engine at the end of the render frame or server tick
        /// </summary>
        public void End()
        {
            if (!Enabled && !PrintSlowTicks) return;

            long ticks = stopwatch.ElapsedTicks;
            elems["end"] = ticks - start;
            start = ticks;

            elemsPrevFrame = elems;

            if (PrintSlowTicks)
            {

                long total = 0;
                foreach (var val in elems)
                {
                    total += val.Value;
                }

                double ms = ((double)total / Stopwatch.Frequency) * 1000;

                if (ms > PrintSlowTicksThreshold)
                {
                    StringBuilder strib = new StringBuilder();
                    strib.Append("A tick took " + ms.ToString("#.##") + "ms\r\n");
                    var myList = elems.ToList();
                    myList.Sort((x, y) => y.Value.CompareTo(x.Value));
                    for (int i = 0; i < Math.Min(myList.Count, 8); i++)
                    {
                        var val = myList[i];
                        
                        strib.Append(val.Key + ": " + (((double)val.Value / Stopwatch.Frequency) * 1000).ToString("#.##") + "ms\r\n");
                    }
                    
                    logger.Notification(strib.ToString());
                }

                summary = "Stopwatched total= " + total + "ms";
            }

            elems = new Dictionary<string, long>();

        }
    }
}
