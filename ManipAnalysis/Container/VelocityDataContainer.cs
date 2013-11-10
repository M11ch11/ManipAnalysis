using System;

namespace ManipAnalysis.Container
{
    internal class VelocityDataContainer
    {
        public readonly int PositionStatus;
        public readonly int SzenarioTrialNumber;
        public readonly int TargetNumber;
        public readonly DateTime TimeStamp;
        public readonly double VelocityX;
        public readonly double VelocityY;
        public readonly double VelocityZ;

        public VelocityDataContainer(DateTime timeStamp, double velocityX, double velocityY, double velocityZ,
            int szenarioTrialNumber, int targetNumber, int positionStatus)
        {
            TimeStamp = timeStamp;
            VelocityX = velocityX;
            VelocityY = velocityY;
            VelocityZ = velocityZ;
            SzenarioTrialNumber = szenarioTrialNumber;
            TargetNumber = targetNumber;
            PositionStatus = positionStatus;
        }
    }
}