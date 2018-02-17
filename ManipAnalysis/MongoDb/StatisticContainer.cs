using System.Collections.Generic;

namespace ManipAnalysis_v2.MongoDb
{
    public class StatisticContainer
    {
        public List<PerpendicularDisplacementContainer> AbsolutePerpendicularDisplacement =
            new List<PerpendicularDisplacementContainer>();

        public List<PerpendicularDisplacementContainer> SignedPerpendicularDisplacement =
            new List<PerpendicularDisplacementContainer>();

        public double AbsoluteTrajectoryLength { get; set; }

        public double AbsoluteMaximalPerpendicularDisplacement { get; set; }

        public double SignedMaximalPerpendicularDisplacement { get; set; }

        public double AbsoluteMaximalPerpendicularDisplacementVmax { get; set; }

        public double SignedMaximalPerpendicularDisplacementVmax { get; set; }
        
        public double EnclosedArea { get; set; }
        
        public double PerpendicularMidMovementForceRaw { get; set; }
        
        public double ForcefieldCompenstionFactorRaw { get; set; }
    }
}