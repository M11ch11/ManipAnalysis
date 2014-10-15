﻿using ManipAnalysis_v2.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManipAnalysis_v2.SzenarioParseDefinitions
{
    class RLd_02_Training_Transfer : ISzenarioDefinition
    {
        public const string StudyName = "Study07";
        public const string SzenarioName = "RL_02_RLd_Training&Transfer";

        public override Trial setTrialMetadata(ManipAnalysisGui myManipAnalysisGui, Trial trial)
        {
            if (trial.Target.Number == 10 || trial.Target.Number == 20 || trial.Target.Number == 30) // Target 10/20/30 == StartTrial
            {
                myManipAnalysisGui.WriteToLogBox("Skipping Start-Trial. " + trial.Szenario + ", Trail " + trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
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
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
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
                    myManipAnalysisGui.WriteToLogBox("Invalid Target-Number. " + trial.Szenario + ", Trail " + trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                    trial = null;
                }

                if (trial.Target.Number == 1)
                {
                    trial.Target.XPos = Math.Cos(DegreeToRadian(135));
                    trial.Target.YPos = Math.Sin(DegreeToRadian(135));
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.175;
                }
                else if (trial.Target.Number == 2)
                {
                    trial.Target.XPos = Math.Cos(DegreeToRadian(90));
                    trial.Target.YPos = Math.Sin(DegreeToRadian(90));
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.175;
                }
                else if (trial.Target.Number == 3)
                {
                    trial.Target.XPos = Math.Cos(DegreeToRadian(45));
                    trial.Target.YPos = Math.Sin(DegreeToRadian(45));
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.175;
                }
                else if (trial.Target.Number == 11 || trial.Target.Number == 12 || trial.Target.Number == 13)
                {
                    trial.Target.XPos = 0;
                    trial.Target.YPos = 0;
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.175;
                }

                if (trial.TrialNumberInSzenario < 1 || trial.TrialNumberInSzenario > 354)
                {
                    myManipAnalysisGui.WriteToLogBox("Invalid Trial-Number. " + trial.Szenario + ", Trail " + trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                    trial = null;
                }
                else if (trial.TrialNumberInSzenario >= 1 && trial.TrialNumberInSzenario <= 12)
                {
                    trial.Szenario = "RL_Base2b";
                    trial.Handedness = Trial.HandednessEnum.RightHand;
                }
                else if (trial.TrialNumberInSzenario >= 13 && trial.TrialNumberInSzenario <= 180)
                {
                    trial.Szenario = "RL_Training";
                    trial.Handedness = Trial.HandednessEnum.RightHand;
                    trial.TrialNumberInSzenario = trial.TrialNumberInSzenario - 12;
                }
                else if (trial.TrialNumberInSzenario >= 181 && trial.TrialNumberInSzenario <= 186)
                {
                    trial.Szenario = "RL_Direct-EC-Transfer";
                    trial.Handedness = Trial.HandednessEnum.LeftHand;
                    trial.TrialNumberInSzenario = trial.TrialNumberInSzenario - 180;
                }
                else if (trial.TrialNumberInSzenario >= 187 && trial.TrialNumberInSzenario <= 354)
                {
                    trial.Szenario = "RL_Direct-FF-Transfer";
                    trial.Handedness = Trial.HandednessEnum.LeftHand;
                    trial.TrialNumberInSzenario = trial.TrialNumberInSzenario - 186;
                }
            }

            return trial;
        }

        public override bool checkTrialCount(int trialCount)
        {
            return trialCount == 354;
        }
    }
}