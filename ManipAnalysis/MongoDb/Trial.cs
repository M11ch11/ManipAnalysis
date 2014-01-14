using System.Collections.Generic;
using MongoDB.Bson;

namespace ManipAnalysis.MongoDb
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
        public List<StatisticContainer> Statistics;
        public List<VelocityContainer> VelocityFiltered;
        public List<VelocityContainer> VelocityNormalized;
        public ObjectId Id { get; set; }
        public ObjectId BaselineObjectId { get; set; }

        public string  Study{ get; set; }
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