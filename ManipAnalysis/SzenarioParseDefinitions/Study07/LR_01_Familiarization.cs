using ManipAnalysis_v2.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManipAnalysis_v2.SzenarioParseDefinitions
{
    class LR_01_Familiarization : ISzenarioDefinition
    {
        public const string StudyName = "Study07";
        public const string SzenarioName = "LR_01_Familiarization";

        public override Trial setTrialMetadata(ManipAnalysisGui myManipAnalysisGui, Trial trial)
        {
            if (trial.Target.Number == 10 || trial.Target.Number == 20 || trial.Target.Number == 30) // Target 10/20/30 == StartTrial
            {
                myManipAnalysisGui.WriteToLogBox("Skipping Start-Trial, " + trial.Szenario + " Trail, " + trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                trial = null;
            }
            else
            {
                if ((trial.Target.Number >= 1 && trial.Target.Number <= 3) || (trial.Target.Number >= 11 && trial.Target.Number <= 13)) // NullField
                {
                    trial.Target.Number = trial.Target.Number;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if ((trial.Target.Number >= 4 && trial.Target.Number <= 6) || (trial.Target.Number >= 14 && trial.Target.Number <= 16)) // CW ForceField
                {
                    trial.Target.Number = trial.Target.Number - 3;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if ((trial.Target.Number >= 21 && trial.Target.Number <= 23) || (trial.Target.Number >= 31 && trial.Target.Number <= 33)) // ErrorClampTrial
                {
                    trial.Target.Number = trial.Target.Number - 20;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                else if ((trial.Target.Number >= 24 && trial.Target.Number <= 26) || (trial.Target.Number >= 34 && trial.Target.Number <= 36)) // ErrorClampTrial + CW ForceField
                {
                    trial.Target.Number = trial.Target.Number - 23;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                else if (trial.Target.Number >= 41 && trial.Target.Number <= 43) // 30s Pause
                {
                    trial.Target.Number = trial.Target.Number - 30;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if (trial.Target.Number >= 51 && trial.Target.Number <= 53) // 30s Pause + Wechsel R=>L
                {
                    trial.Target.Number = trial.Target.Number - 40;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if (trial.Target.Number >= 54 && trial.Target.Number <= 56) // 30s Pause + Wechsel L=>R
                {
                    trial.Target.Number = trial.Target.Number - 43;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if (trial.Target.Number >= 61 && trial.Target.Number <= 63) // 30s Pause + Wechsel R=>L + CW ForceField
                {
                    trial.Target.Number = trial.Target.Number - 50;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if (trial.Target.Number >= 64 && trial.Target.Number <= 66) // 30s Pause + Wechsel L=>R + CW ForceField
                {
                    trial.Target.Number = trial.Target.Number - 53;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if (trial.Target.Number >= 71 && trial.Target.Number <= 73) // 30s Pause + Wechsel R=>L + ErrorClampTrial
                {
                    trial.Target.Number = trial.Target.Number - 60;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                else if (trial.Target.Number >= 74 && trial.Target.Number <= 76) // 30s Pause + Wechsel L=>R + ErrorClampTrial
                {
                    trial.Target.Number = trial.Target.Number - 63;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                else
                {
                    myManipAnalysisGui.WriteToLogBox("Invalid Target-Number: " + trial.Target.Number);
                    trial = null;
                }


                if (trial.TrialNumberInSzenario < 1 || trial.TrialNumberInSzenario > 300)
                {
                    myManipAnalysisGui.WriteToLogBox("Invalid Trial-Number: " + trial.TrialNumberInSzenario);
                    trial = null;
                }
                else if (trial.TrialNumberInSzenario >= 1 && trial.TrialNumberInSzenario <= 48)
                {
                    trial.Szenario = "LR_Familiarization";
                    trial.Handedness = Trial.HandednessEnum.LeftHand;
                }
                else if (trial.TrialNumberInSzenario >= 49 && trial.TrialNumberInSzenario <= 96)
                {
                    trial.Szenario = "LR_Familiarization";
                    trial.Handedness = Trial.HandednessEnum.RightHand;
                }
                else if (trial.TrialNumberInSzenario >= 97 && trial.TrialNumberInSzenario <= 144)
                {
                    trial.Szenario = "LR_Familiarization";
                    trial.Handedness = Trial.HandednessEnum.LeftHand;
                }
                else if (trial.TrialNumberInSzenario >= 145 && trial.TrialNumberInSzenario <= 192)
                {
                    trial.Szenario = "LR_Familiarization";
                    trial.Handedness = Trial.HandednessEnum.RightHand;
                }
                else if (trial.TrialNumberInSzenario >= 193 && trial.TrialNumberInSzenario <= 240)
                {
                    trial.Szenario = "LR_Base1";
                    trial.Handedness = Trial.HandednessEnum.LeftHand;
                    trial.TrialNumberInSzenario = trial.TrialNumberInSzenario - 192;
                }
                else if (trial.TrialNumberInSzenario >= 241 && trial.TrialNumberInSzenario <= 288)
                {
                    trial.Szenario = "LR_Base1";
                    trial.Handedness = Trial.HandednessEnum.RightHand;
                    trial.TrialNumberInSzenario = trial.TrialNumberInSzenario - 192;
                }
                else if (trial.TrialNumberInSzenario >= 289 && trial.TrialNumberInSzenario <= 300)
                {
                    trial.Szenario = "LR_Base2";
                    trial.Handedness = Trial.HandednessEnum.RightHand;
                    trial.TrialNumberInSzenario = trial.TrialNumberInSzenario - 288;
                }
            }

            return trial;
        }
    }
}