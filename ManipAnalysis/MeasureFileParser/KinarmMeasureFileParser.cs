using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using ManipAnalysis_v2.MongoDb;
using System.Threading.Tasks;

namespace ManipAnalysis_v2.MeasureFileParser
{
    internal class KinarmMeasureFileParser
    {
        private readonly ManipAnalysisGui _myManipAnalysisGui;

        private string[] _c3DFiles;

        private string _groupName;

        private DateTime _measureFileCreationDateTime;

        private string _measureFileHash;

        private string _measureFilePath;

        private Vector3 _offset;

        private string _probandId;

        private string _studyName;

        private string _szenarioName;


        public KinarmMeasureFileParser(ManipAnalysisGui myManipAnalysisGui)
        {
            _myManipAnalysisGui = myManipAnalysisGui;
            _offset = new Vector3();
        }

        public List<Trial> TrialsContainer { get; private set; }

        public static bool IsValidFile(ManipAnalysisGui myManipAnalysisGui,
            ManipAnalysisFunctions myManipAnalysisFunctions, string filePath)
        {
            var retVal = false;
            try
            {
                var fileName = Path.GetFileName(filePath);

                if (fileName != null && fileName.EndsWith(".zip"))
                {
                    if (fileName.Split('_').Length == 3)
                    {
                        if (!myManipAnalysisFunctions.CheckIfMeasureFileHashAlreadyExists(Md5.ComputeHash(filePath)))
                        {
                            retVal = true;
                        }
                    }
                }
            }
            catch (Exception
                ex)
            {
                myManipAnalysisGui.WriteToLogBox("ParseFileInfo-Error: " + ex);
            }

            return retVal;
        }

        /// <summary>
        /// sets the measureFilePath in the KinarmMeasureFileParser
        /// then calls ParseFileInfo-method to parse c3d information
        /// if ParseFileInfo-method succeeds:
        /// Measurefile data is pushed into trialsContainer with the ParseMeasureData-method
        /// </summary>
        /// <param name="zipFilepath">path to the zip file that contains c3d data</param>
        /// <param name="dtpFilePath">list of all dtp files that were entered into the listBox in the GUI
        /// We need all of those because we can not determine the matching dtp file from the zipfile alone, 
        /// instead we have to unpack it first and match it with the szenarioName from the c3dfile which happens in this class.</param>
        /// <returns>true/false whether all those operations succeeded</returns>
        public bool ParseFile(string zipFilepath, List<string> ListOfdtpFilePaths)
        {
            /*

            dtpName can be obtained by looking in the description of the c3d file. With that name you then only need to search the dtp files list for it.


            */
            var retVal = false;
            if (zipFilepath != null)
            {
                _measureFilePath = zipFilepath;
                var c3DReader = new C3DReader();

                try
                {
                    _measureFileHash = Md5.ComputeHash(_measureFilePath);
                    var fileName = Path.GetFileName(_measureFilePath);
                    var tempPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\temp";

                    if (Directory.Exists(tempPath))
                    {
                        Directory.Delete(tempPath, true);
                    }

                    Directory.CreateDirectory(tempPath);
                    ZipFile.ExtractToDirectory(_measureFilePath, tempPath);

                    _c3DFiles = Directory.EnumerateFiles(tempPath + @"\raw", "*_*_*.c3d*").ToArray();
                    var commonFile = tempPath + @"\raw\common.c3d";

                    c3DReader.Open(commonFile);
                    /*

                    Here we open the so called common.c3d file. This file is located in any zipArchive that gets
                    created from BKIN/KinArm and it contains general information about all trials that are 
                    contained in the zipFile. *.c3d files that come from the same zip-Archive must
                    share the same szenarioName, studyName etc...

                    */
                    _szenarioName = c3DReader.GetParameter<string[]>("EXPERIMENT:TASK_PROTOCOL")[0];
                    _studyName = c3DReader.GetParameter<string[]>("EXPERIMENT:STUDY")[0];
                    _groupName = c3DReader.GetParameter<string[]>("EXPERIMENT:SUBJECT_CLASSIFICATION")[0];
                    _offset.X = (c3DReader.GetParameter<float[]>("TARGET_TABLE:X")[0] -
                                 c3DReader.GetParameter<float[]>("TARGET_TABLE:X_GLOBAL")[0]) / 100.0f;
                    _offset.Y = (c3DReader.GetParameter<float[]>("TARGET_TABLE:Y")[0] -
                                 c3DReader.GetParameter<float[]>("TARGET_TABLE:Y_GLOBAL")[0]) / 100.0f;

                    c3DReader.Close();
                    //The ProbandID can be acquired by splitting the zipFile-Name and looking at the first split
                    //because BKIN names their zipFiles conveniently (lucky for us I guess?)
                    //This is no clean solution but I don't know of any better so far.
                    _probandId = fileName.Split('_')[0].Trim();
                    var datetime = fileName.Split('_')[1].Replace('-', '.') + " " +
                                   fileName.Split('_')[2].Replace(".zip", "").Replace('-', ':');
                    _measureFileCreationDateTime = DateTime.ParseExact(datetime, "yyyy.MM.dd HH:mm:ss",
                        CultureInfo.InvariantCulture);


                    //Why do we instantiate the TrialsContainer here?? I don't know, but it works, so better not touch it...
                    TrialsContainer = new List<Trial>();
                }
                catch (Exception
                    ex)
                {
                    _myManipAnalysisGui.WriteToLogBox("ParseFileInfo-Error: " + ex);
                    c3DReader.Close();
                }



                //Getting the matching dtpFile for the given *.c3d file by looking at the szenarioName and adding .dtp at the end
                //We are allowed to do this here, because all the *.c3d files belong to the same szenario, therefore use the same *.dtp to store their MetaData
                string dtpFilePath = "";
                for (int i = 0; i < ListOfdtpFilePaths.Count; i++)
                {
                    if (ListOfdtpFilePaths[i].Contains(_szenarioName + ".dtp"))
                    {
                        dtpFilePath = ListOfdtpFilePaths[i];
                    }
                }
                if (dtpFilePath.Equals(""))
                {
                    dtpFilePath = null;
                    _myManipAnalysisGui.WriteToLogBox("No matching *.dtp file found for the following szenario: " + _szenarioName);
                }


                //Call to parseMeasureData:
                //In this method the xmlparser is being called
                List<Trial> trialsCont = ParseMetaData(dtpFilePath, _myManipAnalysisGui, _c3DFiles,
                        _measureFileCreationDateTime, _measureFileHash, _measureFilePath, _probandId, _groupName, _studyName,
                        _szenarioName, _offset);
                if (trialsCont.Any())
                {
                    TrialsContainer.AddRange(trialsCont);
                    retVal = true;
                }

            }
            return retVal;
        }



