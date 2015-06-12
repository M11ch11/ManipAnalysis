using System;
using ManipAnalysis_v2.MongoDb;

namespace ManipAnalysis_v2.SzenarioParseDefinitions
{
    internal class Szenario42 : ISzenarioDefinition
    {
        public const string StudyName = "Study06";

        public const string SzenarioName = "Szenario42";

        public override Trial setTrialMetadata(ManipAnalysisGui myManipAnalysisGui, Trial trial)
        {
            if (trial.Target.Number == 17) // Target 17 == StartTrial
            {
                myManipAnalysisGui.WriteToLogBox("Skipping Start-Trial. " + trial.Szenario + ", Trail " + trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                trial = null;
            }
            else
            {
                if ((trial.Target.Number >= 1 && trial.Target.Number <= 8) || (trial.Target.Number >= 9 && trial.Target.Number <= 16)) // NullField
                {
                    trial.Target.Number = trial.Target.Number;
                    trial.ForceFieldType = Trial.ForceFieldTypeEnum.NullField;
                    trial.TrialType = Trial.TrialTypeEnum.StandardTrial;
                }
                else
                {
                    myManipAnalysisGui.WriteToLogBox("Invalid Target-Number. " + trial.Szenario + ", Trail " + trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                    trial = null;
                }

                if (trial.Target.Number == 1)
                {
                    trial.Target.XPos = 0;
                    trial.Target.YPos = 0.15;
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.25;
                }
                else if (trial.Target.Number == 2)
                {
                    trial.Target.XPos = 0.07071;
                    trial.Target.YPos = 0.12071;
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.25;
                }
                else if (trial.Target.Number == 3)
                {
                    trial.Target.XPos = 0.1;
                    trial.Target.YPos = 0.05;
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.25;
                }
                else if (trial.Target.Number == 4)
                {
                    trial.Target.XPos = 0.07071;
                    trial.Target.YPos = -0.02071;
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.25;
                }
                else if (trial.Target.Number == 5)
                {
                    trial.Target.XPos = 0;
                    trial.Target.YPos = -0.05;
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.25;
                }
                else if (trial.Target.Number == 6)
                {
                    trial.Target.XPos = -0.07071;
                    trial.Target.YPos = -0.02071;
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.25;
                }
                else if (trial.Target.Number == 7)
                {
                    trial.Target.XPos = -0.1;
                    trial.Target.YPos = 0.05;
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.25;
                }
                else if (trial.Target.Number == 8)
                {
                    trial.Target.XPos = -0.07071;
                    trial.Target.YPos = 0.12071;
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.25;
                }
                else if (trial.Target.Number >= 9 && trial.Target.Number <= 16)
                {
                    trial.Target.XPos = 0;
                    trial.Target.YPos = 0;
                    trial.Target.ZPos = 0;
                    trial.Target.Radius = 0.25;
                }

                trial.Handedness = Trial.HandednessEnum.RightHand;

                if (trial.TrialNumberInSzenario < 1 || trial.TrialNumberInSzenario > 96)
                {
                    myManipAnalysisGui.WriteToLogBox("Invalid Trial-Number. " + trial.Szenario + ", Trail " + trial.TrialNumberInSzenario + ", Target " + trial.Target.Number);
                    trial = null;
                }
            }

            return trial;
        }

        public override bool checkTrialCount(int trialCount)
        {
            return trialCount == 96;
        }
    }
}