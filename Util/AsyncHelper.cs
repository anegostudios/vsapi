using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Vintagestory.API.Common;

namespace Vintagestory.API.Util
{
    /// <summary>
    /// A class to provide general helper functions for multi-threaded (asynchronous) operations
    /// </summary>
    public class AsyncHelper
    {
        /// <summary>
        /// For situations where a task should proceed only if another thread is not already working on the same task on the same object.  For example, a main thread and a worker thread both iterating through a list of objects to process, this will ensure each object is processed exactly once.
        /// <br/>Both threads should call this method before proceeding to do the task, both using an int field in each object to mark when the work has started.
        /// <br/><br/>Optionally, the referenced field can be a volatile int.  It is not necessary for it to be volatile for correct Interlocked operation, but it can be a good idea if the value will be set or read elsewhere in the calling code.
        /// </summary>
        /// <param name="started">Pass by reference.  <br/>If this is volatile, it is safe to ignore the compiler warning "a reference to a volatile field will not be treated as volatile"</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanProceedOnThisThread(ref int started)
        {
            return Interlocked.CompareExchange(ref started, 1, 0) == 0;
        }



        public class Multithreaded
        {
            protected volatile int activeThreads;

            protected void ResetThreading()
            {
                activeThreads = 0;
            }

            protected bool WorkerThreadsInProgress()
            {
                return activeThreads != 0;
            }

            protected void StartWorkerThread(Action task)
            {
                TyronThreadPool.QueueTask(() => OnWorkerThread(task), "asynchelper");
            }

            protected void OnWorkerThread(Action task)
            {
                Interlocked.Increment(ref activeThreads);
                try
                {
                    task();
                }
                finally
                {
                    Interlocked.Decrement(ref activeThreads);
                }
            }
        }
    }
}