        /// <summary>
        /// Here we link in the xmlParser. This whole method was copied from some older code thereforce it might look a little bit sloppy...
        /// Therefore we also need the proper dtp file.
        /// </summary>
        /// <param name="dtpPath"></param>
        /// <param name="myManipAnalysisGui"></param>
        /// <param name="c3DFiles">ListOfPaths to c3d files</param>
        /// <param name="measureFileCreationDateTime"></param>
        /// <param name="measureFileHash"></param>
        /// <param name="measureFilePath"></param>
        /// <param name="probandId"></param>
        /// <param name="groupName"></param>
        /// <param name="studyName"></param>
        /// <param name="szenarioName"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private List<Trial> ParseMetaData(string dtpFile, ManipAnalysisGui myManipAnalysisGui, string[] c3DFiles,
            DateTime measureFileCreationDateTime, string measureFileHash, string measureFilePath, string probandId,
            string groupName, string studyName, string szenarioName, Vector3 offset)
        {
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

                        var frameTimeInc = 1.0f / c3DReader.Header.FrameRate;
                        int targetTrialNumber = c3DReader.GetParameter<short>("TRIAL:TP_NUM");
                        //The TP_NUM is an ID for the trial within the szenario (aka enumeration of all trials)
                        //Check last block of this funct.
                        var szenarioTrialNumber = c3DReader.GetParameter<short>("TRIAL:TRIAL_NUM");

                        int tpNumber = c3DReader.GetParameter<short>("TRIAL:TP");

                        measureFileContainer.CreationTime = measureFileCreationDateTime;
                        measureFileContainer.FileHash = measureFileHash;
                        measureFileContainer.FileName = Path.GetFileName(measureFilePath);

                        subjectContainer.PId = probandId;

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

                        //Here we now use the xmlParser to parse metaData from the *.dtp files instead of the cumbersome szenarioParseDefinitions
                        XMLParser parser = new XMLParser(dtpFile, tpNumber, currentTrial);
                        if (parser != null)
                        {
                            currentTrial = parser.parseTrial();
                        }
                        else
                        {
                            _myManipAnalysisGui.WriteToLogBox("Parser had nullreference! :(");
                        }
                        if (currentTrial != null)
                        {
                            if (0 < c3DReader.FramesCount && c3DReader.FramesCount < 32768)
                            {
                                for (var frame = 0; frame < c3DReader.FramesCount; frame++)
                                {
                                    var measuredForcesRaw = new ForceContainer();
                                    var momentForcesRaw = new ForceContainer();
                                    var positionRaw = new PositionContainer();
                                    double timeOffset = frameTimeInc * frame;
                                    var timeStamp = DateTime.Parse(startTime).AddSeconds(timeOffset);

                                    // Returns an array of all points, it is necessary to call this method in each cycle [from Matthias!]
                                    var positionDataVector = c3DReader.ReadFrame()[0];
                                    // [0] == Right Hand


                                    //For newer Imports use PositionStatus, for older ones ACH4[from Matthias]

                                    //TODO: Somehow the positionStatus is not written anymore?!
                                    //TODO: The positionStatus is not written anymore in the c3d file!
                                    //This is needed for the TimeNormalization though!!

                                    // In the current "generalStudy"-Szenario we only use ACH4, so I don't think we need PositionStatus anymore...
                                    //I will keep it though, just in case.
                                    //var positionStatus = Convert.ToInt32(c3DReader.AnalogData["PositionStatus", 0]) - 2;
                                    var positionStatus = Convert.ToInt32(c3DReader.AnalogData["ACH4", 0]) - 2;

                                    /*
                                    [from Matthias]
                                    * Study 11 Special analog data field
                                    var forceFieldStrength = Convert.ToDouble(c3DReader.AnalogData["ACH5", 0]);
                                    currentTrial.ForceFieldMatrix[0, 0] = 0;
                                    currentTrial.ForceFieldMatrix[0, 1] = -forceFieldStrength;
                                    currentTrial.ForceFieldMatrix[1, 0] = forceFieldStrength;
                                    currentTrial.ForceFieldMatrix[1, 1] = 0;
                                    */
                                    positionRaw.PositionStatus= positionStatus;
                                    positionRaw.TimeStamp = timeStamp;
                                    positionRaw.X = positionDataVector.X + currentTrial.PositionOffset.X;
                                    positionRaw.Y = positionDataVector.Y + currentTrial.PositionOffset.Y;
                                    positionRaw.Z = positionDataVector.Z + currentTrial.PositionOffset.Z;

                                    // Get analog data for this frame[comment from Matthias]
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

                                    // Fill Trial[from Matthias]
                                    currentTrial.MeasuredForcesRaw.Add(measuredForcesRaw);
                                    currentTrial.MomentForcesRaw.Add(momentForcesRaw);
                                    currentTrial.PositionRaw.Add(positionRaw);
                                }
                            }
                            else
                            {
                                throw new Exception("Illegal C3D Frame count. Value must be between 0 and 32767, was " + c3DReader.FramesCount);
                            }

                            if (currentTrial.MeasuredForcesRaw.Count == 0)
                            {
                                throw new Exception("No data frames found in szenario trial " + currentTrial.TrialNumberInSzenario);
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

                // Set TargetTrialNumberInSzenario Field 
                // I have no clue why we need this... But I will let it live here for now...
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
                //set the TrialNumberInSzenario properly, because PositionControlTrials do not create *.c3d files.
                //They still increase the TRIAL_NUM though. We don't want gaps, whenever a PositionControl or StartTrial appeared.
                //Therefore we enumerate TrialNumberInSzenario anew.
                if (trialsContainer.Any())
                {
                    trialsContainer.Sort((x, y) => x.TrialNumberInSzenario.CompareTo(y.TrialNumberInSzenario));
                    for (int i = 0; i < trialsContainer.Count(); i++)
                    {
                        trialsContainer[i].TrialNumberInSzenario = i + 1;
                    }
                }
                return trialsContainer;
            }
        }


    }
}