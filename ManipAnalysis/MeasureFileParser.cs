using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace ManipAnalysis
{
    class MeasureFileParser
    {
        string measureFilePath;
        DataContainer dataContainer;
        ManipAnalysis myManipAnalysisGUI;

        public MeasureFileParser(DataContainer _container, ManipAnalysis _myManipAnalysisGUI)
        {
            myManipAnalysisGUI = _myManipAnalysisGUI;
            dataContainer = _container;
        }

        public bool parseFile(string path)
        {
            bool retVal;

            measureFilePath = path;

            retVal = parseFileInfo();

            if (retVal)
            {
                retVal = parseMeasureData();
            }

            return retVal;
        }

        private bool parseFileInfo()
        {
            bool retVal = false;

            string filenameInfoString = measureFilePath.Substring(measureFilePath.LastIndexOf("\\Szenario") + 1);
            filenameInfoString = filenameInfoString.Remove(filenameInfoString.IndexOf(".csv"));
            string[] filenameInfoStringArray = filenameInfoString.Split('-');

            /*
            if (filenameInfoStringArray.Count() == 6)   // Study 1
            {
                dataContainer.measureFileHash = MD5.computeHash(measureFilePath);
                dataContainer.studyName = "Study 1";
                dataContainer.szenarioName = filenameInfoStringArray[0].Trim();
                dataContainer.groupName = filenameInfoStringArray[5].Trim();
                dataContainer.subjectName = filenameInfoStringArray[4].Trim();
                dataContainer.subjectID = filenameInfoStringArray[3].Trim();
                dataContainer.measureFileCreationDate = filenameInfoStringArray[1];
                dataContainer.measureFileCreationTime = filenameInfoStringArray[2].Replace('.', ':');

                retVal = true;
            }
            */
            if (filenameInfoStringArray.Count() == 7)   // Study 2
            {
                dataContainer.measureFileHash = MD5.computeHash(measureFilePath);
                dataContainer.studyName = filenameInfoStringArray[3].Trim();
                dataContainer.szenarioName = filenameInfoStringArray[0].Trim();
                dataContainer.groupName = filenameInfoStringArray[4].Trim();
                dataContainer.subjectName = filenameInfoStringArray[5].Trim();
                dataContainer.subjectID = filenameInfoStringArray[6].Trim();
                dataContainer.measureFileCreationDate = filenameInfoStringArray[1];
                dataContainer.measureFileCreationTime = filenameInfoStringArray[2].Replace('.', ':');

                retVal = true;
            }          

            return retVal;
        }

        private bool parseMeasureData()
        {
            bool retVal = true;

            if (dataContainer.measureFileHash != null)
            {
                FileStream measureFileStream = new FileStream(measureFilePath, FileMode.Open, FileAccess.Read);
                StreamReader measureFileReader = new StreamReader(measureFileStream);
                dataContainer.measureDataRaw.Clear();

                int expectedSzenarioTrialCount = 0;
                int expectedTargetTrialCount = 0;

                if (dataContainer.szenarioName == "Szenario02")
                {
                    expectedSzenarioTrialCount = 96;
                    expectedTargetTrialCount = 6;
                }
                else if (dataContainer.szenarioName == "Szenario03" ||
                            dataContainer.szenarioName == "Szenario04" ||
                            dataContainer.szenarioName == "Szenario05" ||
                            dataContainer.szenarioName == "Szenario06"
                        )
                {
                    expectedSzenarioTrialCount = 640;
                    expectedTargetTrialCount = 40;
                }
                else if (dataContainer.szenarioName == "Szenario07" ||
                            dataContainer.szenarioName == "Szenario08" ||
                            dataContainer.szenarioName == "Szenario09" ||
                            dataContainer.szenarioName == "Szenario10"
                        )
                {
                    expectedSzenarioTrialCount = 256;
                    expectedTargetTrialCount = 16;
                }
                else if (dataContainer.szenarioName == "Szenario11" ||
                            dataContainer.szenarioName == "Szenario12" ||
                            dataContainer.szenarioName == "Szenario13" ||
                            dataContainer.szenarioName == "Szenario14" ||
                            dataContainer.szenarioName == "Szenario15" ||
                            dataContainer.szenarioName == "Szenario16" ||
                            dataContainer.szenarioName == "Szenario17" ||
                            dataContainer.szenarioName == "Szenario18"
                        )
                {
                    expectedSzenarioTrialCount = 400;
                    expectedTargetTrialCount = 25;
                }

                //string checkHeader = "Time, ForceActualX, ForceActualY, ForceActualZ, ForceNominalX, ForceNominalY, ForceNominalZ, ForceMomentX, ForceMomentY, ForceMomentZ, PositionCartesianX, PositionCartesianY, PositionCartesianZ, OldTarget, ActiveTarget, TargetNumber, TrialNumber, isCatchTrial, hasLeftTarget";  // Study 1
                string checkHeader = "Time, ForceActualX, ForceActualY, ForceActualZ, ForceNominalX, ForceNominalY, ForceNominalZ, ForceMomentX, ForceMomentY, ForceMomentZ, PositionCartesianX, PositionCartesianY, PositionCartesianZ, OldTarget, ActiveTarget, TargetNumber, TrialNumber, IsCatchTrial, PositionStatus";   // Study 2 
                

                if (checkHeader == measureFileReader.ReadLine())
                {
                    while (!measureFileReader.EndOfStream)
                    {
                        string[] measureFileLine = measureFileReader.ReadLine().Split(new string[] { ", " }, StringSplitOptions.None);

                        if (measureFileLine.Count() == 19)
                        {
                            if (    (dataContainer.measureDataRaw.Count > 0) &&
                                    (DateTime.Parse(dataContainer.measureFileCreationDate + " " + measureFileLine[0]).Subtract(dataContainer.measureDataRaw.Last().time_stamp).TotalMilliseconds > 500)
                               )
                            {
                                while ( (Convert.ToInt32(measureFileLine[16]) == dataContainer.measureDataRaw.Last().szenario_trial_number) &&
                                        (!measureFileReader.EndOfStream)
                                     )
                                {
                                    measureFileLine = measureFileReader.ReadLine().Split(new string[] { ", " }, StringSplitOptions.None);
                                }
                            }
                            else if ((dataContainer.measureDataRaw.Count > 0) &&
                                        (Convert.ToInt32(measureFileLine[15]) != dataContainer.measureDataRaw.Last().target_number) &&
                                        (Convert.ToInt32(measureFileLine[16]) == dataContainer.measureDataRaw.Last().szenario_trial_number)
                                    )
                            {
                                while (
                                        (Convert.ToInt32(measureFileLine[15]) != dataContainer.measureDataRaw.Last().target_number) &&
                                        (Convert.ToInt32(measureFileLine[16]) == dataContainer.measureDataRaw.Last().szenario_trial_number) &&
                                        (!measureFileReader.EndOfStream)
                                      )
                                {
                                    measureFileLine = measureFileReader.ReadLine().Split(new string[] { ", " }, StringSplitOptions.None);
                                }
                            }
                            
                            if (Convert.ToInt32(measureFileLine[16]) <= expectedSzenarioTrialCount)
                            {
                                dataContainer.measureDataRaw.Add(new MeasureDataContainer(
                                                            DateTime.Parse(dataContainer.measureFileCreationDate + " " + measureFileLine[0]),
                                                            Convert.ToDouble(measureFileLine[1]) / 1E3, //Forces in Newton
                                                            Convert.ToDouble(measureFileLine[2]) / 1E3,
                                                            Convert.ToDouble(measureFileLine[3]) / 1E3,
                                                            Convert.ToDouble(measureFileLine[4]) / 1E3,
                                                            Convert.ToDouble(measureFileLine[5]) / 1E3,
                                                            Convert.ToDouble(measureFileLine[6]) / 1E3,
                                                            Convert.ToDouble(measureFileLine[7]) / 1E3,
                                                            Convert.ToDouble(measureFileLine[8]) / 1E3,
                                                            Convert.ToDouble(measureFileLine[9]) / 1E3,
                                                            Convert.ToDouble(measureFileLine[10]) / 1E6, //Positions in Meter
                                                            Convert.ToDouble(measureFileLine[11]) / 1E6,
                                                            Convert.ToDouble(measureFileLine[12]) / 1E6,
                                                            Convert.ToInt32(measureFileLine[15]),
                                                            -1,
                                                            Convert.ToInt32(measureFileLine[16]),
                                                            Convert.ToBoolean(measureFileLine[17]),
                                                            //Convert.ToInt32(Convert.ToBoolean(measureFileLine[18]))  //Study 1
                                                            Convert.ToInt32(measureFileLine[18])  //Study 2
                                                            
                                                        ));                             
                            }
                        }
                        else
                        {
                            //("Measure file line error: invalid column count");
                            retVal = false;
                        }                        
                    }

                    int maxTrialCount = dataContainer.measureDataRaw.Max(t => t.szenario_trial_number);
                    int realTrialCount = dataContainer.measureDataRaw.Select(t => t.szenario_trial_number).Distinct().Count();
                    if ((maxTrialCount != expectedSzenarioTrialCount) || (realTrialCount != expectedSzenarioTrialCount))
                    {
                        myManipAnalysisGUI.writeToLogBox("Trial count error: expected " + expectedSzenarioTrialCount + " trials but " + realTrialCount + " were found.");
                        retVal = false;
                    }
                }
                else
                {
                    myManipAnalysisGUI.writeToLogBox("Error in measure file header.");
                    retVal = false;
                }
                measureFileReader.Close();
                measureFileStream.Close();

                int[] targetArray = dataContainer.measureDataRaw.Select( t => t.target_number ).Distinct().ToArray();

                for (int i = 0; i < targetArray.Length; i++)
                {
                    int[] szenarioTrialNumberArray = dataContainer.measureDataRaw.Where(t => t.target_number == targetArray[i]).Select(t => t.szenario_trial_number).Distinct().OrderBy(t => t).ToArray();

                    if (expectedTargetTrialCount == szenarioTrialNumberArray.Length)
                    {
                        for (int j = 0; j < szenarioTrialNumberArray.Length; j++)
                        {
                            List<MeasureDataContainer> tempList = dataContainer.measureDataRaw.Where(t => t.szenario_trial_number == szenarioTrialNumberArray[j]).OrderBy(t => t.time_stamp).ToList();

                            for (int k = 0; k < tempList.Count; k++)
                            {
                                tempList.ElementAt(k).target_trial_number = j + 1;
                            }
                        }
                    }
                    else
                    {
                        myManipAnalysisGUI.writeToLogBox("Target Trial number incorrect.");
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
                dataContainer.measureDataRaw.Clear();
            }           

            return retVal;
        }
    }
}
