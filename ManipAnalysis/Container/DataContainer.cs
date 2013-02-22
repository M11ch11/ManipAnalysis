using System.Collections.Generic;

namespace ManipAnalysis.Container
{
    internal class DataContainer
    {
        public List<BaselineDataContainer> BaselineData;
        public string GroupName;
        public readonly List<MeasureDataContainer> MeasureDataFiltered = new List<MeasureDataContainer>();
        public readonly List<MeasureDataContainer> MeasureDataNormalized = new List<MeasureDataContainer>();
        public readonly List<MeasureDataContainer> MeasureDataRaw = new List<MeasureDataContainer>();
        public string MeasureFileCreationDate;
        public string MeasureFileCreationTime;
        public string MeasureFileHash;

        public string StudyName;
        public string SubjectID;
        public string SubjectName;
        public readonly List<SzenarioMeanTimeDataContainer> SzenarioMeanTimeData = new List<SzenarioMeanTimeDataContainer>();
        public string SzenarioName;

        public readonly List<VelocityDataContainer> VelocityDataFiltered = new List<VelocityDataContainer>();
        public readonly List<VelocityDataContainer> VelocityDataNormalized = new List<VelocityDataContainer>();
    }
}