using System.Collections.Generic;
using MongoDB.Bson;

namespace ManipAnalysis.MongoDb
{
    internal class Baseline
    {
        public List<PositionContainer> Position = new List<PositionContainer>();
        public List<VelocityContainer> Velocity = new List<VelocityContainer>();
        public ObjectId Id { get; set; }
        public string Study { get; set; }
        public string Group { get; set; }
        public SubjectContainer Subject { get; set; }
        public string Szenario { get; set; }
        public TargetContainer Target { get; set; }
        public MeasureFileContainer MeasureFile { get; set; }
    }
}