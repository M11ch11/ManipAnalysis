﻿using System;
using ManipAnalysis_v2.MongoDb;

namespace ManipAnalysis_v2.SzenarioParseDefinitions
{
    internal class _05_test_for_transfer_LEFT_T5 : AbstractSzenarioDefinition
    {
        public new const string StudyName = "Study_10_sleep";

        public new const string SzenarioName = "05_test_for_transfer_LEFT_T5";

        public override int TrialCount => 46; //Number of valid trials(without skipped/nulled Trials for PositionControl

        public override bool CheckValidTrialNumberInSzenarioSequence => false;

        public override Trial SetTrialMetadata(ManipAnalysisGui myManipAnalysisGui, Trial trial)
        {
            trial.Study = "Study 10_DAVOS";
            trial.Szenario = SzenarioName;
            trial.PositionOffset.Y -= 0.05;

            if (trial.Target.Number == 10) // Target 10 == StartTrial, skip
            {
                myManipAnalysisGui.WriteToLogBox("Skipping Start-Trial. " + trial.Szenario + ", Trail " +
                                                 trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                trial = null;
            }
            else
            {
                if (trial.Target.Number >= 1 && trial.Target.Number <= 6) // Nullfield
                {
                    trial.Target.Number = trial.Target.Number;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if (trial.Target.Number >= 11 && trial.Target.Number <= 16) // Nullfield, Position control, skip
                {
                    trial = null;
                }
                else if (trial.Target.Number >= 21 && trial.Target.Number <= 26) // CW weak
                {
                    trial.Target.Number = trial.Target.Number - 20;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                    //sollte hier nicht 10  und -10 statt 15 stehen?
                    trial.ForceFieldMatrix[0, 0] = 0;
                    trial.ForceFieldMatrix[0, 1] = 15;
                    trial.ForceFieldMatrix[1, 0] = -15;
                    trial.ForceFieldMatrix[1, 1] = 0;
                }
                else if (trial.Target.Number >= 31 && trial.Target.Number <= 36) // CW medium
                {
                    trial.Target.Number = trial.Target.Number - 30;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                    //Hier stimmen die Werte?
                    trial.ForceFieldMatrix[0, 0] = 0;
                    trial.ForceFieldMatrix[0, 1] = 15;
                    trial.ForceFieldMatrix[1, 0] = -15;
                    trial.ForceFieldMatrix[1, 1] = 0;
                }
                else if (trial.Target.Number >= 41 && trial.Target.Number <= 46) // CW strong
                {
                    trial.Target.Number = trial.Target.Number - 40;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                    //Sollte hier nicht 20 und -20 statt 15 stehen?
                    trial.ForceFieldMatrix[0, 0] = 0;
                    trial.ForceFieldMatrix[0, 1] = 15;
                    trial.ForceFieldMatrix[1, 0] = -15;
                    trial.ForceFieldMatrix[1, 1] = 0;
                }
                else if (trial.Target.Number >= 51 && trial.Target.Number <= 56) // Nullfield, error clamp
                {
                    trial.Target.Number = trial.Target.Number - 50;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                    //trial.ForceFieldMatrix[0, 0] = 0;
                    //trial.ForceFieldMatrix[0, 1] = 15;
                    //trial.ForceFieldMatrix[1, 0] = -15;
                    //trial.ForceFieldMatrix[1, 1] = 0;
                }
                else if (trial.Target.Number >= 61 && trial.Target.Number <= 66) // Pause + Position control, skip
                {
                    trial = null;
                }
                else if (trial.Target.Number >= 71 && trial.Target.Number <= 76) // Position control, Null, skip
                {
                    trial = null;
                    //trial.Target.Number = trial.Target.Number - 70;
                    //trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    //trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                    // This is intentional! Same ForceFieldMatrix for CCW as for CW
                    //trial.ForceFieldMatrix[0, 0] = 0;
                    //trial.ForceFieldMatrix[0, 1] = 15;
                    //trial.ForceFieldMatrix[1, 0] = -15;
                    //trial.ForceFieldMatrix[1, 1] = 0;
                }
                else
                {
                    myManipAnalysisGui.WriteToLogBox("Invalid Target-Number. " + trial.Szenario + ", Trail " +
                                                     trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
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
                    else if (trial.Target.Number == 11)
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
                        myManipAnalysisGui.WriteToLogBox("Invalid Target-Number. " + trial.Szenario + ", Trail " +
                                                         trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                        trial = null;
                    }
                }

                if (trial != null)
                {
                    trial.Handedness = Trial.HandednessEnum.LeftHand;
                }

                if (trial != null && (trial.TrialNumberInSzenario < 1 || trial.TrialNumberInSzenario > 93)) //93 is the number of all trials in this szenario
                {
                    myManipAnalysisGui.WriteToLogBox("Invalid Trial-Number. " + trial.Szenario + ", Trail " +
                                                     trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                    trial = null;
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