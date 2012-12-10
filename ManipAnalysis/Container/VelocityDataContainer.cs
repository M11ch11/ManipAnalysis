using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManipAnalysis
{
    class VelocityDataContainer
    {
        public DateTime time_stamp;
        public double velocity_x;
        public double velocity_y;
        public double velocity_z;
        public int szenario_trial_number;
        public int target_number;
        public int position_status;

        public VelocityDataContainer(DateTime _time_stamp, double _velocity_x, double _velocity_y, double _velocity_z, int _szenario_trial_number, int _target_number, int _position_status)
        {
            time_stamp = _time_stamp;
            velocity_x = _velocity_x;
            velocity_y = _velocity_y;
            velocity_z = _velocity_z;
            szenario_trial_number = _szenario_trial_number;
            target_number = _target_number;
            position_status = _position_status;
        }

    }
}
