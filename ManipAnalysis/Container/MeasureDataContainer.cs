using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManipAnalysis
{
    class MeasureDataContainer
    {
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

        public int target_number;

        public int target_trial_number;

        public int szenario_trial_number;

        public bool is_catch_trial;

        public int position_status;

        public bool contains_duplicates;

        public MeasureDataContainer(DateTime _time_stamp,
                            double _force_actual_x,
                            double _force_actual_y,
                            double _force_actual_z,
                            double _force_nominal_x,
                            double _force_nominal_y,
                            double _force_nominal_z,
                            double _force_moment_x,
                            double _force_moment_y,
                            double _force_moment_z,
                            double _position_cartesian_x,
                            double _position_cartesian_y,
                            double _position_cartesian_z,
                            int _target_number,
                            int _target_trial_number,
                            int _szenario_trial_number,
                            bool _is_catch_trial,
                            int _position_status)
        {
            time_stamp = _time_stamp;
            force_actual_x = _force_actual_x;
            force_actual_y = _force_actual_y;
            force_actual_z = _force_actual_z;
            force_nominal_x = _force_nominal_x;
            force_nominal_y = _force_nominal_y;
            force_nominal_z = _force_nominal_z;
            force_moment_x = _force_moment_x;
            force_moment_y = _force_moment_y;
            force_moment_z = _force_moment_z;
            position_cartesian_x = _position_cartesian_x;
            position_cartesian_y = _position_cartesian_y;
            position_cartesian_z = _position_cartesian_z;
            target_number = _target_number;
            target_trial_number = _target_trial_number;
            szenario_trial_number = _szenario_trial_number;
            is_catch_trial = _is_catch_trial;
            position_status = _position_status;
            contains_duplicates = false;
        }
    }
}
