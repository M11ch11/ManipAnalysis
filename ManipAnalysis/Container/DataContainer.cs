using System.Collections.Generic;

namespace ManipAnalysis_v2.Container
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

        public List<BaselineDataContainer> BaselineData = null;

        public string GroupName = null;

        public string MeasureFileCreationDate = null;

        public string MeasureFileCreationTime = null;

        public string MeasureFileHash = null;

        public string StudyName = null;

        public string SubjectID = null;

        public string SubjectName = null;

        public string SzenarioName = null;
    }
}