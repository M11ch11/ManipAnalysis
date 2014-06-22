using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManipAnalysis_v2.MongoDb
{
    class SzenarioContainer
    {
        public string GroupName;
        public DateTime MeasureFileCreationDateTime;
        public string MeasureFileHash;
        public string ProbandId;
        public string StudyName;
        public string SzenarioName;
        public List<Trial> TrialsContainer;
    }
}
