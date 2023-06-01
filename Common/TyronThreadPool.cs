using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vintagestory.API.Config;

namespace Vintagestory.API.Common
{
    public class TyronThreadPool
    {
        /*public Thread[] threads;
        public ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();*/

        public static TyronThreadPool Inst = new TyronThreadPool();
        public ILogger Logger;

        public ConcurrentDictionary<int, string> RunningTasks = new ConcurrentDictionary<int, string>();
        int keyCounter = 0;


        public TyronThreadPool() { 
            /*threads = new Thread[quantityThreads];
            for (int i = 0; i < quantityThreads; i++)
            {
                ThreadStart ts = new ThreadStart(() =>
                {
                    while (true)
                    {
                        if (queue.Count > 0)
                        {
                            Action callback;
                            if (queue.TryDequeue(out callback))
                            {
                                callback();
                            }
                        }
                        Thread.Sleep(1);
                    }
                });

                threads[i] = new Thread(ts);
                threads[i].IsBackground = true;
                threads[i].Start();
            }*/
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

            return "Current threadpool tasks: " + sb.ToString();
        }

        public string ListAllThreads()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("All threads:");

            ProcessThreadCollection threads = Process.GetCurrentProcess().Threads;
            foreach (ProcessThread thread in threads)
            {
                if (thread.ThreadState == System.Diagnostics.ThreadState.Wait) continue;
                sb.Append(thread.StartTime);
                sb.Append(": P ");
                sb.Append(thread.CurrentPriority);
                sb.Append(": ");
                sb.AppendLine(thread.ThreadState.ToString());
                sb.Append(": T ");
                sb.Append(thread.UserProcessorTime);
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
            //Inst.queue.Enqueue(callback);
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

            //Inst.queue.Enqueue(callback);
            ThreadPool.QueueUserWorkItem((a) =>
            {
                callback();
            });
        }
    }
}
