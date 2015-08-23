using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace ManipAnalysis_v2.MongoDb
{
    internal class Trial
    {
        public enum ForceFieldTypeEnum
        {
            NullField = 0,

            ForceFieldCW = 1,

            ForceFieldCCW = 2
        };

        public enum HandednessEnum
        {
            RightHand = 0,

            LeftHand = 1,

            Unknown = 2
        };

        public enum TrialTypeEnum
        {
            StandardTrial = 0,

            ErrorClampTrial = 1,

            CatchTrial = 2,

            PositionControlTrial = 3
        };

        public List<ForceContainer> MeasuredForcesFiltered;

        public List<ForceContainer> MeasuredForcesNormalized;

        public List<ForceContainer> MeasuredForcesRaw;

        public List<ForceContainer> MomentForcesFiltered;

        public List<ForceContainer> MomentForcesNormalized;

        public List<ForceContainer> MomentForcesRaw;

        public List<ForceContainer> NominalForcesFiltered;

        public List<ForceContainer> NominalForcesNormalized;

        public List<ForceContainer> NominalForcesRaw;

        public List<PositionContainer> PositionFiltered;

        public List<PositionContainer> PositionNormalized;

        public PositionContainer PositionOffset = new PositionContainer();

        public List<PositionContainer> PositionRaw;

        public StatisticContainer Statistics;

        public List<VelocityContainer> VelocityFiltered;

        public List<VelocityContainer> VelocityNormalized;

        public byte[] ZippedMeasuredForcesFiltered;

        public byte[] ZippedMeasuredForcesNormalized;

        public byte[] ZippedMeasuredForcesRaw;

        public byte[] ZippedMomentForcesFiltered;

        public byte[] ZippedMomentForcesNormalized;

        public byte[] ZippedMomentForcesRaw;

        public byte[] ZippedNominalForcesFiltered;

        public byte[] ZippedNominalForcesNormalized;

        public byte[] ZippedNominalForcesRaw;

        public byte[] ZippedPositionFiltered;

        public byte[] ZippedPositionNormalized;

        public byte[] ZippedPositionRaw;

        public byte[] ZippedStatistics;

        public byte[] ZippedVelocityFiltered;

        public byte[] ZippedVelocityNormalized;

        public ObjectId Id { get; set; }

        public ObjectId BaselineObjectId { get; set; }

        public string Study { get; set; }

        public string Group { get; set; }

        public SubjectContainer Subject { get; set; }

        public string Szenario { get; set; }

        public TargetContainer Target { get; set; }

        public MeasureFileContainer MeasureFile { get; set; }

        public int TrialNumberInSzenario { get; set; }

        public int TargetTrialNumberInSzenario { get; set; }

        public TrialTypeEnum TrialType { get; set; }

        public ForceFieldTypeEnum ForceFieldType { get; set; }

        public HandednessEnum Handedness { get; set; }

        public int PositionDataFilterCutoffFrequency { get; set; }

        public int ForceDataFilterCutoffFrequency { get; set; }

        public int PositionDataFilterOrder { get; set; }

        public int ForceDataFilterOrder { get; set; }

        public double VelocityTrimThresholdPercent { get; set; }

        public double VelocityTrimThresholdForTrial { get; set; }

        // in m/s
        public int RawDataSampleRate { get; set; }

        public int FilteredDataSampleRate { get; set; }

        public int NormalizedDataSampleRate { get; set; }

        public DateTime StartDateTimeOfTrialRecording { get; set; }

        public string TrialVersion { get; set; }
    }
}