using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ManipAnalysis.Container;

namespace ManipAnalysis
{
    internal class KinarmMeasureFileParser
    {
        private readonly ManipAnalysisGui _myManipAnalysisGui;
        private string _measureFilePath;
        private string[] _c3dFiles;


        public KinarmMeasureFileParser(DataContainer container, ManipAnalysisGui myManipAnalysisGui)
        {
            _myManipAnalysisGui = myManipAnalysisGui;
            _dataContainer = container;
        }

        public bool ParseFile(string path)
        {
            _measureFilePath = path;

            bool retVal = ParseFileInfo();

            if (retVal)
            {
                retVal = ParseMeasureData();
            }

            return retVal;
        }

        private bool ParseFileInfo()
        {
            bool retVal = false;

            string creationDate = null;
            string creationTime = null;
            string szenarioName = null;
            string studyName = null;
            string probandId = null;
            

            string tempPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\temp";
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }

            Directory.CreateDirectory(tempPath);
            ZipFile.ExtractToDirectory(_measureFilePath, tempPath);

            _c3dFiles = Directory.EnumerateFiles(tempPath + @"\raw", "*_*_*.c3d*").ToArray();

            using (var fs = new FileStream(tempPath + @"\exam_info_3.txt", FileMode.Open))
            {
                var sr = new StreamReader(fs);
                string[] szenarioInfo = szenarioInfo = sr.ReadToEnd().Split(new[] { ';' });
                szenarioName = szenarioInfo[1];
                studyName = szenarioInfo[4];
                probandId = szenarioInfo[10];
            }

            creationDate = _measureFilePath.Split('_')[1].Replace('-', '.'); //Date
            creationTime = _measureFilePath.Split('_')[2].Replace('-', ':'); //Time
            
            _dataContainer.MeasureFileHash = Md5.ComputeHash(_measureFilePath);
            _dataContainer.StudyName = studyName.Trim();
            _dataContainer.SzenarioName = szenarioName.Trim();
            _dataContainer.SubjectID = probandId.Trim();
            _dataContainer.MeasureFileCreationDate = creationDate;
            _dataContainer.MeasureFileCreationTime = creationTime;

            retVal = true;
            
            return retVal;
        }

