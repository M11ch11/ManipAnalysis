using System.Collections.Generic;

namespace ManipAnalysisLib.Container
{
    internal class DataContainer
    {
        public readonly List<MeasureDataContainer> MeasureDataFiltered = new List<MeasureDataContainer>();
        public readonly List<MeasureDataContainer> MeasureDataNormalized = new List<MeasureDataContainer>();
        public readonly List<MeasureDataContainer> MeasureDataRaw = new List<MeasureDataContainer>();

        public readonly List<SzenarioMeanTimeDataContainer> SzenarioMeanTimeData =
            new List<SzenarioMeanTimeDataContainer>();

        public readonly List<VelocityDataContainer> VelocityDataFiltered = new List<VelocityDataContainer>();
        public readonly List<VelocityDataContainer> VelocityDataNormalized = new List<VelocityDataContainer>();
        public List<BaselineDataContainer> BaselineData;
        public string GroupName;

        public string MeasureFileCreationDate;
        public string MeasureFileCreationTime;
        public string MeasureFileHash;

        public string StudyName;
        public string SubjectID;
        public string SubjectName;

        public string SzenarioName;
    }
}