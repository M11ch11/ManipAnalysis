using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManipAnalysis_v2.MongoDb;

namespace ManipAnalysis_v2.SzenarioParseDefinitions
{
    internal class RLvf : AbstractSzenarioDefinition
    {
        public new const string StudyName = "Study_12_HEiKA";

        public new const string SzenarioName = "RLvf";

        public override int TrialCount => 655; //655 = Anzahl an Trials ohne Pausen/Starttrials

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
                else if ((trial.Target.Number >= 81 && trial.Target.Number <= 83) ||
                         (trial.Target.Number >= 91 && trial.Target.Number <= 93)) // ForceChannel(ErrorClampTrial) + ViscousForceField(ForceFieldCW)
                {
                    trial.Target.Number = trial.Target.Number - 80;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.ForceFieldCW;
                    trial.TrialType = Trial.TrialTypeEnum.ErrorClampTrial;
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

                if (trial != null && (trial.TrialNumberInSzenario < 1 || trial.TrialNumberInSzenario > 660)) //660 = Anzahl an Trials insgesamt, inkl. Pausen und Starttrials
                {
                    myManipAnalysisGui.WriteToLogBox("Invalid Trial-Number. " + trial.Szenario + ", Trail " +
                                                     trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                    trial = null;
                }
                else if (trial.TrialNumberInSzenario >= 1 && trial.TrialNumberInSzenario <= 96)
                {
                    trial.Szenario = "RLvf_Familiarization";
                    if (trial.TrialNumberInSzenario >= 1 && trial.TrialNumberInSzenario <= 48) // Trials mit rechter Hand
                    {
                        trial.Handedness = Trial.HandednessEnum.RightHand;
                    } else if(trial.TrialNumberInSzenario >= 49 && trial.TrialNumberInSzenario <= 96) // Trials mit linker Hand
                    {
                        trial.Handedness = Trial.HandednessEnum.LeftHand;
                    }
                }
                else if (trial.TrialNumberInSzenario >= 97 && trial.TrialNumberInSzenario <= 314)
                {
                    trial.Szenario = "RLvf_Base1";
                    if ((trial.TrialNumberInSzenario >= 97 && trial.TrialNumberInSzenario <= 144) ||
                            (trial.TrialNumberInSzenario >= 193 && trial.TrialNumberInSzenario <= 240) ||
                            (trial.TrialNumberInSzenario >= 302 && trial.TrialNumberInSzenario <= 313)) // Trials mit rechter Hand
                    {
                        trial.Handedness = Trial.HandednessEnum.RightHand;
                        if ((trial.TrialNumberInSzenario == 104) || (trial.TrialNumberInSzenario == 116) || (trial.TrialNumberInSzenario == 128) ||
                            (trial.TrialNumberInSzenario == 139) || (trial.TrialNumberInSzenario == 201) || (trial.TrialNumberInSzenario == 215) ||
                            (trial.TrialNumberInSzenario >= 229 && trial.TrialNumberInSzenario <= 240) ||
                            (trial.TrialNumberInSzenario >= 302 && trial.TrialNumberInSzenario <= 313)) // Davon ViconTrials:
                        {
                            trial.Handedness = Trial.HandednessEnum.RightHandVicon;
                        }
                    } else if ((trial.TrialNumberInSzenario >= 145 && trial.TrialNumberInSzenario <= 192) ||
                            (trial.TrialNumberInSzenario >= 241 && trial.TrialNumberInSzenario <= 288)) // Trials mit linker Hand
                    {
                        trial.Handedness = Trial.HandednessEnum.LeftHand;
                        if ((trial.TrialNumberInSzenario == 152) || (trial.TrialNumberInSzenario == 164) || (trial.TrialNumberInSzenario == 176) ||
                            (trial.TrialNumberInSzenario == 187) || (trial.TrialNumberInSzenario == 249) || (trial.TrialNumberInSzenario == 263) ||
                            (trial.TrialNumberInSzenario >= 277 && trial.TrialNumberInSzenario <= 301)) // Davon ViconTrials:
                        {
                            trial.Handedness = Trial.HandednessEnum.LeftHandVicon;
                        }
                    }
                    if (trial.TrialNumberInSzenario >= 290 && trial.TrialNumberInSzenario <= 313)
                    {
                        trial.TrialNumberInSzenario -= 1;
                    }
                    trial.TrialNumberInSzenario = trial.TrialNumberInSzenario - 96;
                }
                else if (trial.TrialNumberInSzenario >= 315 && trial.TrialNumberInSzenario <= 483)
                {
                    trial.Szenario = "RLvf_Training";
                    trial.Handedness = Trial.HandednessEnum.RightHand;
                    if ((trial.TrialNumberInSzenario >= 315 && trial.TrialNumberInSzenario <= 339) ||
                        (trial.TrialNumberInSzenario >= 358 && trial.TrialNumberInSzenario <= 369) ||
                        (trial.TrialNumberInSzenario >= 388 && trial.TrialNumberInSzenario <= 399) ||
                        (trial.TrialNumberInSzenario >= 424 && trial.TrialNumberInSzenario <= 435) ||
                        (trial.TrialNumberInSzenario >= 472 && trial.TrialNumberInSzenario <= 483)) //Davon ViconTrials:
                    {
                        trial.Handedness = Trial.HandednessEnum.RightHandVicon;
                    }
                    if (trial.TrialNumberInSzenario >= 328 && trial.TrialNumberInSzenario <= 483)
                    {
                        trial.TrialNumberInSzenario -= 1;
                    }
                    trial.TrialNumberInSzenario = trial.TrialNumberInSzenario - 314;
                }
                else if (trial.TrialNumberInSzenario >= 484 && trial.TrialNumberInSzenario <= 660)
                {
                    trial.Szenario = "RLvf_Generalization";
                    trial.Handedness = Trial.HandednessEnum.LeftHand;
                    if ((trial.TrialNumberInSzenario >= 484 && trial.TrialNumberInSzenario <= 515) ||
                        (trial.TrialNumberInSzenario >= 534 && trial.TrialNumberInSzenario <= 545) ||
                        (trial.TrialNumberInSzenario >= 564 && trial.TrialNumberInSzenario <= 575) ||
                        (trial.TrialNumberInSzenario >= 600 && trial.TrialNumberInSzenario <= 611) ||
                        (trial.TrialNumberInSzenario >= 648 && trial.TrialNumberInSzenario <= 659))
                    {
                        trial.Handedness = Trial.HandednessEnum.LeftHandVicon;
                    }
                    if (trial.TrialNumberInSzenario >= 491 && trial.TrialNumberInSzenario <= 502)
                    {
                        trial.TrialNumberInSzenario -= 1;
                    } else if (trial.TrialNumberInSzenario >= 504 && trial.TrialNumberInSzenario <= 660)
                    {
                        trial.TrialNumberInSzenario -= 2;
                    }
                    trial.TrialNumberInSzenario = trial.TrialNumberInSzenario - 483;
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
