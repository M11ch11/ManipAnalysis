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

        //TODO: Raus!
        public double AbsoluteBaselineTrajectoryLengthRatio { get; set; }

        public double AbsoluteMaximalPerpendicularDisplacement { get; set; }

        public double SignedMaximalPerpendicularDisplacement { get; set; }

        public double AbsoluteMaximalPerpendicularDisplacementVmax { get; set; }

        public double SignedMaximalPerpendicularDisplacementVmax { get; set; }

        //TODO: Raus!
        public double AbsoluteMeanPerpendicularDisplacement { get; set; }

        public double EnclosedArea { get; set; }

        public double RMSE { get; set; }

        public double ParallelMidMovementForce { get; set; }

        public double PerpendicularMidMovementForce { get; set; }

        public double PerpendicularMidMovementForceRaw { get; set; }

        public double AbsoluteMidMovementForce { get; set; }

        public double ForcefieldCompenstionFactor { get; set; }

        public double ForcefieldCompenstionFactorRaw { get; set; }

        public double PredictionAngle { get; set; }

        public double FeedbackAngle { get; set; }
    }
}