        private bool ParseMeasureData()
        {
            bool retVal = true;

            foreach (string c3dFile in _c3dFiles)
            {
                var c3dReader = new C3dReader();
                c3dReader.Open(c3dFile);

                string startTime = c3dReader.GetParameter<string[]>("TRIAL:TIME")[0];
                float[] eventTimes = c3dReader.GetParameter<float[]>("EVENTS:TIMES");
                string[] eventLabels = c3dReader.GetParameter<string[]>("EVENTS:LABELS");
                float frameTimeInc = 1.0f / c3dReader.Header.FrameRate;
                int targetNumber = c3dReader.GetParameter<Int16>("TRIAL:TP");
                int targetTrialNumber = c3dReader.GetParameter<Int16>("TRIAL:TP_NUM");
                int szenarioTrialNumber = c3dReader.GetParameter<Int16>("TRIAL:TRIAL_NUM");
                //_dataContainer.GroupName = filenameInfoStringArray[4].Trim();
                //_dataContainer.SubjectName = filenameInfoStringArray[5].Trim();
                _dataContainer.
                var positionData = new Vector3[c3dReader.FramesCount];
                var forceX = new float[c3dReader.FramesCount];
                var forceY = new float[c3dReader.FramesCount];
                var forceZ = new float[c3dReader.FramesCount];
                var momentX = new float[c3dReader.FramesCount];
                var momentY = new float[c3dReader.FramesCount];
                var momentZ = new float[c3dReader.FramesCount];
                var timeStamp = new DateTime[c3dReader.FramesCount];
                var positionStatus = new int[c3dReader.FramesCount];

                for (int frame = 0; frame < c3dReader.FramesCount; frame++)
                {
                    // returns an array of all points, it is necessary to call this method in each cycle
                    positionData[frame] = c3dReader.ReadFrame()[0]; // Right Hand

                    // get analog data for this frame
                    forceX[frame] = c3dReader.AnalogData["Right_FS_ForceX", 0];
                    forceY[frame] = c3dReader.AnalogData["Right_FS_ForceY", 0];
                    forceZ[frame] = c3dReader.AnalogData["Right_FS_ForceZ", 0];

                    momentX[frame] = c3dReader.AnalogData["Right_FS_TorqueX", 0];
                    momentY[frame] = c3dReader.AnalogData["Right_FS_TorqueX", 0];
                    momentZ[frame] = c3dReader.AnalogData["Right_FS_TorqueX", 0];

                    float timeOffset = frameTimeInc * frame;
                    timeStamp[frame] = DateTime.Parse(startTime).AddSeconds(timeOffset);
                    //var temp = c3dReader.AnalogData["Right_FS_TimeStamp", 0];

                    for (int eventCounter = 0; eventCounter < eventTimes.Length; eventCounter++)
                    {
                        if (eventTimes[eventCounter] <= timeOffset)
                        {
                            switch (eventLabels[eventCounter])
                            {
                                case "SUBJECT_IS_IN_FIRST_TARGET":
                                    positionStatus[frame] = 0;
                                    break;
                                case "SUBJECT_HAS_LEFT_FIRST_TARGET":
                                    positionStatus[frame] = 1;
                                    break;
                                case "SUBJECT_IS_IN_SECOND_TARGET":
                                    positionStatus[frame] = 2;
                                    break;
                                case "SUBJECT_HAS_LEFT_SECOND_TARGET":
                                    positionStatus[frame] = 3;
                                    break;
                                default:
                                    MessageBox.Show("PositionStatus Error");
                                    break;
                            }
                        }
                    }
                }

                // Don't forget to close the reader
                c3dReader.Close();
            }

            /*
            if (_dataContainer.MeasureFileHash != null)
            {
                
                var measureFileStream = new FileStream(_measureFilePath, FileMode.Open, FileAccess.Read);
                var measureFileReader = new StreamReader(measureFileStream);
                _dataContainer.MeasureDataRaw.Clear();

                int expectedSzenarioTrialCount = 0;
                int expectedTargetTrialCount = 0;

                if (_dataContainer.SzenarioName == "Szenario02" ||
                    _dataContainer.SzenarioName == "Szenario30" ||
                    _dataContainer.SzenarioName == "Szenario42_N" ||
                    _dataContainer.SzenarioName == "Szenario42_R"
                    )
                {
                    expectedSzenarioTrialCount = 96;
                    expectedTargetTrialCount = 6;
                }
                else if (_dataContainer.SzenarioName == "Szenario03" ||
                         _dataContainer.SzenarioName == "Szenario04" ||
                         _dataContainer.SzenarioName == "Szenario05" ||
                         _dataContainer.SzenarioName == "Szenario06"
                    )
                {
                    expectedSzenarioTrialCount = 640;
                    expectedTargetTrialCount = 40;
                }
                else if (_dataContainer.SzenarioName == "Szenario07" ||
                         _dataContainer.SzenarioName == "Szenario08" ||
                         _dataContainer.SzenarioName == "Szenario09" ||
                         _dataContainer.SzenarioName == "Szenario10" ||
                         _dataContainer.SzenarioName == "Szenario31" ||
                         _dataContainer.SzenarioName == "Szenario32"
                    )
                {
                    expectedSzenarioTrialCount = 256;
                    expectedTargetTrialCount = 16;
                }
                else if (_dataContainer.SzenarioName == "Szenario11" ||
                         _dataContainer.SzenarioName == "Szenario12" ||
                         _dataContainer.SzenarioName == "Szenario13" ||
                         _dataContainer.SzenarioName == "Szenario14" ||
                         _dataContainer.SzenarioName == "Szenario15" ||
                         _dataContainer.SzenarioName == "Szenario16" ||
                         _dataContainer.SzenarioName == "Szenario17" ||
                         _dataContainer.SzenarioName == "Szenario18" ||
                         _dataContainer.SzenarioName == "Szenario19" ||
                         _dataContainer.SzenarioName == "Szenario20" ||
                         _dataContainer.SzenarioName == "Szenario50" ||
                         _dataContainer.SzenarioName == "Szenario51"
                    )
                {
                    expectedSzenarioTrialCount = 400;
                    expectedTargetTrialCount = 25;
                }
                else if (_dataContainer.SzenarioName == "Szenario43_N" ||
                         _dataContainer.SzenarioName == "Szenario43_R" ||
                         _dataContainer.SzenarioName == "Szenario44_N" ||
                         _dataContainer.SzenarioName == "Szenario44_R"
                    )
                {
                    expectedSzenarioTrialCount = 480;
                    expectedTargetTrialCount = 30;
                }
                else if (_dataContainer.SzenarioName == "Szenario45_N" ||
                         _dataContainer.SzenarioName == "Szenario45_R"
                    )
                {
                    expectedSzenarioTrialCount = 32;
                    expectedTargetTrialCount = 2;
                }

                //const string checkHeader = "Time, ForceActualX, ForceActualY, ForceActualZ, ForceNominalX, ForceNominalY, ForceNominalZ, ForceMomentX, ForceMomentY, ForceMomentZ, PositionCartesianX, PositionCartesianY, PositionCartesianZ, OldTarget, ActiveTarget, TargetNumber, TrialNumber, isCatchTrial, hasLeftTarget";  // Study 1
                //const string checkHeader = "Time, ForceActualX, ForceActualY, ForceActualZ, ForceNominalX, ForceNominalY, ForceNominalZ, ForceMomentX, ForceMomentY, ForceMomentZ, PositionCartesianX, PositionCartesianY, PositionCartesianZ, OldTarget, ActiveTarget, TargetNumber, TrialNumber, IsCatchTrial, PositionStatus"; // Study 2 & 3
                const string checkHeader =
                    "Time, ForceActualX, ForceActualY, ForceActualZ, ForceNominalX, ForceNominalY, ForceNominalZ, ForceMomentX, ForceMomentY, ForceMomentZ, PositionCartesianX, PositionCartesianY, PositionCartesianZ, OldTarget, ActiveTarget, TargetNumber, TrialNumber, IsCatchTrial, IsErrorClampTrial, PositionStatus";
                // Study 4


                if (checkHeader == measureFileReader.ReadLine())
                {
                    while (!measureFileReader.EndOfStream)
                    {
                        string readLine = measureFileReader.ReadLine();
                        if (readLine != null)
                        {
                            string[] measureFileLine = readLine.Split(new[] { ", " }, StringSplitOptions.None);

                            if (measureFileLine.Count() == 20) // Study 4
                            {
                                if ((_dataContainer.MeasureDataRaw.Count > 0) &&
                                    (DateTime.Parse(_dataContainer.MeasureFileCreationDate + " " + measureFileLine[0])
                                        .Subtract(_dataContainer.MeasureDataRaw.Last().TimeStamp)
                                        .TotalMilliseconds > 500)
                                    )
                                {
                                    while ((Convert.ToInt32(measureFileLine[16]) ==
                                            _dataContainer.MeasureDataRaw.Last().SzenarioTrialNumber) &&
                                           (!measureFileReader.EndOfStream)
                                        )
                                    {
                                        readLine = measureFileReader.ReadLine();
                                        if (readLine != null)
                                        {
                                            measureFileLine = readLine
                                                .Split(new[] { ", " },
                                                    StringSplitOptions.None);
                                        }
                                    }
                                }
                                else if ((_dataContainer.MeasureDataRaw.Count > 0) &&
                                         (Convert.ToInt32(measureFileLine[15]) !=
                                          _dataContainer.MeasureDataRaw.Last().TargetNumber) &&
                                         (Convert.ToInt32(measureFileLine[16]) ==
                                          _dataContainer.MeasureDataRaw.Last().SzenarioTrialNumber)
                                    )
                                {
                                    while (
                                        (Convert.ToInt32(measureFileLine[15]) !=
                                         _dataContainer.MeasureDataRaw.Last().TargetNumber) &&
                                        (Convert.ToInt32(measureFileLine[16]) ==
                                         _dataContainer.MeasureDataRaw.Last().SzenarioTrialNumber) &&
                                        (!measureFileReader.EndOfStream)
                                        )
                                    {
                                        readLine = measureFileReader.ReadLine();
                                        if (readLine != null)
                                      {
                                            measureFileLine = readLine
                                                .Split(new[] {", "},
                                                    StringSplitOptions.None);
                                        }
                                    }
                                }

                                if (Convert.ToInt32(measureFileLine[16]) <= expectedSzenarioTrialCount)
                                {
                                    _dataContainer.MeasureDataRaw.Add(new MeasureDataContainer(
                                        DateTime.Parse(
                                            _dataContainer.MeasureFileCreationDate +
                                            " " +
                                            measureFileLine[0]),
                                        //Forces in Newton
                                        Convert.ToDouble(measureFileLine[1])/1E3,
                                        Convert.ToDouble(measureFileLine[2])/1E3,
                                        Convert.ToDouble(measureFileLine[3])/1E3,
                                        Convert.ToDouble(measureFileLine[4])/1E3,
                                        Convert.ToDouble(measureFileLine[5])/1E3,
                                        Convert.ToDouble(measureFileLine[6])/1E3,
                                        Convert.ToDouble(measureFileLine[7])/1E3,
                                        Convert.ToDouble(measureFileLine[8])/1E3,
                                        Convert.ToDouble(measureFileLine[9])/1E3,
                                        //Positions in Meter
                                        Convert.ToDouble(measureFileLine[10])/1E6,
                                        Convert.ToDouble(measureFileLine[11])/1E6,
                                        Convert.ToDouble(measureFileLine[12])/1E6,
                                        Convert.ToInt32(measureFileLine[15]),
                                        -1,
                                        Convert.ToInt32(measureFileLine[16]),
                                        Convert.ToBoolean(measureFileLine[17]),
                                        Convert.ToBoolean(measureFileLine[18]),
                                        Convert.ToInt32(measureFileLine[19])
                                        ));
                                }
                            }
                            else
                            {
                                _myManipAnalysisGui.WriteToLogBox("Measure file header error: invalid column count");
                                retVal = false;
                            }
                        }
                    }
             
            
                    int maxTrialCount = _dataContainer.MeasureDataRaw.Max(t => t.SzenarioTrialNumber);
                    int realTrialCount =
                        _dataContainer.MeasureDataRaw.Select(t => t.SzenarioTrialNumber).Distinct().Count();


                    if ((maxTrialCount != expectedSzenarioTrialCount) || (realTrialCount != expectedSzenarioTrialCount))
                    {
                        _myManipAnalysisGui.WriteToLogBox("Trial count error: expected " + expectedSzenarioTrialCount +
                                                          " trials but " + realTrialCount + " were found.");
                        retVal = false;
                    }
                }
                else
                {
                    _myManipAnalysisGui.WriteToLogBox("Error in measure file header.");
                    retVal = false;
                }
                measureFileReader.Close();

                int[] targetArray = _dataContainer.MeasureDataRaw.Select(t => t.TargetNumber).Distinct().ToArray();

                for (int i = 0; i < targetArray.Length; i++)
                {
                    int[] szenarioTrialNumberArray =
                        _dataContainer.MeasureDataRaw.Where(t => t.TargetNumber == targetArray[i])
                            .Select(t => t.SzenarioTrialNumber)
                            .Distinct()
                            .OrderBy(t => t)
                            .ToArray();

                    if (expectedTargetTrialCount == szenarioTrialNumberArray.Length)
                    {
                        for (int j = 0; j < szenarioTrialNumberArray.Length; j++)
                        {
                            List<MeasureDataContainer> tempList =
                                _dataContainer.MeasureDataRaw.Where(
                                    t => t.SzenarioTrialNumber == szenarioTrialNumberArray[j])
                                    .OrderBy(t => t.TimeStamp)
                                    .ToList();

                            for (int k = 0; k < tempList.Count; k++)
                            {
                                tempList.ElementAt(k).TargetTrialNumber = j + 1;
                            }
                        }
                    }
                    else
                    {
                        _myManipAnalysisGui.WriteToLogBox("Target Trial number incorrect.");
                        retVal = false;
                    }
                }
            }
            else
            {
                retVal = false;
            }

            if (!retVal)
            {
                _dataContainer.MeasureDataRaw.Clear();
            }
            */
            return retVal;
        }
    }
}
