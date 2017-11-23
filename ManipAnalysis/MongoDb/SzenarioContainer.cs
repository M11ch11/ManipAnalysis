using System;
using System.Collections.Generic;

namespace ManipAnalysis_v2.MongoDb
{
    public class SzenarioContainer
    {
        public string GroupName = null;

        public DateTime MeasureFileCreationDateTime = new DateTime();

        public string MeasureFileHash = null;

        public string ProbandId = null;

        public string StudyName = null;

        public string SzenarioName = null;

        public List<Trial> TrialsContainer = null;
    }
}