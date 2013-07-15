using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ManipAnalysis.Container;

namespace ManipAnalysis
{
    internal class MeasureFileParser
    {
        private readonly DataContainer _dataContainer;
        private readonly ManipAnalysisGui _myManipAnalysisGui;
        private string _measureFilePath;

        public MeasureFileParser(DataContainer container, ManipAnalysisGui myManipAnalysisGui)
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

            string filenameInfoString =
                _measureFilePath.Substring(_measureFilePath.LastIndexOf("\\Szenario", StringComparison.Ordinal) + 1);
            filenameInfoString = filenameInfoString.Remove(filenameInfoString.IndexOf(".csv", StringComparison.Ordinal));
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
            if (filenameInfoStringArray.Count() == 7) // Study 2 and above
            {
                _dataContainer.MeasureFileHash = Md5.ComputeHash(_measureFilePath);
                _dataContainer.StudyName = filenameInfoStringArray[3].Trim();
                _dataContainer.SzenarioName = filenameInfoStringArray[0].Trim();
                _dataContainer.GroupName = filenameInfoStringArray[4].Trim();
                _dataContainer.SubjectName = filenameInfoStringArray[5].Trim();
                _dataContainer.SubjectID = filenameInfoStringArray[6].Trim();
                _dataContainer.MeasureFileCreationDate = filenameInfoStringArray[1];
                _dataContainer.MeasureFileCreationTime = filenameInfoStringArray[2].Replace('.', ':');

                retVal = true;
            }

            return retVal;
        }

        private bool ParseMeasureData()
        {
            bool retVal = true;

            if (_dataContainer.MeasureFileHash != null)
            {
                var measureFileStream = new FileStream(_measureFilePath, FileMode.Open, FileAccess.Read);
                var measureFileReader = new StreamReader(measureFileStream);
                _dataContainer.MeasureDataRaw.Clear();

                int expectedSzenarioTrialCount = 0;
                int expectedTargetTrialCount = 0;

                if (_dataContainer.SzenarioName == "Szenario02" ||
                    _dataContainer.SzenarioName == "Szenario30")
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
                         _dataContainer.SzenarioName == "Szenario20"
                    )
                {
                    expectedSzenarioTrialCount = 400;
                    expectedTargetTrialCount = 25;
                }

                //const string checkHeader = "Time, ForceActualX, ForceActualY, ForceActualZ, ForceNominalX, ForceNominalY, ForceNominalZ, ForceMomentX, ForceMomentY, ForceMomentZ, PositionCartesianX, PositionCartesianY, PositionCartesianZ, OldTarget, ActiveTarget, TargetNumber, TrialNumber, isCatchTrial, hasLeftTarget";  // Study 1
                //const string checkHeader = "Time, ForceActualX, ForceActualY, ForceActualZ, ForceNominalX, ForceNominalY, ForceNominalZ, ForceMomentX, ForceMomentY, ForceMomentZ, PositionCartesianX, PositionCartesianY, PositionCartesianZ, OldTarget, ActiveTarget, TargetNumber, TrialNumber, IsCatchTrial, PositionStatus"; // Study 2 & 3
                const string checkHeader = "Time, ForceActualX, ForceActualY, ForceActualZ, ForceNominalX, ForceNominalY, ForceNominalZ, ForceMomentX, ForceMomentY, ForceMomentZ, PositionCartesianX, PositionCartesianY, PositionCartesianZ, OldTarget, ActiveTarget, TargetNumber, TrialNumber, IsCatchTrial, IsErrorClampTrial, PositionStatus"; // Study 4


                if (checkHeader == measureFileReader.ReadLine())
                {
                    while (!measureFileReader.EndOfStream)
                    {
                        var readLine = measureFileReader.ReadLine();
                        if (readLine != null)
                        {
                            string[] measureFileLine = readLine.Split(new string[] {", "}, StringSplitOptions.None);

                            if (measureFileLine.Count() == 19)
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
                                        measureFileLine = readLine
                                            .Split(new string[] {", "},
                                                   StringSplitOptions.None);
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
                                        measureFileLine = readLine
                                            .Split(new string[] {", "},
                                                   StringSplitOptions.None);
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
                                //("Measure file line error: invalid column count");
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

            return retVal;
        }
    }
}