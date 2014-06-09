using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using ManipAnalysis_v2.MongoDb;

namespace ManipAnalysis_v2.MeasureFileParser
{
    internal class KinarmMeasureFileParser
    {
        private readonly ManipAnalysisGui _myManipAnalysisGui;
        private readonly string _probandName;
        private string[] _c3DFiles;
        private string _groupName;
        private DateTime _measureFileCreationDateTime;
        private string _measureFileHash;
        private string _measureFilePath;
        private string _probandId;
        private string _studyName;
        private string _szenarioName;
        private List<Trial> _trialsContainer;

        public KinarmMeasureFileParser(ManipAnalysisGui myManipAnalysisGui)
        {
            _myManipAnalysisGui = myManipAnalysisGui;
            _probandName = "----NOT_IMPLEMENTED---";
        }

        public List<Trial> TrialsContainer
        {
            get { return _trialsContainer; }
        }

        public static bool IsValidFile(ManipAnalysisGui myManipAnalysisGui, ManipAnalysisFunctions myManipAnalysisFunctions,
            string filePath)
        {
            bool retVal = false;
            try
            {
                string fileName = Path.GetFileName(filePath);

                if (fileName.EndsWith(".zip"))
                {
                    if (fileName.Split('_').Count() == 3)
                    {
                        if (!myManipAnalysisFunctions.CheckIfMeasureFileHashAlreadyExists(Md5.ComputeHash(filePath)))
                        {
                            retVal = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                myManipAnalysisGui.WriteToLogBox("ParseFileInfo-Error: " + ex);
            }

            return retVal;
        }

        public bool ParseFile(string path)
        {
            bool retVal = false;
            if (path != null)
            {
                _measureFilePath = path;

                retVal = ParseFileInfo();

                if (retVal)
                {
                    retVal = ParseMeasureData();
                }
            }
            return retVal;
        }

        private bool ParseFileInfo()
        {
            bool retVal = false;
            try
            {
                _measureFileHash = Md5.ComputeHash(_measureFilePath);
                string fileName = Path.GetFileName(_measureFilePath);
                string tempPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\temp";

                if (Directory.Exists(tempPath))
                {
                    Directory.Delete(tempPath, true);
                }

                Directory.CreateDirectory(tempPath);
                ZipFile.ExtractToDirectory(_measureFilePath, tempPath);

                _c3DFiles = Directory.EnumerateFiles(tempPath + @"\raw", "*_*_*.c3d*").ToArray();
                string _commonFile = tempPath + @"\raw\common.c3d";

                var c3DReader = new C3dReader();
                c3DReader.Open(_commonFile);

                _szenarioName = c3DReader.GetParameter<string[]>("EXPERIMENT:TASK_PROTOCOL")[0];
                _studyName = c3DReader.GetParameter<string[]>("EXPERIMENT:STUDY")[0];
                _groupName = c3DReader.GetParameter<string[]>("EXPERIMENT:SUBJECT_CLASSIFICATION")[0];

                if (_studyName == "")
                {
                    _studyName = "No Study";
                }

                c3DReader.Close();

                _probandId = fileName.Split('_')[0].Trim();
                string datetime = fileName.Split('_')[1].Replace('-', '.') + " " +
                                  fileName.Split('_')[2].Replace(".zip", "").Replace('-', ':');
                _measureFileCreationDateTime = DateTime.ParseExact(datetime, "yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);

                _trialsContainer = new List<Trial>();

                if (_szenarioName != "00_Demo-Szenario")
                {
                    retVal = true;
                }
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("ParseFileInfo-Error: " + ex);
            }

            return retVal;
        }

        private bool ParseMeasureData()
        {
            bool retVal = true;
            try
            {
                for (int filesCounter = 0; filesCounter < _c3DFiles.Length; filesCounter++)
                {
                    var c3DReader = new C3dReader();
                    c3DReader.Open(_c3DFiles[filesCounter]);

                    string startTime = c3DReader.GetParameter<string[]>("TRIAL:TIME")[0];
                    var eventTimes = c3DReader.GetParameter<float[]>("EVENTS:TIMES");
                    var eventLabels = c3DReader.GetParameter<string[]>("EVENTS:LABELS");
                    float frameTimeInc = 1.0f/c3DReader.Header.FrameRate;
                    int targetTrialNumber = c3DReader.GetParameter<Int16>("TRIAL:TP_NUM");
                    // -1 == Compensation of first Trial
                    int szenarioTrialNumber = c3DReader.GetParameter<Int16>("TRIAL:TRIAL_NUM") - 1;
                    int targetNumber = c3DReader.GetParameter<Int16>("TRIAL:TP");

                    if (targetNumber != 10 && targetNumber != 20 && targetNumber != 30) // Target 10/20/30 == StartTrial
                    {
                        var currentTrial = new Trial();
                        var measureFileContainer = new MeasureFileContainer();
                        var subjectContainer = new SubjectContainer();
                        var targetContainer = new TargetContainer();

                        if ((targetNumber >= 4 && targetNumber <= 6) || (targetNumber >= 14 && targetNumber <= 16)) // CW ForceField
                        {
                            targetNumber = targetNumber - 3;
                        }
                        else if ((targetNumber >= 21 && targetNumber <= 23) || (targetNumber >= 31 && targetNumber <= 33)) // ErrorClampTrial
                        {
                            targetNumber = targetNumber - 20;
                            currentTrial.ErrorClampTrial = true;
                        }
                        else if ((targetNumber >= 24 && targetNumber <= 26) || (targetNumber >= 34 && targetNumber <= 36)) // ErrorClampTrial + CW ForceField
                        {
                            targetNumber = targetNumber - 23;
                            currentTrial.ErrorClampTrial = true;
                        }
                        else if (targetNumber >= 41 && targetNumber <= 43) // 30s Pause
                        {
                            targetNumber = targetNumber - 30;
                        }
                        else if (targetNumber >= 51 && targetNumber <= 53) // 30s Pause + Wechsel R=>L
                        {
                            targetNumber = targetNumber - 40;
                        }
                        else if (targetNumber >= 54 && targetNumber <= 56) // 30s Pause + Wechsel L=>R
                        {
                            targetNumber = targetNumber - 43;
                        }
                        else if (targetNumber >= 61 && targetNumber <= 63) // 30s Pause + Wechsel R=>L + CW ForceField
                        {
                            targetNumber = targetNumber - 50;
                        }
                        else if (targetNumber >= 64 && targetNumber <= 66) // 30s Pause + Wechsel L=>R + CW ForceField
                        {
                            targetNumber = targetNumber - 53;
                        }
                        else if (targetNumber >= 71 && targetNumber <= 73) // 30s Pause + Wechsel R=>L + ErrorClampTrial
                        {
                            targetNumber = targetNumber - 60;
                            currentTrial.ErrorClampTrial = true;
                        }
                        else if (targetNumber >= 74 && targetNumber <= 76) // 30s Pause + Wechsel L=>R + ErrorClampTrial
                        {
                            targetNumber = targetNumber - 63;
                            currentTrial.ErrorClampTrial = true;
                        }

                        measureFileContainer.CreationTime = _measureFileCreationDateTime;
                        measureFileContainer.FileHash = _measureFileHash;
                        measureFileContainer.FileName = Path.GetFileName(_measureFilePath);

                        subjectContainer.Name = _probandName;
                        subjectContainer.PId = _probandId;

                        targetContainer.Number = targetNumber;

                        currentTrial.MeasuredForcesRaw = new List<ForceContainer>();
                        currentTrial.MomentForcesRaw = new List<ForceContainer>();
                        currentTrial.PositionRaw = new List<PositionContainer>();
                        currentTrial.Group = _groupName;
                        currentTrial.MeasureFile = measureFileContainer;
                        currentTrial.Study = _studyName;
                        currentTrial.Subject = subjectContainer;
                        currentTrial.Szenario = _szenarioName;
                        currentTrial.Target = targetContainer;
                        currentTrial.TargetTrialNumberInSzenario = targetTrialNumber;
                        currentTrial.RawDataSampleRate = Convert.ToInt32(c3DReader.Header.FrameRate);
                        currentTrial.TrialNumberInSzenario = szenarioTrialNumber;
                        currentTrial.TrialVersion = "KINARM_1.0";
                        currentTrial.PositionOffset.Y = -0.14;

                        for (int frame = 0; frame < c3DReader.FramesCount; frame++)
                        {
                            var measuredForcesRaw = new ForceContainer();
                            var momentForcesRaw = new ForceContainer();
                            var positionRaw = new PositionContainer();
                            double timeOffset = frameTimeInc*frame;
                            DateTime timeStamp = DateTime.Parse(startTime).AddSeconds(timeOffset);

                            // Returns an array of all points, it is necessary to call this method in each cycle
                            Vector3 positionDataVector = c3DReader.ReadFrame()[0]; // [0] == Right Hand

                             /*
                             <Event code="1" name="TRIAL_STARTED"  desc="Trial has started" />
                             <Event code="2" name="SUBJECT_IS_IN_FIRST_TARGET"  desc="Subject is in the first target" />
                             <Event code="3" name="SUBJECT_HAS_LEFT_FIRST_TARGET"  desc="Subject has left the first target" />
                             <Event code="4" name="SUBJECT_IS_IN_SECOND_TARGET"  desc="Subject is in the second target" />
                             <Event code="5" name="SUBJECT_HAS_LEFT_SECOND_TARGET"  desc="Subject has left the second target" />
                             <Event code="6" name="TRIAL_ENDED"  desc="Trial has ended" /> 
                            */
                            int positionStatus = Convert.ToInt32(c3DReader.AnalogData["ACH4", 0]) - 2;

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
                            momentForcesRaw.Y = c3DReader.AnalogData["Right_FS_TorqueX", 0];
                            momentForcesRaw.Z = c3DReader.AnalogData["Right_FS_TorqueX", 0];

                            // Fill Trial
                            currentTrial.MeasuredForcesRaw.Add(measuredForcesRaw);
                            currentTrial.MomentForcesRaw.Add(momentForcesRaw);
                            currentTrial.PositionRaw.Add(positionRaw);
                        }
                        _trialsContainer.Add(currentTrial); // Add trial
                    }
                    // Don't forget to close the reader
                    c3DReader.Close();
                }
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("ParseMeasureData-Error: " + ex);
                retVal = false;
            }

            return retVal;
        }
    }
}