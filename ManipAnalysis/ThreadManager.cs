using System.Collections.Generic;
using System.Threading;

namespace ManipAnalysis
{
    internal static class ThreadManager
    {
        private static readonly List<Thread> RunningThreads = new List<Thread>();
        public static bool Pause;

        public static void PushBack(Thread thread)
        {
            RunningThreads.Add(thread);
        }

        public static int GetIndex(Thread thread)
        {
            return RunningThreads.IndexOf(thread);
        }

        public static void Remove(Thread thread)
        {
            RunningThreads.Remove(thread);
        }
    }
}