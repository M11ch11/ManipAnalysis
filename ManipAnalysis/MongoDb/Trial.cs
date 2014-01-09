using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace ManipAnalysis.MongoDb
{
    class Trial
    {
        public ObjectId Id { get; set; }
        public ObjectId StudyObjectId { get; set; }
        public ObjectId GroupObjectId { get; set; }
        public ObjectId SubjectObjectId { get; set; }
        public ObjectId SzenarioObjectId { get; set; }
        public ObjectId TargetObjectId { get; set; }
        public ObjectId MeasureFileObjectId { get; set; }
        public ObjectId TrialInformationObjectId { get; set; }

        public int TrialNumberInSzenario { get; set; }
        public int TargetTrialNumberInSzenario { get; set; }

        public List<ForceContainer> ActualForcesRaw = new List<ForceContainer>();
        public List<ForceContainer> NominalForcesRaw = new List<ForceContainer>();
        public List<ForceContainer> MomentForcesRaw = new List<ForceContainer>();

        public List<ForceContainer> ActualForcesFiltered = new List<ForceContainer>();
        public List<ForceContainer> NominalForcesFiltered = new List<ForceContainer>();
        public List<ForceContainer> MomentForcesFiltered = new List<ForceContainer>();

        public List<ForceContainer> ActualForcesNormalized = new List<ForceContainer>();
        public List<ForceContainer> NominalForcesNormalized = new List<ForceContainer>();
        public List<ForceContainer> MomentForcesNormalized = new List<ForceContainer>();

        public List<PositionContainer> PositionRaw = new List<PositionContainer>();
        public List<PositionContainer> PositionFiltered = new List<PositionContainer>();
        public List<PositionContainer> PositionNormalized = new List<PositionContainer>();

        public List<VelocityContainer> VelocityRaw = new List<VelocityContainer>();
        public List<VelocityContainer> VelocityFiltered = new List<VelocityContainer>();
        public List<VelocityContainer> VelocityNormalized = new List<VelocityContainer>();

        public List<StatisticContainer> Statistics = new List<StatisticContainer>();
    }
}
