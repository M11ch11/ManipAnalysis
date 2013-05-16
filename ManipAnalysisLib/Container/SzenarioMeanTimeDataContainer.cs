using System;

namespace ManipAnalysisLib.Container
{
    internal class SzenarioMeanTimeDataContainer
    {
        public readonly int TargetNumber;
        public TimeSpan MeanTime;
        public TimeSpan MeanTimeStd;

        public SzenarioMeanTimeDataContainer(TimeSpan meanTime, TimeSpan meanTimeStd, int targetNumber)
        {
            MeanTime = meanTime;
            MeanTimeStd = meanTimeStd;
            TargetNumber = targetNumber;
        }
    }
}