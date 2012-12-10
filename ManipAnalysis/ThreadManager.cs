using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ManipAnalysis
{
    static class ThreadManager
    {
        private static List<Thread> runningThreads = new List<Thread>();
        public static bool pause = false;

        public static void pushBack(Thread _thread)
        {
            runningThreads.Add(_thread);
        }

        public static void pushFront(Thread _thread)
        {
            runningThreads.Insert(0, _thread);
        }

        public static int getIndex(Thread _thread)
        {
            return runningThreads.IndexOf(_thread);
        }

        public static void remove(Thread _thread)
        {
            runningThreads.Remove(_thread);
        }

        public static int getThreadCount()
        {
            return runningThreads.Count;
        }

        public static List<Thread> getThreadList()
        {
            return runningThreads;
        }
    }
}
