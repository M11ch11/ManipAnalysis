using System.Collections.Generic;
using MongoDB.Bson;

namespace ManipAnalysis.MongoDb
{
    internal class Trial
    {
        public List<ForceContainer> MeasuredForcesFiltered = new List<ForceContainer>();
        public List<ForceContainer> MeasuredForcesNormalized = new List<ForceContainer>();
        public List<ForceContainer> MeasuredForcesRaw = new List<ForceContainer>();
        public List<ForceContainer> MomentForcesFiltered = new List<ForceContainer>();
        public List<ForceContainer> MomentForcesNormalized = new List<ForceContainer>();
        public List<ForceContainer> MomentForcesRaw = new List<ForceContainer>();
        public List<ForceContainer> NominalForcesFiltered = new List<ForceContainer>();
        public List<ForceContainer> NominalForcesNormalized = new List<ForceContainer>();
        public List<ForceContainer> NominalForcesRaw = new List<ForceContainer>();
        public List<PositionContainer> PositionFiltered = new List<PositionContainer>();
        public List<PositionContainer> PositionNormalized = new List<PositionContainer>();
        public List<PositionContainer> PositionRaw = new List<PositionContainer>();
        public List<StatisticContainer> Statistics = new List<StatisticContainer>();
        public List<VelocityContainer> VelocityFiltered = new List<VelocityContainer>();
        public List<VelocityContainer> VelocityNormalized = new List<VelocityContainer>();
        public List<VelocityContainer> VelocityRaw = new List<VelocityContainer>();
        public ObjectId Id { get; set; }
        public ObjectId BaselineObjectId { get; set; }

        public string Study { get; set; }
        public string Group { get; set; }
        public SubjectContainer Subject { get; set; }
        public string Szenario { get; set; }
        public TargetContainer Target { get; set; }
        public MeasureFileContainer MeasureFile { get; set; }
        public TrialInformationContainer TrialInformation { get; set; }

        public int TrialNumberInSzenario { get; set; }
        public int TargetTrialNumberInSzenario { get; set; }
    }
}