using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManipAnalysis
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

        public static int GetIndex(int? taskID)
        {
            int retVal = -1;

            for (int i = 0; i < RunningTasks.Count; i++)
            {
                if (taskID == RunningTasks[i].Id)
                {
                    retVal = RunningTasks.IndexOf(RunningTasks[i]);
                }
            }
            return retVal;
        }

        public static void Remove(int? taskID)
        {
            for (int i = 0; i < RunningTasks.Count; i++)
            {
                if (taskID == RunningTasks[i].Id)
                {
                    RunningTasks.Remove(RunningTasks[i]);
                }
            }
        }
    }
}