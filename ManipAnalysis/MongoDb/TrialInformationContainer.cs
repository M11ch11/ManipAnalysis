using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManipAnalysis.MongoDb
{
    class TrialInformationContainer
    {
        public bool FaultyTrial { get; set; }
        public bool CatchTrial { get; set; }
        public bool ErrorClampTrial { get; set; }

        public int PositionDataFilterCutoffFrequency { get; set; }
        public int ForceDataFilterCutoffFrequency { get; set; }
        public int PositionDataFilterOrder { get; set; }
        public int ForceDataFilterOrder { get; set; }
        public int VelocityTrimThresholdPercent { get; set; }
    }
}
