using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ManipAnalysis_v2.MongoDb;

namespace ManipAnalysis_v2.SzenarioParseDefinitions
{
    internal abstract class AbstractSzenarioDefinition
    {
        public const string StudyName = "Unknown";

        public const string SzenarioName = "Unknown";

        public virtual int TrialCount => 0;

        public virtual bool CheckForConsecutiveTrialNumberSequence => true;

        public List<Trial> ParseMeasureFile(ManipAnalysisGui myManipAnalysisGui, string[] c3DFiles,
            DateTime measureFileCreationDateTime, string measureFileHash, string measureFilePath, string probandId,
            string groupName, string studyName, string szenarioName, Vector3 offset)
        {
            var trialsContainer = new List<Trial>();

            Parallel.For(0, c3DFiles.Length, filesCounter =>
            {
                try
                {
                    var c3DReader = new C3DReader();
                    c3DReader.Open(c3DFiles[filesCounter]);

                    var currentTrial = new Trial();
                    var measureFileContainer = new MeasureFileContainer();
                    var subjectContainer = new SubjectContainer();
                    var targetContainer = new TargetContainer();
                    var originContainer = new TargetContainer();

                    var startTime = c3DReader.GetParameter<string[]>("TRIAL:TIME")[0];
                    /*
                    var eventTimes = c3DReader.GetParameter<float[]>("EVENTS:TIMES");
                    var eventLabels = c3DReader.GetParameter<string[]>("EVENTS:LABELS");
                    */
                    var frameTimeInc = 1.0f / c3DReader.Header.FrameRate;
                    int targetTrialNumber = c3DReader.GetParameter<short>("TRIAL:TP_NUM");
                    // -1 == Compensation of first Trial
                    var szenarioTrialNumber = c3DReader.GetParameter<short>("TRIAL:TRIAL_NUM") - 1;
                    int targetNumber = c3DReader.GetParameter<short>("TRIAL:TP");

                    measureFileContainer.CreationTime = measureFileCreationDateTime;
                    measureFileContainer.FileHash = measureFileHash;
                    measureFileContainer.FileName = Path.GetFileName(measureFilePath);

                    subjectContainer.PId = probandId;

                    targetContainer.Number = targetNumber;

                    currentTrial.StartDateTimeOfTrialRecording = DateTime.Parse(startTime);
                    currentTrial.MeasuredForcesRaw = new List<ForceContainer>();
                    currentTrial.MomentForcesRaw = new List<ForceContainer>();
                    currentTrial.PositionRaw = new List<PositionContainer>();
                    currentTrial.Group = groupName;
                    currentTrial.MeasureFile = measureFileContainer;
                    currentTrial.Study = studyName;
                    currentTrial.Subject = subjectContainer;
                    currentTrial.Szenario = szenarioName;
                    currentTrial.Target = targetContainer;
                    currentTrial.Origin = originContainer;
                    currentTrial.TargetTrialNumberInSzenario = targetTrialNumber;
                    currentTrial.RawDataSampleRate = Convert.ToInt32(c3DReader.Header.FrameRate);
                    currentTrial.TrialNumberInSzenario = szenarioTrialNumber;
                    currentTrial.TrialVersion = "KINARM_1.0";
                    currentTrial.PositionOffset.X = offset.X;
                    currentTrial.PositionOffset.Y = offset.Y;

                    for (var frame = 0; frame < c3DReader.FramesCount; frame++)
                    {
                        var measuredForcesRaw = new ForceContainer();
                        var momentForcesRaw = new ForceContainer();
                        var positionRaw = new PositionContainer();
                        double timeOffset = frameTimeInc * frame;
                        var timeStamp = DateTime.Parse(startTime).AddSeconds(timeOffset);

                        // Returns an array of all points, it is necessary to call this method in each cycle
                        var positionDataVector = c3DReader.ReadFrame()[0];
                        // [0] == Right Hand

                        /*
                        <Event code="1" name="TRIAL_STARTED"  desc="Trial has started" />
                        <Event code="2" name="SUBJECT_IS_IN_FIRST_TARGET"  desc="Subject is in the first target" />
                        <Event code="3" name="SUBJECT_HAS_LEFT_FIRST_TARGET"  desc="Subject has left the first target" />
                        <Event code="4" name="SUBJECT_IS_IN_SECOND_TARGET"  desc="Subject is in the second target" />
                        <Event code="5" name="SUBJECT_HAS_LEFT_SECOND_TARGET"  desc="Subject has left the second target" />
                        <Event code="6" name="TRIAL_ENDED"  desc="Trial has ended" /> 
                       */
                        var positionStatus = Convert.ToInt32(c3DReader.AnalogData["ACH4", 0]) - 2;

                        positionRaw.PositionStatus = positionStatus;
                        positionRaw.TimeStamp = timeStamp;
                        positionRaw.X = positionDataVector.X + currentTrial.PositionOffset.X;
                        positionRaw.Y = positionDataVector.Y + currentTrial.PositionOffset.Y;
                        positionRaw.Z = positionDataVector.Z + currentTrial.PositionOffset.Z;

                        // Get analog data for this frame
                        measuredForcesRaw.PositionStatus = positionStatus;
                        measuredForcesRaw.TimeStamp = timeStamp;
                        measuredForcesRaw.X = c3DReader.AnalogData["Right_FS_ForceX", 0];
                        measuredForcesRaw.Y = c3DReader.AnalogData["Right_FS_ForceY", 0];
                        measuredForcesRaw.Z = c3DReader.AnalogData["Right_FS_ForceZ", 0];

                        momentForcesRaw.PositionStatus = positionStatus;
                        momentForcesRaw.TimeStamp = timeStamp;
                        momentForcesRaw.X = c3DReader.AnalogData["Right_FS_TorqueX", 0];
                        momentForcesRaw.Y = c3DReader.AnalogData["Right_FS_TorqueY", 0];
                        momentForcesRaw.Z = c3DReader.AnalogData["Right_FS_TorqueZ", 0];

                        // Fill Trial
                        currentTrial.MeasuredForcesRaw.Add(measuredForcesRaw);
                        currentTrial.MomentForcesRaw.Add(momentForcesRaw);
                        currentTrial.PositionRaw.Add(positionRaw);
                    }

                    currentTrial = SetTrialMetadata(myManipAnalysisGui, currentTrial);
                    if (currentTrial != null)
                    {
                        if (currentTrial.MeasuredForcesRaw.Count == 0)
                        {
                            throw new Exception("No data frames found in szenario trial " +
                                                currentTrial.TrialNumberInSzenario);
                        }
                        lock (trialsContainer)
                        {
                            trialsContainer.Add(currentTrial);
                        }
                    }

                    // Don't forget to close the reader
                    c3DReader.Close();
                }
                catch (Exception ex)
                {
                    myManipAnalysisGui.WriteToLogBox("Error parsing c3d-file:\n" + ex.ToString());
                    trialsContainer.Clear();
                }
            });

            if (!CheckTrialCount(trialsContainer.Count))
            {
                myManipAnalysisGui.WriteToLogBox("Invalid TrialCount (" + trialsContainer.Count + ") in file " +
                                                 measureFilePath + "\nSkipping File.");
                trialsContainer.Clear();
            }

            // Check for valid TrialNumberInSzenario Sequence
            if (trialsContainer.Any())
            {
                foreach (var szenario in trialsContainer.Select(t => t.Szenario).Distinct())
                {
                    var trialNumberList =
                        trialsContainer.Where(t => t.Szenario == szenario).Select(t => t.TrialNumberInSzenario);
                    if (!IsValidTrialNumberInSzenarioSequence(trialNumberList))
                    {
                        myManipAnalysisGui.WriteToLogBox("Invalid TrialNumberInSzenario Sequence in szenario \"" +
                                                         szenario + "\" in file " + measureFilePath + "\nSkipping File.");
                        trialsContainer.Clear();
                        break;
                    }
                }
            }

            // Set TargetTrialNumberInSzenario Field 
            if (trialsContainer.Any())
            {
                foreach (var szenario in trialsContainer.Select(t => t.Szenario).Distinct())
                {
                    foreach (
                        var target in
                            trialsContainer.Where(t => t.Szenario == szenario).Select(t => t.Target.Number).Distinct())
                    {
                        var tempList =
                            trialsContainer.Where(t => t.Szenario == szenario && t.Target.Number == target)
                                .OrderBy(t => t.StartDateTimeOfTrialRecording);
                        for (var i = 0; i < tempList.Count(); i++)
                        {
                            tempList.ElementAt(i).TargetTrialNumberInSzenario = i + 1;
                        }
                    }
                }
            }

            return trialsContainer;
        }

        protected double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private bool IsValidTrialNumberInSzenarioSequence(IEnumerable<int> trialNumbersInSzenario)
        {
            var orderedTrialNumbersInSzenario = trialNumbersInSzenario.OrderBy(t => t).ToList();

            var isConsecutive = !orderedTrialNumbersInSzenario.Select((i, j) => i - j).Distinct().Skip(1).Any();
            var isValidStart = orderedTrialNumbersInSzenario.First() == 1;

            if (CheckForConsecutiveTrialNumberSequence)
            {
                return isConsecutive && isValidStart;
            }
            else
            {
                return isValidStart;
            }
        }

        public abstract Trial SetTrialMetadata(ManipAnalysisGui myManipAnalysisGui, Trial trial);

        public abstract bool CheckTrialCount(int trialCount);
    }
}