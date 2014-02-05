﻿using System.Collections.Generic;
using MongoDB.Bson;

namespace ManipAnalysis_v2.MongoDb
{
    internal class Trial
    {
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
        public List<PositionContainer> PositionRaw;
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
        public byte[] ZippedVelocityFiltered;
        public byte[] ZippedVelocityNormalized;
        public ObjectId Id { get; set; }
        public ObjectId BaselineObjectId { get; set; }
        public StatisticContainer Statistics;

        public string  Study{ get; set; }
        public string Group { get; set; }
        public SubjectContainer Subject { get; set; }
        public string Szenario { get; set; }
        public TargetContainer Target { get; set; }
        public MeasureFileContainer MeasureFile { get; set; }

        public int TrialNumberInSzenario { get; set; }
        public int TargetTrialNumberInSzenario { get; set; }

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

        public string TrialVersion { get; set; }

        public PositionContainer PositionOffset = new PositionContainer();
    }
}