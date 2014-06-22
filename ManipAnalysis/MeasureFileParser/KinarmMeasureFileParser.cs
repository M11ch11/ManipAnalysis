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
        private string[] _c3DFiles;
        private List<Trial> _trialsContainer;
        private string _measureFilePath;
        private string _measureFileHash;
        private string _studyName;
        private string _groupName;
        private string _szenarioName;
        private string _probandId;
        private DateTime _measureFileCreationDateTime;


        public KinarmMeasureFileParser(ManipAnalysisGui myManipAnalysisGui)
        {
            _myManipAnalysisGui = myManipAnalysisGui;
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

                Type szenarioDefinitionType = ParseFileInfo();

                if (szenarioDefinitionType != null)
                {
                    retVal = ParseMeasureData(szenarioDefinitionType);
                }
                else
                {
                    _myManipAnalysisGui.WriteToLogBox("No szenario definition found for: " + _studyName + " - " + _szenarioName + ".");
                }
            }
            return retVal;
        }

        private Type ParseFileInfo()
        {
            Type szenarioDefinitionType = null;

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

                foreach (Type szenarioDefinitionIterable in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && t.Namespace == "ManipAnalysis_v2.SzenarioParseDefinitions"))
                {
                    if (_szenarioName == (string)szenarioDefinitionIterable.GetField("SzenarioName").GetValue(null)) //&& _studyName == (string)szenarioDefinitionIterable.GetField("StudyName").GetValue(null))
                    {
                        szenarioDefinitionType = szenarioDefinitionIterable;
                        break;
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
                string datetime = fileName.Split('_')[1].Replace('-', '.') + " " +
                                  fileName.Split('_')[2].Replace(".zip", "").Replace('-', ':');
                _measureFileCreationDateTime = DateTime.ParseExact(datetime, "yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);

                _trialsContainer = new List<Trial>();
            }
            catch (Exception ex)
            {
                _myManipAnalysisGui.WriteToLogBox("ParseFileInfo-Error: " + ex);
                szenarioDefinitionType = null;
            }

            return szenarioDefinitionType;
        }

        private bool ParseMeasureData(Type szenarioDefinitionType)
        {
            bool retVal = true;
            try
            {
                var szenarioDefinition = (SzenarioParseDefinitions.ISzenarioDefinition)Activator.CreateInstance(szenarioDefinitionType);
                _trialsContainer.AddRange(szenarioDefinition.parseMeasureFile(_myManipAnalysisGui, _c3DFiles, _measureFileCreationDateTime, _measureFileHash, _measureFilePath, _probandId, _groupName, _studyName, _szenarioName));
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