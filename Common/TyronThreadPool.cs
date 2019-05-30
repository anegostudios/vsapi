using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
