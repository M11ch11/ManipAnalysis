﻿using System.Collections.Generic;
using MongoDB.Bson;

namespace ManipAnalysis_v2.MongoDb
{
    internal class Baseline
    {
        public List<ForceContainer> MeasuredForces;

        public List<ForceContainer> MomentForces;

        public List<ForceContainer> NominalForces;

        public List<PositionContainer> Position;

        public List<VelocityContainer> Velocity;

        public byte[] ZippedMeasuredForces;

        public byte[] ZippedMomentForces;

        public byte[] ZippedNominalForces;

        public byte[] ZippedPosition;

        public byte[] ZippedVelocity;

        public Trial.TrialTypeEnum TrialType { get; set; }

        public Trial.ForceFieldTypeEnum ForceFieldType { get; set; }

        public Trial.HandednessEnum Handedness { get; set; }

        public ObjectId Id { get; set; }

        public string Study { get; set; }

        public string Group { get; set; }

        public SubjectContainer Subject { get; set; }

        public string Szenario { get; set; }

        public TargetContainer Target { get; set; }

        public MeasureFileContainer MeasureFile { get; set; }
    }
}