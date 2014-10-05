using System.Collections.Generic;
using MongoDB.Bson;

namespace ManipAnalysis_v2.MongoDb
{
    internal class Baseline
    {
        public enum TrialTypeEnum
        {
            StandardTrial = 0, ErrorClampTrial = 1, CatchTrial = 2
        };

        public enum ForceFieldTypeEnum
        {
            NullField = 0, ForceFieldCW = 1, ForceFieldCCW = 2
        };

        public enum HandednessEnum
        {
            RightHand = 0, LeftHand = 1, Unknown = 2
        };

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
        public ObjectId Id { get; set; }
        public string Study { get; set; }
        public string Group { get; set; }
        public SubjectContainer Subject { get; set; }
        public string Szenario { get; set; }
        public TargetContainer Target { get; set; }
        public MeasureFileContainer MeasureFile { get; set; }
    }
}