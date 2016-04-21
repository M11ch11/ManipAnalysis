using System;
using ManipAnalysis_v2.MongoDb;

namespace ManipAnalysis_v2.SzenarioParseDefinitions
{
    internal class _01_Adapt_T4T3_Day1 : AbstractSzenarioDefinition
    {
        public new const string StudyName = "Study_11";

        public new const string SzenarioName = "01_Adapt_T4T3_Day1";

        public override int TrialCount => 872;

        public override bool CheckValidTrialNumberInSzenarioSequence => true;

        public override Trial SetTrialMetadata(ManipAnalysisGui myManipAnalysisGui, Trial trial)
        {
            trial.Study = "Study 11";
            trial.Szenario = SzenarioName;

            if (trial.Target.Number == 10) // Target 10 == StartTrial
            {
                myManipAnalysisGui.WriteToLogBox("Skipping Start-Trial. " + trial.Szenario + ", Trail " +
                                                 trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                trial = null;
            }
            else
            {
                if ((trial.Target.Number >= 1 && trial.Target.Number <= 3) ||
                    (trial.Target.Number >= 11 && trial.Target.Number <= 13)) // NullField
                {
                    trial.Target.Number = trial.Target.Number;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if ((trial.Target.Number >= 4 && trial.Target.Number <= 6) ||
                         (trial.Target.Number >= 14 && trial.Target.Number <= 16)) // CCW ForceField + ContextCue
                {
                    trial.Target.Number = trial.Target.Number - 3;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if ((trial.Target.Number >= 7 && trial.Target.Number <= 9) ||
                         (trial.Target.Number >= 17 && trial.Target.Number <= 19)) // NullField + ContextCue
                {
                    trial.Target.Number = trial.Target.Number - 6;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if ((trial.Target.Number >= 21 && trial.Target.Number <= 23) ||
                         (trial.Target.Number >= 31 && trial.Target.Number <= 33)) // NullField + ErrorClampTrial + ContextCue
                {
                    trial.Target.Number = trial.Target.Number - 20;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                else if ((trial.Target.Number >= 24 && trial.Target.Number <= 26) ||
                         (trial.Target.Number >= 34 && trial.Target.Number <= 36)) // ErrorClampTrial + CCW ForceField + ContextCue
                {
                    trial.Target.Number = trial.Target.Number - 23;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                else if ((trial.Target.Number >= 41 && trial.Target.Number <= 43) ||
                         (trial.Target.Number >= 51 && trial.Target.Number <= 53)) // CCW ForceField 6.5N + ContextCue
                {
                    trial.Target.Number = trial.Target.Number - 40;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if ((trial.Target.Number >= 44 && trial.Target.Number <= 46) ||
                         (trial.Target.Number >= 54 && trial.Target.Number <= 56)) // CCW ForceField 8.5N + ContextCue
                {
                    trial.Target.Number = trial.Target.Number - 43;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if (trial.Target.Number == 47 || trial.Target.Number == 57) // CCW ForceField 6.5N + ErrorClamp + ContextCue
                {
                    trial.Target.Number = trial.Target.Number - 45;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                else if (trial.Target.Number == 48 || trial.Target.Number == 58) // CCW ForceField 8.5N + ErrorClamp + ContextCue
                {
                    trial.Target.Number = trial.Target.Number - 46;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                else if (trial.Target.Number == 95) // 30s Pause + CCW ForceField 6.5N + ErrorClamp + ContextCue
                {
                    trial.Target.Number = trial.Target.Number - 83;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                else if (trial.Target.Number == 96) // 30s Pause + CCW ForceField 8.5N + ErrorClamp + ContextCue
                {
                    trial.Target.Number = trial.Target.Number - 84;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                else if (trial.Target.Number == 97) // 30s Pause + CCW Variable ForceField + ContextCue
                {
                    trial.Target.Number = trial.Target.Number - 84;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if (trial.Target.Number == 98) // 30s Pause + CCW ForceField + ContextCue
                {
                    trial.Target.Number = trial.Target.Number - 85;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if (trial.Target.Number == 99) // 3s Pause + CCW ForceField 6.5N + ErrorClamp + ContextCue
                {
                    trial.Target.Number = trial.Target.Number - 97;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                else if (trial.Target.Number == 100) // 3s Pause + CCW ForceField 8.5N + ErrorClamp + ContextCue
                {
                    trial.Target.Number = trial.Target.Number - 97;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                else
                {
                    myManipAnalysisGui.WriteToLogBox("Invalid Target-Number. " + trial.Szenario + ", Trail " +
                        trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                    trial = null;
                }

                if (trial.Target.Number == 1)
                {
                    trial.Target.XPos = 0.1 * Math.Cos(DegreeToRadian(135));
                    trial.Target.YPos = 0.1 * Math.Sin(DegreeToRadian(135));
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.00175;
                    trial.Origin.XPos = 0;
                    trial.Origin.YPos = 0;
                    trial.Origin.ZPos = 0;
                    trial.Origin.Radius = 0.00175;
                }
                else if (trial.Target.Number == 2)
                {
                    trial.Target.XPos = 0.1 * Math.Cos(DegreeToRadian(90));
                    trial.Target.YPos = 0.1 * Math.Sin(DegreeToRadian(90));
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.00175;
                    trial.Origin.XPos = 0;
                    trial.Origin.YPos = 0;
                    trial.Origin.ZPos = 0;
                    trial.Origin.Radius = 0.00175;
                }
                else if (trial.Target.Number == 3)
                {
                    trial.Target.XPos = 0.1 * Math.Cos(DegreeToRadian(45));
                    trial.Target.YPos = 0.1 * Math.Sin(DegreeToRadian(45));
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.00175;
                    trial.Origin.XPos = 0;
                    trial.Origin.YPos = 0;
                    trial.Origin.ZPos = 0;
                    trial.Origin.Radius = 0.00175;
                }
                else if (trial.Target.Number == 11)
                {
                    trial.Origin.XPos = 0.1 * Math.Cos(DegreeToRadian(135));
                    trial.Origin.YPos = 0.1 * Math.Sin(DegreeToRadian(135));
                    trial.Origin.ZPos = 0;
                    trial.Origin.Radius = 0.00175;
                    trial.Target.XPos = 0;
                    trial.Target.YPos = 0;
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.00175;
                }
                else if (trial.Target.Number == 12)
                {
                    trial.Origin.XPos = 0.1 * Math.Cos(DegreeToRadian(90));
                    trial.Origin.YPos = 0.1 * Math.Sin(DegreeToRadian(90));
                    trial.Origin.ZPos = 0;
                    trial.Origin.Radius = 0.00175;
                    trial.Target.XPos = 0;
                    trial.Target.YPos = 0;
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.00175;
                }
                else if (trial.Target.Number == 13)
                {
                    trial.Origin.XPos = 0.1 * Math.Cos(DegreeToRadian(45));
                    trial.Origin.YPos = 0.1 * Math.Sin(DegreeToRadian(45));
                    trial.Origin.ZPos = 0;
                    trial.Origin.Radius = 0.00175;
                    trial.Target.XPos = 0;
                    trial.Target.YPos = 0;
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.00175;
                }
                else
                {
                    myManipAnalysisGui.WriteToLogBox("Invalid Target-Number. " + trial.Szenario + ", Trail " +
                                                     trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                    trial = null;
                }

                if (trial != null)
                {
                    trial.Handedness = Trial.HandednessEnum.RightHand;
                }

                if (trial != null && (trial.TrialNumberInSzenario < 1 || trial.TrialNumberInSzenario > TrialCount))
                {
                    myManipAnalysisGui.WriteToLogBox("Invalid Trial-Number. " + trial.Szenario + ", Trail " +
                                                     trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                    trial = null;
                }
                else if (trial.TrialNumberInSzenario >= 1 && trial.TrialNumberInSzenario <= 72)
                {
                    trial.Szenario = "Familiarization";
                }
                else if (trial.TrialNumberInSzenario >= 73 && trial.TrialNumberInSzenario <= 148)
                {
                    trial.Szenario = "Baseline";
                    trial.TrialNumberInSzenario = trial.TrialNumberInSzenario - 72;
                }
                else if (trial.TrialNumberInSzenario >= 149 && trial.TrialNumberInSzenario <= 616)
                {
                    trial.Szenario = "Practise";
                    trial.TrialNumberInSzenario = trial.TrialNumberInSzenario - 148;
                }
                else if (trial.TrialNumberInSzenario >= 617 && trial.TrialNumberInSzenario <= 872)
                {
                    trial.Szenario = "TransferEarly";
                    trial.TrialNumberInSzenario = trial.TrialNumberInSzenario - 616;
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