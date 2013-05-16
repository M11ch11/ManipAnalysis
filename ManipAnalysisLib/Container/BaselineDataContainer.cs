using System;

namespace ManipAnalysisLib.Container
{
    internal class BaselineDataContainer
    {
        public readonly double BaselinePositionCartesianX;
        public readonly double BaselinePositionCartesianY;
        public readonly double BaselinePositionCartesianZ;

        public readonly double BaselineVelocityX;
        public readonly double BaselineVelocityY;
        public readonly double BaselineVelocityZ;

        public readonly int TargetNumber;
        public DateTime PseudoTimeStamp;

        public BaselineDataContainer(DateTime pseudoTimeStamp,
                                     double baselinePositionCartesianX, double baselinePositionCartesianY,
                                     double baselinePositionCartesianZ,
                                     double baselineVelocityX, double baselineVelocityY, double baselineVelocityZ,
                                     int targetNumber)
        {
            PseudoTimeStamp = pseudoTimeStamp;

            BaselinePositionCartesianX = baselinePositionCartesianX;
            BaselinePositionCartesianY = baselinePositionCartesianY;
            BaselinePositionCartesianZ = baselinePositionCartesianZ;

            BaselineVelocityX = baselineVelocityX;
            BaselineVelocityY = baselineVelocityY;
            BaselineVelocityZ = baselineVelocityZ;

            TargetNumber = targetNumber;
        }
    }
}