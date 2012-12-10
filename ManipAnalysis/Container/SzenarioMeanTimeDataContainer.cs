using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManipAnalysis
{
    class SzenarioMeanTimeDataContainer
    {
        public TimeSpan mean_time;
        public TimeSpan mean_time_std;

        public int target_number;

        public SzenarioMeanTimeDataContainer(TimeSpan _mean_time, TimeSpan _mean_time_std, int _target_number)
        {
            mean_time = _mean_time;
            mean_time_std = _mean_time_std;
            target_number = _target_number;
        }
    }
}
