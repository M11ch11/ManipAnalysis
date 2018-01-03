using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using ManipAnalysis_v2.MongoDb;
using ManipAnalysis_v2.SzenarioParseDefinitions;

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
        /// <param name="path">path to the c3d file</param>
        /// <returns>true/false whether all those operations succeeded</returns>
        public bool ParseFile(string path)
        {
            var retVal = false;
            if (path != null)
            {
                _measureFilePath = path;

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
            }
            return retVal;
        }


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
    }
}