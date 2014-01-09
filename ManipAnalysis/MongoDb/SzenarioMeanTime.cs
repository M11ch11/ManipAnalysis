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
        public string Study { get; set; }
        public string Group { get; set; }
        public SubjectContainer Subject { get; set; }
        public string Szenario { get; set; }
        public TargetContainer Target { get; set; }
        public MeasureFileContainer MeasureFile { get; set; }
        public TimeSpan MeanTime { get; set; }
        public TimeSpan MeanTimeStd { get; set; }
        }
    }
}
