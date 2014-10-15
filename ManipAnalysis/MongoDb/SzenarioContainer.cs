using System;
using System.Collections.Generic;

namespace ManipAnalysis_v2.MongoDb
{
    internal class SzenarioContainer
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