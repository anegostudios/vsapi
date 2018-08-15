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

        public void Begin()
        {
            if (!Enabled && !PrintSlowTicks) return;

            elems["begin"] = stopwatch.ElapsedTicks - start;
            start = stopwatch.ElapsedTicks;
            frameStart = start;
        }

        public void Mark(string elem)
        {
            if (!Enabled && !PrintSlowTicks) return;

            long ms = 0;
            elems.TryGetValue(elem, out ms);

            elems[elem] = ms + stopwatch.ElapsedTicks - start;
            start = stopwatch.ElapsedTicks;
        }

        public void Skip()
        {
            start = stopwatch.ElapsedTicks;
        }

        public void Set(string elem, long ellapsedTicks)
        {
            if (!Enabled && !PrintSlowTicks) return;

            elems[elem] = ellapsedTicks;
        }

        public void End()
        {
            if (!Enabled && !PrintSlowTicks) return;

            elems["end"] = stopwatch.ElapsedTicks - start;
            start = stopwatch.ElapsedTicks;

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
