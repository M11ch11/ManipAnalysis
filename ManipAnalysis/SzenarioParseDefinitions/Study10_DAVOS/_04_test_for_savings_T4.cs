﻿using System;
using ManipAnalysis_v2.MongoDb;

namespace ManipAnalysis_v2.SzenarioParseDefinitions
{
    internal class _04_test_for_savings_T4 : AbstractSzenarioDefinition
    {
        public new const string StudyName = "Study_10_sleep";

        public new const string SzenarioName = "04_test_for_savings_T4";

        public override int TrialCount => 46; //Number of valid trials(without skipped/nulled Trials for PositionControl

        public override bool CheckValidTrialNumberInSzenarioSequence => false;

        public override Trial SetTrialMetadata(ManipAnalysisGui myManipAnalysisGui, Trial trial)
        {
            trial.Study = "Study 10_DAVOS";
            trial.Szenario = SzenarioName;
            trial.PositionOffset.Y -= 0.05;

            //Because some Subjects have to have a different ForcefieldFactor but you can not distinct them by their trial.group and out of lazyness
            //I did one switch case that works for all T1 - T6 and just copy pasta 
            int i = 15;
            switch (trial.Subject.PId)
            {
                //Group T1 and T5 from Sleepblocked and Wakeblocked with ff of 20nms
                case "DAVOS1015":
                case "DAVOS1007":
                case "DAVOS1029":
                case "DAVOS1017":
                case "DAVOS1046":
                case "DAVOS1041":
                case "DAVOS1085":
                case "DAVOS1095":
                    i = 20;
                    break;
                //Group T2 and T6 with Sleepblocked and Wakeblocked with ff of 10 nms
                case "DAVOS1053":
                case "DAVOS1011":
                case "DAVOS1018":
                case "DAVOS1010":
                case "DAVOS1055":
                case "DAVOS1054":
                case "DAVOS1089":
                case "DAVOS1090":
                    i = 10;
                    break;
                //Rest (T3 and T4) from Sleepblocked and Wakeblocked with ff of 15 nms
                default:
                    i = 15;
                    break;
            }

            if (trial.Target.Number == 10) // Target 10 == StartTrial
            {
                myManipAnalysisGui.WriteToLogBox("Skipping Start-Trial. " + trial.Szenario + ", Trail " +
                                                 trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                trial = null;
            }
            else
            {
                if (trial.Target.Number >= 1 && trial.Target.Number <= 6) // Null
                {
                    trial.Target.Number = trial.Target.Number;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if (trial.Target.Number >= 11 && trial.Target.Number <= 16) // Null, Position control, skip
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
                    trial.ForceFieldMatrix[0, 1] = i;
                    trial.ForceFieldMatrix[1, 0] = -i;
                    trial.ForceFieldMatrix[1, 1] = 0;
                }
                else if (trial.Target.Number >= 31 && trial.Target.Number <= 36) // CW medium
                {
                    trial.Target.Number = trial.Target.Number - 30;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                    //Hier stimmen die Werte?
                    trial.ForceFieldMatrix[0, 0] = 0;
                    trial.ForceFieldMatrix[0, 1] = i;
                    trial.ForceFieldMatrix[1, 0] = -i;
                    trial.ForceFieldMatrix[1, 1] = 0;
                }
                else if (trial.Target.Number >= 41 && trial.Target.Number <= 46) // CW strong
                {
                    trial.Target.Number = trial.Target.Number - 40;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                    //Sollte hier nicht 20 und -20 statt 15 stehen?
                    trial.ForceFieldMatrix[0, 0] = 0;
                    trial.ForceFieldMatrix[0, 1] = i;
                    trial.ForceFieldMatrix[1, 0] = -i;
                    trial.ForceFieldMatrix[1, 1] = 0;
                }
                else if (trial.Target.Number >= 51 && trial.Target.Number <= 56) // Nullfield, error clamp
                {
                    trial.Target.Number = trial.Target.Number - 50;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                    trial.ForceFieldMatrix[0, 0] = 0;
                    trial.ForceFieldMatrix[0, 1] = i;
                    trial.ForceFieldMatrix[1, 0] = -i;
                    trial.ForceFieldMatrix[1, 1] = 0;
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
                        trial.Target.Radius = 0.00175;
                        trial.Origin.XPos = 0;
                        trial.Origin.YPos = 0;
                        trial.Origin.ZPos = 0;
                        trial.Origin.Radius = 0.00175;
                    }
                    else if (trial.Target.Number == 2)
                    {
                        trial.Target.XPos = 0.1 * Math.Cos(DegreeToRadian(30));
                        trial.Target.YPos = 0.1 * Math.Sin(DegreeToRadian(30));
                        trial.Target.ZPos = 0;
                        trial.Target.Radius = 0.00175;
                        trial.Origin.XPos = 0;
                        trial.Origin.YPos = 0;
                        trial.Origin.ZPos = 0;
                        trial.Origin.Radius = 0.00175;
                    }
                    else if (trial.Target.Number == 3)
                    {
                        trial.Target.XPos = 0.1 * Math.Cos(DegreeToRadian(330));
                        trial.Target.YPos = 0.1 * Math.Sin(DegreeToRadian(330));
                        trial.Target.ZPos = 0;
                        trial.Target.Radius = 0.00175;
                        trial.Origin.XPos = 0;
                        trial.Origin.YPos = 0;
                        trial.Origin.ZPos = 0;
                        trial.Origin.Radius = 0.00175;
                    }
                    else if (trial.Target.Number == 4)
                    {
                        trial.Target.XPos = 0.1 * Math.Cos(DegreeToRadian(270));
                        trial.Target.YPos = 0.1 * Math.Sin(DegreeToRadian(270));
                        trial.Target.ZPos = 0;
                        trial.Target.Radius = 0.00175;
                        trial.Origin.XPos = 0;
                        trial.Origin.YPos = 0;
                        trial.Origin.ZPos = 0;
                        trial.Origin.Radius = 0.00175;
                    }
                    else if (trial.Target.Number == 5)
                    {
                        trial.Target.XPos = 0.1 * Math.Cos(DegreeToRadian(210));
                        trial.Target.YPos = 0.1 * Math.Sin(DegreeToRadian(210));
                        trial.Target.ZPos = 0;
                        trial.Target.Radius = 0.00175;
                        trial.Origin.XPos = 0;
                        trial.Origin.YPos = 0;
                        trial.Origin.ZPos = 0;
                        trial.Origin.Radius = 0.00175;
                    }
                    else if (trial.Target.Number == 6)
                    {
                        trial.Target.XPos = 0.1 * Math.Cos(DegreeToRadian(150));
                        trial.Target.YPos = 0.1 * Math.Sin(DegreeToRadian(150));
                        trial.Target.ZPos = 0;
                        trial.Target.Radius = 0.00175;
                        trial.Origin.XPos = 0;
                        trial.Origin.YPos = 0;
                        trial.Origin.ZPos = 0;
                        trial.Origin.Radius = 0.00175;
                    }
                    //These target numbers will not happen in our study
                    //else if (trial.Target.Number == 7)
                    //{
                    //    trial.Target.XPos = 0.1 * Math.Cos(DegreeToRadian(180));
                    //    trial.Target.YPos = 0.1 * Math.Sin(DegreeToRadian(180));
                    //    trial.Target.ZPos = 0;
                    //    trial.Target.Radius = 0.0025;
                    //    trial.Origin.XPos = 0;
                    //    trial.Origin.YPos = 0;
                    //    trial.Origin.ZPos = 0;
                    //    trial.Origin.Radius = 0.0025;
                    //}
                    //else if (trial.Target.Number == 8)
                    //{
                    //    trial.Target.XPos = 0.1 * Math.Cos(DegreeToRadian(135));
                    //    trial.Target.YPos = 0.1 * Math.Sin(DegreeToRadian(135));
                    //    trial.Target.ZPos = 0;
                    //    trial.Target.Radius = 0.0025;
                    //    trial.Origin.XPos = 0;
                    //    trial.Origin.YPos = 0;
                    //    trial.Origin.ZPos = 0;
                    //    trial.Origin.Radius = 0.0025;
                    //}
                    //else if (trial.Target.Number == 11)
                    //{
                    //    trial.Origin.XPos = 0.1 * Math.Cos(DegreeToRadian(90));
                    //    trial.Origin.YPos = 0.1 * Math.Sin(DegreeToRadian(90));
                    //    trial.Origin.ZPos = 0;
                    //    trial.Origin.Radius = 0.0025;
                    //    trial.Target.XPos = 0;
                    //    trial.Target.YPos = 0;
                    //    trial.Target.ZPos = 0;
                    //    trial.Target.Radius = 0.0025;
                    //}
                    //else if (trial.Target.Number == 12)
                    //{
                    //    trial.Origin.XPos = 0.1 * Math.Cos(DegreeToRadian(45));
                    //    trial.Origin.YPos = 0.1 * Math.Sin(DegreeToRadian(45));
                    //    trial.Origin.ZPos = 0;
                    //    trial.Origin.Radius = 0.0025;
                    //    trial.Target.XPos = 0;
                    //    trial.Target.YPos = 0;
                    //    trial.Target.ZPos = 0;
                    //    trial.Target.Radius = 0.0025;
                    //}
                    //else if (trial.Target.Number == 13)
                    //{
                    //    trial.Origin.XPos = 0.1 * Math.Cos(DegreeToRadian(0));
                    //    trial.Origin.YPos = 0.1 * Math.Sin(DegreeToRadian(0));
                    //    trial.Origin.ZPos = 0;
                    //    trial.Origin.Radius = 0.0025;
                    //    trial.Target.XPos = 0;
                    //    trial.Target.YPos = 0;
                    //    trial.Target.ZPos = 0;
                    //    trial.Target.Radius = 0.0025;
                    //}
                    //else if (trial.Target.Number == 14)
                    //{
                    //    trial.Origin.XPos = 0.1 * Math.Cos(DegreeToRadian(315));
                    //    trial.Origin.YPos = 0.1 * Math.Sin(DegreeToRadian(315));
                    //    trial.Origin.ZPos = 0;
                    //    trial.Origin.Radius = 0.0025;
                    //    trial.Target.XPos = 0;
                    //    trial.Target.YPos = 0;
                    //    trial.Target.ZPos = 0;
                    //    trial.Target.Radius = 0.0025;
                    //}
                    //else if (trial.Target.Number == 15)
                    //{
                    //    trial.Origin.XPos = 0.1 * Math.Cos(DegreeToRadian(270));
                    //    trial.Origin.YPos = 0.1 * Math.Sin(DegreeToRadian(270));
                    //    trial.Origin.ZPos = 0;
                    //    trial.Origin.Radius = 0.0025;
                    //    trial.Target.XPos = 0;
                    //    trial.Target.YPos = 0;
                    //    trial.Target.ZPos = 0;
                    //    trial.Target.Radius = 0.0025;
                    //}
                    //else if (trial.Target.Number == 16)
                    //{
                    //    trial.Origin.XPos = 0.1 * Math.Cos(DegreeToRadian(225));
                    //    trial.Origin.YPos = 0.1 * Math.Sin(DegreeToRadian(225));
                    //    trial.Origin.ZPos = 0;
                    //    trial.Origin.Radius = 0.0025;
                    //    trial.Target.XPos = 0;
                    //    trial.Target.YPos = 0;
                    //    trial.Target.ZPos = 0;
                    //    trial.Target.Radius = 0.0025;
                    //}
                    //else if (trial.Target.Number == 17)
                    //{
                    //    trial.Origin.XPos = 0.1 * Math.Cos(DegreeToRadian(180));
                    //    trial.Origin.YPos = 0.1 * Math.Sin(DegreeToRadian(180));
                    //    trial.Origin.ZPos = 0;
                    //    trial.Origin.Radius = 0.0025;
                    //    trial.Target.XPos = 0;
                    //    trial.Target.YPos = 0;
                    //    trial.Target.ZPos = 0;
                    //    trial.Target.Radius = 0.0025;
                    //}
                    //else if (trial.Target.Number == 18)
                    //{
                    //    trial.Origin.XPos = 0.1 * Math.Cos(DegreeToRadian(135));
                    //    trial.Origin.YPos = 0.1 * Math.Sin(DegreeToRadian(135));
                    //    trial.Origin.ZPos = 0;
                    //    trial.Origin.Radius = 0.0025;
                    //    trial.Target.XPos = 0;
                    //    trial.Target.YPos = 0;
                    //    trial.Target.ZPos = 0;
                    //    trial.Target.Radius = 0.0025;
                    //}
                    else
                    {
                        myManipAnalysisGui.WriteToLogBox("Invalid Target-Number. " + trial.Szenario + ", Trail " +
                                                         trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                        trial = null;
                    }
                }

                if (trial != null)
                {
                    trial.Handedness = Trial.HandednessEnum.RightHand;
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