using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManipAnalysis.MongoDb
{
    class SzenarioMeanTime
    {
        public ObjectId Id { get; set; }
        public ObjectId StudyObjectId { get; set; }
        public ObjectId GroupObjectId { get; set; }
        public ObjectId SubjectObjectId { get; set; }
        public ObjectId SzenarioObjectId { get; set; }
        public ObjectId TargetObjectId { get; set; }
        public ObjectId MeasureFileObjectId { get; set; }

        /// <summary>
        /// Convert to UTC First!!!
        /// </summary>
        public TimeSpan MeanTime { get; set; }
        public TimeSpan MeanTimeStd { get; set; }
    }
}
