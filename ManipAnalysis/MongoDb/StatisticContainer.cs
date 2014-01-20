namespace ManipAnalysis.MongoDb
{
    internal class StatisticContainer
    {
        public double VelocityVectorCorrelation { get; set; }
        public double AbsoluteTrajectoryLength { get; set; }
        public double AbsoluteBaselineTrajectoryLengthRatio { get; set; }
        public double AbsolutePerpendicularDisplacement300Ms { get; set; }
        public double SignedPerpendicularDisplacement300Ms { get; set; }
        public double AbsoluteMaximalPerpendicularDisplacement { get; set; }
        public double SignedMaximalPerpendicularDisplacement { get; set; }
        public double AbsoluteMeanPerpendicularDisplacement { get; set; }
        public double EnclosedArea { get; set; }
        public double RMSE { get; set; }
    }
}