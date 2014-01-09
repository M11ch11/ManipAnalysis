using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ManipAnalysis.Container;
using ManipAnalysis.MongoDb;

namespace ManipAnalysis
{
    internal class KinarmMeasureFileParser
    {
        private readonly ManipAnalysisGui _myManipAnalysisGui;
        private string _measureFilePath;
        private string _measureFileHash;
        private DateTime _measureFileCreationDateTime;
        private string _szenarioName;
        private string _studyName;
        private string _groupName;
        private string _probandName;
        private string _probandId;
        private string[] _c3dFiles;
        private MongoDb.Trial[] _trialsContainer;

        public KinarmMeasureFileParser(MongoDb.Trial[] trialsContainer, ManipAnalysisGui myManipAnalysisGui)
        {
            _myManipAnalysisGui = myManipAnalysisGui;
            _trialsContainer = trialsContainer;
            _probandName = "----NOT_IMPLEMENTED---";
            _groupName = "----NOT_IMPLEMENTED---";
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
            try
            {
                _measureFileHash = Md5.ComputeHash(_measureFilePath);

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
                    string[] szenarioInfo = szenarioInfo = sr.ReadToEnd().Split(new[] {';'});
                    _szenarioName = szenarioInfo[1].Trim();
                    _studyName = szenarioInfo[4].Trim();
                    _probandId = szenarioInfo[10].Trim();
                }
                
                _measureFileCreationDateTime =
                    DateTime.Parse(Path.GetFileName(_measureFilePath).Split('_')[1].Replace('-', '.') + " " +
                                   Path.GetFileName(_measureFilePath).Split('_')[2].Replace('-', ':'));

                _trialsContainer = new Trial[_c3dFiles.Length]; // Trials deklarieren

                retVal = true;
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("ParseFileInfo-Error: " + ex.ToString());
            }

            return retVal;
        }

        private bool ParseMeasureData()
        {
            bool retVal = true;
            try
            {
                for (int filesCounter = 0; filesCounter < _c3dFiles.Length; filesCounter++)
                {
                    var c3dReader = new C3dReader();
                    c3dReader.Open(_c3dFiles[filesCounter]);

                    
                    string startTime = c3dReader.GetParameter<string[]>("TRIAL:TIME")[0];
                    float[] eventTimes = c3dReader.GetParameter<float[]>("EVENTS:TIMES");
                    string[] eventLabels = c3dReader.GetParameter<string[]>("EVENTS:LABELS");
                    float frameTimeInc = 1.0f/c3dReader.Header.FrameRate;
                    int targetNumber = c3dReader.GetParameter<Int16>("TRIAL:TP");
                    int targetTrialNumber = c3dReader.GetParameter<Int16>("TRIAL:TP_NUM");
                    int szenarioTrialNumber = c3dReader.GetParameter<Int16>("TRIAL:TRIAL_NUM");


                    _trialsContainer[filesCounter] = new Trial();
                    MeasureFileContainer measureFileContainer = new MeasureFileContainer();
                    SubjectContainer subjectContainer = new SubjectContainer();
                    TargetContainer targetContainer = new TargetContainer();

                    measureFileContainer.CreationTime = _measureFileCreationDateTime;
                    measureFileContainer.FileHash = _measureFileHash;
                    measureFileContainer.FileName = Path.GetFileName(_measureFilePath);

                    subjectContainer.Name = _probandName;
                    subjectContainer.PId = _probandId;

                    targetContainer.Number = targetNumber;

                    _trialsContainer[filesCounter].ActualForcesRaw = new List<ForceContainer>();
                    _trialsContainer[filesCounter].MomentForcesRaw = new List<ForceContainer>();
                    _trialsContainer[filesCounter].NominalForcesRaw = new List<ForceContainer>();
                    _trialsContainer[filesCounter].PositionRaw = new List<PositionContainer>();
                    _trialsContainer[filesCounter].Group = _groupName;
                    _trialsContainer[filesCounter].MeasureFile = measureFileContainer;
                    _trialsContainer[filesCounter].Study = _studyName;
                    _trialsContainer[filesCounter].Subject = subjectContainer;
                    _trialsContainer[filesCounter].Szenario = _szenarioName;
                    _trialsContainer[filesCounter].Target = targetContainer;
                    _trialsContainer[filesCounter].TargetTrialNumberInSzenario = targetTrialNumber;
                    _trialsContainer[filesCounter].TrialNumberInSzenario = szenarioTrialNumber;

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

                        float timeOffset = frameTimeInc*frame;
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

                        //Fill Trial

                        
                        _trialsContainer[filesCounter].ActualForcesFiltered
                    }

                    // Don't forget to close the reader
                    c3dReader.Close();
                }
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("ParseMeasureData-Error: " + ex.ToString());
                retVal = false;
            }

            return retVal;
        }
    }
}
