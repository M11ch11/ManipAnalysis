using System;
using ManipAnalysis_v2.MongoDb;

namespace ManipAnalysis_v2.SzenarioParseDefinitions
{
    internal class _06_test_for_transfer_CCW : ISzenarioDefinition
    {
        new public const string StudyName = "Study 10";

        new public const string SzenarioName = "06_test_for_transfer_CCW";

        new public const int TrialCount = 40;

        public override Trial setTrialMetadata(ManipAnalysisGui myManipAnalysisGui, Trial trial)
        {
            trial.Study = StudyName;
            trial.Szenario = SzenarioName;

            if (trial.Target.Number == 10) // Target 10 == StartTrial
            {
                myManipAnalysisGui.WriteToLogBox("Skipping Start-Trial. " + trial.Szenario + ", Trail " + trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                trial = null;
            }
            else
            {
                if (trial.Target.Number >= 1 && trial.Target.Number <= 8) // Null
                {
                    trial.Target.Number = trial.Target.Number;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if (trial.Target.Number >= 11 && trial.Target.Number <= 18) // Null, Position control, skip
                {
                    trial.Target.Number = trial.Target.Number;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.PositionControlTrial;
                    trial = null;
                }
                else if ((trial.Target.Number >= 21 && trial.Target.Number <= 28)) // CW weak
                {
                    trial.Target.Number = trial.Target.Number - 20;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                    trial.ForceFieldMatrix[0, 0] = 0;
                    trial.ForceFieldMatrix[0, 1] = 15;
                    trial.ForceFieldMatrix[1, 0] = -15;
                    trial.ForceFieldMatrix[1, 1] = 0;
                }
                else if (trial.Target.Number >= 31 && trial.Target.Number <= 38) // CW medium
                {
                    trial.Target.Number = trial.Target.Number - 30;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                    trial.ForceFieldMatrix[0, 0] = 0;
                    trial.ForceFieldMatrix[0, 1] = 15;
                    trial.ForceFieldMatrix[1, 0] = -15;
                    trial.ForceFieldMatrix[1, 1] = 0;
                }
                else if (trial.Target.Number >= 41 && trial.Target.Number <= 48) // CW strong
                {
                    trial.Target.Number = trial.Target.Number - 40;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                    trial.ForceFieldMatrix[0, 0] = 0;
                    trial.ForceFieldMatrix[0, 1] = 15;
                    trial.ForceFieldMatrix[1, 0] = -15;
                    trial.ForceFieldMatrix[1, 1] = 0;
                }
                else if (trial.Target.Number >= 51 && trial.Target.Number <= 58) // CW strong, error clamp
                {
                    trial.Target.Number = trial.Target.Number - 50;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                    trial.ForceFieldMatrix[0, 0] = 0;
                    trial.ForceFieldMatrix[0, 1] = 15;
                    trial.ForceFieldMatrix[1, 0] = -15;
                    trial.ForceFieldMatrix[1, 1] = 0;
                }
                else if (trial.Target.Number >= 61 && trial.Target.Number <= 68) // Pause + Position control, skip
                {
                    trial.Target.Number = trial.Target.Number - 52;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.PositionControlTrial;
                    trial = null;
                }
                else if (trial.Target.Number >= 71 && trial.Target.Number <= 78) // CCW medium
                {
                    trial.Target.Number = trial.Target.Number - 70;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                    // This is intentional! Same ForceFieldMatrix for CCW as for CW
                    trial.ForceFieldMatrix[0, 0] = 0;
                    trial.ForceFieldMatrix[0, 1] = 15;
                    trial.ForceFieldMatrix[1, 0] = -15;
                    trial.ForceFieldMatrix[1, 1] = 0;
                }
                else
                {
                    myManipAnalysisGui.WriteToLogBox("Invalid Target-Number. " + trial.Szenario + ", Trail " + trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                    trial = null;
                }

                if (trial != null)
                {
                    if (trial.Target.Number == 1)
                    {
                        trial.Target.XPos = 0.1 * Math.Cos(DegreeToRadian(90));
                        trial.Target.YPos = 0.1 * Math.Sin(DegreeToRadian(90));
                        trial.Target.ZPos = 0;
                        trial.Target.Radius = 0.0025;
                        trial.Origin.XPos = 0;
                        trial.Origin.YPos = 0;
                        trial.Origin.ZPos = 0;
                        trial.Origin.Radius = 0.0025;
                    }
                    else if (trial.Target.Number == 2)
                    {
                        trial.Target.XPos = 0.1 * Math.Cos(DegreeToRadian(45));
                        trial.Target.YPos = 0.1 * Math.Sin(DegreeToRadian(45));
                        trial.Target.ZPos = 0;
                        trial.Target.Radius = 0.0025;
                        trial.Origin.XPos = 0;
                        trial.Origin.YPos = 0;
                        trial.Origin.ZPos = 0;
                        trial.Origin.Radius = 0.0025;
                    }
                    else if (trial.Target.Number == 3)
                    {
                        trial.Target.XPos = 0.1 * Math.Cos(DegreeToRadian(0));
                        trial.Target.YPos = 0.1 * Math.Sin(DegreeToRadian(0));
                        trial.Target.ZPos = 0;
                        trial.Target.Radius = 0.0025;
                        trial.Origin.XPos = 0;
                        trial.Origin.YPos = 0;
                        trial.Origin.ZPos = 0;
                        trial.Origin.Radius = 0.0025;
                    }
                    else if (trial.Target.Number == 4)
                    {
                        trial.Target.XPos = 0.1 * Math.Cos(DegreeToRadian(315));
                        trial.Target.YPos = 0.1 * Math.Sin(DegreeToRadian(315));
                        trial.Target.ZPos = 0;
                        trial.Target.Radius = 0.0025;
                        trial.Origin.XPos = 0;
                        trial.Origin.YPos = 0;
                        trial.Origin.ZPos = 0;
                        trial.Origin.Radius = 0.0025;
                    }
                    else if (trial.Target.Number == 5)
                    {
                        trial.Target.XPos = 0.1 * Math.Cos(DegreeToRadian(270));
                        trial.Target.YPos = 0.1 * Math.Sin(DegreeToRadian(270));
                        trial.Target.ZPos = 0;
                        trial.Target.Radius = 0.0025;
                        trial.Origin.XPos = 0;
                        trial.Origin.YPos = 0;
                        trial.Origin.ZPos = 0;
                        trial.Origin.Radius = 0.0025;
                    }
                    else if (trial.Target.Number == 6)
                    {
                        trial.Target.XPos = 0.1 * Math.Cos(DegreeToRadian(225));
                        trial.Target.YPos = 0.1 * Math.Sin(DegreeToRadian(225));
                        trial.Target.ZPos = 0;
                        trial.Target.Radius = 0.0025;
                        trial.Origin.XPos = 0;
                        trial.Origin.YPos = 0;
                        trial.Origin.ZPos = 0;
                        trial.Origin.Radius = 0.0025;
                    }
                    else if (trial.Target.Number == 7)
                    {
                        trial.Target.XPos = 0.1 * Math.Cos(DegreeToRadian(180));
                        trial.Target.YPos = 0.1 * Math.Sin(DegreeToRadian(180));
                        trial.Target.ZPos = 0;
                        trial.Target.Radius = 0.0025;
                        trial.Origin.XPos = 0;
                        trial.Origin.YPos = 0;
                        trial.Origin.ZPos = 0;
                        trial.Origin.Radius = 0.0025;
                    }
                    else if (trial.Target.Number == 8)
                    {
                        trial.Target.XPos = 0.1 * Math.Cos(DegreeToRadian(135));
                        trial.Target.YPos = 0.1 * Math.Sin(DegreeToRadian(135));
                        trial.Target.ZPos = 0;
                        trial.Target.Radius = 0.0025;
                        trial.Origin.XPos = 0;
                        trial.Origin.YPos = 0;
                        trial.Origin.ZPos = 0;
                        trial.Origin.Radius = 0.0025;
                    }
                    if (trial.Target.Number == 11)
                    {
                        trial.Origin.XPos = 0.1 * Math.Cos(DegreeToRadian(90));
                        trial.Origin.YPos = 0.1 * Math.Sin(DegreeToRadian(90));
                        trial.Origin.ZPos = 0;
                        trial.Origin.Radius = 0.0025;
                        trial.Target.XPos = 0;
                        trial.Target.YPos = 0;
                        trial.Target.ZPos = 0;
                        trial.Target.Radius = 0.0025;
                    }
                    else if (trial.Target.Number == 12)
                    {
                        trial.Origin.XPos = 0.1 * Math.Cos(DegreeToRadian(45));
                        trial.Origin.YPos = 0.1 * Math.Sin(DegreeToRadian(45));
                        trial.Origin.ZPos = 0;
                        trial.Origin.Radius = 0.0025;
                        trial.Target.XPos = 0;
                        trial.Target.YPos = 0;
                        trial.Target.ZPos = 0;
                        trial.Target.Radius = 0.0025;
                    }
                    else if (trial.Target.Number == 13)
                    {
                        trial.Origin.XPos = 0.1 * Math.Cos(DegreeToRadian(0));
                        trial.Origin.YPos = 0.1 * Math.Sin(DegreeToRadian(0));
                        trial.Origin.ZPos = 0;
                        trial.Origin.Radius = 0.0025;
                        trial.Target.XPos = 0;
                        trial.Target.YPos = 0;
                        trial.Target.ZPos = 0;
                        trial.Target.Radius = 0.0025;
                    }
                    else if (trial.Target.Number == 14)
                    {
                        trial.Origin.XPos = 0.1 * Math.Cos(DegreeToRadian(315));
                        trial.Origin.YPos = 0.1 * Math.Sin(DegreeToRadian(315));
                        trial.Origin.ZPos = 0;
                        trial.Origin.Radius = 0.0025;
                        trial.Target.XPos = 0;
                        trial.Target.YPos = 0;
                        trial.Target.ZPos = 0;
                        trial.Target.Radius = 0.0025;
                    }
                    else if (trial.Target.Number == 15)
                    {
                        trial.Origin.XPos = 0.1 * Math.Cos(DegreeToRadian(270));
                        trial.Origin.YPos = 0.1 * Math.Sin(DegreeToRadian(270));
                        trial.Origin.ZPos = 0;
                        trial.Origin.Radius = 0.0025;
                        trial.Target.XPos = 0;
                        trial.Target.YPos = 0;
                        trial.Target.ZPos = 0;
                        trial.Target.Radius = 0.0025;
                    }
                    else if (trial.Target.Number == 16)
                    {
                        trial.Origin.XPos = 0.1 * Math.Cos(DegreeToRadian(225));
                        trial.Origin.YPos = 0.1 * Math.Sin(DegreeToRadian(225));
                        trial.Origin.ZPos = 0;
                        trial.Origin.Radius = 0.0025;
                        trial.Target.XPos = 0;
                        trial.Target.YPos = 0;
                        trial.Target.ZPos = 0;
                        trial.Target.Radius = 0.0025;
                    }
                    else if (trial.Target.Number == 17)
                    {
                        trial.Origin.XPos = 0.1 * Math.Cos(DegreeToRadian(180));
                        trial.Origin.YPos = 0.1 * Math.Sin(DegreeToRadian(180));
                        trial.Origin.ZPos = 0;
                        trial.Origin.Radius = 0.0025;
                        trial.Target.XPos = 0;
                        trial.Target.YPos = 0;
                        trial.Target.ZPos = 0;
                        trial.Target.Radius = 0.0025;
                    }
                    else if (trial.Target.Number == 18)
                    {
                        trial.Origin.XPos = 0.1 * Math.Cos(DegreeToRadian(135));
                        trial.Origin.YPos = 0.1 * Math.Sin(DegreeToRadian(135));
                        trial.Origin.ZPos = 0;
                        trial.Origin.Radius = 0.0025;
                        trial.Target.XPos = 0;
                        trial.Target.YPos = 0;
                        trial.Target.ZPos = 0;
                        trial.Target.Radius = 0.0025;
                    }
                    else
                    {
                        myManipAnalysisGui.WriteToLogBox("Invalid Target-Number. " + trial.Szenario + ", Trail " + trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                        trial = null;
                    }
                }

                if (trial != null)
                {
                    trial.Handedness = Trial.HandednessEnum.RightHand;
                }

                if (trial != null && (trial.TrialNumberInSzenario < 1 || trial.TrialNumberInSzenario > 80))
                {
                    myManipAnalysisGui.WriteToLogBox("Invalid Trial-Number. " + trial.Szenario + ", Trail " + trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                    trial = null;
                }
            }

            return trial;
        }

        public override bool checkTrialCount(int trialCount)
        {
            return trialCount == TrialCount;
        }
    }
}