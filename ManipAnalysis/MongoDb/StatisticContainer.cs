using System;
using System.Collections.Generic;

namespace ManipAnalysis_v2.MongoDb
{
    public class StatisticContainer
    {
        public List<PerpendicularDisplacementContainer> AbsolutePerpendicularDisplacement =
            new List<PerpendicularDisplacementContainer>();
        public List<PerpendicularDisplacementContainer> SignedPerpendicularDisplacement =
            new List<PerpendicularDisplacementContainer>();
        public double VelocityVectorCorrelation { get; set; }
        public double AbsoluteTrajectoryLength { get; set; }
        public double AbsoluteBaselineTrajectoryLengthRatio { get; set; }
        public double AbsoluteMaximalPerpendicularDisplacement { get; set; }
        public double SignedMaximalPerpendicularDisplacement { get; set; }
        public double AbsoluteMaximalPerpendicularDisplacementVmax { get; set; }
        public double SignedMaximalPerpendicularDisplacementVmax { get; set; }
        public double AbsoluteMeanPerpendicularDisplacement { get; set; }
        public double EnclosedArea { get; set; }
        public double RMSE { get; set; }
        public double ParallelMidMovementForce { get; set; }
        public double PerpendicularMidMovementForce { get; set; }
        public double PerpendicularMidMovementForceRaw { get; set; }
        public double AbsoluteMidMovementForce { get; set; }
    }
}