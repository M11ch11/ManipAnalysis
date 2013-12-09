using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class TrialContainerNode
    {
        public int trialId;
        public DateTime time_stamp;
        public double force_actual_x;
        public double force_actual_y;
        public double force_actual_z;
        public double force_nominal_x;
        public double force_nominal_y;
        public double force_nominal_z;
        public double force_moment_x;
        public double force_moment_y;
        public double force_moment_z;
        public double position_cartesian_x;
        public double position_cartesian_y;
        public double position_cartesian_z;
        public int position_status;
    }
}
