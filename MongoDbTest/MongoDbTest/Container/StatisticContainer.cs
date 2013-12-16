using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest.Container
{
    class StatisticContainer
    {
        public double VelocityVectorCorrelation { get; set; }
        public double AbsoluteTrajectoryLength { get; set; }
        public double AbsoluteBaselineTrajectoryLength { get; set; }
        public double AbsolutePerpendicularDisplacement300Ms { get; set; }
        public double SignedPerpendicularDisplacement300Ms { get; set; }
        public double AbsoluteMaximalPerpendicularDisplacement { get; set; }
        public double SignedMaximalPerpendicularDisplacement { get; set; }
        public double AbsoluteMeanPerpendicularDisplacement { get; set; }
        public double EnclosedArea { get; set; }
        public double RMSE { get; set; }
    }
}
