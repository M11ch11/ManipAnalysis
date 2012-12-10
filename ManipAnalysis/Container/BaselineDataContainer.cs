using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManipAnalysis
{
    class BaselineDataContainer
    {
        public DateTime pseudo_time_stamp;

        public double baseline_position_cartesian_x;
        public double baseline_position_cartesian_y;
        public double baseline_position_cartesian_z;

        public double baseline_velocity_x;
        public double baseline_velocity_y;
        public double baseline_velocity_z;

        public int target_number;

        public BaselineDataContainer(DateTime _virtual_time_stamp,
                                        double _baseline_position_cartesian_x, double _baseline_position_cartesian_y, double _baseline_position_cartesian_z,
                                        double _baseline_velocity_x, double _baseline_velocity_y, double _baseline_velocity_z,
                                        int _target_number)
        {
            pseudo_time_stamp = _virtual_time_stamp;

            baseline_position_cartesian_x = _baseline_position_cartesian_x;
            baseline_position_cartesian_y = _baseline_position_cartesian_y;
            baseline_position_cartesian_z = _baseline_position_cartesian_z;

            baseline_velocity_x = _baseline_velocity_x;
            baseline_velocity_y = _baseline_velocity_y;
            baseline_velocity_z = _baseline_velocity_z;

            target_number = _target_number;
        }

    }
}
