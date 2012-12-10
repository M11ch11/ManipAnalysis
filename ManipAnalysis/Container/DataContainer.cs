using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManipAnalysis
{
    class DataContainer
    {
        public string measureFileHash;
        public string measureFileCreationDate;
        public string measureFileCreationTime;

        public string studyName;
        public string szenarioName;
        public string groupName;
        public string subjectName;
        public string subjectID;

        public List<MeasureDataContainer> measureDataRaw = new List<MeasureDataContainer>();
        public List<MeasureDataContainer> measureDataFiltered = new List<MeasureDataContainer>();
        public List<MeasureDataContainer> measureDataNormalized = new List<MeasureDataContainer>();
        public List<VelocityDataContainer> velocityDataFiltered = new List<VelocityDataContainer>();
        public List<VelocityDataContainer> velocityDataNormalized = new List<VelocityDataContainer>();
        public List<SzenarioMeanTimeDataContainer> szenarioMeanTimeData = new List<SzenarioMeanTimeDataContainer>();

        public List<BaselineDataContainer> baselineData = null;

    }
}
