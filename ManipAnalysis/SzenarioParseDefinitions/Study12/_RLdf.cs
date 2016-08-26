using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManipAnalysis_v2.MongoDb;

namespace ManipAnalysis_v2.SzenarioParseDefinitions.Study12
{
    internal class _RLdf : AbstractSzenarioDefinition
    {
        public new const string StudyName = "Study_12";

        public new const string SzenarioName = "RLdf";

        public override int TrialCount => 654;

        public override bool CheckValidTrialNumberInSzenarioSequence => true;

        public override Trial SetTrialMetadata(ManipAnalysisGui myManipAnalysisGui, Trial trial)
        {
            trial.Study = "Study 12";
            trial.Szenario = SzenarioName;

            if (trial.Target.Number == 10 || trial.Target.Number == 20) // Target 10/20 == StartTrial(RightHand/LeftHand)
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
                         (trial.Target.Number >= 14 && trial.Target.Number <= 16)) // NullField + Vicon
                {
                    trial.Target.Number = trial.Target.Number - 3;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if ((trial.Target.Number >= 7 && trial.Target.Number <= 9) ||
                         (trial.Target.Number >= 17 && trial.Target.Number <= 19)) // ForceChannel Null(ErrorClampTrial) + Vicon
                {
                    trial.Target.Number = trial.Target.Number - 6;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                else if ((trial.Target.Number >= 21 && trial.Target.Number <= 23) ||
                         (trial.Target.Number >= 31 && trial.Target.Number <= 33)) // ViscousForceField(ForceFieldCW)
                {
                    trial.Target.Number = trial.Target.Number - 20;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if ((trial.Target.Number >= 24 && trial.Target.Number <= 26) ||
                         (trial.Target.Number >= 34 && trial.Target.Number <= 36)) // ViscousForceField(ForceFieldCW) + Vicon
                {
                    trial.Target.Number = trial.Target.Number - 23;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if ((trial.Target.Number >= 27 && trial.Target.Number <= 29) ||
                         (trial.Target.Number >= 37 && trial.Target.Number <= 39)) // ForceChannel(ErrorClampTrial) VF + Vicon
                {
                    trial.Target.Number = trial.Target.Number - 26;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                else if ((trial.Target.Number >= 41 && trial.Target.Number <= 43) ||
                         (trial.Target.Number >= 51 && trial.Target.Number <= 53)) // DivergentForceField(ForceFieldDF)
                {
                    trial.Target.Number = trial.Target.Number - 40;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldDF;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if ((trial.Target.Number >= 44 && trial.Target.Number <= 46) ||
                         (trial.Target.Number >= 54 && trial.Target.Number <= 56)) // DivergentForceField(ForceFieldDF) + Vicon
                {
                    trial.Target.Number = trial.Target.Number - 43;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldDF;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if ((trial.Target.Number >= 47 && trial.Target.Number <= 49) ||
                         (trial.Target.Number >= 57 && trial.Target.Number <= 59)) // ForceChannel(ErrorclampTrial)DF + Vicon
                {
                    trial.Target.Number = trial.Target.Number - 46;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldDF;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                /*
                 * Will never happen in this study
                else if (trial.Target.Number == 47 || trial.Target.Number == 57) // CCW ForceField 6.5N + ErrorClamp + ContextCue
                {
                    trial.Target.Number = trial.Target.Number - 45;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                */
                /*
                 * Will never happen in this study
                else if (trial.Target.Number == 48 || trial.Target.Number == 58) // CCW ForceField 8.5N + ErrorClamp + ContextCue
                {
                    trial.Target.Number = trial.Target.Number - 46;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                */
                else if ((trial.Target.Number >= 61 && trial.Target.Number <= 63) ||
                         (trial.Target.Number >= 71 && trial.Target.Number <= 73)) // ForceChannel(ErrorclampTrial) DF
                {
                    trial.Target.Number = trial.Target.Number - 60;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldDF;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                else if ((trial.Target.Number >= 64 && trial.Target.Number <= 66)) // 30s Pause + NullField + Handwechsel -> L
                {
                    trial.Target.Number = trial.Target.Number - 53;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if ((trial.Target.Number >= 67 && trial.Target.Number <= 69)) //30s Pause + NullField + Handwechsel -> R
                {
                    trial.Target.Number = trial.Target.Number - 56;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if ((trial.Target.Number >= 74 && trial.Target.Number <= 76)) // 30s Pause + ViscousForceField(ForceFieldCW) + Vicon + Handwechsel -> L
                {
                    trial.Target.Number = trial.Target.Number - 63;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if ((trial.Target.Number >= 77 && trial.Target.Number <= 79)) // 30s Pause + ViscousForceField(ForceFieldCW) + Vicon + Handwechsel -> R
                {
                    trial.Target.Number = trial.Target.Number - 66;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if ((trial.Target.Number >= 84 && trial.Target.Number <= 86)) // 30s Pause + ForceFieldDF + Vicon + Handwechsel -> L
                {
                    trial.Target.Number = trial.Target.Number - 73;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldDF;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if ((trial.Target.Number >= 87 && trial.Target.Number <= 89)) // 30s Pause + ForceFieldDF + Vicon + Handwechsel -> R
                {
                    trial.Target.Number = trial.Target.Number - 76;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldDF;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else if ((trial.Target.Number >= 94 && trial.Target.Number <= 96)) // 30s Pause + NullField + ForceChannel(ErrorclampTrial) + Vicon + Handwechsel -> L
                {
                    trial.Target.Number = trial.Target.Number - 83;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                else if ((trial.Target.Number >= 97 && trial.Target.Number <= 99)) // 30s Pause + NullField + ForceChannel(ErrorclampTrial) + Vicon + Handwechsel -> R
                {
                    trial.Target.Number = trial.Target.Number - 86;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                /* 
                 * Not happening in this study
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
                    trial.Target.Number = trial.Target.Number - 98;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCCW;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
                }
                */
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

                if (trial != null && (trial.TrialNumberInSzenario < 1 || trial.TrialNumberInSzenario > TrialCount))
                {
                    myManipAnalysisGui.WriteToLogBox("Invalid Trial-Number. " + trial.Szenario + ", Trail " +
                                                     trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                    trial = null;
                }
                else if (trial.TrialNumberInSzenario >= 1 && trial.TrialNumberInSzenario <= 96)
                {
                    trial.Szenario = "RLdf_Familiarization";
                    if (trial.TrialNumberInSzenario >= 1 && trial.TrialNumberInSzenario <= 48) // Trials mit rechter Hand
                    {
                        trial.Handedness = Trial.HandednessEnum.RightHand;
                    }
                    else if (trial.TrialNumberInSzenario >= 49 && trial.TrialNumberInSzenario <= 96) // Trials mit linker Hand
                    {
                        trial.Handedness = Trial.HandednessEnum.LeftHand;
                    }
                }
                else if (trial.TrialNumberInSzenario >= 97 && trial.TrialNumberInSzenario <= 313)
                {
                    trial.Szenario = "RLdf_Base1";
                    if ((trial.TrialNumberInSzenario >= 97 && trial.TrialNumberInSzenario <= (97 + 48)) ||
                            (trial.TrialNumberInSzenario >= (97 + 97) && trial.TrialNumberInSzenario <= (97 + 144)) ||
                            (trial.TrialNumberInSzenario >= (97 + 205) && trial.TrialNumberInSzenario <= (97 + 216))) // Trials mit rechter Hand
                    {
                        trial.Handedness = Trial.HandednessEnum.RightHand;
                        if ((trial.TrialNumberInSzenario == (97 + 8)) || (trial.TrialNumberInSzenario == (97 + 20)) || (trial.TrialNumberInSzenario == (97 + 32)) ||
                            (trial.TrialNumberInSzenario == (97 + 43)) || (trial.TrialNumberInSzenario == (97 + 105)) || (trial.TrialNumberInSzenario == (97 + 119)) ||
                            (trial.TrialNumberInSzenario >= (97 + 133) && trial.TrialNumberInSzenario <= (97 + 144)) ||
                            (trial.TrialNumberInSzenario >= (97 + 205) && trial.TrialNumberInSzenario <= (97 + 216))) // Davon ViconTrials:
                        {
                            trial.Handedness = Trial.HandednessEnum.RightHandVicon;
                        }
                    }
                    else if ((trial.TrialNumberInSzenario >= (97 + 49) && trial.TrialNumberInSzenario <= (97 + 96)) ||
                          (trial.TrialNumberInSzenario >= (97 + 145) && trial.TrialNumberInSzenario <= (97 + 204))) // Trials mit linker Hand
                    {
                        trial.Handedness = Trial.HandednessEnum.LeftHand;
                        if ((trial.TrialNumberInSzenario == (97 + 56)) || (trial.TrialNumberInSzenario == (97 + 68)) || (trial.TrialNumberInSzenario == (97 + 80)) ||
                            (trial.TrialNumberInSzenario == (97 + 91)) || (trial.TrialNumberInSzenario == (97 + 153)) || (trial.TrialNumberInSzenario == (97 + 167)) ||
                            (trial.TrialNumberInSzenario >= (97 + 181) && trial.TrialNumberInSzenario <= (97 + 204))) // Davon ViconTrials:
                        {
                            trial.Handedness = Trial.HandednessEnum.LeftHandVicon;
                        }
                    }
                    trial.TrialNumberInSzenario = trial.TrialNumberInSzenario - 96;
                }
                else if (trial.TrialNumberInSzenario >= 314 && trial.TrialNumberInSzenario <= 482)
                {
                    trial.Szenario = "RLdf_Training";
                    trial.Handedness = Trial.HandednessEnum.RightHand;
                    if ((trial.TrialNumberInSzenario >= 314 && trial.TrialNumberInSzenario <= (314 + 24)) ||
                        (trial.TrialNumberInSzenario >= (314 + 43) && trial.TrialNumberInSzenario <= (314 + 54)) ||
                        (trial.TrialNumberInSzenario >= (314 + 73) && trial.TrialNumberInSzenario <= (314 + 84)) ||
                        (trial.TrialNumberInSzenario >= (314 + 109) && trial.TrialNumberInSzenario <= (314 + 120)) ||
                        (trial.TrialNumberInSzenario >= (314 + 157) && trial.TrialNumberInSzenario <= (314 + 168))) //Davon ViconTrials:
                    {
                        trial.Handedness = Trial.HandednessEnum.RightHandVicon;
                    }
                    trial.TrialNumberInSzenario = trial.TrialNumberInSzenario - 313;
                }
                else if (trial.TrialNumberInSzenario >= 483 && trial.TrialNumberInSzenario <= TrialCount)
                {
                    trial.Szenario = "RLdf_Generalization";
                    trial.Handedness = Trial.HandednessEnum.LeftHand;
                    if ((trial.TrialNumberInSzenario >= 483 && trial.TrialNumberInSzenario <= (483 + 6 + 24)) ||
                        (trial.TrialNumberInSzenario >= (483 + 6 + 43) && trial.TrialNumberInSzenario <= (483 + 6 + 54)) ||
                        (trial.TrialNumberInSzenario >= (483 + 6 + 73) && trial.TrialNumberInSzenario <= (483 + 6 + 84)) ||
                        (trial.TrialNumberInSzenario >= (483 + 6 + 109) && trial.TrialNumberInSzenario <= (483 + 6 + 120)) ||
                        (trial.TrialNumberInSzenario >= (483 + 6 + 157) && trial.TrialNumberInSzenario <= (483 + 6 + 168)))
                    {
                        trial.Handedness = Trial.HandednessEnum.LeftHandVicon;
                    }
                    trial.TrialNumberInSzenario = trial.TrialNumberInSzenario - 482;
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
