using System;

namespace ManipAnalysis.Container
{
    internal class SzenarioMeanTimeDataContainer
    {
        public TimeSpan MeanTime;
        public TimeSpan MeanTimeStd;

        public readonly int TargetNumber;

        public SzenarioMeanTimeDataContainer(TimeSpan meanTime, TimeSpan meanTimeStd, int targetNumber)
        {
            MeanTime = meanTime;
            MeanTimeStd = meanTimeStd;
            TargetNumber = targetNumber;
        }
    }
}