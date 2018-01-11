using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using ManipAnalysis_v2.MongoDb;
using ManipAnalysis_v2.SzenarioParseDefinitions;
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

                //Instead of returning a szenarioDefinitionType, the ParseFileInfo should now
                //return a path to the dtp file while also writing the necessary meta data that it already does.
                //############################################
                //Instead of calling ParseFileInfo and searching our own assembly only do the useful stuff from ParseFileInfo!
                //############################################

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


                    //TODO Why do we instantiate the TrialsContainer here??
                    TrialsContainer = new List<Trial>();
                }
                catch (Exception
                    ex)
                {
                    _myManipAnalysisGui.WriteToLogBox("ParseFileInfo-Error: " + ex);
                    c3DReader.Close();
                }

                //############################################
                //Instead of calling ParseFileInfo and searching our own assembly only do the useful stuff from ParseFileInfo!
                //############################################
                /*
                var szenarioDefinitionType = ParseFileInfo();

                if (szenarioDefinitionType != null)
                {
                    retVal = ParseMeasureData(szenarioDefinitionType);
                }
                else
                {
                    _myManipAnalysisGui.WriteToLogBox("No szenario definition found for: " + _studyName + " - " +
                                                      _szenarioName + ".");
                }
                */


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
        /// parseMetaDatafunction copied from the AbstractSzenarioDefinition where we replace the setMetadatafunction with the xmlParser.
        /// Therefore we also need the proper dtp file.
        /// </summary>
        /// <param name="dtpPath"></param>
        /// <param name="myManipAnalysisGui"></param>
        /// <param name="c3DFiles"></param>
        /// <param name="measureFileCreationDateTime"></param>
        /// <param name="measureFileHash"></param>
        /// <param name="measureFilePath"></param>
        /// <param name="probandId"></param>
        /// <param name="groupName"></param>
        /// <param name="studyName"></param>
        /// <param name="szenarioName"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private List<Trial>ParseMetaData(string dtpFile, ManipAnalysisGui myManipAnalysisGui, string[] c3DFiles,
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
                        /*
                        var eventTimes = c3DReader.GetParameter<float[]>("EVENTS:TIMES");
                        var eventLabels = c3DReader.GetParameter<string[]>("EVENTS:LABELS");
                        */
                        var frameTimeInc = 1.0f / c3DReader.Header.FrameRate;
                        int targetTrialNumber = c3DReader.GetParameter<short>("TRIAL:TP_NUM");
                        //The TP_NUM is an ID for the trial within the szenario (aka enumeration of all trials)
                        // -1 == Compensation of first Trial
                        //var szenarioTrialNumber = c3DReader.GetParameter<short>("TRIAL:TRIAL_NUM") - 1;
                        //We don't need a -1 here, in the end we will sort the container by trialNumberInSzenario and enumerate properly anyways!
                        //Check last block of this funct.
                        var szenarioTrialNumber = c3DReader.GetParameter<short>("TRIAL:TRIAL_NUM");
                        //This is not the targetNumber but the tpNumber.
                        //Only because of the convention that we can in most cases determine the targetNumber from the tpNumber this works...
                        int tpNumber = c3DReader.GetParameter<short>("TRIAL:TP");

                        measureFileContainer.CreationTime = measureFileCreationDateTime;
                        measureFileContainer.FileHash = measureFileHash;
                        measureFileContainer.FileName = Path.GetFileName(measureFilePath);

                        subjectContainer.PId = probandId;
                        
                        //This line does not write the target.Number! Instead it wrote the tpNumber into the EndTarget and then did some 
                        //stupid calculations on it to determine the real target.Number. We will not do this anymore!
                        //Instead we now get the target.Number from the *.dtp file in the XMLParser!
                        //targetContainer.Number = targetNumber;

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
                        //Maybe it makes sense to divide szenarioTrialNumber by 2 first, because in current study TRIAL_NUM gets increased by 2 instead of 1, starting by 2...
                        //Also in study 10 DAVOS, every other trial was invalid and filtered afaik?
                        /*
                        It seems that positionControlTrials are immediately filtered from Dexterit?
                        Therefore the trialNumberInSZenario should be determined by newly enumerating all trials within a szenario
                        from 1 to end
                        */
                        currentTrial.TrialNumberInSzenario = szenarioTrialNumber;
                        currentTrial.TrialVersion = "KINARM_1.0";
                        currentTrial.PositionOffset.X = offset.X;
                        currentTrial.PositionOffset.Y = offset.Y;

                        //TODO: Insert the metadata parser instead of this method
                        //The dtp path might be getable from the szenario field, as the szenariofield has the same name as the .dtp file.
                        //So as long as we know the general path to where dtp files are stored, we can then find the corresponding dtp file from the szenarioField.
                        //We just need to search all the dtp files in the dtplist for the one that matches the szenarioField.
                        //Alternatively we could also specifiy one general folder hardcoded...

                        //currentTrial = SetTrialMetadata(myManipAnalysisGui, currentTrial);
                        XMLParser parser = new XMLParser(dtpFile, tpNumber, currentTrial);
                        if (parser != null)
                        {
                            currentTrial = parser.parseTrial();
                        } else
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
                                    //For newer Imports use PositionStatus, for older ones ACH4
                                    //PositionStatus is afaik a variable that is used as a trigger and set in the trialControlBlock to give signals to the Vicon/EEG system?

                                    //var positionStatus = Convert.ToInt32(c3DReader.AnalogData["PositionStatus", 0]) - 2;

                                    var positionStatus = Convert.ToInt32(c3DReader.AnalogData["ACH4", 0]) - 2;

                                    /*
                                    * Study 11 Special analog data field
                                    var forceFieldStrength = Convert.ToDouble(c3DReader.AnalogData["ACH5", 0]);
                                    currentTrial.ForceFieldMatrix[0, 0] = 0;
                                    currentTrial.ForceFieldMatrix[0, 1] = -forceFieldStrength;
                                    currentTrial.ForceFieldMatrix[1, 0] = forceFieldStrength;
                                    currentTrial.ForceFieldMatrix[1, 1] = 0;
                                    */
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

                /*
                if (!CheckTrialCount(trialsContainer.Count))
                {
                    myManipAnalysisGui.WriteToLogBox("Invalid TrialCount (" + trialsContainer.Count + ") in file " +
                                                     measureFilePath + "\nSkipping File.");
                    trialsContainer.Clear();
                }
                */

                /*
                // Check for valid TrialNumberInSzenario Sequence
                if (CheckValidTrialNumberInSzenarioSequence && trialsContainer.Any())
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
                */
                // Set TargetTrialNumberInSzenario Field 
                // I have no clue why we need this...
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
                //set the TrialNumberInSzenario properly, because PositionControlTrials do not create *.c3d files
                //tut they still increase the TRIAL_NUM. We don't want gaps, whenever a PositionControl or StartTrial appeared.
                if (trialsContainer.Any())
                {
                    trialsContainer.Sort((x, y) => x.TrialNumberInSzenario.CompareTo(y.TrialNumberInSzenario));
                    for (int i = 1; i < trialsContainer.Count(); i++)
                    {
                        trialsContainer[i].TrialNumberInSzenario = i + 1;
                    }
                }
                return trialsContainer;
            }
        }


        //NOT NEEDED ANYMORE!!
        /// <summary>
        /// Calls the szenarioParseDefinition with the use of the SzenarioDefinitionType which then writes the MetaData 
        /// for a list of trials and pushes those into the TrialsContainer.
        /// </summary>
        /// <param name="szenarioDefinitionType">type that points to the szenarioParseDefinition that fits for the c3d-file</param>
        /// <returns></returns>
        private bool ParseMeasureData(Type szenarioDefinitionType)
        {
            var retVal = true;
            try
            {
                var szenarioDefinition = (AbstractSzenarioDefinition)Activator.CreateInstance(szenarioDefinitionType);
                //Here ParseMeasureFile in the proper SzenarioParseDefinition is being called, which then writes the metadata for a list of trials
                //that then get pushed into the TrialsContainer
                //It is enough to only call the general ParseMeasureFile function from AbstractSzenarioDefinition
                //In there only change the setMetadata call to use the parser instead.
                TrialsContainer.AddRange(szenarioDefinition.ParseMeasureFile(_myManipAnalysisGui, _c3DFiles,
                    _measureFileCreationDateTime, _measureFileHash, _measureFilePath, _probandId, _groupName, _studyName,
                    _szenarioName, _offset));
            }
            catch (Exception
                ex)
            {
                _myManipAnalysisGui.WriteToLogBox("ParseMeasureData-Error: " + ex);
                retVal = false;
            }

            return retVal;
        }







        //NOT NEEDED ANYMORE!
        /// <summary>
        /// creates a temporary directory where the zipfiles of the c3d archive are extracted.
        /// reads metadata information like szenarioname, groupname, studyname etc. from the so called "common"-file (*.c3d).
        /// If no metadata for these fields was found, replaces the information with "unknown"
        /// 
        /// Then searches the execution assembly for the SzenarioParseDefinitions that match the parse-file (DEPRECATED!)
        /// 
        /// extracts ProbandID and MeasureFileCreationDateTime from the path to the c3d file
        /// 
        /// </summary>
        /// <returns>a SzenarioDefinitionType that points to the matching SzenarioParseDefinition to parse the MetaData</returns>
        private Type ParseFileInfo()
        {
            Type szenarioDefinitionType = null;
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

                _szenarioName = c3DReader.GetParameter<string[]>("EXPERIMENT:TASK_PROTOCOL")[0];
                _studyName = c3DReader.GetParameter<string[]>("EXPERIMENT:STUDY")[0];
                _groupName = c3DReader.GetParameter<string[]>("EXPERIMENT:SUBJECT_CLASSIFICATION")[0];
                _offset.X = (c3DReader.GetParameter<float[]>("TARGET_TABLE:X")[0] -
                             c3DReader.GetParameter<float[]>("TARGET_TABLE:X_GLOBAL")[0]) / 100.0f;
                _offset.Y = (c3DReader.GetParameter<float[]>("TARGET_TABLE:Y")[0] -
                             c3DReader.GetParameter<float[]>("TARGET_TABLE:Y_GLOBAL")[0]) / 100.0f;


                //TODO: Don't use SzenarioParseDefinitions to parse metadata anymore.
                // instead use the XMLParse with a path to the matching dtp file.
                //Here the whole execution assembly is being traversed and searched for the szenarioParseDefinitions.
                //These SzenarioParseDefinitions then are checked for matching szenarioName and studyName
                //... which is tbh completely inefficient and cumbersome.
                //Instead just use the xmlparser for that job!
                foreach (
                        var szenarioDefinitionIterable in
                            Assembly.GetExecutingAssembly()
                                .GetTypes()
                                .Where(
                                    t =>
                                        !t.IsInterface && !t.IsAbstract && t.IsClass &&
                                        t.Namespace == "ManipAnalysis_v2.SzenarioParseDefinitions"))
                //Checking the import classes for matching szenario and study name
                {
                    try
                    {
                        if (_szenarioName == (string)szenarioDefinitionIterable.GetField("SzenarioName").GetValue(null)
                            && _studyName == (string)szenarioDefinitionIterable.GetField("StudyName").GetValue(null))
                        {
                            szenarioDefinitionType = szenarioDefinitionIterable;
                            break;
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }

                if (_szenarioName == "")
                {
                    _szenarioName = "Unknown";
                }
                if (_studyName == "")
                {
                    _studyName = "Unknown";
                }
                if (_groupName == "")
                {
                    _groupName = "Unknown";
                }

                c3DReader.Close();

                _probandId = fileName.Split('_')[0].Trim();
                var datetime = fileName.Split('_')[1].Replace('-', '.') + " " +
                               fileName.Split('_')[2].Replace(".zip", "").Replace('-', ':');
                _measureFileCreationDateTime = DateTime.ParseExact(datetime, "yyyy.MM.dd HH:mm:ss",
                    CultureInfo.InvariantCulture);


                //TODO Why do we instantiate the TrialsContainer here??
                TrialsContainer = new List<Trial>();
            }
            catch (Exception
                ex)
            {
                _myManipAnalysisGui.WriteToLogBox("ParseFileInfo-Error: " + ex);
                c3DReader.Close();
                szenarioDefinitionType = null;
            }

            return szenarioDefinitionType;
        }

    }
}