using System;

namespace ManipAnalysis_v2.Container
{
    internal class MeasureDataContainer
    {
        public enum TrialTypeEnum
        {
            StandardTrial, ErrorClampTrial, CatchTrial
        };

        public enum ForceFieldTypeEnum
        {
            ForceFieldCW, ForceFieldCCW, NullField
        };

        public readonly double ForceActualX;
        public readonly double ForceActualY;
        public readonly double ForceActualZ;

        public readonly double ForceMomentX;
        public readonly double ForceMomentY;
        public readonly double ForceMomentZ;
        public readonly double ForceNominalX;
        public readonly double ForceNominalY;
        public readonly double ForceNominalZ;

        public readonly TrialTypeEnum TrialType;
        public readonly ForceFieldTypeEnum ForceFieldType;

        public readonly double PositionCartesianX;
        public readonly double PositionCartesianY;
        public readonly double PositionCartesianZ;
        public readonly int PositionStatus;

        public readonly int TargetNumber;
        public bool ContainsDuplicates;
        public int SzenarioTrialNumber;

        public int TargetTrialNumber;
        public DateTime TimeStamp;

        public MeasureDataContainer(DateTime timeStamp,
            double forceActualX,
            double forceActualY,
            double forceActualZ,
            double forceNominalX,
            double forceNominalY,
            double forceNominalZ,
            double forceMomentX,
            double forceMomentY,
            double forceMomentZ,
            double positionCartesianX,
            double positionCartesianY,
            double positionCartesianZ,
            int targetNumber,
            int targetTrialNumber,
            int szenarioTrialNumber,
            TrialTypeEnum trialType,
            ForceFieldTypeEnum forceFieldType,
            int positionStatus)
        {
            TimeStamp = timeStamp;
            ForceActualX = forceActualX;
            ForceActualY = forceActualY;
            ForceActualZ = forceActualZ;
            ForceNominalX = forceNominalX;
            ForceNominalY = forceNominalY;
            ForceNominalZ = forceNominalZ;
            ForceMomentX = forceMomentX;
            ForceMomentY = forceMomentY;
            ForceMomentZ = forceMomentZ;
            PositionCartesianX = positionCartesianX;
            PositionCartesianY = positionCartesianY;
            PositionCartesianZ = positionCartesianZ;
            TargetNumber = targetNumber;
            TargetTrialNumber = targetTrialNumber;
            SzenarioTrialNumber = szenarioTrialNumber;
            TrialType = trialType;
            ForceFieldType = forceFieldType;
            PositionStatus = positionStatus;
            ContainsDuplicates = false;
        }
    }
}