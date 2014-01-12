namespace ManipAnalysis.MongoDb
{
    internal class TrialInformationContainer
    {
        public bool FaultyTrial { get; set; }
        public bool CatchTrial { get; set; }
        public bool ErrorClampTrial { get; set; }

        public int PositionDataFilterCutoffFrequency { get; set; }
        public int ForceDataFilterCutoffFrequency { get; set; }
        public int PositionDataFilterOrder { get; set; }
        public int ForceDataFilterOrder { get; set; }
        public int VelocityTrimThresholdPercent { get; set; }
        public double VelocityTrimThresholdForTrial { get; set; } // in m/s
        public int RawDataSampleRate { get; set; }
        public int FilteredDataSampleRate { get; set; }
        public int NormalizedDataSampleRate { get; set; }
    }
}