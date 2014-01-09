using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManipAnalysis.MongoDb
{
    class Baseline
    {
        public ObjectId Id { get; set; }
        public ObjectId StudyObjectId { get; set; }
        public ObjectId GroupObjectId { get; set; }
        public ObjectId SubjectObjectId { get; set; }
        public ObjectId SzenarioObjectId { get; set; }
        public ObjectId TargetObjectId { get; set; }
        public ObjectId MeasureFileObjectId { get; set; }

        public List<PositionContainer> Position = new List<PositionContainer>();
        public List<VelocityContainer> Velocity = new List<VelocityContainer>(); 
    }
}
