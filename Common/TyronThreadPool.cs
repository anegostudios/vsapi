using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Threading;
using Vintagestory.API.Config;

namespace Vintagestory.API.Common
{
    public class TyronThreadPool
    {
        public static TyronThreadPool Inst = new TyronThreadPool();
        public ILogger Logger;

        public ConcurrentDictionary<int, string> RunningTasks = new ConcurrentDictionary<int, string>();
        public ConcurrentDictionary<string, Thread> DedicatedThreads = new ConcurrentDictionary<string, Thread>();
        int keyCounter = 0;
        int dedicatedCounter = 0;

        public TyronThreadPool()
        {
            // radfast note 13.3.25:  setting below the number of physical CPU cores has no effect, see https://learn.microsoft.com/en-us/dotnet/api/system.threading.threadpool.setmaxthreads?view=net-8.0
            ThreadPool.SetMaxThreads(10, 1);
        }

        private int MarkStarted(string caller)
        {
            int key = keyCounter++;
            RunningTasks[key] = caller;
            return key;
        }

        private void MarkEnded(int key)
        {
            RunningTasks.TryRemove(key, out string _);
        }

        public string ListAllRunningTasks()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string name in RunningTasks.Values)
            {
                if (sb.Length > 0) sb.Append(", ");
                sb.Append(name);
            }
            if (sb.Length == 0) sb.Append("[empty]");
            sb.AppendLine();

            return "Current threadpool tasks: " + sb.ToString() + "\nThread pool thread count: " + ThreadPool.ThreadCount;
        }

        public string ListAllThreads()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Server threads ("+DedicatedThreads.Count+"):");
            foreach (var entry in DedicatedThreads)
            {
                Thread t = entry.Value;
                if (t.ThreadState == System.Threading.ThreadState.Stopped) continue;
                sb.Append("tid" + t.ManagedThreadId + " ");
                sb.Append(entry.Key);
                sb.Append(": ");
                sb.AppendLine(t.ThreadState.ToString());
            }
            ProcessThreadCollection threadcoll = Process.GetCurrentProcess().Threads;
            List<ProcessThread> threads = new List<ProcessThread>();
            foreach (ProcessThread thread in threadcoll) if (thread != null) threads.Add(thread);
            threads = threads.OrderByDescending(t => t.UserProcessorTime.Ticks).ToList();

            sb.AppendLine("\nAll process threads ("+threads.Count+"):");
            foreach (ProcessThread thread in threads)
            {
                if (thread == null) continue;
                sb.Append(thread.ThreadState + " ");
                sb.Append("tid" + thread.Id + " ");

                if (RuntimeEnv.OS != OS.Mac)
                {
#pragma warning disable CA1416
                    sb.Append(thread.StartTime);
#pragma warning restore CA1416
                }
                sb.Append(": P ");
                sb.Append(thread.CurrentPriority);
                sb.Append(": ");
                sb.Append(thread.ThreadState.ToString());
                sb.Append(": T ");
                sb.Append(thread.UserProcessorTime.ToString());
                sb.Append(": T Total ");
                sb.AppendLine(thread.TotalProcessorTime.ToString());
            }

            return sb.ToString();
        }

        public static void QueueTask(Action callback, string caller)
        {
            int key = Inst.MarkStarted(caller);
            QueueTask(callback);
            Inst.MarkEnded(key);
        }

        public static void QueueLongDurationTask(Action callback, string caller)
        {
            int key = Inst.MarkStarted(caller);
            QueueLongDurationTask(callback);
            Inst.MarkEnded(key);
        }


        public static void QueueTask(Action callback)
        {
            if (RuntimeEnv.DebugThreadPool)
            {
                Inst.Logger.VerboseDebug("QueueTask." + Environment.StackTrace);
            }
            
            ThreadPool.QueueUserWorkItem((a) =>
            {
                callback();
            });
        }

        public static void QueueLongDurationTask(Action callback)
        {
            if (RuntimeEnv.DebugThreadPool)
            {
                Inst.Logger.VerboseDebug("QueueTask." + Environment.StackTrace);
            }

            ThreadPool.QueueUserWorkItem((a) =>
            {
                callback();
            });
        }

        public static Thread CreateDedicatedThread(ThreadStart starter, string name)
        {
            Thread thread = new Thread(starter);
            thread.IsBackground = true;
            thread.Name = name;
            Inst.DedicatedThreads[name + "." + Inst.dedicatedCounter++] = thread;
            return thread;
        }

        public void Dispose()
        {
            RunningTasks.Clear();
            DedicatedThreads.Clear();
            keyCounter = 0;
            dedicatedCounter = 0;
        }
    }
}
