using System;
using ManipAnalysis_v2.MongoDb;

namespace ManipAnalysis_v2.SzenarioParseDefinitions
{
    internal class C0_cw_01_Fam_Base_TrainA : ISzenarioDefinition
    {
        public new const string StudyName = "Study 08";

        public new const string SzenarioName = "C0_cw_01_Fam_Base_TrainA";

        public new const int TrialCount = 436;

        public override Trial SetTrialMetadata(ManipAnalysisGui myManipAnalysisGui, Trial trial)
        {
            trial.Study = StudyName;
            trial.Szenario = SzenarioName;

            if (trial.TrialNumberInSzenario >= 129 && trial.TrialNumberInSzenario <= 131 ||
                trial.TrialNumberInSzenario >= 176 && trial.TrialNumberInSzenario <= 179)
            {
                myManipAnalysisGui.WriteToLogBox("Skipping invalidated trial. " + trial.Szenario + ", Trail " +
                                                 trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                trial = null;
            }
            else if (trial.Target.Number == 10 || trial.Target.Number == 20 || trial.Target.Number == 30)
                // Target 10/20/30 == StartTrial
            {
                myManipAnalysisGui.WriteToLogBox("Skipping Start-Trial. " + trial.Szenario + ", Trail " +
                                                 trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                trial = null;
            }
            else
            {
                // Correcting TrialNumberInSzenario count
                if (trial.TrialNumberInSzenario > 131 && trial.TrialNumberInSzenario < 177)
                {
                    trial.TrialNumberInSzenario -= 3;
                }
                else if (trial.TrialNumberInSzenario > 179)
                {
                    trial.TrialNumberInSzenario -= 7;
                }

                if ((trial.Target.Number >= 1 && trial.Target.Number <= 3) ||
                    (trial.Target.Number >= 11 && trial.Target.Number <= 13)) // NullField
                {
                    trial.Target.Number = trial.Target.Number;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if ((trial.Target.Number >= 4 && trial.Target.Number <= 6) ||
                         (trial.Target.Number >= 14 && trial.Target.Number <= 16)) // CW ForceField
                {
                    trial.Target.Number = trial.Target.Number - 3;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                    trial.ForceFieldMatrix[0, 0] = 0;
                    trial.ForceFieldMatrix[0, 1] = 15;
                    trial.ForceFieldMatrix[1, 0] = -15;
                    trial.ForceFieldMatrix[1, 1] = 0;
                }
                else if ((trial.Target.Number >= 7 && trial.Target.Number <= 9) ||
                         (trial.Target.Number >= 17 && trial.Target.Number <= 19)) // CCW ForceField
                {
                    trial.Target.Number = trial.Target.Number - 6;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                    trial.ForceFieldMatrix[0, 0] = 0;
                    trial.ForceFieldMatrix[0, 1] = -15;
                    trial.ForceFieldMatrix[1, 0] = 15;
                    trial.ForceFieldMatrix[1, 1] = 0;
                }
                else if ((trial.Target.Number >= 21 && trial.Target.Number <= 23) ||
                         (trial.Target.Number >= 31 && trial.Target.Number <= 33)) // ErrorClampTrial
                {
                    trial.Target.Number = trial.Target.Number - 20;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                else if ((trial.Target.Number >= 24 && trial.Target.Number <= 26) ||
                         (trial.Target.Number >= 34 && trial.Target.Number <= 36))
                    // ErrorClampTrial + CW ForceField
                {
                    trial.Target.Number = trial.Target.Number - 23;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                    trial.ForceFieldMatrix[0, 0] = 0;
                    trial.ForceFieldMatrix[0, 1] = 15;
                    trial.ForceFieldMatrix[1, 0] = -15;
                    trial.ForceFieldMatrix[1, 1] = 0;
                }
                else if ((trial.Target.Number >= 27 && trial.Target.Number <= 29) ||
                         (trial.Target.Number >= 37 && trial.Target.Number <= 39))
                    // ErrorClampTrial + CCW ForceField
                {
                    trial.Target.Number = trial.Target.Number - 26;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                    trial.ForceFieldMatrix[0, 0] = 0;
                    trial.ForceFieldMatrix[0, 1] = -15;
                    trial.ForceFieldMatrix[1, 0] = 15;
                    trial.ForceFieldMatrix[1, 1] = 0;
                }
                else if (trial.Target.Number >= 41 && trial.Target.Number <= 43) // 30s Pause CW
                {
                    trial.Target.Number = trial.Target.Number - 30;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                    trial.ForceFieldMatrix[0, 0] = 0;
                    trial.ForceFieldMatrix[0, 1] = 15;
                    trial.ForceFieldMatrix[1, 0] = -15;
                    trial.ForceFieldMatrix[1, 1] = 0;
                }
                else if (trial.Target.Number >= 44 && trial.Target.Number <= 46) // 30s Pause CCW
                {
                    trial.Target.Number = trial.Target.Number - 33;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                    trial.ForceFieldMatrix[0, 0] = 0;
                    trial.ForceFieldMatrix[0, 1] = -15;
                    trial.ForceFieldMatrix[1, 0] = 15;
                    trial.ForceFieldMatrix[1, 1] = 0;
                }
                else if (trial.Target.Number >= 51 && trial.Target.Number <= 53) // 30s Pause + Wechsel R=>L
                {
                    trial.Target.Number = trial.Target.Number - 40;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if (trial.Target.Number >= 54 && trial.Target.Number <= 56) // 30s Pause + Wechsel L=>R
                {
                    trial.Target.Number = trial.Target.Number - 43;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if (trial.Target.Number >= 61 && trial.Target.Number <= 63)
                    // 30s Pause + Wechsel R=>L + CW ForceField
                {
                    trial.Target.Number = trial.Target.Number - 50;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                    trial.ForceFieldMatrix[0, 0] = 0;
                    trial.ForceFieldMatrix[0, 1] = 15;
                    trial.ForceFieldMatrix[1, 0] = -15;
                    trial.ForceFieldMatrix[1, 1] = 0;
                }
                else if (trial.Target.Number >= 64 && trial.Target.Number <= 66)
                    // 30s Pause + Wechsel L=>R + CW ForceField
                {
                    trial.Target.Number = trial.Target.Number - 53;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                    trial.ForceFieldMatrix[0, 0] = 0;
                    trial.ForceFieldMatrix[0, 1] = 15;
                    trial.ForceFieldMatrix[1, 0] = -15;
                    trial.ForceFieldMatrix[1, 1] = 0;
                }
                else if (trial.Target.Number >= 71 && trial.Target.Number <= 73)
                    // 30s Pause + Wechsel R=>L + ErrorClampTrial
                {
                    trial.Target.Number = trial.Target.Number - 60;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                else if (trial.Target.Number >= 74 && trial.Target.Number <= 76)
                    // 30s Pause + Wechsel L=>R + ErrorClampTrial
                {
                    trial.Target.Number = trial.Target.Number - 63;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                else if (trial.Target.Number >= 81 && trial.Target.Number <= 83)
                    // 30s Pause + Wechsel R=>L + CCW ForceField
                {
                    trial.Target.Number = trial.Target.Number - 70;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                    trial.ForceFieldMatrix[0, 0] = 0;
                    trial.ForceFieldMatrix[0, 1] = -15;
                    trial.ForceFieldMatrix[1, 0] = 15;
                    trial.ForceFieldMatrix[1, 1] = 0;
                }
                else if (trial.Target.Number >= 84 && trial.Target.Number <= 86)
                    // 30s Pause + Wechsel L=>R + CCW ForceField
                {
                    trial.Target.Number = trial.Target.Number - 73;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                    trial.ForceFieldMatrix[0, 0] = 0;
                    trial.ForceFieldMatrix[0, 1] = -15;
                    trial.ForceFieldMatrix[1, 0] = 15;
                    trial.ForceFieldMatrix[1, 1] = 0;
                }
                else
                {
                    myManipAnalysisGui.WriteToLogBox("Invalid Target-Number. " +
                                                     trial.Szenario + ", Trail " +
                                                     trial.TrialNumberInSzenario +
                                                     ", Target " + trial.Target.Number);
                    trial = null;
                }

                if (trial.Target.Number == 1)
                {
                    trial.Target.XPos = 0.1*Math.Cos(DegreeToRadian(135));
                    trial.Target.YPos = 0.1*Math.Sin(DegreeToRadian(135));
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.00175;
                    trial.Origin.XPos = 0;
                    trial.Origin.YPos = 0;
                    trial.Origin.ZPos = 0;
                    trial.Origin.Radius = 0.00175;
                }
                else if (trial.Target.Number == 2)
                {
                    trial.Target.XPos = 0.1*Math.Cos(DegreeToRadian(90));
                    trial.Target.YPos = 0.1*Math.Sin(DegreeToRadian(90));
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.00175;
                    trial.Origin.XPos = 0;
                    trial.Origin.YPos = 0;
                    trial.Origin.ZPos = 0;
                    trial.Origin.Radius = 0.00175;
                }
                else if (trial.Target.Number == 3)
                {
                    trial.Target.XPos = 0.1*Math.Cos(DegreeToRadian(45));
                    trial.Target.YPos = 0.1*Math.Sin(DegreeToRadian(45));
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.00175;
                    trial.Origin.XPos = 0;
                    trial.Origin.YPos = 0;
                    trial.Origin.ZPos = 0;
                    trial.Origin.Radius = 0.00175;
                }
                if (trial.Target.Number == 11)
                {
                    trial.Origin.XPos = 0.1*Math.Cos(DegreeToRadian(135));
                    trial.Origin.YPos = 0.1*Math.Sin(DegreeToRadian(135));
                    trial.Origin.ZPos = 0;
                    trial.Origin.Radius = 0.00175;
                    trial.Target.XPos = 0;
                    trial.Target.YPos = 0;
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.00175;
                }
                else if (trial.Target.Number == 12)
                {
                    trial.Origin.XPos = 0.1*Math.Cos(DegreeToRadian(90));
                    trial.Origin.YPos = 0.1*Math.Sin(DegreeToRadian(90));
                    trial.Origin.ZPos = 0;
                    trial.Origin.Radius = 0.00175;
                    trial.Target.XPos = 0;
                    trial.Target.YPos = 0;
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.00175;
                }
                else if (trial.Target.Number == 13)
                {
                    trial.Origin.XPos = 0.1*Math.Cos(DegreeToRadian(45));
                    trial.Origin.YPos = 0.1*Math.Sin(DegreeToRadian(45));
                    trial.Origin.ZPos = 0;
                    trial.Origin.Radius = 0.00175;
                    trial.Target.XPos = 0;
                    trial.Target.YPos = 0;
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.00175;
                }

                if (trial.TrialNumberInSzenario < 1 || trial.TrialNumberInSzenario > TrialCount)
                {
                    myManipAnalysisGui.WriteToLogBox("Invalid Trial-Number. " + trial.Szenario + ", Trail " +
                                                     trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                    trial = null;
                }
                else if (trial.TrialNumberInSzenario >= 1 && trial.TrialNumberInSzenario <= 42)
                {
                    trial.Szenario = "Familiarization";
                    trial.Handedness = Trial.HandednessEnum.RightHand;
                }
                else if (trial.TrialNumberInSzenario >= 43 && trial.TrialNumberInSzenario <= 84)
                {
                    trial.Szenario = "Familiarization";
                    trial.Handedness = Trial.HandednessEnum.LeftHand;
                }
                else if (trial.TrialNumberInSzenario >= 85 && trial.TrialNumberInSzenario <= 128)
                {
                    trial.Szenario = "Base1";
                    trial.Handedness = Trial.HandednessEnum.RightHand;
                    trial.TrialNumberInSzenario = trial.TrialNumberInSzenario - 84;
                }
                else if (trial.TrialNumberInSzenario >= 129 && trial.TrialNumberInSzenario <= 172)
                {
                    trial.Szenario = "Base1";
                    trial.Handedness = Trial.HandednessEnum.LeftHand;
                    trial.TrialNumberInSzenario = trial.TrialNumberInSzenario - 84;
                }
                else if (trial.TrialNumberInSzenario >= 173 && trial.TrialNumberInSzenario <= 220)
                {
                    trial.Szenario = "Base1";
                    trial.Handedness = Trial.HandednessEnum.RightHand;
                    trial.TrialNumberInSzenario = trial.TrialNumberInSzenario - 84;
                }
                else if (trial.TrialNumberInSzenario >= 221 && trial.TrialNumberInSzenario <= 268)
                {
                    trial.Szenario = "Base1";
                    trial.Handedness = Trial.HandednessEnum.LeftHand;
                    trial.TrialNumberInSzenario = trial.TrialNumberInSzenario - 84;
                }
                else if (trial.TrialNumberInSzenario >= 269 && trial.TrialNumberInSzenario <= 280)
                {
                    trial.Szenario = "Base2";
                    trial.Handedness = Trial.HandednessEnum.LeftHand;
                    trial.TrialNumberInSzenario = trial.TrialNumberInSzenario - 268;
                }
                else if (trial.TrialNumberInSzenario >= 281 && trial.TrialNumberInSzenario <= 292)
                {
                    trial.Szenario = "Base2";
                    trial.Handedness = Trial.HandednessEnum.RightHand;
                    trial.TrialNumberInSzenario = trial.TrialNumberInSzenario - 268;
                }
                else if (trial.TrialNumberInSzenario >= 293 && trial.TrialNumberInSzenario <= TrialCount)
                {
                    trial.Szenario = "Training";
                    trial.Handedness = Trial.HandednessEnum.RightHand;
                    trial.TrialNumberInSzenario = trial.TrialNumberInSzenario - 292;
                }
            }

            return trial;
        }

        public override bool CheckTrialCount(int trialCount)
        {
            return trialCount == TrialCount;
        }
    }
}