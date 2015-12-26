using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManipAnalysis_v2
{
    internal static class TaskManager
    {
        private static readonly List<Task> RunningTasks = new List<Task>();

        public static bool Pause;

        public static bool Cancel;

        public static void PushBack(Task task)
        {
            RunningTasks.Add(task);
        }

        public static int GetIndex(int? taskId)
        {
            var retVal = -1;

            foreach (var t in RunningTasks)
            {
                if (taskId == t.Id)
                {
                    retVal = RunningTasks.IndexOf(t);
                }
            }
            return retVal;
        }

        public static void Remove(int? taskId)
        {
            for (var i = 0;
                i < RunningTasks.Count;
                i
                    ++)
            {
                if (taskId == RunningTasks[i].Id)
                {
                    RunningTasks.Remove(RunningTasks[i]);
                }
            }
        }
    }
}